using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudClipboard.Models
{
    public class AuthenticationResult
    {
        public string UserId { get; set; }
        public ObservableCollection<string> SharedDevices { get; set; }
    }
}
