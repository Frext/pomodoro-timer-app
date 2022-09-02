using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroApp
{
    class ViewModel : ObservableObject
    {
        private string _timeLeftString;

        public string TimeLeftString
        {
            get
            {
                return _timeLeftString;
            }
            set
            {
                SetProperty(ref _timeLeftString, value);
            }
        }

        private string _pomodoroCountString;

        public string PomodoroCountString
        {
            get
            {
                return _pomodoroCountString;
            }
            set
            {
                SetProperty(ref _pomodoroCountString, value);
            }
        }

        private string _pomodoroPhaseString;

        public string PomodoroPhaseString
        {
            get
            {
                return _pomodoroPhaseString;
            }
            set
            {
                SetProperty(ref _pomodoroPhaseString, value);
            }
        }
    }
}
