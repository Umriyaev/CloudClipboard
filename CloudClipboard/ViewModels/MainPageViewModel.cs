using CloudClipboard.Models;
using CloudClipboard.Services;
using Facebook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Core;

namespace CloudClipboard.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<string> groups = new ObservableCollection<string>();
        public ObservableCollection<string> Groups
        {
            get { return groups; }
            set
            {
                groups = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Groups)));
            }
        }

        private ObservableCollection<HistoryData> history = new ObservableCollection<HistoryData>();
        public ObservableCollection<HistoryData> History
        {
            get { return history; }
            set
            {
                history = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(History)));
            }
        }

        private string username = null;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
            }
        }



        private string joinedGroup = null;
        public string JoinedGroup
        {
            get { return joinedGroup; }
            set
            {
                joinedGroup = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JoinedGroup)));
            }
        }

        private bool isJoinEnabled = false;
        public bool IsJoinEnabled
        {
            get { return isJoinEnabled; }
            set
            {
                isJoinEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsJoinEnabled)));
            }
        }

        private bool loggedIn = false;
        public bool LoggedIn
        {
            get { return loggedIn; }
            set
            {
                loggedIn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoggedIn)));

            }
        }

        private bool isShareEnabled = false;
        public bool IsShareEnabled
        {
            get { return isShareEnabled; }
            set
            {
                isShareEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShareEnabled)));
                if (!IsShareEnabled)
                    ShareDeviceWithGroup();

            }
        }

        string groupName;
        public string GroupName
        {
            get { return groupName; }
            set
            {
                groupName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupName)));
            }
        }

        SharedDataInfo sharedData;
        public SharedDataInfo SharedData
        {
            get { return sharedData; }
            set
            {
                sharedData = value;
                toastService.ShowToast(sharedData.SharedText, "Clipboard updated");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SharedData)));
            }
        }

        DeviceShareParameterService deviceShareParameterService = new DeviceShareParameterService();
        LoginService loginService = new LoginService();
        ToastService toastService = new ToastService();
        ClipboardService clipboardService = new ClipboardService();
        SyncService syncService = new SyncService();
        PushNotificationService pushNotificationService = new PushNotificationService();
        EasClientDeviceInformation eas = new EasClientDeviceInformation();


        public async void ShareDeviceWithGroup()
        {
            string oldGroupName = null;
            string newGroupName = IsShareEnabled ? GroupName : null;
            deviceShareParameterService.GetGroupName(out oldGroupName);
            deviceShareParameterService.SetGroupName(newGroupName);
            var authResult = await pushNotificationService.AddGroupTagToDeviceInstallation(newGroupName, oldGroupName);
            Groups = authResult.SharedDevices;
            string deviceSharedGroupName;
            if (deviceShareParameterService.GetGroupName(out deviceSharedGroupName))
            {
                Groups.Remove(deviceSharedGroupName);
            }
        }

        public async void Login()
        {
            try
            {
                // object resultString;
                if (App.User == null)
                {
                    App.User = await loginService.AuthenticateAsync();
                    LoggedIn = true;
                }
                else
                {
                    toastService.ShowToast($"User {App.User.UserName} is already authenticated", "User authenticated");
                }
                var authResult = await pushNotificationService.CreateInstallation();
                App.User.UserName = authResult.UserId;
                Username = authResult.UserId;
                toastService.ShowToast($"User {Username} has authenticated successfully", "Success");
                Groups = authResult.SharedDevices;
                clipboardService.DataCopiedToClipboard += (s, data) =>
                {
                    SharedData = data;
                };

                pushNotificationService.NotificationReceived += async (s, e) =>
                {
                    e.Cancel = true;
                    XmlDocument xDoc = e.ToastNotification.Content;
                    string copiedText = xDoc.InnerText;
                    App.IncomingCopiedText = copiedText;



                    await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        async () =>
                        {
                            bool result = await clipboardService.IsTextSameWithClipboard(copiedText);
                            if (!result)
                            {
                                DataPackage clipboardContent = new DataPackage();
                                clipboardContent.SetText(copiedText);

                                Clipboard.SetContent(clipboardContent);
                            }
                        });


                };
                GetHistoryData();


            }

            catch (Exception e)
            {
                toastService.ShowToast($"Error occured while authenticating user: {e.Message}", "Authentication failed");
            }
        }

        public async void Sync()
        {
            if (IsJoinEnabled)
                await syncService.SyncCopiedTextData(
                    textData: SharedData.SharedText,
                    userTag: App.User.SID,
                    deviceTag: App.DeviceTag,
                    joinedGroup: JoinedGroup, 
                    deviceInfo: $"{eas.SystemManufacturer}: {eas.SystemProductName}");
            else
                await syncService.SyncCopiedTextData(
                    textData: SharedData.SharedText,
                    userTag: App.User.SID, 
                    deviceTag: App.DeviceTag, 
                    deviceInfo: $"{eas.SystemManufacturer}: {eas.SystemProductName}");
            GetHistoryData();
        }

        public async void GetHistoryData()
        {
            var result = await syncService.GetHistoryData();
            History = result;
        }

    }
}
