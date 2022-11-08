using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Pipes;
using System.Net.Mime;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.Forms;


namespace ActivityManager.Controls
{
    public class ActivityManagerTimeline : ContentView
    {

        #region bindable properties
        public static readonly BindableProperty ActivityTimelineProperty =
           BindableProperty.Create(nameof(ActivityTimeline), typeof(ObservableCollection<string>), typeof(ActivityManagerTimeline),
               defaultValue: (new ObservableCollection<string>()), BindingMode.OneWay, propertyChanged: ActivityTimelineChanged);

        public ObservableCollection<string> ActivityTimeline
        {
            get
            {
                return (ObservableCollection<string>)GetValue(ActivityTimelineProperty);
            }
            set
            {
                SetValue(ActivityTimelineProperty, value);
                CreateActivityTimeline();

            }
        }

        private static void ActivityTimelineChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var obj = ((ActivityManagerTimeline)bindable);
            if (obj.ActivityTimeline != null)
                obj.ActivityTimeline = (ObservableCollection<string>)newValue;
        }
        #endregion


        #region commands

        public ICommand ButtonAddPressedCommand
        {
            get; private set;
        }public ICommand ButtonRemovePressedCommand
        {
            get; private set;
        }

        #endregion

        AbsoluteLayout absoluteLayout;

        StackLayout underlay;

        StackLayout overlay;


        public ActivityManagerTimeline()
        {


            ButtonAddPressedCommand = new Command(() => AddButtonPressed());
            ButtonRemovePressedCommand = new Command(() => RemoveButtonPressed());

            // TODO: MOVE VIEWMODEL & CODE BEHIND TO THIS CONTROL!!!!
            CreateDefaultContentPage();
            DefaultTimestamps();
            ActivityTimeline = ActivityTimeline;

            Content = absoluteLayout;

        }


        public void CreateActivityTimeline()
        {

            ListView listView = new ListView { ItemsSource = ActivityTimeline };

            underlay.Children.Add(listView);


        }




        private void CreateDefaultContentPage()
        {
            underlay = new StackLayout()
            {

                BackgroundColor = Color.GreenYellow,



            };
            AbsoluteLayout.SetLayoutFlags(underlay, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(underlay, new Rectangle(0, 0, 1, 1));

            overlay = new StackLayout()
            {
                BackgroundColor = Color.AntiqueWhite



            };


            absoluteLayout = new AbsoluteLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Beige,
                BindingContext = ActivityTimeline,


                Children = {

                    underlay,


                    new Button {
                        BorderColor = Color.Black,
                        BackgroundColor = Color.Blue,
                        Command = ButtonAddPressedCommand, TranslationX = 100, TranslationY = 730 },

                    new Button { BorderColor = Color.Black, Command = ButtonRemovePressedCommand, TranslationX = 200, TranslationY = 730

                    } }





            };





        }

        private void AddButtonPressed()
        {
            ActivityTimeline.Add("hihihi");
        }
        private void RemoveButtonPressed()
        {
            ActivityTimeline.Clear();
        }
        private void DefaultTimestamps()
        {

            for (int i = 0; i < 24; i++)
            {

                ActivityTimeline.Add(i.ToString() + ":00");

            }


        }






    }
}
