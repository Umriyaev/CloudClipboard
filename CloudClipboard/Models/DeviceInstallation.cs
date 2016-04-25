using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudClipboard.Models
{
    public class DeviceInstallation
    {
        public string InstallationId { get; set; }
        public string Platform { get; set; }
        public string PushChannel { get; set; }
        public string[] Tags { get; set; }
    }
}
