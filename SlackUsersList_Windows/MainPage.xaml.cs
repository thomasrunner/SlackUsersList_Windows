using SlackUsersList.ViewModel;
using SlackUsersList_Windows.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SlackUsersList_Windows
{
    public sealed partial class MainPage : Page
    {
        private UsersListViewModel userviewmodel;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            userviewmodel = new UsersListViewModel();

            //These are the little color indicator in the Right Panel which appears when clicking the top right side user status button.
            RightPanelAdminTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["admin"];
            RightPanelOwnerTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["owner"];
            RightPanelActiveStatusBorder.Background = (App.Current as App).usercolorstatusdict["active"];
            RightPanelBotStatusBorder.Background = (App.Current as App).usercolorstatusdict["bots"];
            RightPanelDeletedStatusBorder.Background = (App.Current as App).usercolorstatusdict["deleted"];
            RightPanelAwayStatusBorder.Background = (App.Current as App).usercolorstatusdict["away"];
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //If the page has no data try downloading
            if (UsersListLiveView.DataContext == null)
            {
                await userviewmodel.PopulateUsers(false);

                UsersListLiveView.DataContext = userviewmodel.UsersList;
            }

            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Collapsed;

            //Hides the StatusBar
            //await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();

            //Hardware Back Button
            //HardwareButtons.BackPressed += OnBackButton;

            //No Network Connections
            bool hasNetworkConnection = NetworkInterface.GetIsNetworkAvailable();

            if (hasNetworkConnection == false)
            {
                var messageDialog = new MessageDialog("No Internet Connection.");
                messageDialog.Commands.Add(new UICommand("Ok", (command) =>
                {
                    return;
                }));
                messageDialog.DefaultCommandIndex = 1;
                await messageDialog.ShowAsync();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Collapsed;
            //HardwareButtons.BackPressed -= OnBackButton;
        }

        //private void OnBackButton(object sender, BackPressedEventArgs e)
        //{
        //    if (RightPanelUserStatusTypeListBorder.Visibility == Visibility.Visible)
        //    {
        //        RightPanelUserStatusTypeListBorder.Visibility = Visibility.Collapsed;
        //        e.Handled = true;
        //        return;
        //    }

        //    e.Handled = false;
        //}

        /// <summary>
        /// This allows users to search a specific user by both profile name and first name as well as use some key words to quick filter large list
        /// KEY WORDS (admin, away, owner, bots, active, deleted, "" = all)
        /// ADDITION KEY WORDS (marketing, android, ios, customer, ceo, cfo, cto, wp, server, etc...) <- Excellent for larger teams
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserSearchTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if ((App.Current as App).localuserlistcollection == null) return;
            if ((App.Current as App).localuserlistcollection.Count == 0) return;

            TextBox usersearchtextbox = (TextBox)sender;
            String namesearch = usersearchtextbox.Text.ToLower();

            if (namesearch == "admin")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.is_admin == true).ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["admin"];
                TitleMemberListTypeTextBlock.Text = "Admin";
            }
            else if (namesearch == "owner")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.is_owner == true).ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["owner"];
                TitleMemberListTypeTextBlock.Text = "Owner";
            }
            else if (namesearch == "bots")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.IsSlackBot == true).ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["bots"];
                TitleMemberListTypeTextBlock.Text = "Bots";
            }
            else if (namesearch == "active")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.presence != "away").ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["active"];
                TitleMemberListTypeTextBlock.Text = "Active";
            }
            else if (namesearch == "deleted")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.deleted == true).ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["deleted"];
                TitleMemberListTypeTextBlock.Text = "Deleted";
            }
            else if (namesearch == "all")
            {
                userviewmodel.UsersList = (App.Current as App).localuserlistcollection;
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["all"];
                TitleMemberListTypeTextBlock.Text = "All";
            }
            else if (namesearch == "away")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.presence == "away")).ToList();
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["away"];
                TitleMemberListTypeTextBlock.Text = "Away";
            }
            else if (namesearch.Trim().Length > 0)
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.name.ToLower().StartsWith(namesearch.ToLower()) || x.profile.first_name.ToLower().StartsWith(namesearch.ToLower()) || x.profile.title.ToLower().StartsWith(namesearch.ToLower())).ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["all"];
                TitleMemberListTypeTextBlock.Text = "All";
            }
            else
            {
                userviewmodel.UsersList = (App.Current as App).localuserlistcollection;
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["all"];
                TitleMemberListTypeTextBlock.Text = "All";
            }
            UsersListLiveView.DataContext = userviewmodel.UsersList;
        }

        /// <summary>
        /// Placeholder Function to help users understand this is a search field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserSearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserSearchTextBox.Text == "search")
            {
                UserSearchTextBox.Text = "";
            }
            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Collapsed;
        }

        private void UserSearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UserSearchTextBox.Text.Trim().Length == 0)
            {
                UserSearchTextBox.Text = "search";
            }
        }

        /// <summary>
        /// These 2 functions are just a fun colourful addition for users to filter based on user status. This shows the menu
        /// and when a stack item is selected I used the item name to apply the search as well as teach user some keyword search tricks
        /// by putting the word into the search box, subtle but very effective over time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleStatusTypeButtonBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((App.Current as App).localuserlistcollection == null) return;
            if ((App.Current as App).localuserlistcollection.Count == 0) return;
            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Visible;
        }

        private void RightPanelUserStatusTypeStackItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((App.Current as App).localuserlistcollection == null) return;
            if ((App.Current as App).localuserlistcollection.Count == 0) return;

            Border StatusTypeItem = (Border)sender;
            if (StatusTypeItem.Name == "RightPanelStatusTypeItemAllBorder")
            {
                userviewmodel.UsersList = (App.Current as App).localuserlistcollection;
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["all"];
                TitleMemberListTypeTextBlock.Text = "All";
                UserSearchTextBox.Text = "all";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemAdminBorder")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.is_admin == true).ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["admin"];
                TitleMemberListTypeTextBlock.Text = "Admin";
                UserSearchTextBox.Text = "admin";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemOwnerBorder")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.is_owner == true).ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["owner"];
                TitleMemberListTypeTextBlock.Text = "Owner";
                UserSearchTextBox.Text = "owner";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemActiveBorder")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.presence != "away").ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["active"];
                TitleMemberListTypeTextBlock.Text = "Active";
                UserSearchTextBox.Text = "active";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemBotsBorder")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.IsSlackBot == true).ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["bots"];
                TitleMemberListTypeTextBlock.Text = "Bots";
                UserSearchTextBox.Text = "bots";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemDeletedBorder")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.deleted == true).ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["deleted"];
                TitleMemberListTypeTextBlock.Text = "Deleted";
                UserSearchTextBox.Text = "deleted";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemAwayBorder")
            {
                userviewmodel.UsersList = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.presence == "away").ToList());
                TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["away"];
                TitleMemberListTypeTextBlock.Text = "Away";
                UserSearchTextBox.Text = "away";
            }

            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Collapsed;
            UsersListLiveView.DataContext = userviewmodel.UsersList;
        }

        /// <summary>
        /// This is just a simple refresh button to force downloading of fresh data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SlackLogoImage_Tapped(object sender, TappedRoutedEventArgs e)
        {

            //No Network Connections
            bool hasNetworkConnection = NetworkInterface.GetIsNetworkAvailable();

            if (hasNetworkConnection == true)
            {
                await userviewmodel.PopulateUsers(false);
                UsersListLiveView.DataContext = userviewmodel.UsersList;
            }
        }
    }
}
