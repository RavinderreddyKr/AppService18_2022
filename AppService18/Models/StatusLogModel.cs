using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class StatusLogModel
    {
        public string userId { get; set; }
        public string currentStatus { get; set; }
        public string statusDescription { get; set; }
        public string loggedInUserId { get; set; }
        public string returnStatus { get; set; }
    }
}