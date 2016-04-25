using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using CloudClipboard.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace CloudClipboard.Services
{
    public class SyncService
    {
        public async Task<bool> SyncCopiedTextData(string textData, string userTag, string deviceTag, string deviceInfo, string joinedGroup = null)
        {
            //MobileServiceClient testClient = new MobileServiceClient("http://localhost:51802/");
            var apiParameters = new Dictionary<string, string>();
            apiParameters.Add("copiedData", textData);
            apiParameters.Add("userTag", userTag);
            apiParameters.Add("deviceTag", deviceTag);
            apiParameters.Add("deviceInfo", deviceInfo);
            if (joinedGroup != null)
                apiParameters.Add("groupTag", joinedGroup);

            var result = await App.MobileService.InvokeApiAsync(
                "sharedata",
                HttpMethod.Post,
                apiParameters);

            return true;
        }

        public async Task<ObservableCollection<HistoryData>> GetHistoryData()
        {
            var apiParameters = new Dictionary<string, string>();
            apiParameters.Add("userSid", App.User.SID);
            var result = await App.MobileService.InvokeApiAsync(
                "sharehistory",
                HttpMethod.Get,
                apiParameters);
            return await Task.Factory.StartNew(()=>JsonConvert.DeserializeObject<ObservableCollection<HistoryData>>(result.Root.ToString()));
        }
    }
}
