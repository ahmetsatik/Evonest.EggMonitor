using CommunityToolkit.Maui.Views;
using Evonest.EggMonitor.Models;
using System;

namespace Evonest.EggMonitor
{
    public partial class HatchDetailsPopup : Popup
    {
        public HatchDetailsPopup(IncubationPresetModel hatch)
        {
            InitializeComponent();

            // Verileri XAML'a bağlıyoruz
            BindingContext = hatch;
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            // Butona basıldığında Popup'ı kapat
            CloseAsync();
        }
    }
}