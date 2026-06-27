using Avalonia.Controls;
using Avalonia.Interactivity;
using KursMVVM.Models;

namespace KursMVVM
{
    public partial class WorkshopWindow : Window
    {
        public Workshop Workshop { get; private set; }
        public WorkshopWindow(Workshop workshop)
        {
            InitializeComponent();
            Workshop = workshop;
            DataContext = this;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Workshop.Name = NameBox.Text;
            Workshop.Manager = ManagerBox.Text;
            Close(Workshop);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
