using QuickproPos.Data;

namespace QuickproPos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var databaseContext = new DatabaseContext();
            //databaseContext.InitializeAsync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "QuickproPos" };
        }
    }
}
