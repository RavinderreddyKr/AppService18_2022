using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class UserModel
    {
        public Int64 UID { get; set; }
        public Int64 contractId { get; set; }
        public Int64 projectId { get; set; }
        public string userName { get; set; }
        public string companyStartDate { get; set; }
        public string jobTitle { get; set; }
        public string clientName { get; set; }
        public string vendorName { get; set; }
        public string implementorName { get; set; }
        public string startDateClient { get; set; }
        public string lCASalary { get; set; }
        public string supervisorName { get; set; }
        public string supervisorPhone { get; set; }
        public string supervisorEmailId { get; set; }
        public string techSupervisorName { get; set; }
        public string techSupervisorPhone { get; set; }
        public string techSupervisorEmailId { get; set; }
        public string clientLocation { get; set; }
        public string residenceAddress { get; set; }
        public string jobChangeLCAfileddate { get; set; }
        public string jobChangeAddress { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
        public string loggedInUserId { get; set; }
        public string action { get; set; }
        public Int32 statusFlag { get; set; }
        public string returnMsg { get; set; }
        public string userInfoTitle { get; set; }
        public string previousData { get; set; }
        public string updateData { get; set; }
        public string AdminTOMail { get; set; }
        public string AdminCCMail { get; set; }
        public string AdminBCCMail { get; set; }
        public string UserMailSubject { get; set; }
        public string UserMailBody { get; set; }
        public string AdminMailSubject { get; set; }
        public string AdminMailBody { get; set; }
    }
}