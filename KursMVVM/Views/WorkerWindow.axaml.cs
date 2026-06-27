using Avalonia.Controls;
using Avalonia.Interactivity;
using KursMVVM.Models;
using KursMVVM.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace KursMVVM
{
    public partial class WorkerWindow : Window
    {
        public Worker Worker { get; private set; }
        public ObservableCollection<Workshop> WorkshopList { get; set; }
        public WorkerWindow(Worker worker)
        {
            InitializeComponent();
            Worker = worker;
            using (KursContext db = new KursContext())
            {
                WorkshopList = new ObservableCollection<Workshop>(db.Workshops.ToList());
            }
            DataContext = this;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Worker.FIO = FIOBox.Text;
            Worker.Position = PositionBox.Text;
            Worker.Discount = (int)(DiscountBox.Value ?? 0);
            if (WorkshopCombo.SelectedItem is Workshop ws)
            {
                Worker.WorkshopId = ws.Id;
            }
            Close(Worker);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
