using ActivityManagerDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace ActivityManager.ViewModels
{

    public class ActivityManagerViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public EventHandler TimeStampsUpdate;
        public ICommand ButtonPressedCommand
        {
            get; private set;
        }

        private ObservableCollection<string> timeStamps;

        public ObservableCollection<string> TimeStamps
        {

            get => timeStamps; set => SetProperty(ref timeStamps, value);
        }


        public ActivityManagerViewModel()
        {
            ButtonPressedCommand = new Command(() => ButtonPressed());


            TimeStamps = new ObservableCollection<string>();

            for (int i = 0; i < 24; i++)
            {
                var stringTimeStamp = i.ToString() + ":00";
                TimeStamps.Insert(i, stringTimeStamp);
            }

           


        }

        private void ButtonPressed()
        {

            timeStamps.Add("test");
            TimeStampsUpdate?.Invoke(this,null);


        }



    }
}
