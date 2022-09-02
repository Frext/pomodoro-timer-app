using System;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace PomodoroApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer1 = new DispatcherTimer();
        private ViewModel viewModel1 = new ViewModel();

        private int PomodoroCount = 0;
        private const string POMODORO_COUNT_IS = "Pomodoro Count : ";

        private static class TimerValue
        {
            public static int seconds;
            public static int minutes;
        }

        private static class PomodoroPhaseDurationsInMinutes
        {
            public static int WorkDuration = 0;
            public static int ShortBreakDuration = 5;
            public static int LongBreakDuration = 15;
        }

        private static class PomodoroPhaseMessages
        {
            // Visible on the top-left corner of the app.
            public static string WorkMessage = "Work";
            public static string ShortBreakMessage = "Break";
            public static string LongBreakMessage = "Long Break";
        }

        public MainWindow()
        {
            // Set up the timer1 settings.
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = new TimeSpan(0, 0, 1);

            DataContext = viewModel1;

            InitializeComponent();

            // For the first app screen
            ResetApp();
        }

        private void ResetApp()
        {
            SetPomodoroPhaseTo(PomodoroPhaseMessages.WorkMessage);

            SetPomodoroCountTo(0);

            PauseTimerAndDisablePauseButton();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DecreaseTimerValue();

            if (TimerValue.minutes.Equals(0) && TimerValue.seconds.Equals(0)) // If the timer has ended
            {
                PauseTimerAndDisablePauseButton();
            }

            UpdateTimeLeftString(TimerValue.minutes, TimerValue.seconds);
        }

        #region Button Click Events
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartTimerAndDisableStartButton();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            PauseTimerAndDisablePauseButton();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetApp();
        }
        #endregion

        #region Timer Methods
        private void SetTimerValueTo(int minutes)
        {
            TimerValue.minutes = minutes;
            TimerValue.seconds = 1;
        }

        private void StartTimerAndDisableStartButton()
        {
            timer1.Start();

            btnStart.IsEnabled = false;
            btnPause.IsEnabled = true;
        }

        private void PauseTimerAndDisablePauseButton()
        {
            timer1.Stop();

            btnStart.IsEnabled = true;
            btnPause.IsEnabled = false;
        }

        private void DecreaseTimerValue()
        {
            if (TimerValue.seconds == 0)
            {
                if (TimerValue.minutes > 0)
                {
                    TimerValue.minutes--;
                    TimerValue.seconds = 59;
                }
            }
            else
            {
                TimerValue.seconds--;
            }
        }

        private void PlayTimerFinishedSound()
        {
            using (var soundPlayer = new SoundPlayer(@"Resources/finish_sound_effect.wav"))
            {
                soundPlayer.Play();
            }
        }
        #endregion

        #region Text Update Method
        private void UpdateTimeLeftString(int minutes, int seconds)
        {
            // Got help from : https://stackoverflow.com/questions/5972949/number-formatting-how-to-convert-1-to-01-2-to-02-etc/5972961
            viewModel1.TimeLeftString = $"{minutes}:{seconds.ToString("D2")}"; // D2 = 2 Digits

            if (TimerValue.minutes.Equals(0) && TimerValue.seconds.Equals(0)) // If the timer has ended
            {
                SwitchToNextPomodoroPhase();

                PlayTimerFinishedSound();

                MakeDesktopIconBlink();
            }
        }

        #endregion

        #region Pomodoro Methods
        private void SwitchToNextPomodoroPhase()
        {
            if (viewModel1.PomodoroPhaseString == PomodoroPhaseMessages.WorkMessage) // Work phase -> Short Break phase
            {
                SetPomodoroPhaseTo(PomodoroPhaseMessages.ShortBreakMessage);
            }
            else if (viewModel1.PomodoroPhaseString == PomodoroPhaseMessages.ShortBreakMessage) // Short Break phase -> Long Break  OR  Work phase
            {
                // A work phase and a short break phase is counted as a pomodoro
                SetPomodoroCountTo(PomodoroCount + 1);

                if (PomodoroCount % 4 == 0) // For every 4 pomodoros, take a long break                 Short Break phase -> Long Break phase
                {
                    SetPomodoroPhaseTo(PomodoroPhaseMessages.LongBreakMessage);
                }
                else // Short Break phase -> Work phase
                {
                    SetPomodoroPhaseTo(PomodoroPhaseMessages.WorkMessage);
                }
            }
            else if (viewModel1.PomodoroPhaseString == PomodoroPhaseMessages.LongBreakMessage) // Long Break phase -> Work phase
            {
                SetPomodoroPhaseTo(PomodoroPhaseMessages.WorkMessage);
            }
        }

        private void SetPomodoroCountTo(int pomodoroCountToSet)
        {
            PomodoroCount = pomodoroCountToSet;

            viewModel1.PomodoroCountString = POMODORO_COUNT_IS + PomodoroCount;
        }

        private void SetPomodoroPhaseTo(string pomodoroPhaseMessage)
        {
            if (pomodoroPhaseMessage == PomodoroPhaseMessages.WorkMessage)
            {
                viewModel1.PomodoroPhaseString = PomodoroPhaseMessages.WorkMessage;

                SetTimerValueTo(PomodoroPhaseDurationsInMinutes.WorkDuration);
                UpdateTimeLeftString(PomodoroPhaseDurationsInMinutes.WorkDuration, 0);
            }
            else if (pomodoroPhaseMessage == PomodoroPhaseMessages.ShortBreakMessage)
            {
                viewModel1.PomodoroPhaseString = PomodoroPhaseMessages.ShortBreakMessage;

                SetTimerValueTo(PomodoroPhaseDurationsInMinutes.ShortBreakDuration);
                UpdateTimeLeftString(PomodoroPhaseDurationsInMinutes.ShortBreakDuration, 0);
            }
            else if (pomodoroPhaseMessage == PomodoroPhaseMessages.LongBreakMessage)
            {
                viewModel1.PomodoroPhaseString = PomodoroPhaseMessages.LongBreakMessage;

                SetTimerValueTo(PomodoroPhaseDurationsInMinutes.LongBreakDuration);
                UpdateTimeLeftString(PomodoroPhaseDurationsInMinutes.LongBreakDuration, 0);
            }
        }
#endregion

        [DllImport("user32")]
        public static extern int FlashWindow(IntPtr hwnd, bool bInvert);

        private void MakeDesktopIconBlink()
        {
            // Got help from https://stackoverflow.com/questions/5118226/how-to-make-a-wpf-window-to-blink-on-the-taskbar

            WindowInteropHelper wih = new WindowInteropHelper(myWindow);
            FlashWindow(wih.Handle, true);
            taskBarItem.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
        }
    }
}
