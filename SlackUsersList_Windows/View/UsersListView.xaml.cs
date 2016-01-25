///
/// Developed By Thomas Lock
/// Email: tlock@fhotoroom.com
/// Project: SlackUsersList
/// 
using SlackUsersList_Windows.Model;
using System;
using System.Collections.Generic;
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
/// The UsersListView as part of the Slack User List Model View ViewModel
/// This View uses the User Model and UsersListViewModel ViewModel
/// </summary> 
namespace SlackUsersList_Windows.View
{
    public sealed partial class UsersListView : UserControl
    {
        public UsersListView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This is just a general function to load user profiles. Additional functions to manage user is very simple if admin account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GaneralProfileLink_Tapped(object sender, TappedRoutedEventArgs e)
        {
            User user = ((FrameworkElement)e.OriginalSource).DataContext as User;
            if (user != null)
            {
                //(Window.Current.Content as Frame).Navigate(typeof(UserProfilePage), user.id);
            }
        }


    }
}
