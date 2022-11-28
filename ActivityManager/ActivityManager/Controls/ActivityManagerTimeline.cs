using ActivityManager.Models;
using ActivityManagerDemo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ActivityManager.Controls
{
    // TODO: FIX Timeline not showing on launch  (somehow force refresh???)

    // change binding to ObservableCollection<ActivityModel>. instead of its own class. timestamps to property inside of managerclass.
    public class ActivityManagerTimeline : ContentView
    {
        #region bindable properties
        public static readonly BindableProperty ActivityTimelineProperty = BindableProperty.Create(
            nameof(ActivityTimeline),
            typeof(List<ActivityModel>),
            typeof(List<ActivityModel>),
            defaultValue: (DummyActivties),
            BindingMode.OneWay,
            propertyChanged: ActivityTimelineChanged
        );

        public List<ActivityModel> ActivityTimeline
        {
            get { return (List<ActivityModel>)GetValue(ActivityTimelineProperty); }
            set
            {
                SetValue(ActivityTimelineProperty, value);
                CreateActivityTimeline();

                OnPropertyChanged(nameof(underlay));
                OnPropertyChanged(nameof(overlay));
                OnPropertyChanged(nameof(absoluteLayout));
                OnPropertyChanged(nameof(ActivityTimeline));
            }
        }

        private static void ActivityTimelineChanged(
            BindableObject bindable,
            object oldValue,
            object newValue
        )
        {
            var obj = ((ActivityManagerTimeline)bindable);
            if (obj.ActivityTimeline != null)
            {
                obj.ActivityTimeline = (List<ActivityModel>)newValue;
            }
        }

        #endregion


        #region commands


        public ICommand ButtonAddPressedCommand { get; private set; }
        public ICommand ButtonRemovePressedCommand { get; private set; }

        public ICommand ButtonResetPressedCommand { get; private set; }

        #endregion

        AbsoluteLayout absoluteLayout;

        StackLayout underlay;

        AbsoluteLayout overlay;

        public ActivityManagerTimeline()
        {
            ButtonAddPressedCommand = new Command(() => AddButtonPressed());
            ButtonRemovePressedCommand = new Command(() => RemoveButtonPressed());
            ButtonResetPressedCommand = new Command(() => ResetButtonPressed());

            CreateActivityTimeline();

            Device.BeginInvokeOnMainThread(
                () => ActivityTimelineChanged(this, null, ActivityTimeline)
            );
        }

        public void CreateActivityTimeline()
        {
            // check if layout exists, if it doesn't create basic page structure.
            CreateDefaultContentViewStructure();

            // clear the children
            underlay.Children.Clear();
            overlay.Children.Clear();

            var timeSpans = SortTimeStamps(CreateHourlyTimestamps());

            var maxWidth = GetTimeSlotMaxWidth(timeSpans);
            underlay.Children.Clear();

            foreach (var timestamp in timeSpans)
            {
                ContentView timeSlot = new ContentView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 50 // dynamic
                };

                Label timeStamp = new Label
                {
                    // BackgroundColor = Color.BlueViolet,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    Text = timestamp.ToString(@"hh\:mm"),
                };
                AbsoluteLayout.SetLayoutFlags(timeStamp, AbsoluteLayoutFlags.None);
                AbsoluteLayout.SetLayoutBounds(timeStamp, timeSlot.Bounds);
                timeSlot.Content = timeStamp;
                underlay.Children.Add(timeSlot);

                CreateActivityFrame(timeSlot, maxWidth);
            }
        }

        private double GetTimeSlotMaxWidth(List<TimeSpan> timespans)
        {
            var maxWidth = new double();
            // foreach for biggest width
            foreach (var timestamp in timespans)
            {
                ContentView timeSlot = new ContentView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 50 // dynamic
                };

                Label timeStamp = new Label
                {
                    BackgroundColor = Color.BlueViolet,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    Text = timestamp.ToString(@"hh\:mm"),
                };
                timeSlot.Content = timeStamp;
                underlay.Children.Add(timeSlot);

                if (maxWidth <= timeStamp.Width)
                    maxWidth = timeStamp.Width;

                underlay.Children.Clear();
            }

            return maxWidth;
        }

        private void CreateActivityFrame(ContentView timeSlot, double maxWidth)
        {
            var timestamp = (Label)timeSlot.Content;

            Frame activityFrame = new Frame
            {
                BackgroundColor = Color.White,
                CornerRadius = 5,
                BorderColor = Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            foreach (var activity in ActivityTimeline)
            {
                var endtime = (DateTime)activity.EndTime;
                string endtimeString = endtime.ToString("HH:mm");
                if (endtimeString.Equals(timestamp.Text))
                {
                    AbsoluteLayout.SetLayoutFlags(activityFrame, AbsoluteLayoutFlags.None);

                    overlay.Children.Add(activityFrame);
                    var width = timeSlot.Width - maxWidth - 10;

                    var rect = new Rectangle(
                        timeSlot.X + maxWidth + 5,
                        timeSlot.Y - 10,
                        width,
                        timeSlot.Height + 5
                    );

                    AbsoluteLayout.SetLayoutBounds(activityFrame, rect);

                    Label ActivityTextLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Text =
                            activity.Name
                            + " "
                            + activity.StartTime.ToString("HH:mm")
                            + " - "
                            + endtimeString
                    };
                    AbsoluteLayout.SetLayoutBounds(ActivityTextLabel, rect);

                    overlay.Children.Add(ActivityTextLabel);

                    activityFrame.LayoutTo(rect);
                }
            }
        }

        public static List<ActivityModel> DummyActivties
        {
            get
            {
                var output = new List<ActivityModel>()
                {
                    new ActivityModel
                    {
                        Name = "badminton",
                        StartTime = DateTime.Parse("01:00"),
                        EndTime = DateTime.Parse("03:00")
                    },
                    new ActivityModel
                    {
                        Name = "tennis",
                        StartTime = DateTime.Parse("03:20"),
                        EndTime = DateTime.Parse("04:00")
                    },
                    new ActivityModel
                    {
                        Name = "chilling",
                        StartTime = DateTime.Parse("05:07"),
                        EndTime = DateTime.Parse("06:00")
                    },
                    new ActivityModel
                    {
                        Name = "lols",
                        StartTime = DateTime.Parse("12:54"),
                        EndTime = DateTime.Parse("13:30")
                    },
                };

                return output;
            }
        }

        private void CreateDefaultContentViewStructure()
        {
            underlay = new StackLayout()
            {
                // BackgroundColor = Color.GreenYellow,
                IsVisible = true,
                Opacity = 1
            };
            AbsoluteLayout.SetLayoutFlags(underlay, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(underlay, new Rectangle(0.5, 0.5, 1, 1));

            overlay = new AbsoluteLayout() { };
            AbsoluteLayout.SetLayoutFlags(overlay, AbsoluteLayout.GetLayoutFlags(underlay));
            AbsoluteLayout.SetLayoutBounds(overlay, AbsoluteLayout.GetLayoutBounds(underlay));

            absoluteLayout = new AbsoluteLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    underlay,
                    overlay,
                    new Button
                    {
                        BorderColor = Color.Black,
                        BackgroundColor = Color.Green,
                        Command = ButtonAddPressedCommand,
                        TranslationX = 100,
                        TranslationY = 730,
                        IsVisible = true,
                        Opacity = 0.7,
                        Text = "+",
                        TextColor = Color.White
                    },
                    new Button
                    {
                        BorderColor = Color.Black,
                        BackgroundColor = Color.Red,
                        Command = ButtonRemovePressedCommand,
                        TranslationX = 200,
                        TranslationY = 730,
                        IsVisible = true,
                        Opacity = 0.7,
                        Text = "-",
                        TextColor = Color.White
                    },
                    new Button
                    {
                        BorderColor = Color.Black,
                        BackgroundColor = Color.Blue,
                        Command = ButtonResetPressedCommand,
                        TranslationX = 300,
                        TranslationY = 730,
                        IsVisible = true,
                        Opacity = 0.7,
                        Text = "reset",
                        TextColor = Color.White,
                    }
                }
            };

            Content = absoluteLayout;
        }

        private void AddButtonPressed()
        {
            ActivityTimeline.Add(
                new ActivityModel
                {
                    Name = "test",
                    StartTime = DateTime.Parse("00:05"),
                    EndTime = DateTime.Parse("00:55")
                }
            );
            ActivityTimeline.Add(
                new ActivityModel
                {
                    Name = "test",
                    StartTime = DateTime.Parse("13:45"),
                    EndTime = DateTime.Parse("14:22")
                }
            );
            ActivityTimeline.Add(
                new ActivityModel
                {
                    Name = "test",
                    StartTime = DateTime.Parse("15:25"),
                    EndTime = DateTime.Parse("16:01")
                }
            );

            ActivityTimelineChanged(this, null, ActivityTimeline);
        }

        private void RemoveButtonPressed()
        {
            ActivityTimeline.Clear();
            ActivityTimelineChanged(this, null, ActivityTimeline);
        }

        private void ResetButtonPressed()
        {
            ActivityTimeline.Clear();
            var activities2 = new ObservableCollection<ActivityModel>
            {
                new ActivityModel
                {
                    Name = "badminton",
                    StartTime = DateTime.Parse("01:00"),
                    EndTime = DateTime.Parse("03:00")
                },
                new ActivityModel
                {
                    Name = "tennis",
                    StartTime = DateTime.Parse("03:20"),
                    EndTime = DateTime.Parse("04:00")
                },
                new ActivityModel
                {
                    Name = "chilling",
                    StartTime = DateTime.Parse("08:07"),
                    EndTime = DateTime.Parse("09:00")
                },
                new ActivityModel
                {
                    Name = "lols",
                    StartTime = DateTime.Parse("12:54"),
                    EndTime = DateTime.Parse("13:30")
                },
            };

            foreach (var activity in activities2)
            {
                ActivityTimeline.Add(activity);
            }

            ActivityTimelineChanged(this, null, ActivityTimeline);
        }

        private List<TimeSpan> CreateHourlyTimestamps()
        {
            List<TimeSpan> timeSpans = new List<TimeSpan>();

            for (int i = 0; i < 24; i++)
            {
                timeSpans.Add(TimeSpan.FromHours(i));
            }
            // timeSpan.toString(@"hh\:mm");
            return timeSpans;
        }

        private List<TimeSpan> SortTimeStamps(List<TimeSpan> timeSpans)
        {
            foreach (var activityTimes in ActivityTimeline)
            {
                var startTime = TimeSpan.Parse(activityTimes.StartTime.ToString("HH:mm"));

                timeSpans.Add(startTime);

                var temp = (DateTime)activityTimes.EndTime;
                var endTime = TimeSpan.Parse(temp.ToString("HH:mm"));
                timeSpans.Add(endTime);
            }

            var noDuplicates = timeSpans.Distinct().ToList();
            noDuplicates.Sort((x, y) => TimeSpan.Compare(x, y));

            foreach (var activity in ActivityTimeline)
            {
                var startTime = TimeSpan.Parse(activity.StartTime.ToString("HH:mm"));
                var temp = (DateTime)activity.EndTime;
                var endTime = TimeSpan.Parse(temp.ToString("HH:mm"));

                var startIndex = noDuplicates.IndexOf(startTime);
                var endIndex = noDuplicates.IndexOf(endTime);

                noDuplicates.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
            }
            timeSpans.Clear();

            foreach (var timeSpan in noDuplicates)
            {
                timeSpans.Add(timeSpan);
            }
            return timeSpans;
        }
    }
}
