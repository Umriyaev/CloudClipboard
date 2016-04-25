using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudClipboard.Models
{
    public class HistoryData
    {
        public string DeviceName { get; set; }
        public string Timestamp { get; set; }
        public string SharedText { get; set; }
        public DateTime SharedDate { get; set; }
        public DataType DataType { get; set; }
    }

    public enum DataType
    {
        Text,
        PhoneNumber,
        URL
    }
}
