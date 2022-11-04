using ActivityManager.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
namespace ActivityManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityManagerView : ContentView
    {
        ActivityManagerViewModel vm;
        public ActivityManagerView()
        {

            InitializeComponent();
            vm = BindingContext as ActivityManagerViewModel;

            //   AbsoluteLayout.SetLayoutFlags(overlay, AbsoluteLayout.GetLayoutFlags(underlay));
            // AbsoluteLayout.SetLayoutBounds(overlay, AbsoluteLayout.GetLayoutBounds(underlay));

            TimeStampLabels(this,null);

            vm.TimeStampsUpdate += TimeStampLabels;

          


        }

        


        private void TimeStampLabels(object sender, EventArgs e)
        {
            underlay.Children.Clear();
            for (int i = 0; i < vm.TimeStamps.Count; i++)
            {

                Label timeStampLabel = new Label { Text = vm.TimeStamps[i] };
                underlay.Children.Add(timeStampLabel);

            }

          //  OnPropertyChanged(nameof(underlay));


        }

        //   BoxView red;


        //private void RandomEvent(object sender, EventArgs e)
        //{
        //    Label timeStamp = sender as Label;
        //    Debug.WriteLine(timeStamp.Text);


        //    red = new BoxView { Color = Color.Red };
        //    overlay.Children.Add(red);
        //    Console.WriteLine("////////// Red 1" + red.X + " " + red.Y + " " + red.Width + " " + red.Height);
        //    Console.WriteLine("////////// Whatever 1" + timeStamp.X + " " + timeStamp.Y + " " + timeStamp.Width + " " + timeStamp.Height);
        //    var width = container.Width - timeStamp.Width;
        //    //  var rect = new Rectangle(container.X + timeStamp.Width + (width / 10), container.Y, width - (width / 10), container.Height);
        //    var rect = new Rectangle(timeStamp.X, timeStamp.Y, timeStamp.Width, timeStamp.Height);
        //    red.LayoutTo(rect);
        //    Console.WriteLine("////////// Red 1" + red.X + " " + red.Y + " " + red.Width + " " + red.Height);
        //    Console.WriteLine("////////// Whatever 1" + timeStamp.X + " " + timeStamp.Y + " " + timeStamp.Width + " " + timeStamp.Height);


        //}


    }
}