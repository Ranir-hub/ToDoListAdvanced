namespace ToDoListAdvanced
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new ToDoListViewModel();
        }
    }
}
