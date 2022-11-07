using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.Forms;

namespace ActivityManager.Controls
{



    public class ActivityManagerTimeline : ContentView
    {

        public EventHandler TimeStampsUpdate;
        public ICommand ButtonPressedCommand
        {
            get; private set;
        }

        private ObservableCollection<string> timeStamps;

        public ObservableCollection<string> TimeStamps
        {

            get => timeStamps; set => timeStamps = value;
        }

        StackLayout underlay;

        public ActivityManagerTimeline()
        {

            ButtonPressedCommand = new Command(() => ButtonPressed());
          
            // TODO: MOVE VIEWMODEL & CODE BEHIND TO THIS CONTROL!!!!
            CreateDefaultContentPage();
          //  DefaultTimestamps();

        }




        private void CreateDefaultContentPage()
        {
            underlay = new StackLayout()
            {

                BackgroundColor = Color.GreenYellow,


            };
          //  AbsoluteLayout.SetLayoutFlags(underlay, AbsoluteLayoutFlags.All);
         //   AbsoluteLayout.SetLayoutBounds(underlay, new Rectangle(0, 0, 1, 1));


            Content = new AbsoluteLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Beige,


                Children = { new Button {
                    BorderColor = Color.Black, BackgroundColor = Color.Blue, Command = ButtonPressedCommand, AnchorX = 10 },

                underlay

                }



            };






        }

        private void ButtonPressed()
        {

            var buttonPressedLabel = new Label { Text = "you pressed the button" };

            underlay.Children.Add(buttonPressedLabel);
            


        }



        private void DefaultTimestamps()
        {



            for (int i = 0; i < 24; i++)
            {
                Label timeStamp = new Label { Text = i.ToString() + ":00" };
                underlay.Children.Add(timeStamp);

            }



        }






    }
}
