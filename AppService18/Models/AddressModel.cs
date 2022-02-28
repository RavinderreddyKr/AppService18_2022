using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class AddressModel
    {
        public Int64 UID { get; set; }
        public string previousCountry{ get; set; }
        public string previousState { get; set; }
        public string previousCity { get; set; }
        public string previousZipCode { get; set; }
        public string previousStreet { get; set; }
        public string previousSuite { get; set; }
        public string currentCountry { get; set; }
        public string currentState { get; set; }
        public string currentCity { get; set; }
        public string currentZipCode { get; set; }
        public string currentStreet { get; set; }
        public string currentSuite { get; set; }
        public string currentHomePhoneNo { get; set; }
        public string currentMobileNo { get; set; }
        public string action { get; set; }
        public bool status { get; set; }
        public string loggedInUserId { get; set; }
        public string returnStatus{ get; set; }
        public string returnMsg { get; set; }
        public string AdminTOMail { get; set; }
        public string AdminCCMail { get; set; }
        public string AdminBCCMail { get; set; }
        public string UserMailSubject { get; set; }
        public string UserMailBody { get; set; }
        public string AdminMailSubject { get; set; }
        public string AdminMailBody { get; set; }
        public string PreviousResidenceAddress { get; set; }
        public string CurrentResidenceAddress { get; set; }
        public string CurrentWLAddress { get; set; }
        public string EffectiveDate { get; set; }
    }
}