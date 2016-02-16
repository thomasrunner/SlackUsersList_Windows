using SlackUsersList_Windows;
using SlackUsersList_Windows.ViewModel;
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
        private UsersListViewModel teamuserlist;
        private User selecteduser;
        private SlackConstants slackconstants;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            teamuserlist = new UsersListViewModel();
            slackconstants = new SlackConstants();

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            //These are the little color indicator in the Right Panel which appears when clicking the top right side user status button.
            RightPanelAdminTypeStatusBorder.Background = slackconstants.getColor("admin");
            RightPanelOwnerTypeStatusBorder.Background = slackconstants.getColor("owner");
            RightPanelActiveStatusBorder.Background = slackconstants.getColor("active");
            RightPanelBotStatusBorder.Background = slackconstants.getColor("bots");
            RightPanelDeletedStatusBorder.Background = slackconstants.getColor("deleted");
            RightPanelAwayStatusBorder.Background = slackconstants.getColor("away");

            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Collapsed;

            
            //Network Connection Check
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //If the page has no data try downloading
            if (UsersListLiveView.DataContext == null)
            {
                //This async but do not block the page loading
                await teamuserlist.PopulateUsers(false);

                UsersListLiveView.DataContext = teamuserlist;
            }

            //This is just a data page filler and should use the loggedin user account in a more complete solution.
            if (teamuserlist != null)
            {

                User user = new List<User>(teamuserlist.TeamUsersList.Where(x => x.is_admin == true || x.is_owner == true))[0];

                MainPanelProfileNameTextBlock.Text = user.NamewithAtSymbol;

                MainPanelProfileStatusIndicatorBorder.Background = user.UserPresenceExcludingAwayStatusColor;
                if (user.presence == "away")
                {
                    MainPanelProfileStatusTextBlock.Text = "away";
                }
                ImageBrush imagebrush = new ImageBrush();
                imagebrush.ImageSource = user.image_192;
                MainPanelProfilePhotoBorder.Background = imagebrush;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Collapsed;
            
        }

        /// This allows users to search a specific user by both profile name and first name as well as use some key words to quick filter large list
        /// KEY WORDS (admin, away, owner, bots, active, deleted, "" = all)
        /// ADDITION KEY WORDS (marketing, android, ios, customer, ceo, cfo, cto, wp, server, etc...) <- Excellent for larger teams
        private void UserSearchTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox usersearchtextbox = (TextBox)sender;
            String searchstring = usersearchtextbox.Text.ToLower();

            teamuserlist.SearchTeamList(searchstring);

            //Update UI to reflect changes.
            if (searchstring == "admin")
            {
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("admin");
                TitleMemberListTypeTextBlock.Text = "Admin";
            }
            else if (searchstring == "owner")
            {
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("owner");
                TitleMemberListTypeTextBlock.Text = "Owner";
            }
            else if (searchstring == "bots")
            {
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("bots");
                TitleMemberListTypeTextBlock.Text = "Bots";
            }
            else if (searchstring == "active")
            {
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("active");
                TitleMemberListTypeTextBlock.Text = "Active";
            }
            else if (searchstring == "deleted")
            {
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("deleted");
                TitleMemberListTypeTextBlock.Text = "Deleted";
            }
            else if (searchstring == "all")
            {
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("all");
                TitleMemberListTypeTextBlock.Text = "All";
            }
            else if (searchstring == "away")
            {
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("away");
                TitleMemberListTypeTextBlock.Text = "Away";
            }
            else if (searchstring.Trim().Length > 0)
            {
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("all");
                TitleMemberListTypeTextBlock.Text = "All";
            }
            else
            {
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("all");
                TitleMemberListTypeTextBlock.Text = "All";
            }
        }

        /// Placeholder Function to help users understand this is a search field.
        private void UserSearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserSearchTextBox.Text == "search (name, firstname or role)")
            {
                UserSearchTextBox.Text = "";
            }
            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Collapsed;
        }

        private void UserSearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UserSearchTextBox.Text.Trim().Length == 0)
            {
                UserSearchTextBox.Text = "search (name, firstname or role)";
            }
        }

        /// These next 2 functions are just a fun colourful addition for users to filter based on user status. This shows the menu
        /// and when a stack item is selected I used the item name to apply the search as well as teach user some keyword search tricks
        /// by putting the word into the search box, subtle but very effective over time.
        private void TitleStatusTypeButtonBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Visible;
        }

        private void RightPanelUserStatusTypeStackItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Border StatusTypeItem = (Border)sender;
            if (StatusTypeItem.Name == "RightPanelStatusTypeItemAllBorder")
            {
                teamuserlist.FilterTeamList("all");
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("all");
                TitleMemberListTypeTextBlock.Text = "All";
                UserSearchTextBox.Text = "all";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemAdminBorder")
            {
                teamuserlist.FilterTeamList("admin");
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("admin");
                TitleMemberListTypeTextBlock.Text = "Admin";
                UserSearchTextBox.Text = "admin";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemOwnerBorder")
            {
                teamuserlist.FilterTeamList("owner");
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("owner");
                TitleMemberListTypeTextBlock.Text = "Owner";
                UserSearchTextBox.Text = "owner";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemActiveBorder")
            {
                teamuserlist.FilterTeamList("active");
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("active");
                TitleMemberListTypeTextBlock.Text = "Active";
                UserSearchTextBox.Text = "active";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemBotsBorder")
            {
                teamuserlist.FilterTeamList("bots");
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("bots");
                TitleMemberListTypeTextBlock.Text = "Bots";
                UserSearchTextBox.Text = "bots";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemDeletedBorder")
            {
                teamuserlist.FilterTeamList("deleted");
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("deleted");
                TitleMemberListTypeTextBlock.Text = "Deleted";
                UserSearchTextBox.Text = "deleted";
            }
            else if (StatusTypeItem.Name == "RightPanelStatusTypeItemAwayBorder")
            {
                teamuserlist.FilterTeamList("away");
                TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("away");
                TitleMemberListTypeTextBlock.Text = "Away";
                UserSearchTextBox.Text = "away";
            }

            RightPanelUserStatusTypeListBorder.Visibility = Visibility.Collapsed;
        }

        /// This is just a simple refresh button to force downloading of fresh data.
        private async void ReloadUserListBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Network Connections Check
            bool hasNetworkConnection = NetworkInterface.GetIsNetworkAvailable();

            if (hasNetworkConnection == true)
            {
                await teamuserlist.PopulateUsers(false);
                UsersListLiveView.DataContext = teamuserlist;
                teamuserlist.FilterTeamList("all");
                
            }

            //Resets the search UI elements
            TitleMemberListTypeStatusBorder.Background = slackconstants.getColor("all");
            TitleMemberListTypeTextBlock.Text = "All";
            UserSearchTextBox.Text = "search (name, firstname or role)";
        }

        private void UserProfilePhoneBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Add Code to Launch Phone in UWA
        }

        /// Launches email app with email of selected user. This option is only visible if the selected user has an email address
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
            //Layout filler code that should launch the profile edit but when the active user is looking at their own profile.
        }


        // This event gets called when swiping item to the left
        async void UsersListLiveView_SwipeCallUser(object sender, EventArgs e)
        {
            User user = teamuserlist.SelectTeamUser(UsersListLiveView.SelectUserID);

            if (user == null) return;
            if (user.profile.skype == "") return;

            //launch skype
            var skypeto = new Uri("skype:" + user.profile.skype + "?call");
            await Windows.System.Launcher.LaunchUriAsync(skypeto);

        }

        // This even gets called when swiping item to the right
        async void UsersListLiveView_SwipeEmailUser(object sender, EventArgs e)
        {
            User user = teamuserlist.SelectTeamUser(UsersListLiveView.SelectUserID);

            if (user == null) return;
            if (user.profile.email == "") return;

            var mailto = new Uri("mailto:" + user.profile.email);
            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }

        /// This event gets triggered when an item is selected from the UsersListView View ListView.
        private async void UsersListLiveView_SelectedUserListViewItem(object sender, EventArgs e)
        {
            //Ideally this should be some sort of local database
            selecteduser = teamuserlist.SelectTeamUser(UsersListLiveView.SelectUserID);

            if (selecteduser == null) return;

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

            //Network Connection Check, data will still load so this is simply a warning not an error.
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
