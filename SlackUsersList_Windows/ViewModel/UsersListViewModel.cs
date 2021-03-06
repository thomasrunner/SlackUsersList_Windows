﻿///
/// Developed By Thomas Lock
/// Email: tlock@fhotoroom.com
/// Project: SlackUsersList
/// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackUsersList_Windows.Model;
using System.Runtime.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO;
using System.Net;
using SlackUsersList.SlackAPI;
using System.ComponentModel;

/// <summary>
/// This ViewModel uses the User Model and UsersListView View
/// Responsibilities is to Connect to App SlackAPI layer, parse the JSON results and build List of Users to be presented.
/// </summary> 
namespace SlackUsersList_Windows.ViewModel
{
    public class UsersListViewModel : INotifyPropertyChanged
    {
        static string USERLISTFILENAME = "users.json";

        private List<User> teamfulllist;
        private List<User> teamlist;

        public event PropertyChangedEventHandler PropertyChanged;


        public List<User> TeamUsersList
        {
            get { return teamlist; }
        }

        //select a specific user from existing selected user list
        public User SelectTeamUser(string userid)
        {
            if (userid == "") return null;
            return new List<User>(teamlist.Where(x => x.id == userid))[0];
        }

        //Indexer Method
        public User this[string userid]
        {
            get
            {
                if (userid == "") return null;
                return new List<User>(teamlist.Where(x => x.id == userid))[0];
            }
        }

        //filter team list
        public void FilterTeamList(string filtername)
        {
            filtername = filtername.ToLower();
            if (filtername == SlackConstants.ALLSTATUS)
            {
                teamlist = new List<User>(teamfulllist);
            }
            else if (filtername == SlackConstants.ADMINSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.is_admin == true).ToList());
            }
            else if (filtername == SlackConstants.OWNERSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.is_owner == true).ToList());
            }
            else if (filtername == SlackConstants.ACTIVESTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.presence != SlackConstants.AWAYSTATUS).ToList());
            }
            else if (filtername == SlackConstants.BOTSSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.IsSlackBot == true).ToList());
            }
            else if (filtername == SlackConstants.DELETEDSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.deleted == true).ToList());
            }
            else if (filtername == SlackConstants.AWAYSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.presence == SlackConstants.AWAYSTATUS).ToList());
            }

            //Update UI with changes
            OnPropertyChanged("TeamUsersList");

        }

        // search for team member(s)
        public void SearchTeamList(string searchstring)
        {
            searchstring = searchstring.ToLower();
            if (searchstring == SlackConstants.ALLSTATUS)
            {
                teamlist = new List<User>(teamfulllist);
            }
            else if (searchstring == SlackConstants.ADMINSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.is_admin == true).ToList());
            }
            else if (searchstring == SlackConstants.OWNERSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.is_owner == true).ToList());
            }
            else if (searchstring == SlackConstants.ACTIVESTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.presence != SlackConstants.AWAYSTATUS).ToList());
            }
            else if (searchstring == SlackConstants.BOTSSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.IsSlackBot == true).ToList());
            }
            else if (searchstring == SlackConstants.DELETEDSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.deleted == true).ToList());
            }
            else if (searchstring == SlackConstants.AWAYSTATUS)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.presence == SlackConstants.AWAYSTATUS).ToList());
            }
            else if (searchstring.Trim().Length > 0)
            {
                teamlist = new List<User>(teamfulllist.Where(x => x.name.ToLower().StartsWith(searchstring.ToLower()) || x.profile.first_name.ToLower().StartsWith(searchstring.ToLower()) || x.profile.title.ToLower().StartsWith(searchstring.ToLower())).ToList());
            }
            else
            {
                teamlist = new List<User>(teamfulllist);
            }

            //Update UI with changes
            OnPropertyChanged("TeamUsersList");
        }

        /// public function to populate the user list, with the option to check locally (if exists) otherwise a fresh copy is pulled from Slack.
        public async Task PopulateUsers(bool loadlocalcopy)
        {
            if (loadlocalcopy == true)
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                bool isFile = await FileExists(local, USERLISTFILENAME);

                if (isFile == true)
                {
                    String savedjsonstring = await readJSONStringFromLocalFile(USERLISTFILENAME);
                    ParseJSONStringData(savedjsonstring);
                }
            }
            else
            {
                await DownloadData();

                if (teamlist == null)
                {
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    bool isFile = await FileExists(local, USERLISTFILENAME);

                    if (isFile == true)
                    {
                        String savedjsonstring = await readJSONStringFromLocalFile(USERLISTFILENAME);
                        ParseJSONStringData(savedjsonstring);
                    }
                }
            }

            return;
        }

        /// Checks if the file exists before trying to load it.
        private static async Task<bool> FileExists(StorageFolder folder, string fileName)
        {
            try { StorageFile file = await folder.GetFileAsync(fileName); }
            catch { return false; }
            return true;
        }

        /// This is the specific download request for the user list from Slack and saves it to the load storage for quick reloading if internet is not available, Ideally this is would have been stored in database
        private async Task DownloadData()
        {
            SlackAPIRequests slackapirequests = new SlackAPIRequests();
            slackapirequests.AddUrlParameter("presence", "1");

            //In a more complete solution I would put token as part of a logged in User Security Class, this allows for easier profile switching and portability as it can also be saved in the 
            //users roaming profile for Windows 8.1/10 or other Windows Phones. HERE IS JUST A SAMPLE TOKEN JUST REPLACE WITH YOUR OWN TEAMS TOKEN TO PLAY WITH
            String contentstring = await slackapirequests.SlackAPIGETRequest("xoxp-5048173296-5048487710-19045732087-b5427e3b46", "https://slack.com/api/users.list");

            if (contentstring.StartsWith("Error: No ") == false && contentstring.StartsWith("Connection Error: ") == false)
            {
                ParseJSONStringData(contentstring);
            }
            else
            {
                //If error occurs with token they need to be handled here, such as re-authentication, if connection error, the users has already been notified about this issue with a popup.
            }
        }

        /// Parse the JSON string for this specific class and populated UsersList List
        private async void ParseJSONStringData(string jsonstring)
        {
            //Some values can be either "" or null
            try
            {
                JToken token = JObject.Parse(jsonstring);
                var ok = token.SelectToken("ok");
                var members = token.SelectToken("members").ToArray();

                if (ok.ToString().ToLower() == "true")
                {
                    List<User> a = new List<User>();

                    foreach (var member in members)
                    {
                        User newmember = new User();

                        newmember.id = member.SelectToken("id").ToString();
                        newmember.name = member.SelectToken("name").ToString();

                        if (member.SelectToken(SlackConstants.DELETEDSTATUS) != null)
                        {
                            if (member.SelectToken(SlackConstants.DELETEDSTATUS).ToString().ToLower() == "true")
                            {
                                newmember.deleted = true;
                            }
                        }

                        if (member.SelectToken("is_admin") != null)
                        {
                            if (member.SelectToken("is_admin").ToString().ToLower() == "true")
                            {
                                newmember.is_admin = true;
                            }
                        }

                        if (member.SelectToken("is_owner") != null)
                        {
                            if (member.SelectToken("is_owner").ToString().ToLower() == "true")
                            {
                                newmember.is_owner = true;
                            }
                        }

                        if (member.SelectToken("has_files") != null)
                        {
                            if (member.SelectToken("has_files") != null)
                            {
                                if (member.SelectToken("has_files").ToString().ToLower() == "true")
                                {
                                    newmember.has_files = true;
                                }
                            }
                        }

                        if (member.SelectToken("has_2fa") != null)
                        {
                            if (member.SelectToken("has_2fa").ToString().ToLower() == "true")
                            {
                                newmember.has_2fa = true;
                            }
                        }

                        if (member.SelectToken("presence") != null)
                        {
                            if (member.SelectToken("presence").ToString() != "")
                            {
                                newmember.presence = member.SelectToken("presence").ToString(); ;
                            }
                        }

                        //User Profile Class which is a subclass of User
                        UserProfile newmemberprofile = new UserProfile();
                        JToken profilejtoken = member.SelectToken("profile");

                        if (profilejtoken.SelectToken("first_name") != null)
                        {
                            newmemberprofile.first_name = profilejtoken.SelectToken("first_name").ToString();
                        }

                        if (profilejtoken.SelectToken("last_name") != null)
                        {
                            newmemberprofile.last_name = profilejtoken.SelectToken("last_name").ToString();
                        }

                        if (profilejtoken.SelectToken("real_name") != null)
                        {
                            newmemberprofile.real_name = profilejtoken.SelectToken("real_name").ToString();
                        }

                        if (profilejtoken.SelectToken("title") != null)
                        {
                            newmemberprofile.title = profilejtoken.SelectToken("title").ToString();
                        }

                        if (profilejtoken.SelectToken("email") != null)
                        {
                            newmemberprofile.email = profilejtoken.SelectToken("email").ToString();
                        }

                        if (profilejtoken.SelectToken("skype") != null)
                        {
                            newmemberprofile.skype = profilejtoken.SelectToken("skype").ToString();
                        }

                        if (profilejtoken.SelectToken("phone") != null)
                        {
                            newmemberprofile.phone = profilejtoken.SelectToken("phone").ToString();
                        }

                        if (profilejtoken.SelectToken("image_24") != null)
                        {
                            newmemberprofile.image_24 = profilejtoken.SelectToken("image_24").ToString();
                        }

                        if (profilejtoken.SelectToken("image_32") != null)
                        {
                            newmemberprofile.image_32 = profilejtoken.SelectToken("image_32").ToString();
                        }

                        if (profilejtoken.SelectToken("image_48") != null)
                        {
                            newmemberprofile.image_48 = profilejtoken.SelectToken("image_48").ToString();
                        }

                        if (profilejtoken.SelectToken("image_72") != null)
                        {
                            newmemberprofile.image_72 = profilejtoken.SelectToken("image_72").ToString();
                        }

                        if (profilejtoken.SelectToken("image_192") != null)
                        {
                            newmemberprofile.image_192 = profilejtoken.SelectToken("image_192").ToString();
                        }

                        newmember.profile = newmemberprofile;

                        a.Add(newmember);

                    }

                    teamlist = a;

                    teamfulllist = a;

                    //Save Good Parsable results only
                    if (teamlist != null)
                    {
                        if (teamlist.Count > 0)
                        {
                            await saveJSONStringToLocalFile(USERLISTFILENAME, jsonstring);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        /// Saves the valid JSON string response to the local storage
        private async Task saveJSONStringToLocalFile(string filename, string jsonstring)
        {
            // Saves the string 'jsonstring' to a file 'filename' in the app's local storage folder
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(jsonstring.ToCharArray());

            // Create a file with the given filename in the local folder; replace any existing file with the same name
            StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            // Write the char array created from the content string into the file
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        /// Reads the locally JSON string copy of users.list response for parsing
        private static async Task<string> readJSONStringFromLocalFile(string filename)
        {
            // reads the contents of file 'filename' in the app's local storage folder and returns it as a string

            // access the local folder
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            // open the file 'filename' for reading
            Stream stream = await local.OpenStreamForReadAsync(filename);
            string jsonstring;

            // copy the file contents into the string 'text'
            using (StreamReader reader = new StreamReader(stream))
            {
                jsonstring = reader.ReadToEnd();
            }

            return jsonstring;
        }


        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
