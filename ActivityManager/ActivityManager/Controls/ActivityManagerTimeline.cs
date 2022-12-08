using ActivityManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ActivityManager.Controls
{
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
        ContentView ActivityStartTimeSlot;

        #region commands

        public ICommand ButtonAddPressedCommand { get; private set; }
        public ICommand ButtonRemovePressedCommand { get; private set; }
        public ICommand ButtonResetPressedCommand { get; private set; }

        #endregion


        /// <summary>
        /// Initiates the actvitiymanagerTimeline
        /// </summary>
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

        /// <summary>
        /// creates the activityTimeline, by first creating the basic structure then creating timestamps & adding activities.
        /// </summary>
        public void CreateActivityTimeline()
        {
            // create basic view structure
            AbsoluteLayout absoluteLayout = CreateDefaultContentViewStructure();
            StackLayout underlay = (StackLayout)absoluteLayout.Children.ElementAt(0);
            AbsoluteLayout overlay = (AbsoluteLayout)absoluteLayout.Children.ElementAt(1);

            // clear the children
            underlay.Children.Clear();
            overlay.Children.Clear();

            // creates hourlytimestamps, then sorts & adds all the activity start + end timestamps.
            var timeSpans = SortTimeStamps(CreateHourlyTimestamps());

            // gets the highest width needed for timestamps, so we can calculate activity width & avoid overlapping with timestamp
            // currently just creates the timestamps like the foreach below, can most likely be optimized
            var maxWidth = GetTimeSlotMaxWidth(underlay, timeSpans);

            // loop through the timestamps, create label, bounds etc. & see if an activity needs to be added here.
            // also needs a more elegant solution, maybe going through underlay.timeslots.labels in a form of search. would be more efficienct
            foreach (var timestamp in timeSpans)
            {
                ContentView timeSlot = new ContentView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 45, // dynamic
                    Padding = 0,
                    BackgroundColor = Color.Yellow
                };

                Label timeStamp = new Label
                {
                    BackgroundColor = Color.BlueViolet,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    Text = timestamp.ToString(@"hh\:mm"),
                    Padding = 0,
                };

                AbsoluteLayout.SetLayoutFlags(timeStamp, AbsoluteLayoutFlags.None);
                AbsoluteLayout.SetLayoutBounds(timeStamp, timeSlot.Bounds);
                timeSlot.Content = timeStamp;
                underlay.Children.Add(timeSlot);

                CreateActivityFrame(overlay, timeSlot, maxWidth);
            }
        }

        private double GetTimeSlotMaxWidth(StackLayout underlay, List<TimeSpan> timespans)
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
            }
            underlay.Children.Clear();
            return maxWidth;
        }

        /// <summary>
        /// create the activity frame
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="timeSlot"></param>
        /// <param name="maxWidth"></param>
        private void CreateActivityFrame(
            AbsoluteLayout overlay,
            ContentView timeSlot,
            double maxWidth
        )
        {
            // make the frame
            Frame activityFrame = new Frame
            {
                BackgroundColor = Color.White,
                CornerRadius = 5,
                BorderColor = Color.Black,
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            // get the current timestamp we are working with from timeslot input
            var timestamp = (Label)timeSlot.Content;

            // loop through our activities, might need to rename ActivityTimeline to Activities. could be more readable
            foreach (var activity in ActivityTimeline)
            {
                // check if the current timestamp equals the startime of our activity, if it does we save that timeslot
                var startTime = activity.StartTime.ToString("HH:mm");

                if (startTime.Equals(timestamp.Text))
                {
                    ActivityStartTimeSlot = timeSlot;
                }

                // check if the current timeslot equals the endtime of our activity, if it does we create the activity.
                var endtime = (DateTime)activity.EndTime;
                string endtimeString = endtime.ToString("HH:mm");
                if (endtimeString.Equals(timestamp.Text))
                {
                    AbsoluteLayout.SetLayoutFlags(activityFrame, AbsoluteLayoutFlags.YProportional);

                    overlay.Children.Add(activityFrame);

                    // Activity Frame calculations
                    var width = timeSlot.Width - maxWidth - 10;
                    var height = timeSlot.Y - ActivityStartTimeSlot.Y;
                    var x = timeSlot.X + maxWidth + 5;
                    var y = ActivityStartTimeSlot.Y + ActivityStartTimeSlot.Height;

                    //min height, needs to be moved to the timeslot creation
                    if (height < 25)
                        height = 25;

                    var rect = new Rectangle(x, y, width, height);
                    ActivityStartTimeSlot = new ContentView();

                    AbsoluteLayout.SetLayoutBounds(activityFrame, rect);

                    // insert the info you want displayed in the activity frame here.
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
                        StartTime = DateTime.Parse("20:54"),
                        EndTime = DateTime.Parse("03:00")
                    },
                };

                return output;
            }
        }

        private AbsoluteLayout CreateDefaultContentViewStructure()
        {
            StackLayout underlay = new StackLayout()
            {
                BackgroundColor = Color.GreenYellow,
                IsVisible = true,
                Opacity = 1,
            };
            AbsoluteLayout.SetLayoutFlags(underlay, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(underlay, new Rectangle(0.5, 0.5, 1, 1));

            AbsoluteLayout overlay = new AbsoluteLayout() { };
            AbsoluteLayout.SetLayoutFlags(overlay, AbsoluteLayout.GetLayoutFlags(underlay));
            AbsoluteLayout.SetLayoutBounds(overlay, AbsoluteLayout.GetLayoutBounds(underlay));

            AbsoluteLayout absoluteLayout = new AbsoluteLayout
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
            return absoluteLayout;
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

            // Uncomment this if you want to remove timestamps between starting & endtime of activities
            //foreach (var activity in ActivityTimeline)
            //{
            //    //var startTime = TimeSpan.Parse(activity.StartTime.ToString("HH:mm"));
            //    //var temp = (DateTime)activity.EndTime;
            //    //var endTime = TimeSpan.Parse(temp.ToString("HH:mm"));

            //    //var startIndex = noDuplicates.IndexOf(startTime);
            //    //var endIndex = noDuplicates.IndexOf(endTime);

            //    //noDuplicates.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
            //}
            timeSpans.Clear();

            foreach (var timeSpan in noDuplicates)
            {
                timeSpans.Add(timeSpan);
            }
            return timeSpans;
        }

        #region test buttons
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
                    StartTime = DateTime.Parse("13:25"),
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

        #endregion
    }
}
