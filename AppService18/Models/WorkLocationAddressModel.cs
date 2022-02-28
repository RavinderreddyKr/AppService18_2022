using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class WorkLocationAddressModel
    {
        public Int64 UID { get; set; }
        public Int64 projectId { get; set; }    
        public string clientName { get; set; }
        public string address1 { get; set; }
        public string state1 { get; set; }
        public string otherState1 { get; set; }
        public string city1 { get; set; }
        public string zipCode1 { get; set; }
        public string address2 { get; set; }
        public string state2 { get; set; }
        public string otherState2 { get; set; }
        public string city2 { get; set; }
        public string zipCode2 { get; set; }
        public string address3 { get; set; }
        public string state3{ get; set; }
        public string otherState3 { get; set; }
        public string city3 { get; set; }
        public string zipCode3 { get; set; }
        public string action { get; set; }
        public string loggedInUserId { get; set; }
        public string returnStatus { get; set; }
        public string returnMsg { get; set; }
        public string AdminTOMail { get; set; }
        public string AdminCCMail { get; set; }
        public string AdminBCCMail { get; set; }
        public string UserMailSubject { get; set; }
        public string UserMailBody { get; set; }
        public string AdminMailSubject { get; set; }
        public string AdminMailBody { get; set; }
        public string CurrentResidenceAddress { get; set; }
        public string CurrentWLAddress { get; set; }
        public string PreviousWLAddress { get; set; }
        public string EffectiveDate { get; set; }
    }
}