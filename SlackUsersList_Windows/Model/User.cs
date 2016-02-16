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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

/// <summary>
//  Slack User Data Model parent of the UserProfile Data Model
/// </summary>
namespace SlackUsersList_Windows.Model
{
    public class User
    {
        private SlackConstants slackconstants = new SlackConstants();

        public string id { get; set; }

        /// <summary>
        /// Slack bot is a special user account on the Slack Network
        /// </summary>
        public bool IsSlackBot
        {
            get
            {
                return id.Equals("USLACKBOT", StringComparison.CurrentCultureIgnoreCase);
            }
        }
        public string name { get; set; }
        public bool deleted { get; set; }
        public string color { get; set; }
        public UserProfile profile { get; set; }
        public bool is_admin { get; set; }
        public bool is_owner { get; set; }
        public bool has_files { get; set; }
        public bool has_2fa { get; set; }
        public string presence { get; set; }

        /// <summary>
        /// Since some values can be either "" or null it is safe to initial here to prevent null return values;
        /// </summary>
        public User()
        {
            this.id = "";
            this.name = "";
            this.name = "";
            this.deleted = false;
            this.color = "";
            this.is_admin = false;
            this.is_owner = false;
            this.has_files = false;
            this.has_2fa = false;
            this.presence = "";
        }

        /// <summary>
        /// This function loads only the image_24 when needed rather than when object is loaded and passes bitmap to the UI.
        /// </summary>
        public ImageSource image_24
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();
                if (this.profile.image_24.Length > 0)
                {
                    try
                    {
                        bitmap = new BitmapImage(new Uri(this.profile.image_24, UriKind.RelativeOrAbsolute));
                        return bitmap;
                    }
                    catch { }
                }

                return null;
            }
        }

        /// <summary>
        /// This function loads only the image_32 when needed rather than when object is loaded and passes bitmap to the UI.
        /// </summary>
        public ImageSource image_32
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();
                if (this.profile.image_32.Length > 0)
                {
                    try
                    {
                        bitmap = new BitmapImage(new Uri(this.profile.image_32, UriKind.RelativeOrAbsolute));
                        return bitmap;
                    }
                    catch { }
                }

                return null;
            }
        }

        /// <summary>
        /// This function loads only the image_48 when needed rather than when object is loaded and passes bitmap to the UI.
        /// </summary>
        public ImageSource image_48
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();
                if (this.profile.image_48.Length > 0)
                {
                    try
                    {
                        bitmap = new BitmapImage(new Uri(this.profile.image_48, UriKind.RelativeOrAbsolute));
                        return bitmap;
                    }
                    catch { }
                }

                return null;
            }
        }

        /// <summary>
        /// This function loads only the image_72 when needed rather than when object is loaded and passes bitmap to the UI.
        /// </summary>
        public ImageSource image_72
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();
                if (this.profile.image_72.Length > 0)
                {
                    try
                    {
                        bitmap = new BitmapImage(new Uri(this.profile.image_72, UriKind.RelativeOrAbsolute));
                        return bitmap;
                    }
                    catch { }
                }

                return null;
            }
        }

        /// <summary>
        /// This function loads only the image_192 when needed rather than when object is loaded and passes bitmap to the UI.
        /// </summary>
        public ImageSource image_192
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();
                if (this.profile.image_192.Length > 0)
                {
                    try
                    {
                        bitmap = new BitmapImage(new Uri(this.profile.image_192, UriKind.RelativeOrAbsolute));
                        return bitmap;
                    }
                    catch { }
                }

                return null;
            }
        }

        /// <summary>
        /// Simple cosmetic formatting function
        /// </summary>
        public string NamewithAtSymbol
        {
            get
            {
                return "@" + name;
            }
        }

        /// <summary>
        /// Simple cosmetic option to show email icon in list using Binding.
        /// </summary>
        public Visibility IsEmailAvailable
        {
            get
            {
                if (profile.email.Length > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Simple cosmetic option to show skype icon in list using Binding.
        /// </summary>
        public Visibility IsSkypeAvailable
        {
            get
            {
                if (profile.skype.Length > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Simple cosmetic option to show skype icon in list using Binding.
        /// </summary>
        public Visibility IsPhoneAvailable
        {
            get
            {
                if (profile.phone.Length > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Since Title can be "" this allows for a failover
        /// </summary>
        public string TitlewithNameFailover
        {
            get
            {
                if(profile.title.Length > 0)
                {
                    return profile.title;
                }
                else
                {
                    if (IsSlackBot == false)
                    {
                        return NamewithAtSymbol;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }

        /// <summary>
        /// Since Title can be "" this allows for a failover
        /// </summary>
        public string RealNamewithNameFailover
        {
            get
            {
                if (profile.real_name.Length > 0)
                {
                    return profile.real_name;
                }
                else
                {
                    if (IsSlackBot == false)
                    {
                        return NamewithAtSymbol;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }

        /// Nice little visual indicator to show the different types of users and the user status
        public SolidColorBrush UserPresense
        {
            get
            {
                if (deleted == false)
                {
                    if (presence == "away")
                    {
                        return slackconstants.getColor("away");
                    }
                    else if (is_admin == true)
                    {
                        return slackconstants.getColor("admin");
                    }
                    else if (is_owner == true)
                    {
                        return slackconstants.getColor("owner");
                    }
                    else if (IsSlackBot == true)
                    {
                        return slackconstants.getColor("bots");
                    }
                    else
                    {
                        return slackconstants.getColor("active");
                    }
                }
                else
                {
                    return slackconstants.getColor("deleted");
                }

            }
        }


        /// Nice little visual indicator to show the different types of users and the user status, specific for user profile
        public SolidColorBrush UserPresenceExcludingAwayStatusColor
        {
            get
            {
                if (deleted == false)
                {
                    if (is_admin == true)
                    {
                        return slackconstants.getColor("admin");
                    }
                    else if (is_owner == true)
                    {
                        return slackconstants.getColor("owner");
                    }
                    else if (IsSlackBot == true)
                    {
                        return slackconstants.getColor("bots");
                    }
                    else
                    {
                        return slackconstants.getColor("active");
                    }
                }
                else
                {
                    return slackconstants.getColor("deleted");
                }

            }
        }
    }
}
