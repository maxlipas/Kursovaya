using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KursMVVM.ViewModels
{
    public partial class HomePageViewModel:ViewModelBase
    {
        [ObservableProperty]
        private string welcomeMessage = "Добро пожаловать в Avalonia!";
    }
}
