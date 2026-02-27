using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ToDoListAdvanced
{
    internal class Task : INotifyPropertyChanged
    {
        private string _title;
        private string _description;
        private DateTime _day;
        private DateTime? _deadline;
        private bool _status;

        public string Title
        {
            get => _title; 
            set {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Status
        {
            get => _status;
            set
            {
                if(value != _status)
                {
                    _status = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TextDecorations));
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

        public DateTime? deadline
        {
            get => _deadline;
            set
            {
                if(value != _deadline)
                {
                    _deadline = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
