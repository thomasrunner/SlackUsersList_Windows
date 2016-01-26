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
using Windows.UI;
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
        private User selecteduser;

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

            //This is just a data page filler and should use the loggedin user account in a more complete solution.
            if((App.Current as App).localuserlistcollection != null)
            {
                if((App.Current as App).localuserlistcollection.Count > 0)
                {
                    User user = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.is_admin == true || x.is_owner == true))[0];

                    MainPanelProfileNameTextBlock.Text = user.NamewithAtSymbol;

                    MainPanelProfileStatusIndicatorBorder.Background = user.UserPresenceExcludingAwayStatusColor;
                    if(user.presence == "away")
                    {
                        MainPanelProfileStatusTextBlock.Text = "away";
                    }
                    ImageBrush imagebrush = new ImageBrush();
                    imagebrush.ImageSource = user.image_192;
                    MainPanelProfilePhotoBorder.Background = imagebrush;
                }
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
        private async void ReloadUserListBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Resets the search UI elements
            UserSearchTextBox.Text = "search";
            TitleMemberListTypeStatusBorder.Background = (App.Current as App).usercolorstatusdict["all"];
            TitleMemberListTypeTextBlock.Text = "All";
            UserSearchTextBox.Text = "all";

            //No Network Connections
            bool hasNetworkConnection = NetworkInterface.GetIsNetworkAvailable();

            if (hasNetworkConnection == true)
            {
                await userviewmodel.PopulateUsers(false);
                UsersListLiveView.DataContext = userviewmodel.UsersList;
            }
        }

        private void UserProfilePhoneBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private async void UserProfileEmailBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(selecteduser != null)
            { 
                var mailto = new Uri("mailto:" + selecteduser.profile.email);
                await Windows.System.Launcher.LaunchUriAsync(mailto);
            }
        }

        private void UserProfileEditButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }


        private async void UsersListLiveView_SelectedUserListViewItem(object sender, EventArgs e)
        {
            //Ideally this should be some sort of local database

            selecteduser = new List<User>((App.Current as App).localuserlistcollection.Where(x => x.id == UsersListLiveView.SelectUserID))[0];

            //Profile Title
            UserProfileTitleNamewithAtTextBlock.Text = selecteduser.NamewithAtSymbol;

            //Name Under Photo
            UserProfileNameBelowPhotoTextBlock.Text = selecteduser.RealNamewithNameFailover;

            //Presence Symbol Color
            UserProfileStatusIndicatorBorder.Background = selecteduser.UserPresenceExcludingAwayStatusColor;

            //Presence Symbol "Z"
            if (selecteduser.presence == "away")
            {
                UserProfileZStatusIndicatorBorder.Visibility = Visibility.Visible;
                UserProfileZStatusIndicatorTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                UserProfileZStatusIndicatorBorder.Visibility = Visibility.Collapsed;
                UserProfileZStatusIndicatorTextBox.Visibility = Visibility.Collapsed;
            }

            //Role
            UserProfileRoleTextBlock.Text = selecteduser.profile.title;

            //Message Button
            UserProfileMessageTextBlock.Text = "Message " + selecteduser.NamewithAtSymbol;

            //Email Button
            if (selecteduser.profile.email.Length > 0)
            {
                UserProfileEmailBorder.Visibility = Visibility.Visible;
                UserProfileEmailTextBlock.Text = selecteduser.profile.email;
            }
            else
            {
                UserProfileEmailBorder.Visibility = Visibility.Collapsed;
            }

            //Skype Button
            if (selecteduser.profile.skype.Length > 0)
            {
                UserProfileSkypeBorder.Visibility = Visibility.Visible;
                UserProfileSkypeTextBlock.Text = selecteduser.profile.skype;
            }
            else
            {
                UserProfileSkypeBorder.Visibility = Visibility.Collapsed;
            }

            //Phone Button
            if (selecteduser.profile.phone.Length > 0)
            {
                UserProfilePhoneBorder.Visibility = Visibility.Visible;
                UserProfilePhoneTextBlock.Text = selecteduser.profile.phone;
            }
            else
            {
                UserProfilePhoneBorder.Visibility = Visibility.Collapsed;
            }

            //Profile Photo
            ImageBrush imagebrush = new ImageBrush();
            imagebrush.ImageSource = selecteduser.image_192;
            UserProfilePhotoBorder.Background = imagebrush;

            //IsSlackBot
            if (selecteduser.IsSlackBot == true)
            {
                UserProfileNameBelowPhotoTextBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
            }
            else
            {
                UserProfileNameBelowPhotoTextBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 170, 52, 255));
            }

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

            UserProfileBorder.Visibility = Visibility.Visible;
        }

        private void UserProfileBackArrowBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UserProfileBorder.Visibility = Visibility.Collapsed;
        }
    }
}
