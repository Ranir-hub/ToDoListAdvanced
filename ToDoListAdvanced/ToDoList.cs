using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.Maui.ApplicationModel; 

namespace ToDoListAdvanced
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        private DateTime _currentDay = DateTime.Today;
        private TimeSpan _deadline = TimeSpan.Zero;
        private string _newTaskTitle = "";
        public string _loginStatus = "Login";
        public ObservableCollection<ToDoTask> CurrentTasks { get; } = new();

        public string LoginStatus
        {
            get => _loginStatus;
            set
            {
                if (value != _loginStatus)
                {
                    _loginStatus = value;
                    OnPropertyChanged();
                }
            }
        }
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
        public TimeSpan NewTaskDeadline
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
            var filtered = App.GlobalTasks?.Where(t => t.Day.Date == CurrentDay.Date).ToList();
            foreach (var task in filtered) CurrentTasks.Add(task);
        }
        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand DragStartingCommand { get; }
        public ICommand DropCommand { get; }
        public ICommand LoginStatusCommand {  get; }
        public ToDoListViewModel()
        {
            _currentTimer = new System.Timers.Timer(1000);
            _currentTimer.Elapsed += async (s, e) =>
            {
                _ = SaveDataAsync();
            };

            _ = LoadDataAsync();

            AddCommand = new Command(async () =>
            {
                if (NewTaskTitle == "") return;
                var newTask = new ToDoTask(NewTaskTitle, CurrentDay, NewTaskDeadline, false);
                App.GlobalTasks.Add(newTask);
                CurrentTasks.Add(newTask);
                NewTaskTitle = "";
                NewTaskDeadline = TimeSpan.Zero;
                newTask.PropertyChanged += Task_PropertyChanged;
                _ = SaveDataAsync();
            });
            RemoveCommand = new Command(async (Object? args) =>
            {
                if (args is ToDoTask task)
                {
                    App.GlobalTasks?.Remove(task);
                    CurrentTasks.Remove(task);
                    task.PropertyChanged -= Task_PropertyChanged;
                    _ = SaveDataAsync();
                }
            });
            DragStartingCommand = new Command<ToDoTask>(OnDragStarting);
            DropCommand = new Command<ToDoTask>(OnDrop);
            LoginStatusCommand = new Command(async () =>
            {
                if (App._cloudSync.IsLoggedIn)
                {
                    try
                    {
                        _ = SaveDataAsync();
                        _ = App._cloudSync.LogoutAsync();
                        await Shell.Current.DisplayAlertAsync("Выход", "Вы успешно вышли из аккаунта", "OK");
                        LoginStatus = "Login";
                    }
                    catch (Exception ex)
                    {
                        await Shell.Current.DisplayAlertAsync("Ошибка", ex.Message, "OK");
                    }
                }
                else
                {
                    try
                    {
                        bool isLoggedIn = await App._cloudSync.LoginAsync();
                        if (isLoggedIn)
                        {
                            bool result = await Shell.Current.DisplayAlertAsync("Вход", "Вы успешно вошли в аккаунт. Загрузить данные с Google?", "Да", "Нет");
                            if (result)
                            {
                                var loaded = await App._cloudSync.LoadFromCloudAsync();
                                if (loaded != null && loaded.Count > 0)
                                {
                                    App.GlobalTasks = loaded;
                                }
                                SubscribeToAllTasks(loaded);
                                _ = Saving.Save(App.GlobalTasks);
                                UpdateUI();
                                await Shell.Current.DisplayAlertAsync("Данные", "Данные успешно загружены", "OK");
                            }
                            else
                            {
                                _ = App._cloudSync.SaveToCloudAsync(App.GlobalTasks);
                            }
                            LoginStatus = "Logout";
                        }
                    }
                    catch (Exception ex)  {
                        await Shell.Current.DisplayAlertAsync("Вход", ex.Message, "OK");
                    }
                }
            });
        }

        private System.Timers.Timer? _currentTimer;
        private async void Task_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _ = Saving.Save(App.GlobalTasks);
            RestartCloudSaveTimer();
        }
        private void SubscribeToAllTasks(IEnumerable<ToDoTask> tasks)
        {
            foreach (var task in tasks)
            {
                task.PropertyChanged += Task_PropertyChanged;
            }
        }
        private void RestartCloudSaveTimer()
        {
            if (!App._cloudSync.IsLoggedIn) return;

            if (_currentTimer != null)
            {
                _currentTimer.Stop();
                _currentTimer.Start();
            }
        }
        private async Task LoadDataAsync()
        {
            var loaded = await Saving.Load();
            if (loaded != null && loaded.Count > 0  )
            {
                App.GlobalTasks = loaded;
            }
            SubscribeToAllTasks(loaded);
            UpdateUI();
        }

        private async Task SaveDataAsync()
        {
            try
            {
                if (App._cloudSync.IsLoggedIn)
                {
                    _ = App._cloudSync.SaveToCloudAsync(App.GlobalTasks);
                }
                _ = Saving.Save(App.GlobalTasks);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Ошибка", ex.Message, "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        ToDoTask? draggedTask;
        public void OnDragStarting(ToDoTask task)
        {
            draggedTask = task;
        }

        public void OnDrop(ToDoTask task)
        {
            if (draggedTask == null || task == null || task == draggedTask) return;

            int newt = App.GlobalTasks.IndexOf(task);
            TimeSpan swapDeadline = task.Deadline;
            task.Deadline = draggedTask.Deadline;
            draggedTask.Deadline = swapDeadline;

            App.GlobalTasks.Remove(draggedTask);
            App.GlobalTasks.Insert(newt, draggedTask);

            draggedTask = null;
            UpdateUI();
            _ = SaveDataAsync();
        }
    }
}
