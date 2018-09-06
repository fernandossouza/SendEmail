using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMailsCommonsPackage.Models
{
    public class Alert
    {
        public int alertId { get; set; }
        public string descAlert { get; set; }
        public string color { get; set; }
        public string startValue { get; set; }
        public string endValue { get; set; }
        public string message { get; set; }
        public List<Weekday> weekdays { get; set; }
        public string scheduleId { get; set; }
        public string scheduleName { get; set; }
        public int groupMailsListId { get; set; }
        public int priority { get; set; }
        //public List<Metric> metrics { get; set; }
    }
}
