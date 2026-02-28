using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ToDoListAdvanced
{
    public class Task
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Day { get; set; }
        public DateTime? Deadline { get; set; }
        public bool Status { get; set; } = false;

        public Task(string title, string desctiption, DateTime day, DateTime? deadline){
            Title = title; Description = desctiption; Day = day; Deadline = deadline; Status = false;
        }
    }

    public class TaskViewModel
    {
        private readonly Task _task;

        // Конструктор принимает модель
        public TaskViewModel(Task task)
        {
            _task = task;
        }

        public string Title
        {
            get => _task.Title;
            set
            {
                if (_task.Title != value)
                {
                    _task.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _task.Description;
            set
            {
                if (_task.Description != value)
                {
                    _task.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Day
        {
            get => _task.Day;
            set
            {
                if (_task.Day != value)
                {
                    _task.Day = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? Deadline
        {
            get => _task.Deadline;
            set
            {
                if (_task.Deadline != value)
                {
                    _task.Deadline = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Status
        {
            get => _task.Status;
            set
            {
                if (_task.Status != value)
                {
                    _task.Status = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsStrikethrough));
                }
            }
        }
        public bool IsStrikethrough => Status;

        public Task tasK => _task;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
