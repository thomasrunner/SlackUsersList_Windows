///
/// Developed By Thomas Lock
/// Email: tlock@fhotoroom.com
/// Project: SlackUsersList
/// 
using SlackUsersList_Windows.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

/// <summary>
/// The UsersListView is part of the Slack User List Model View ViewModel
/// This View uses the User Model and UsersListViewModel ViewModel
/// </summary> 
namespace SlackUsersList_Windows.View
{
    public sealed partial class UsersListView : UserControl
    {
        //Unlike WP8.1 this function replace the Navigation to another page by exposing an event that can be track in the parent class.
        public event EventHandler SelectedUserListViewItem;

        //Swipe Item Events
        public event EventHandler SwipeCallUser;
        public event EventHandler SwipeEmailUser;

        //A User EventArgs class can be created to pass along much more data, but for this example a simple public property is nice and clean solution.
        String selecteduserid = "";
        public String SelectUserID 
        {
            get { return selecteduserid; }
            set { selecteduserid = value; }
        } 

        public UsersListView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This is just a general function to load user profiles. Additional functions to manage users is very simple if admin account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GaneralProfileLink_Tapped(object sender, TappedRoutedEventArgs e)
        {
            User user = ((FrameworkElement)e.OriginalSource).DataContext as User;
            if (user != null)
            {
                if (SelectedUserListViewItem != null)
                {
                    selecteduserid = user.id;
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, ()=> 
                    { 
                        SelectedUserListViewItem(this, EventArgs.Empty);
                    });
                } 
                
            }
        }

        private void ItemLayer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X != 0.0)
            {
                Grid itemlayer = (Grid)sender;
                var transform = (CompositeTransform)itemlayer.RenderTransform;
                User user = ((FrameworkElement)e.OriginalSource).DataContext as User;

                if (user == null) return;

                // Reveals Call
                if (e.Cumulative.Translation.X < -16 && Math.Abs(e.Cumulative.Translation.X) <= 128)
                {

                    transform.TranslateX = e.Cumulative.Translation.X;
                }



                // Reveals Email
                if (e.Cumulative.Translation.X > 16 && e.Cumulative.Translation.X <= 128)
                {

                    transform.TranslateX = e.Cumulative.Translation.X;
                }


                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void ItemLayer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            //Always Reset Translation on Release
            Grid itemlayer = (Grid)sender;
            var transform = (CompositeTransform)itemlayer.RenderTransform;
            User user = ((FrameworkElement)e.OriginalSource).DataContext as User;
            if (transform.TranslateX <= -112)
            {
                selecteduserid = user.id;
                try
                {
                    SwipeCallUser(this, EventArgs.Empty);
                }
                catch { }
            }

            if (transform.TranslateX >= 112)
            {
                selecteduserid = user.id;
                try
                {
                    SwipeEmailUser(this, EventArgs.Empty);
                }
                catch { }
            }

            transform.TranslateX = 0;
        }

    }
}
