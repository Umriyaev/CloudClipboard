using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace CloudClipboard.Services
{
    public class ToastService
    {
        public void ShowToast(string body, string title)
        {
            var content = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = title
                    },
                    BodyTextLine1 = new ToastText()
                    {
                        Text = body
                    }
                }
            };

            var notification = new ToastNotification(content.GetXml());
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(notification);
        }
    }
}
