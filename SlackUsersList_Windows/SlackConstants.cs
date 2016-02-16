using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SlackUsersList_Windows
{

    public class SlackConstants
    {
        //Global Color Values for different user statuses
        private Dictionary<string, SolidColorBrush> usercolorstatusdict;

        public SlackConstants()
        {
            //Setting custom color when app loads.
            if (usercolorstatusdict == null)
            {
                usercolorstatusdict = new Dictionary<string, SolidColorBrush>();
                usercolorstatusdict.Add("all", new SolidColorBrush(Color.FromArgb(255, 224, 224, 224)));
                usercolorstatusdict.Add("away", new SolidColorBrush(Color.FromArgb(255, 224, 224, 224)));
                usercolorstatusdict.Add("admin", new SolidColorBrush(Color.FromArgb(255, 255, 196, 32)));
                usercolorstatusdict.Add("owner", new SolidColorBrush(Color.FromArgb(255, 255, 128, 64)));
                usercolorstatusdict.Add("bots", new SolidColorBrush(Color.FromArgb(255, 64, 196, 255)));
                usercolorstatusdict.Add("deleted", new SolidColorBrush(Color.FromArgb(255, 255, 64, 64)));
                usercolorstatusdict.Add("active", new SolidColorBrush(Color.FromArgb(255, 64, 224, 128)));
            }
        }

        public SolidColorBrush getColor(string userstatus)
        {
            if (userstatus == "") return usercolorstatusdict["all"];
            SolidColorBrush requestcolor = usercolorstatusdict[userstatus];
            if (requestcolor != null)
            {
                return requestcolor;
            }
            else
            {
                return usercolorstatusdict["all"];
            }
        }
    }
}