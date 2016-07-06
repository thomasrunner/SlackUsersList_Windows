using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SlackUsersList_Windows
{

    public static class SlackConstants
    {
        //Global Color Values for different user statuses
        private static Dictionary<string, SolidColorBrush> usercolorstatusdict;

        public const string USERLISTFILENAME = "users.json"; 
        public const string ALLSTATUS = "all"; 
        public const string ADMINSTATUS = "admin"; 
        public const string OWNERSTATUS = "owner"; 
        public const string ACTIVESTATUS = "active"; 
        public const string BOTSSTATUS = "bots"; 
        public const string DELETEDSTATUS = "deleted"; 
        public const string AWAYSTATUS = "away"; 

        static SlackConstants()
        {
            //Setting custom color when app loads.
            if (usercolorstatusdict == null)
            {
                usercolorstatusdict = new Dictionary<string, SolidColorBrush>();
                usercolorstatusdict.Add(SlackConstants.ALLSTATUS, new SolidColorBrush(Color.FromArgb(255, 224, 224, 224)));
                usercolorstatusdict.Add(SlackConstants.AWAYSTATUS, new SolidColorBrush(Color.FromArgb(255, 224, 224, 224)));
                usercolorstatusdict.Add(SlackConstants.ADMINSTATUS, new SolidColorBrush(Color.FromArgb(255, 255, 196, 32)));
                usercolorstatusdict.Add(SlackConstants.OWNERSTATUS, new SolidColorBrush(Color.FromArgb(255, 255, 128, 64)));
                usercolorstatusdict.Add(SlackConstants.BOTSSTATUS, new SolidColorBrush(Color.FromArgb(255, 64, 196, 255)));
                usercolorstatusdict.Add(SlackConstants.DELETEDSTATUS, new SolidColorBrush(Color.FromArgb(255, 255, 64, 64)));
                usercolorstatusdict.Add(SlackConstants.ACTIVESTATUS, new SolidColorBrush(Color.FromArgb(255, 64, 224, 128)));
            }
        }

        public static SolidColorBrush getColor(string userstatus)
        {
            if (userstatus == "") return usercolorstatusdict[SlackConstants.ALLSTATUS];
            SolidColorBrush requestcolor = usercolorstatusdict[userstatus];
            if (requestcolor != null)
            {
                return requestcolor;
            }
            else
            {
                return usercolorstatusdict[SlackConstants.ALLSTATUS];
            }
        }
    }
}