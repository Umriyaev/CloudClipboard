using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CloudClipboard.Services
{
    public class DeviceShareParameterService
    {
        public bool GetGroupName(out string groupName)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("groupName"))
            {
                groupName= (string)ApplicationData.Current.LocalSettings.Values["groupName"];
                return true;
            }
            groupName = null;
            return false;
        }

        public void SetGroupName(string groupName)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("groupName"))
                ApplicationData.Current.LocalSettings.Values.Add("groupName", null);
            ApplicationData.Current.LocalSettings.Values["groupName"] = groupName;

        }
    }
}
