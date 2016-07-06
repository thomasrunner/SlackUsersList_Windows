///
/// Developed By Thomas Lock
/// Email: tlock@fhotoroom.com
/// Project: SlackUsersList
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
    [DataContract]
    public class User
    {
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// Slack bot is a special user account on the Slack Network
        /// </summary>
        [IgnoreDataMember]
        public bool IsSlackBot
        {
            get
            {
                return id.Equals("USLACKBOT", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public bool deleted { get; set; }

        [DataMember]
        public string color { get; set; }

        [DataMember]
        public UserProfile profile { get; set; }

        [DataMember]
        public bool is_admin { get; set; }

        [DataMember]
        public bool is_owner { get; set; }

        [DataMember]
        public bool has_files { get; set; }

        [DataMember]
        public bool has_2fa { get; set; }

        [DataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
        public SolidColorBrush UserPresense
        {
            get
            {
                if (deleted == false)
                {
                    if (presence == SlackConstants.AWAYSTATUS)
                    {
                        return SlackConstants.getColor(SlackConstants.AWAYSTATUS);
                    }
                    else if (is_admin == true)
                    {
                        return SlackConstants.getColor(SlackConstants.ADMINSTATUS);
                    }
                    else if (is_owner == true)
                    {
                        return SlackConstants.getColor(SlackConstants.OWNERSTATUS);
                    }
                    else if (IsSlackBot == true)
                    {
                        return SlackConstants.getColor(SlackConstants.BOTSSTATUS);
                    }
                    else
                    {
                        return SlackConstants.getColor(SlackConstants.ACTIVESTATUS);
                    }
                }
                else
                {
                    return SlackConstants.getColor(SlackConstants.DELETEDSTATUS);
                }

            }
        }


        /// Nice little visual indicator to show the different types of users and the user status, specific for user profile
        [IgnoreDataMember]
        public SolidColorBrush UserPresenceExcludingAwayStatusColor
        {
            get
            {
                if (deleted == false)
                {
                    if (is_admin == true)
                    {
                        return SlackConstants.getColor(SlackConstants.ADMINSTATUS);
                    }
                    else if (is_owner == true)
                    {
                        return SlackConstants.getColor(SlackConstants.OWNERSTATUS);
                    }
                    else if (IsSlackBot == true)
                    {
                        return SlackConstants.getColor(SlackConstants.BOTSSTATUS);
                    }
                    else
                    {
                        return SlackConstants.getColor(SlackConstants.ACTIVESTATUS);
                    }
                }
                else
                {
                    return SlackConstants.getColor(SlackConstants.DELETEDSTATUS);
                }

            }
        }
    }
}
