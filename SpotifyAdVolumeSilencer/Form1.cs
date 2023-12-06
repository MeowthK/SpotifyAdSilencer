using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using AudioSwitcher.AudioApi.CoreAudio;

namespace SpotifyAdVolumeSilencer
{
    public partial class Form1 : Form
    {
        double lastVolume;
        bool activated = false;
        static double preferredVolume;
        static CoreAudioDevice audio = new CoreAudioController().DefaultPlaybackDevice;

        public Form1()
        {
            lastVolume = audio.Volume;

            InitializeComponent();

            btnActivate.Click += BtnActivate_Click;
            btnClose.Click += (obj, ev) => Close();
            btnMinimize.Click += BtnMinimize_Click;
            notifyIcon.Click += (obj, ev) => MinimizeAction(true);
            volume.ValueChanged += (obj, ev) => preferredVolume = volume.Value;

            SpotifyTitleNotifier.StartMonitoring(ms: 1000);
            SpotifyTitleNotifier.TitleChanged += SpotifyTitleNotifier_TitleChanged;
            activateTooltip.SetToolTip(btnActivate, "Activates Spotify Ad volume ducking and minimizes this window.");
        }

        private void MinimizeAction(bool reverse = false)
        {
            notifyIcon.Visible = !reverse;
            Visible = reverse;
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            MinimizeAction();
        }

        private void SpotifyTitleNotifier_TitleChanged(object sender, EventArgs e)
        {
            if (SpotifyTitleNotifier.CurrentTitle == "Advertisement" || SpotifyTitleNotifier.CurrentTitle == "Spotify")
            {
                lastVolume = audio.Volume;
                audio.Volume = preferredVolume;
                return;
            }

            audio.Volume = lastVolume;
        }

        private void BtnActivate_Click(object sender, EventArgs e)
        {
            string[] promptText = { "Activate", "Deactivate" };

            activated = !activated;
            btnActivate.Text = promptText[ConvertBoolean(activated)];

            if (activated)
            {
                lastVolume = audio.Volume;
                MinimizeAction();
            }
            else
            {
                audio.Volume = lastVolume;
            }
        }

        private int ConvertBoolean(bool val)
        {
            return val ? 1 : 0;
        }
    }
}
