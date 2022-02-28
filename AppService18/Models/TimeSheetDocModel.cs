using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class TimeSheetDocModel
    {
        public Int64 UID { get; set; }
        public string UserId { get; set; }
        public Int64 TimeSheetDocId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Month { get; set; }
        public string DocumentUrl { get; set; }
        public string DocumentFileName { get; set; }
        public string DocumentName { get; set; }
        public string DocSubmitStatus { get; set; }
        public string DocReturnMsg { get; set; }
        public string DocAction { get; set; }
        public string DocCreatedDate { get; set; }
        public Int32 DocStatusFlag { get; set; }
        public byte[] Document { get; set; }
        public string AdminTOMail { get; set; }
        public string AdminCCMail { get; set; }
        public string AdminBCCMail { get; set; }
        public string UserMailSubject { get; set; }
        public string UserMailBody { get; set; }
        public string AdminMailSubject { get; set; }
        public string AdminMailBody { get; set; }
    }
}