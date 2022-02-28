using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class TimeSheetModel
    {
        public Int64 UID { get; set; }
        public string UserId { get; set; }
        public string APPPassword { get; set; }
        public string UserFullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WorkAuthType { get; set; }
        public string EMailId { get; set; }
        public Int64 RoleId { get; set; }
        public string ReturnMsg { get; set; }
        public string Action { get; set; }
        public Int32 StatusFlag { get; set; }
        public Int64 PayHoursId { get; set; }
        public Int64 ProjectId { get; set; }  
        public string PayHoursYear { get; set; }
        public string PayHoursMonth { get; set; } 
        public string PayPeriod { get; set; }
        public string PayHours { get; set; }
        public string SubmitStatus { get; set; }
        public string ApprovalStatus { get; set; }
        public string Comments { get; set; }
        public string SearchValue { get; set; }
        public string NewAPPPassword { get; set; }
        public string DefaultAPPPassword { get; set; }
        public string AdminTOMail { get; set; }
        public string AdminCCMail { get; set; }
        public string AdminBCCMail { get; set; }
        public string UserMailSubject { get; set; }
        public string UserMailBody { get; set; }
        public string AdminMailSubject { get; set; }
        public string AdminMailBody { get; set; }
        public int ExpiredStatus { get; set; }
    }
}