using CloudClipboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace CloudClipboard.Services
{
    public class ClipboardService
    {
        public event EventHandler<SharedDataInfo> DataCopiedToClipboard;

        public ClipboardService()
        {
            Clipboard.ContentChanged += async (s, e) =>
            {
                DataPackageView dataPackageView = Clipboard.GetContent();
                if (dataPackageView.Contains(StandardDataFormats.Text))
                {
                    string text = await dataPackageView.GetTextAsync();
                    if (text != App.IncomingCopiedText)
                        DataCopiedToClipboard?.Invoke(
                            this, new SharedDataInfo()
                            {
                                Account = new AccountInfo() { SID = App.User.SID },
                                SharedText = text
                            });
                }
            };
        }

        public async Task<bool> IsTextSameWithClipboard(string text)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                string textFromClipboard = await dataPackageView.GetTextAsync();
                if (textFromClipboard == text)
                    return true;
                else
                    return false;
            }
            return false;
        }
    }
}
