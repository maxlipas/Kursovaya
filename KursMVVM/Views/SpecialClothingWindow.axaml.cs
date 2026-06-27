using Avalonia.Controls;
using Avalonia.Interactivity;
using KursMVVM.Models;

namespace KursMVVM
{
    public partial class SpecialClothingWindow : Window
    {
        public SpecialClothing Clothing { get; private set; }
        public SpecialClothingWindow(SpecialClothing clothing)
        {
            InitializeComponent();
            Clothing = clothing;
            DataContext = this;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Clothing.Type = TypeBox.Text;
            Clothing.WearPeriod = (int)(WearBox.Value ?? 1);
            Clothing.UnitCost = double.Parse(CostBox.Text);
            Close(Clothing);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
