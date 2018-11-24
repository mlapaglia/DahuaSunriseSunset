using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BPUtil;
using SunriseSunset;

namespace SunriseSunset
{
	public static class ServiceWrapper
	{
		private static object SyncLockStartStop = new object();
		private static object SyncLockCameraControl = new object();

		private static Thread threadRunner;

		static ServiceWrapper()
		{
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
		}

		public static void Start()
		{
			lock (SyncLockStartStop)
			{
				Stop();
				threadRunner = new Thread(Scheduler);
				threadRunner.IsBackground = true;
				threadRunner.Name = "Scheduler";
				threadRunner.Start();
			}
		}

		public static void Stop()
		{
			lock (SyncLockStartStop)
			{
				threadRunner?.Abort();
				threadRunner = null;
			}
		}

		private static void Scheduler()
		{
			try
			{
				while (true)
				{
					try
					{
						SunEvent nextEvent = null;

						SunriseSunsetConfig sunEventConfig = new SunriseSunsetConfig();
						sunEventConfig.Load();
						
						SunHelper.SunHelperCalculation sunCalculation = SunHelper.CalculateDailySunEvents(sunEventConfig.Latitude,
							sunEventConfig.Longitude,
							sunEventConfig.SunriseOffsetHours,
							sunEventConfig.SunsetOffsetHours);

						if (!sunCalculation.TimeZoneAndLongitudeAreCompatible)
						{
							Logger.Debug("Pausing scheduler for 1 day due to incompatible time zone and longitude. Please fix the problem and restart the service.");
							Thread.Sleep(TimeSpan.FromDays(1));
							continue;
						}

						if (sunCalculation.NextRise < sunCalculation.NextSet)
						{
							nextEvent = new SunEvent(sunCalculation.NextRise, true);
						}
						else if (sunCalculation.NextRise < sunCalculation.NextRise)
						{
							nextEvent = new SunEvent(sunCalculation.NextSet, false);
						}
						else
						{
							nextEvent = new SunEvent(sunCalculation.NextRise, false); // Rise and set are at the same time ... lets just call it a sunset.
						}

						// Now we know when the next event is and what type it is, so we know what profile the cameras should be now.
						if (nextEvent.Rise)
						{
							// Next event is a sunrise, which means it is currently Night.
							TriggerSunActions(nextEvent.Time, Profile.Night);
						}
						else
						{
							// Next event is a sunset, which means it is currently Day.
							TriggerSunActions(nextEvent.Time, Profile.Day);
						}

						while (DateTime.Now <= nextEvent.Time)
						{
							Thread.Sleep(1000);
						}
					}
					catch (ThreadAbortException) { throw; }
					catch (Exception ex)
					{
						Logger.Debug(ex);
						Thread.Sleep(1000);
					}
				}
			}
			catch (ThreadAbortException) { }
			catch (Exception ex) { Logger.Debug(ex); }
		}

		public static void TriggerSunActions(DateTime nextEventTime, Profile profile)
		{
			Logger.Info("TriggerSunActions");

			lock (SyncLockCameraControl)
			{
				SunriseSunsetConfig sunConfig = new SunriseSunsetConfig();
				sunConfig.Load();

				ParallelOptions parallelOptions = new ParallelOptions();

				parallelOptions.MaxDegreeOfParallelism = NumberUtil.Clamp(sunConfig.Cameras.Count, 1, 8);

				Parallel.ForEach(sunConfig.Cameras, parallelOptions, (camera) =>
				{
					try
					{
						WebClient webClient = new WebClient();
						webClient.Credentials = camera.GetCredentials();
						WebRequestRobust(nextEventTime, webClient, camera.GetNightDayUri(profile), camera.GetNightDayBody(profile));
						HandleZoomAndFocus(nextEventTime, webClient, camera, camera.DayZoom, camera.DayFocus);
					}
					catch (ThreadAbortException) { throw; }
					catch (Exception ex)
					{
						Logger.Debug(ex);
					}
				});
			}
		}

		private static void HandleZoomAndFocus(DateTime nextEventTime, WebClient wc, CameraDefinition cam, string zoom, string focus)
		{
			bool setZoom = !string.IsNullOrWhiteSpace(zoom) && double.TryParse(zoom, out var zoomOutput);
			bool focusIsEmpty = string.IsNullOrWhiteSpace(focus);
			bool manualFocus = focusIsEmpty ? false : double.TryParse(focus, out var focusOutput);

			// There are 4 cases
			if (setZoom)
			{
				if (manualFocus)
				{
					// Case 1: Manual Zoom, Manual Focus
					Thread.Sleep(3000);
					// Do this in a loop, because Dahua's API is buggy.
					// Often, the first command causes only focus to happen.
					// The second typically causes zoom and autofocus.
					// The third or fourth usually causes our manual focus level to be set.
					for (int i = 0; i < 5; i++)
					{
						WebRequestRobust(nextEventTime, wc, cam.GetZoomAndFocusUri(zoom, focus), null);
						Thread.Sleep(Math.Max(1, cam.SecondsBetweenLensCommands) * 1000);
					}
				}
				else
				{
					// Case 2: Manual Zoom, Autofocus
					focus = zoom;
					Thread.Sleep(3000);
					// Do this in a loop, because Dahua's API is buggy.
					// Often, the first command causes only focus to happen.
					// The second typically causes zoom and autofocus.
					// The third or fourth usually causes our manual focus level to be set.
					for (int i = 0; i < 4; i++)
					{
						WebRequestRobust(nextEventTime, wc, cam.GetZoomAndFocusUri(zoom, focus), null);
						Thread.Sleep(Math.Max(1, cam.SecondsBetweenLensCommands) * 1000);
					}

					// This method has been, in my experience, reliable enough to call only once.
					WebRequestRobust(nextEventTime, wc, cam.GetAutoFocusUri(), null);
				}
			}
			else
			{
				if (focusIsEmpty || manualFocus)
				{
					// Case 4: Don't zoom and don't focus.  Do nothing.
				}
				else
				{
					// Case 3: Autofocus Only
					// This method has been, in my experience, reliable enough to call only once.
					WebRequestRobust(nextEventTime, wc, cam.GetAutoFocusUri(), null);
				}
			}
		}

		private static void WebRequestRobust(DateTime nextEventTime, WebClient webClient, Uri uri, string body)
		{
			WaitProgressivelyLonger wpl = new WaitProgressivelyLonger(600000, 5000, 15000);

			var requestAttempts = 0;

			while (true)
			{
				if (DateTime.Now >= nextEventTime)
				{
					Logger.Info("Cancelled web request (" + uri + ") due to the next event time being reached.");
					return;
				}

				try
				{
					if (string.IsNullOrEmpty(body))
					{
						webClient.DownloadStringAsync(uri);
					}
					else
					{
						webClient.UploadStringAsync(uri, "Put", body);
					}

					return;
				}
				catch (ThreadAbortException) { throw; }
				catch (Exception ex)
				{
					Logger.Info("Exception thrown attempting web request (" + uri + "): " + ex.Message);

					if(requestAttempts > 5)
					{
						Logger.Info("Attempted to make a request 5 times, aborting");
						return;
					}

					requestAttempts++;
					wpl.Wait();
				}
			}
		}
	}

	class SunEvent
	{
		public DateTime Time;
		public bool Rise = false;

		public SunEvent(DateTime time, bool rise)
		{
			Time = time;
			Rise = rise;
		}
	}
}