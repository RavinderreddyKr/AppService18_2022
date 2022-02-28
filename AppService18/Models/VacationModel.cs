using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService18.Models
{
    public class VacationModel
    {
        public Int64 UID { get; set; }
        public Int64 vacationId { get; set; }
        public string userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string UserFullName { get; set; }
        public string vacationStartdate { get; set; }
        public string vacationEnddate { get; set; }
        public string approvalStatus { get; set; }
        public string vacationComments { get; set; }
        public string previousComments { get; set; }
        public string vacationDocumentUrl { get; set; }
        public string vactionDocumentFileName { get; set; }
        public string vacationDocumentName { get; set; }
        public Int32 vacationStatusFlag { get; set; }
        public string vacationStatusMsg { get; set; }
        public string vacationAction { get; set; }
        public string vacationCountry { get; set; }
        public byte[] vacationDocument { get; set; }
        public string vacationApprovalStatus { get; set; }
        public string vacationStatus { get; set; }
        public string adminTOMail { get; set; }
        public string adminCCMail { get; set; }
        public string adminBCCMail { get; set; }
        public string userMailSubject { get; set; }
        public string userMailBody { get; set; }
        public string adminMailSubject { get; set; }
        public string adminMailBody { get; set; }
    }
}