namespace Evonest.EggMonitor;

using System;
using System.Collections.ObjectModel;
using Evonest.EggMonitor.Models;

public partial class TrackerPage : ContentPage
{
    public ObservableCollection<TrackersModel> TrackerItems { get; set; }

    public TrackerPage()
    {
        InitializeComponent();

        TrackerItems = new ObservableCollection<TrackersModel>();
        BindingContext = this;

        LoadData();
    }

    private void LoadData()
    {
        for (int i = 1; i <= 21; i++)
        {
            string infoText = "Gelişim devam ediyor...";
            string tempHum = "Isı: 37.7°C - Nem: %50";

            if (i == 1)
            {
                infoText = "Kuluçka süreci başlatıldı. Kabin ısınmaya bırakıldı.";
            }
            else if (i == 7)
            {
                infoText = "1. Döl Kontrolü (Işık Testi) yapılacak. Boş yumurtaları ayıklamayı unutma.";
            }
            else if (i == 18)
            {
                infoText = "Çıkım dönemine girildi! Yumurtalar viyolden çıkım sepetine alındı.";
                tempHum = "Isı: 37.2°C - Nem: %65";
            }
            else if (i >= 19)
            {
                infoText = "Çıkım bekleniyor. Kabin kapağını kesinlikle açmayın!";
                tempHum = "Isı: 37.2°C - Nem: %65-70";
            }

            TrackerItems.Add(new TrackersModel
            {
                DayNumber = i,
                DateText = DateTime.Now.AddDays(i - 1).ToString("dd MMMM yyyy"),
                TempAndHumidity = tempHum,
                Information = infoText
            });
        }
    }
}