using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace ActivityManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityManagerView : ContentView
    {
        public ActivityManagerView()
        {
            InitializeComponent();



            AbsoluteLayout.SetLayoutFlags(overlay, AbsoluteLayout.GetLayoutFlags(underlay));
            AbsoluteLayout.SetLayoutBounds(overlay, AbsoluteLayout.GetLayoutBounds(underlay));
        }

        BoxView red;
        private void Whatever_SizeChanged(object sender, EventArgs e)
        {


            red = new BoxView { Color = Color.Red };
            overlay.Children.Add(red);
            ////Console.WriteLine("////////// Red 1" + red.X + " " + red.Y + " " + red.Width + " " + red.Height);
            ////Console.WriteLine("////////// Whatever 1" + label.X + " " + label.Y + " " + label.Width + " " + label.Height);
            var width = container.Width - label.Width;
            var rect = new Rectangle(container.X + label.Width + (width / 10), container.Y, width - (width / 10), container.Height);
            red.LayoutTo(rect);

            //Console.WriteLine("////////// Red 2" + red.X + " " + red.Y + " " + red.Width + " " + red.Height);
            //Console.WriteLine("////////// Whatever 2" + label.X + " " + label.Y + " " + label.Width + " " + label.Height);
        }


    }
}