using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class CommonModel
    {
        public Int64 countryId { get; set; }
        public Int64 stateId { get; set; }
        public string countryName { get; set; }
        public string stateName { get; set; }
        public string note { get; set; }
        public string action { get; set; }
    }
    public class QAModel
    {
        public Int64 QAAID { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
    }
    public class APPVersionModel
    {
        public string APPVersion { get; set; }
    }
    public class MobileInfoModel
    {
        public Int64 UID { get; set; }
        public string fullUserName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailId { get; set; }
        public string phoneNo { get; set; }
        public string workAuthType { get; set; }
        public string currentVersion { get; set; }
        public string previousVersion { get; set; }
        public string deviceName { get; set; }
        public string deviceId { get; set; }
        public string location { get; set; }
        public string osVersion { get; set; }
        public string osType { get; set; }
        public string lastUpdatedDate { get; set; }
        public string loggedInUserId { get; set; }
        public string searchTxtVal { get; set; }
        public string action { get; set; }
        public string returnStatus { get; set; }
        public string createdDate { get; set; }
        public int iOSCount { get; set; }
        public int androidCount { get; set; }
        public int miPendingCount { get; set; }
    }
}