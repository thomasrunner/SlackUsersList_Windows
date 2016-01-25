///
/// Developed By Thomas Lock
/// Email: tlock@fhotoroom.com
/// Project: SlackUsersList
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

/// <summary>
//Slack User Profile Data Model child of the User Data Model
/// </summary>
namespace SlackUsersList_Windows.Model
{
    public class UserProfile
    {
        public string first_name {get; set;}
        public string last_name { get; set; }
        public string real_name { get; set; }
        public string title { get; set; }
        public string email { get; set; }
        public string skype { get; set; }
        public string phone { get; set; }
        public string image_24 { get; set; }
        public string image_32 { get; set; }
        public string image_48 { get; set; }
        public string image_72 { get; set; }
        public string image_192 { get; set; }

        /// <summary>
        /// Since some values can be either "" or null it is safe to initial here to prevent null return values;
        /// </summary>
        public UserProfile()
        {
            this.first_name = "";
            this.last_name = "";
            this.real_name = "";
            this.title = "";
            this.email = "";
            this.skype = "";
            this.phone = "";
            this.image_24 = "";
            this.image_32 = "";
            this.image_48 = "";
            this.image_72 = "";
            this.image_192 = "";
        }

    }
}
