using ActivityManagerDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace ActivityManager.ViewModels
{
    public class ActivityManagerViewModel: BaseViewModel
    {
        private ObservableCollection<string> timeStamps;

        public ObservableCollection<string> TimeStamps
        {

            get => timeStamps; set => SetProperty(ref timeStamps,value);
        }

        public String TimeNow
        {
            get; set;
        }

        public ActivityManagerViewModel()
        {




            TimeStamps = new ObservableCollection<string> { };
            for (int i = 0; i < 24; i++)
            {
                var stringTimeStamp = i.ToString() + ":00";
                TimeStamps.Insert(i, stringTimeStamp);
            }









        }






    }
}
