using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class MailModel
    {
        public string mailFrom { get; set; }
        public string mailTo { get; set; }
        public string mailCC { get; set; }
        public string mailBcc { get; set; }
        public string mailSubject { get; set; }
        public string mailBody { get; set; }
    }

}