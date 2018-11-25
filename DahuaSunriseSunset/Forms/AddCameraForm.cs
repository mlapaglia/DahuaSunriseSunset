using System;
using System.Net;
using System.Windows.Forms;

namespace SunriseSunset
{
	public partial class AddCameraForm : Form
	{
		public CameraDefinition newCamera;

		public AddCameraForm()
		{
			InitializeComponent();

			newCamera = new DahuaCameraDefinition();

			SetCbItems(cbCameraManufacturer, Enum.GetNames(typeof(CameraManufacturer)), newCamera.Manufacturer);
			SetCbItems(cbSunriseProfile, Enum.GetNames(typeof(Profile)), newCamera.SunriseProfile);
			SetCbItems(cbSunsetProfile, Enum.GetNames(typeof(Profile)), newCamera.SunsetProfile);
		}

		public void ConvertIntoEditForm(CameraDefinition existingCameraData)
		{
			newCamera = existingCameraData;
			txtHostAndPort.Text = newCamera.HostAndPort;
			txtUser.Text = newCamera.Username;
			txtPass.Text = newCamera.Password;
			cbHttps.Checked = newCamera.UseHttps;
			txtDayZoom.Text = newCamera.DayZoom;
			txtDayFocus.Text = newCamera.DayFocus;
			txtNightZoom.Text = newCamera.NightZoom;
			txtNightFocus.Text = newCamera.NightFocus;
			nudLensCmdDelay.Value = newCamera.SecondsBetweenLensCommands;

			SetCbItems(cbCameraManufacturer, Enum.GetNames(typeof(CameraManufacturer)), newCamera.Manufacturer);
			SetCbItems(cbSunriseProfile, Enum.GetNames(typeof(Profile)), newCamera.SunriseProfile);
			SetCbItems(cbSunsetProfile, Enum.GetNames(typeof(Profile)), newCamera.SunsetProfile);

			this.Text = "Edit Camera";
		}

		private void SetCbItems(ComboBox comboBox, string[] items, object selectedValue)
		{
			comboBox.Items.Clear();
			comboBox.Items.AddRange(items);
			string selectedItem = selectedValue.ToString();
			for (int i = 0; i < items.Length; i++)
				if (items[i] == selectedItem)
				{
					comboBox.SelectedIndex = i;
					break;
				}
		}

		private Profile GetSelectedProfile(ComboBox comboBox, Profile defaultValue)
		{
			int idx = comboBox.SelectedIndex;
			if (idx >= 0 && idx < comboBox.Items.Count)
			{
				if (Enum.TryParse<Profile>(comboBox.Items[idx].ToString(), out Profile profile))
					return profile;
			}
			return defaultValue;
		}

		private void btnGetCurrent_Click(object sender, EventArgs e)
		{
			WebClient wc = new WebClient();
			wc.Credentials = new NetworkCredential(txtUser.Text, txtPass.Text);
			wc.DownloadStringCompleted += (sender2, e2) =>
			{
				TextMessageBox.Show(e2.Result);
			};
			wc.DownloadStringAsync(new Uri("http" + (cbHttps.Checked ? "s" : "") + "://" + txtHostAndPort.Text + "/cgi-bin/devVideoInput.cgi?action=getFocusStatus"));
		}

		private void btnHelp_Click(object sender, EventArgs e)
		{
			new CameraHelpForm().Show();
		}

		private void AddCameraForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (cbCameraManufacturer.Text)
			{
				case "Dahua":
					newCamera = new DahuaCameraDefinition(txtHostAndPort.Text,
						txtUser.Text,
						txtPass.Text,
						cbHttps.Checked,
						txtDayZoom.Text,
						txtDayFocus.Text,
						txtNightZoom.Text,
						txtNightFocus.Text,
						(int)nudLensCmdDelay.Value,
						GetSelectedProfile(cbSunriseProfile, Profile.Day),
						GetSelectedProfile(cbSunsetProfile, Profile.Night));
					break;
				case "Hikvision":
					newCamera = new HikvisionCameraDefinition(txtHostAndPort.Text,
						txtUser.Text,
						txtPass.Text,
						cbHttps.Checked,
						txtDayZoom.Text,
						txtDayFocus.Text,
						txtNightZoom.Text,
						txtNightFocus.Text,
						(int)nudLensCmdDelay.Value,
						GetSelectedProfile(cbSunriseProfile, Profile.Day),
						GetSelectedProfile(cbSunsetProfile, Profile.Night));
					break;
			}
		}

		private void lblLensCmd_Click(object sender, EventArgs e)
		{
			MessageBox.Show("It has been my experience that the manual zoom/focus command is unreliable." +
				" For this reason, the program sends the command to the camera 4-5 times depending on the parameters being used." +
				" You can configure how long the program waits between sending the commands. (default: 4 seconds)");
		}

		private void lblLensCmdQ_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			lblLensCmd_Click(sender, e);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
