using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace ToDoListAdvanced
{
    public partial class App : Application
    {
        public static ObservableCollection<ToDoTask> GlobalTasks { get; set; } = new();
        public static GoogleDriveSyncService _cloudSync = new();

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}