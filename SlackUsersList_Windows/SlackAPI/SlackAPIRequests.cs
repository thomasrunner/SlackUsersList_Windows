///
/// Developed By Thomas Lock
/// Email: tlock@fhotoroom.com
/// Project: SlackUsersList
/// 



using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

/// General purpose Slack API layer
namespace SlackUsersList.SlackAPI
{
    public class SlackAPIRequests
    {
        //  Any additional url parameters can be added to API url call using the Dictionary: EXCEPT TOKEN!
        //  Token is required for all calls as part of the request call.
        private Dictionary<String, String> urlparameters = new Dictionary<string, string>();

        public void ClearUrlParameters()
        {
            if (urlparameters.Count > 0) urlparameters.Clear();
        }

        public void AddUrlParameter(String Key, String Value)
        {
            if (Key.Length > 0 && Value.Length > 0)
            {
                urlparameters.Add(Key, Value);
            }
        }

        //  Actual API call Task, the User Token must be added as part of this call and not in the dictionary.
        public async Task<String> SlackAPIRequest(string token, string apiurl)
        {
            if (token.Length == 0) return "Error: No token provided.";
            if (apiurl.Length == 0) return "Error: No api url provided.";

            bool hasNetworkConnection = NetworkInterface.GetIsNetworkAvailable();

            if (hasNetworkConnection == true)
            {
                StringBuilder urlrequeststring = new StringBuilder();

                urlrequeststring.Clear();
                urlrequeststring.Append(apiurl);
                urlrequeststring.Append("?token=");
                urlrequeststring.Append(token);

                if (urlparameters.Count > 0)
                {
                    foreach (var para in urlparameters)
                    {
                        urlrequeststring.Append("&");
                        urlrequeststring.Append(para.Key);
                        urlrequeststring.Append("=");
                        urlrequeststring.Append(para.Value);
                    }
                }

                HttpClient httpClient = new HttpClient();
                var content = await httpClient.GetAsync(new Uri(urlrequeststring.ToString(), UriKind.Absolute));
                var contentstring = await content.Content.ReadAsStringAsync();

                //Parameters are always cleared after present call is finished to ensure to overlap of parameters if same object is used to make
                //multiple calls.
                if (urlparameters.Count > 0) urlparameters.Clear();
                return contentstring;
            }
            else
            {
                return "Connection Error: Please check network connection.";
            }
        }

    }
}