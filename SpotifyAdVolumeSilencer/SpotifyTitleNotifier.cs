using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace SpotifyAdVolumeSilencer
{
    public static class SpotifyTitleNotifier
    {
        public static readonly string SPOTIFY_FREE_STR = "Spotify Free";

        public static event EventHandler TitleChanged;
        private static string _currentTitle = SPOTIFY_FREE_STR;
        private static System.Threading.Timer timer = null;

        private static string currentTitle
        {
            get => _currentTitle;
            set
            {
                if (_currentTitle != value)
                {
                    _currentTitle = value;

                    if (TitleChanged != null)
                        TitleChanged.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static bool PlayerPaused { get => _currentTitle == SPOTIFY_FREE_STR; }

        public static string CurrentTitle { get => currentTitle; }

        public static void StartMonitoring(int ms)
        {
            if (timer == null)
            {
                timer = new System.Threading.Timer(new System.Threading.TimerCallback((o) => Monitor()), null, ms, ms);
                return;
            }

            timer.Change(ms, ms);
        }

        private static void Monitor()
        {
            try
            {
                var proc = Process.GetProcessesByName("Spotify").Where(p => p.MainWindowTitle != string.Empty).ToArray();

                if (proc.Length > 0 && _currentTitle != proc[0].MainWindowTitle)
                    _currentTitle = proc[0].MainWindowTitle;

            } catch (ArgumentNullException)
            {
                Debug.WriteLine("Null exception occured while trying to scan Spotify processes.");
            }

            //foreach (var proc in procs)
            //{
            //    var curProcTitle = proc.MainWindowTitle;

            //    if (curProcTitle.Length > 1 && _currentTitle != curProcTitle)
            //    {
            //        currentTitle = curProcTitle;
            //        break;
            //    }
            //}
        }
    }
}