using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class WeeklyStatusReportModel
    {
        public Int64 UID { get; set; }
        public Int64 visaId { get; set; }
        public Int64 weeklyReportId { get; set; }
        public string title { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string superVisor { get; set; }
        public string activitiesPlanned { get; set; }
        public string activitiesAccomplished { get; set; }
        public string activitiesPlannedNextWeek { get; set; }
        public string projectProgress { get; set; }
        public string specificQuestions { get; set; }
        public bool activeStatus { get; set; }
        public bool approvalStatus { get; set; }
        public string description { get; set; }
        public string loggedInUserId { get; set; }
        public string action { get; set; }
        public string returnStatus { get; set; }
        public string returnMsg { get; set; }
        public string AdminTOMail { get; set; }
        public string AdminCCMail { get; set; }
        public string AdminBCCMail { get; set; }
        public string UserMailSubject { get; set; }
        public string UserMailBody { get; set; }
        public string AdminMailSubject { get; set; }
        public string AdminMailBody { get; set; }
       
    }
}