using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ToDoListAdvanced
{

    public class ToDoTask : INotifyPropertyChanged
    {
        private DateTime _day;
        private TimeSpan _deadline;
        private string _title;
        public bool _complete;

        public ToDoTask(string title, DateTime day, TimeSpan deadline, bool complete)
        {
            Title = title; Day = day; Deadline = deadline; Complete = complete;
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Day
        {
            get => _day;
            set
            {
                if (_day != value)
                {
                    _day = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan Deadline
        {
            get => _deadline;
            set
            {
                if (_deadline != value)
                {
                    _deadline = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Complete
        {
            get => _complete;
            set
            {
                if (_complete != value)
                {
                    _complete = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}