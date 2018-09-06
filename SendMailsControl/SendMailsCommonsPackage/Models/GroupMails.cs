using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMailsCommonsPackage.Models
{
    public class GroupMails
    {
        public int groupMailsId { get; set; }
        public List<User> users { get; set; }
        public List<Email> emails { get; set; }
        public string status { get; set; }
    }
}
