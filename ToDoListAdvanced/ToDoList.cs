using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace ToDoListAdvanced
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Task> CurrentTasks { get; } = new();
        public string NewTaskTitle { get; set; } = "";
        public string NewTaskDescription { get; set; } = "";
        private DateTime _currentDay = DateTime.Today;
        public DateTime CurrentDay { 
            get=>_currentDay;
            set
            {
                if(_currentDay != value)
                {
                    _currentDay = value;
                    OnPropertyChanged();
                    var filtered = Tasks.Where(t => t.Day.Date == CurrentDay.Date).ToList();
                    foreach(var task in filtered)
                    {
                        CurrentTasks.Add(task);
                    }
                }
            }
        }
        public DateTime? NewTaskDeadline { get; set; }
        public ObservableCollection<Task> Tasks { get; } = new();

        Func<Task, DateTime, bool> getTasks = (a, b) => a.Day == b;

        public ICommand Add { get; set; }
        public ICommand Remove { get; set; }

        public ToDoListViewModel()
        {
            Add = new Command(() =>
            {
                CurrentTasks.Add(new Task(NewTaskTitle, NewTaskDescription, CurrentDay, NewTaskDeadline));
                Tasks.Add(new Task(NewTaskTitle, NewTaskDescription, CurrentDay, NewTaskDeadline));
                NewTaskTitle = "";
                NewTaskDescription = "";
            });
            Remove = new Command((Object? args) =>
            {
                if (args is Task task)
                {
                    CurrentTasks.Remove(task);
                    Tasks.Remove(task);
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}