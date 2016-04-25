using CloudClipboard.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace CloudClipboard.Services
{
    public class PushNotificationService
    {
        public event EventHandler<PushNotificationReceivedEventArgs> NotificationReceived;
        public async Task<AuthenticationResult> CreateInstallation()
        {
            var deviceInstallation = await GenerateInstallation();

            string json = JsonConvert.SerializeObject(deviceInstallation);
            var result = await App.MobileService.InvokeApiAsync("createorupdate", HttpMethod.Post, new Dictionary<string, string>() { { "input", json } });
            var messageDialog = new MessageDialog("test");
            App.DeviceTag = deviceInstallation.InstallationId;
                       
            return ConvertToAuthenticationResult(result);
        }

        private AuthenticationResult ConvertToAuthenticationResult(JToken incomingData)
        {
            AuthenticationResult result = JsonConvert.DeserializeObject<AuthenticationResult>(incomingData.Root.ToString());
            
            
            return result; 
        }

        private async Task<DeviceInstallation> GenerateInstallation(string groupName = null)
        {
            App.Channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            App.Channel.PushNotificationReceived += (s, e) =>
            {
                NotificationReceived?.Invoke(s, e);
            };

            string installationId = null;
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("nhInstallationID"))
            {
                installationId = Guid.NewGuid().ToString();
                ApplicationData.Current.LocalSettings.Values.Add("nhInstallationID", installationId);
            }
            else
                installationId = (string)ApplicationData.Current.LocalSettings.Values["nhInstallationID"];

            string userTag = $"{App.User.SID}";
            string deviceTag = $"{installationId}";
            //string userTag = "testtag";

            var deviceInstallation = new DeviceInstallation
            {
                InstallationId = installationId,
                Platform = "wns",
                PushChannel = App.Channel.Uri,
                Tags = groupName == null ? new string[] { userTag, deviceTag } : new string[] { userTag, deviceTag, groupName }
            };

            App.DeviceInstallation = deviceInstallation;
            return deviceInstallation;
        }

        public async Task<AuthenticationResult> AddGroupTagToDeviceInstallation(string newGroupName = null, string oldGroupName = null)
        {
            DeviceInstallation deviceInstallation;
            deviceInstallation =
                newGroupName != null
                ? await GenerateInstallation(groupName: newGroupName)
                : await GenerateInstallation();
            string json = JsonConvert.SerializeObject(deviceInstallation);
            Newtonsoft.Json.Linq.JToken result;
            var apiParameters = new Dictionary<string, string>();
            apiParameters.Add("input", json);
            if (oldGroupName != null)
                apiParameters.Add("oldShareDeviceGroupName", oldGroupName);
            if (newGroupName != null)
                apiParameters.Add("shareDeviceGroupName", newGroupName);

            result = await App.MobileService.InvokeApiAsync("createorupdate", HttpMethod.Post, apiParameters);

            return ConvertToAuthenticationResult(result);
        }


    }
}
