using Avalonia.Controls;
using Avalonia.Interactivity;
using KursMVVM.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KursMVVM
{
    public partial class ReceiptWindow : Window
    {
        public Receipt Receipt { get; private set; }
        public ObservableCollection<Worker> WorkerList { get; set; }
        public ObservableCollection<SpecialClothing> ClothingList { get; set; }
        public ReceiptWindow(Receipt receipt)
        {
            InitializeComponent();
            Receipt = receipt;
            if (Receipt.DateReceived == default)
                Receipt.DateReceived = DateTime.Now;
            using (KursContext db = new KursContext())
            {
                WorkerList = new ObservableCollection<Worker>(db.Workers.ToList());
                ClothingList = new ObservableCollection<SpecialClothing>(db.SpecialClothings.ToList());
            }
            DataContext = this;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (WorkerCombo.SelectedItem is Worker w)
                Receipt.WorkerId = w.Id;
            if (ClothingCombo.SelectedItem is SpecialClothing c)
                Receipt.ClothingId = c.Id;
            Receipt.DateReceived = DatePicker.SelectedDate?.DateTime ?? DateTime.Now;
            Receipt.Signature = SignatureBox.Text;
            Close(Receipt);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
