using CloudClipboard.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudClipboard.Services
{
    public class LoginService
    {
        public async Task<AccountInfo> AuthenticateAsync()
        {
            var provider = "Facebook";
            MobileServiceUser user;
            try
            {
                user = await App.MobileService.LoginAsync(provider);
                return new AccountInfo() { SID = user.UserId };
            }
            catch (MobileServiceInvalidOperationException e)
            {
                throw e;
            }
        }
    }
}
