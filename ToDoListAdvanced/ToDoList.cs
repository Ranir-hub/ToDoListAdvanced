using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;

namespace ToDoListAdvanced
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        private DateTime _currentDay = DateTime.Today;
        private TimeSpan? _deadline = null;
        private string _newTaskTitle = "";
        private string _newTaskDescription = "";

        public ObservableCollection<ToDoTask> CurrentTasks { get; } = new();
        public string NewTaskTitle
        {
            get => _newTaskTitle;
            set
            {
                if (value != _newTaskTitle)
                {
                    _newTaskTitle = value;
                    OnPropertyChanged();
                }
            }
        }
        public string NewTaskDescription
        {
            get => _newTaskDescription;
            set
            {
                if (_newTaskDescription != value)
                {
                    _newTaskDescription = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime CurrentDay
        {
            get => _currentDay;
            set
            {
                if (_currentDay != value)
                {
                    _currentDay = value;
                    UpdateUI();
                    OnPropertyChanged();
                }
            }
        }
        public TimeSpan? NewTaskDeadline
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

        public void UpdateUI() 
        {
            if(CurrentTasks.Count != 0) CurrentTasks.Clear();
            var filtered = App.GlobalTasks.Where(t => t.Day.Date == CurrentDay.Date).ToList();
            foreach (var task in filtered) CurrentTasks.Add(task);
        }


        public ICommand Add { get; set; }
        public ICommand Remove { get; set; }
        public ICommand DragStartingCommand { get; }
        public ICommand DropCommand { get; }

        public ToDoListViewModel()
        {
            CurrentTasks.CollectionChanged += Tasks_CollectionChanged;
            _ = LoadDataAsync();
            Add = new Command(() =>
            {
                if (NewTaskTitle == "") return; //изменить на блок кнопки
                var newTask = new ToDoTask(NewTaskTitle, NewTaskDescription, CurrentDay, NewTaskDeadline, false);
                App.GlobalTasks.Add(newTask);
                CurrentTasks.Add(newTask);
                NewTaskTitle = "";
                NewTaskDescription = "";
            });
            Remove = new Command((Object? args) =>
            {
                if (args is ToDoTask task)
                {
                    App.GlobalTasks.Remove(task);
                    CurrentTasks.Remove(task);
                }
            });
            DragStartingCommand = new Command<ToDoTask>(OnDragStarting);
            DropCommand = new Command<ToDoTask>(OnDrop);
        }
        private async Task LoadDataAsync()
        {
            var loaded = await Saving.Load();
            if (loaded != null)
            {
                foreach (var task in loaded) App.GlobalTasks.Add(task);
                UpdateUI();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private async void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _ = Saving.Save(App.GlobalTasks); 
        }

        ToDoTask? draggedTask;
        public void OnDragStarting(ToDoTask task)
        {
            draggedTask = task;
        }

        public void OnDrop(ToDoTask task)
        {
            if (draggedTask == null || task == null || task == draggedTask) return;

            int oldt = App.GlobalTasks.IndexOf(draggedTask);
            int newt = App.GlobalTasks.IndexOf(task);
            App.GlobalTasks.Remove(draggedTask);
            App.GlobalTasks.Insert(newt, draggedTask);
            draggedTask = null;
            UpdateUI();
        }
    }
}
