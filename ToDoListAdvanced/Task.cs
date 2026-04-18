using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ToDoListAdvanced
{
    public class ToDoTask
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Day { get; set; }
        public TimeSpan? Deadline { get; set; }
        public bool Complete { get; set; } = false;

        public ToDoTask(string Title, string Description, DateTime Day, TimeSpan? Deadline, bool Complete)
        {
            this.Title = Title;
            this.Description = Description;
            this.Day = Day;
            this.Deadline = Deadline;
            this.Complete = Complete;
        }
    }
}
