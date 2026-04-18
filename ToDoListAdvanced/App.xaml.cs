using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace ToDoListAdvanced
{
    public partial class App : Application
    {
        public static ObservableCollection<ToDoTask>? GlobalTasks { get; set; } = new();
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            System.Diagnostics.Debug.WriteLine("[APP] Constructor started");

#if WINDOWS
            MainPage.HandlerChanged += OnMainPageHandlerChanged;
#endif
        }

#if WINDOWS
        private void OnMainPageHandlerChanged(object? sender, EventArgs e)
        {
            if (Application.Current.Windows.Count > 0)
            {
                var window = Application.Current.Windows[0];
                
                if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
                {
                    nativeWindow.Closed += async (s, args) =>
                    {
                        try
                        {
                            await Saving.Save(GlobalTasks);
                        }
                        catch
                        {}
                    };
                }
            }
        }
#endif

        protected override async void OnSleep()
        {
            await Saving.Save(GlobalTasks);
        }
    }
}