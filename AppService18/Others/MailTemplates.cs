using AppService18.ExceptionLogger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace AppService18.Others
{
    public class MailTemplates
    {
        /// <summary>
        /// Registration Mail
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailSubject"></param>
        /// <param name="mailDescription"></param>
        /// <returns></returns>
        public static bool RegistrationMail(string userName,string mailTo,string mailSubject,string mailDescription)
        {
            try
            {
                Hashtable oKeywords = new Hashtable();
                oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                oKeywords.Add(Others.MailKeyWords.MAILDESCRIPTION, mailDescription);
                oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, mailSubject);
                oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());
                return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("APP_USERREGISTRATIONMAIL"), string.Empty, mailTo, string.Empty, string.Empty, mailSubject, oKeywords);
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
           
        }
        /// <summary>
        /// Forgot Mail
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailSubject"></param>
        /// <param name="mailDescription"></param>
        /// <returns></returns>
        public static bool ForgotMail(string userName, string mailTo, string mailSubject, string mailDescription,string defaultPassword)
        {
            try
            {
                Hashtable oKeywords = new Hashtable();
                oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                oKeywords.Add(Others.MailKeyWords.MAILDESCRIPTION, mailDescription);
                oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, mailSubject);
                oKeywords.Add(Others.MailKeyWords.PASSWORD, defaultPassword);
                oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());
                return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("APP_USERFORGOTPASSWORD"), string.Empty, mailTo, string.Empty, string.Empty, mailSubject, oKeywords);
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
          
        }
        /// <summary>
        /// Pay Period Hours Mail
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="payPeriod"></param>
        /// <param name="payHours"></param>
        /// <param name="workAuthType"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailCC"></param>
        /// <param name="userSubject"></param>
        /// <param name="adminSubject"></param>
        /// <param name="userDescription"></param>
        /// <param name="admindescription"></param>
        /// <param name="comments"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static bool PayPeriodHoursMail(string userName,string userid,string firstName, string lastName, string year,string month,string payPeriod,string payHours,string workAuthType,
            string mailTo, string mailCC, string userSubject, string userDescription,string adminSubject, string admindescription,string comments,bool isAdmin)
        {
            try
            {
                Hashtable oKeywords = new Hashtable();
                oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                oKeywords.Add(Others.MailKeyWords.USER_ID, userid);
                oKeywords.Add(Others.MailKeyWords.FIRST_NAME, firstName);
                oKeywords.Add(Others.MailKeyWords.LAST_NAME, lastName);
                oKeywords.Add(Others.MailKeyWords.ADMIN, ConfigurationManager.AppSettings["AdminName"].ToString());
                oKeywords.Add(Others.MailKeyWords.WORKAUTH_TYPE, workAuthType);
                oKeywords.Add(Others.MailKeyWords.TIMESHEET_YEAR, year);
                oKeywords.Add(Others.MailKeyWords.TIMESHEET_MONTH, month);
                oKeywords.Add(Others.MailKeyWords.TIMESHEET_DATERANGE, payPeriod);
                oKeywords.Add(Others.MailKeyWords.TIMESHEET_STHOURS, payHours);
                oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, userSubject);
                oKeywords.Add(Others.MailKeyWords.ADMIN_TEMPLATE_SUBJECT, adminSubject);
                oKeywords.Add(Others.MailKeyWords.TIMESHEET_USER_DESCRIPTION, userDescription);
                oKeywords.Add(Others.MailKeyWords.TIMESHEET_ADMIN_DESCRIPTION, admindescription);
                oKeywords.Add(Others.MailKeyWords.COMMENTS, comments);
                oKeywords.Add(Others.MailKeyWords.PAYHOURS_EXCEEDMSG, Convert.ToDecimal(payHours.ToString()) > Convert.ToDecimal(ConfigurationManager.AppSettings["PAYEXCEEDHOURS"].ToString()) ? "***" + ConfigurationManager.AppSettings["PAYHOURSEXCEEDMSG"].ToString() + "***" : "");
                oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());
                if (isAdmin == true)
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_ADMIN_TIMESHEET_CONFIRMATION"), string.Empty, mailTo,
                        mailCC, string.Empty, userName + ": " + month + " " + payPeriod + " - " + payHours + " " + adminSubject, oKeywords);
                }
                else
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_TIMESHEET_CONFIRMATION"), string.Empty, mailTo, string.Empty, string.Empty,
                        month + " " + payPeriod + " - " + payHours + " " + userSubject, oKeywords);
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
           

        }
        /// <summary>
        /// TimeSheet Document Mail
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="TSDocId"></param>
        /// <param name="userName"></param>
        /// <param name="userid"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="timeStartDate"></param>
        /// <param name="timeEnddate"></param>
        /// <param name="workAuthType"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailCC"></param>
        /// <param name="userSubject"></param>
        /// <param name="userDescription"></param>
        /// <param name="adminSubject"></param>
        /// <param name="admindescription"></param>
        /// <param name="comments"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static bool TimeSheetDocumentMail(Int64 UID, Int64 TSDocId, string userName, string userid,string firstName,string lastName, string timeStartDate, string timeEnddate, string workAuthType,
            string mailTo, string mailCC, string userSubject, string userDescription, string adminSubject, string admindescription, string comments, bool isAdmin)
        {
            Others.CommonFunction commonFunction = new Others.CommonFunction();
            Controllers.CommonController commonProcess = new Controllers.CommonController();
            Models.StatusLogModel statusLogMdl = new Models.StatusLogModel();
            DataSet dsTSDocuments = new DataSet();
            DataSet _dsTimesheetDocStatus = new DataSet();

            double TotalFileSize = 0;
            double DocSize = 0;
            string DocName = string.Empty;
            string MailDescription = string.Empty;
            List<string> DocumentsDescription = new List<string>();
            bool _mailStatus = true;

            try
            {
                dsTSDocuments = commonFunction.ManageTimeSheetsFileView(UID, TSDocId, null, null, null, "GetTimeSheetFileName");

                if (dsTSDocuments.Tables[0].Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in dsTSDocuments.Tables[0].Rows)
                    {
                        if (System.IO.File.Exists(dr["DOCUMENTURL"].ToString()))
                        {
                            FileInfo fileinfo = new FileInfo(dr["DOCUMENTURL"].ToString());
                            double DocsizeInKb = fileinfo.Length / 1024;
                            DocSize = (Math.Round((DocsizeInKb / 1024) * 100) / 100);
                            DocumentsDescription.Add("DocumentName:" + dr["DOCUMENTNAME"].ToString() + "- Size:" + DocSize + " MB");
                            TotalFileSize += DocSize;
                        }
                    }

                    Hashtable oKeywords = new Hashtable();
                    oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                    oKeywords.Add(Others.MailKeyWords.USER_ID, userid);
                    oKeywords.Add(Others.MailKeyWords.FIRST_NAME, firstName);
                    oKeywords.Add(Others.MailKeyWords.LAST_NAME, lastName);
                    oKeywords.Add(Others.MailKeyWords.ADMIN, ConfigurationManager.AppSettings["AdminName"].ToString());
                    oKeywords.Add(Others.MailKeyWords.WORKAUTH_TYPE, workAuthType);
                    oKeywords.Add(Others.MailKeyWords.TIMESHEET_STDATE, timeStartDate);
                    oKeywords.Add(Others.MailKeyWords.TIMESHEET_ENDDATE, timeEnddate);
                    oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, userSubject);
                    oKeywords.Add(Others.MailKeyWords.ADMIN_TEMPLATE_SUBJECT, adminSubject);
                    oKeywords.Add(Others.MailKeyWords.TIMESHEET_USER_DESCRIPTION, userDescription);
                    oKeywords.Add(Others.MailKeyWords.TIMESHEET_ADMIN_DESCRIPTION, admindescription);
                    oKeywords.Add(Others.MailKeyWords.COMMENTS, comments);
                    oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());

                    if (TotalFileSize <= Convert.ToDouble(ConfigurationManager.AppSettings["EmailAttachmentSize"].ToString()))
                    {
                        MailDescription = String.Join(", ", DocumentsDescription.ToArray());
                        _dsTimesheetDocStatus = commonFunction.EmailDocumentStatus(UID, MailDescription, "Success", userid, "Timesheet Documents From MobileApp");

                        if (_dsTimesheetDocStatus.Tables[0].Rows.Count > 0)
                        {
                            statusLogMdl.userId = Convert.ToString(UID);
                            statusLogMdl.loggedInUserId = userid;
                            statusLogMdl.currentStatus = "Timesheet attached documents are delivered successfully From MobileApp";
                            statusLogMdl.statusDescription = "Timesheet attached documents are delivered successfully From MobileApp";

                            commonProcess.StatusLog(statusLogMdl);

                        }

                        oKeywords.Add(Others.MailKeyWords.EMAILATTACHMENT_COMMENTS, "Please find the attached employee timesheet document of the below candidate");

                        if (isAdmin == true)
                        {
                            _mailStatus = MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_ADMIN_TIMESHEET_DOCUMENT_CONFIRMATION"), string.Empty, mailTo,
                               mailCC, string.Empty, userName + " - " + adminSubject, oKeywords, dsTSDocuments);
                        }
                        else
                        {
                            _mailStatus = MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_TIMESHEET_DOCUMENT_CONFIRMATION"), string.Empty, mailTo, string.Empty, string.Empty, userSubject, oKeywords);
                        }
                    }
                    else
                    {
                        MailDescription = String.Join(", ", DocumentsDescription.ToArray());
                        _dsTimesheetDocStatus = commonFunction.EmailDocumentStatus(UID, MailDescription, "Failed", userid, "Timesheet Documents From MobileApp");

                        if (_dsTimesheetDocStatus.Tables[0].Rows.Count > 0)
                        {
                            statusLogMdl.userId = Convert.ToString(UID);
                            statusLogMdl.loggedInUserId = userid;
                            statusLogMdl.currentStatus = "Timesheet attached email documents are failed to delivered From MobileApp";
                            statusLogMdl.statusDescription = "Timesheet attached email documents are failed to delivered From MobileApp";

                            commonProcess.StatusLog(statusLogMdl);

                        }
                        oKeywords.Add(Others.MailKeyWords.EMAILATTACHMENT_COMMENTS, "Unable to deliver the attached documents as the size exceeds 25MB.<br/> Please download the documents from timesheet.");
                        if (isAdmin == true)
                        {
                            _mailStatus = MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_ADMIN_TIMESHEET_DOCUMENT_CONFIRMATION"), string.Empty, mailTo,
                               mailCC, string.Empty, userName + " - " + adminSubject, oKeywords, dsTSDocuments);
                        }
                        else
                        {
                            _mailStatus = MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_TIMESHEET_DOCUMENT_CONFIRMATION"), string.Empty, mailTo, string.Empty, string.Empty, userSubject, oKeywords);
                        }
                    }

                }
                return _mailStatus;
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }

        }
        /// <summary>
        /// Residence Address Mail
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="workAuthType"></param>
        /// <param name="previousResidenceAddress"></param>
        /// <param name="CurrentResidenceAddress"></param>
        /// <param name="CurrentWLAddress"></param>
        /// <param name="EffectiveDate"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailCC"></param>
        /// <param name="userSubject"></param>
        /// <param name="adminSubject"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static bool ResidenceAddressMail(string userName, string userId, string firstName, string lastName, string workAuthType, string previousResidenceAddress,
            string CurrentResidenceAddress, string CurrentWLAddress, string EffectiveDate,string PaycheckId, string mailTo, string mailCC, string userSubject,
            string adminSubject, bool isAdmin,string state)
        {
            try
            {
                Hashtable oKeywords = new Hashtable();
                oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                oKeywords.Add(Others.MailKeyWords.USER_ID, userId);
                oKeywords.Add(Others.MailKeyWords.FIRST_NAME, firstName);
                oKeywords.Add(Others.MailKeyWords.LAST_NAME, lastName);
                oKeywords.Add(Others.MailKeyWords.ADMIN, ConfigurationManager.AppSettings["AdminName"].ToString());
                oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, userSubject);
                oKeywords.Add(Others.MailKeyWords.WORKAUTH_TYPE, workAuthType);
                oKeywords.Add(Others.MailKeyWords.PREVIOUS_RESIDENCEADDRESS, previousResidenceAddress);
                oKeywords.Add(Others.MailKeyWords.CURRENT_RESIDENCEADDRESS, CurrentResidenceAddress);
                oKeywords.Add(Others.MailKeyWords.CURRENT_WLADDRESS, CurrentWLAddress);
                oKeywords.Add(Others.MailKeyWords.PAYCHECK_ID, PaycheckId != null ? PaycheckId + "" : "");
                oKeywords.Add(Others.MailKeyWords.EFFECTIVE_DATE, EffectiveDate != null ? EffectiveDate + "" : "");
                oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());
                if (state == "Ohio" || state == "Pennsylvania")
                {
                    oKeywords.Add(Others.MailKeyWords.LOCALTAX, ConfigurationManager.AppSettings["ACTaxApplicableKey"].ToString());
                }
                else if (state == "Connecticut" || state == "Massachusetts")
                {
                    oKeywords.Add(Others.MailKeyWords.LOCALTAX, ConfigurationManager.AppSettings["PFMLATAXKey"].ToString());
                }
                else
                {
                    oKeywords.Add(Others.MailKeyWords.LOCALTAX, "");
                }

                if (isAdmin == true)
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_ADMIN_ADDRESSCHANGE_EMAIL"), string.Empty, mailTo,
                        mailCC, string.Empty, userName + " - " + adminSubject, oKeywords);
                }
                else
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_ADDRESSCHANGE_EMAIL"), string.Empty, mailTo, string.Empty, string.Empty, userSubject, oKeywords);
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }

        }
        /// <summary>
        /// WorkLocation Address Mail
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="workAuthType"></param>
        /// <param name="currentWLAddress"></param>
        /// <param name="previousWLAddress"></param>
        /// <param name="currentResidenceAddress"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailCC"></param>
        /// <param name="userSubject"></param>
        /// <param name="adminSubject"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static bool WorkLocationAddressMail(string userName, string userId, string firstName, string lastName, string workAuthType,
            string currentWLAddress, string previousWLAddress, string currentResidenceAddress, string effectiveDate, string mailTo,
            string mailCC, string userSubject, string adminSubject, bool isAdmin, string state)
        {
            try
            {
                Hashtable oKeywords = new Hashtable();
                oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                oKeywords.Add(Others.MailKeyWords.USER_ID, userId);
                oKeywords.Add(Others.MailKeyWords.FIRST_NAME, firstName);
                oKeywords.Add(Others.MailKeyWords.LAST_NAME, lastName);
                oKeywords.Add(Others.MailKeyWords.ADMIN, ConfigurationManager.AppSettings["AdminName"].ToString());
                oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, userSubject);
                oKeywords.Add(Others.MailKeyWords.WORKAUTH_TYPE, workAuthType);
                oKeywords.Add(Others.MailKeyWords.CURRENT_WLADDRESS, currentWLAddress);
                oKeywords.Add(Others.MailKeyWords.PREVIOUS_WLADDRESS, previousWLAddress);
                oKeywords.Add(Others.MailKeyWords.CURRENT_RESIDENCEADDRESS, currentResidenceAddress);
                oKeywords.Add(Others.MailKeyWords.EFFECTIVE_DATE, effectiveDate != null ? effectiveDate + "" : "");
                oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());
                if (state == "Ohio" || state == "Pennsylvania")
                {
                    oKeywords.Add(Others.MailKeyWords.LOCALTAX, ConfigurationManager.AppSettings["ACTaxApplicableKey"].ToString());
                }
                else
                {
                    oKeywords.Add(Others.MailKeyWords.LOCALTAX, "");
                }

                if (isAdmin == true)
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_ADMIN_WORKLOCATIONCHANGE_EMAIL"), string.Empty, mailTo,
                        mailCC, string.Empty, userName + " - " + adminSubject, oKeywords);
                }
                else
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_WORKLOCATIONCHANGE_EMAIL"), string.Empty, mailTo, string.Empty, string.Empty, userSubject, oKeywords);
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
        }
        /// <summary>
        ///  Weekly Status Report Mail
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="workAuthType"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="activitiesAccomplished"></param>
        /// <param name="activitiesPlaanedNextWeek"></param>
        /// <param name="projectProgress"></param>
        /// <param name="specificQuestions"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailCC"></param>
        /// <param name="userSubject"></param>
        /// <param name="adminSubject"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static bool WeeklyStatusReportMail(string userName, string firstName, string lastName, string workAuthType, string fromDate, string toDate,
            string activitiesAccomplished, string activitiesPlaanedNextWeek, string projectProgress, string specificQuestions,
            string mailTo, string mailCC, string userSubject, string adminSubject, bool isAdmin)
        {
            try
            {
                Hashtable oKeywords = new Hashtable();
                oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                oKeywords.Add(Others.MailKeyWords.FIRST_NAME, firstName);
                oKeywords.Add(Others.MailKeyWords.LAST_NAME, lastName);
                oKeywords.Add(Others.MailKeyWords.ADMIN, ConfigurationManager.AppSettings["AdminName"].ToString());
                oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, userSubject);
                oKeywords.Add(Others.MailKeyWords.WORKAUTH_TYPE, workAuthType);
                oKeywords.Add(Others.MailKeyWords.FROMDATE, fromDate);
                oKeywords.Add(Others.MailKeyWords.TODATE, toDate);
                oKeywords.Add(Others.MailKeyWords.WEEKLYACTIVITIES_ACCOMPLISHED, activitiesAccomplished);
                oKeywords.Add(Others.MailKeyWords.WEEKLYACTIVITIES_PLANNEDNEXTWEEK, activitiesPlaanedNextWeek);
                oKeywords.Add(Others.MailKeyWords.PROJECT_PROGRESS, projectProgress);
                oKeywords.Add(Others.MailKeyWords.SPECIFIC_QUESTIONS, specificQuestions);
                oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());
                if (isAdmin == true)
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("ADMIN_WEEKLYSTATUSREPORT_CONFIRMATION"), string.Empty, mailTo,
                        mailCC, string.Empty, adminSubject + ": " + userName + ": " + fromDate + " - " + toDate, oKeywords);
                }
                else
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_WEEKLYSTATUSREPORT_CONFIRMATION"), string.Empty,
                        mailTo, string.Empty, string.Empty, userSubject + ": " + fromDate + " - " + toDate, oKeywords);
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }

        }
        /// <summary>
        /// UserInfo Mail
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="workAuthType"></param>
        /// <param name="userinfoTitle"></param>
        /// <param name="previousData"></param>
        /// <param name="UpdateData"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailCC"></param>
        /// <param name="userSubject"></param>
        /// <param name="adminSubject"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static bool UserInfoMail(string userName, string firstName, string lastName, string workAuthType, string userinfoTitle,
               string previousData, string UpdateData, string mailTo, string mailCC, string userSubject, string adminSubject, bool isAdmin)
        {
            try
            {
                Hashtable oKeywords = new Hashtable();
                oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                oKeywords.Add(Others.MailKeyWords.FIRST_NAME, firstName);
                oKeywords.Add(Others.MailKeyWords.LAST_NAME, lastName);
                oKeywords.Add(Others.MailKeyWords.ADMIN, ConfigurationManager.AppSettings["AdminName"].ToString());
                oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, userSubject);
                oKeywords.Add(Others.MailKeyWords.WORKAUTH_TYPE, workAuthType);
                oKeywords.Add(Others.MailKeyWords.USERINFO_TITLE, userinfoTitle);
                oKeywords.Add(Others.MailKeyWords.USERINFO_PREVIOUSDATA, previousData);
                oKeywords.Add(Others.MailKeyWords.USERINFO_UPDATEDATA, UpdateData);
                oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());
                if (isAdmin == true)
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("ADMIN_USERINFO_CONFIRMATION"), string.Empty, mailTo,
                        mailCC, string.Empty, userName + " - " + userinfoTitle + " updated", oKeywords);
                }
                else
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_USERINFO_CONFIRMATION"), string.Empty, mailTo, string.Empty, string.Empty, userinfoTitle + " updated", oKeywords);
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }

        }
        /// <summary>
        /// Vacation Letter Mail
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="workAuthType"></param>
        /// <param name="startdate"></param>
        /// <param name="endDate"></param>
        /// <param name="userCommnets"></param>
        /// <param name="adminComments"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailCC"></param>
        /// <param name="userSubject"></param>
        /// <param name="adminSubject"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static bool VacationLetterMail(string userid, string userName, string firstName, string lastName, string workAuthType, string startdate, string endDate,
            string userCommnets, string adminComments, string mailTo, string mailCC, string userSubject, string adminSubject, bool isAdmin)
        {
            try
            {
                Hashtable oKeywords = new Hashtable();
                oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                oKeywords.Add(Others.MailKeyWords.ADMIN, "Admin");
                oKeywords.Add(Others.MailKeyWords.USER_ID, userid);
                oKeywords.Add(Others.MailKeyWords.FIRST_NAME, firstName);
                oKeywords.Add(Others.MailKeyWords.LAST_NAME, lastName);
                oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());
                oKeywords.Add(Others.MailKeyWords.WORKAUTH_TYPE, workAuthType);
                oKeywords.Add(Others.MailKeyWords.COMMENTS, userCommnets != null ? "Comments : " + userCommnets + "" : "");
                oKeywords.Add(Others.MailKeyWords.ADMIN_COMMENTS, adminComments != null ? "Comments : " + adminComments + "" : "");
                oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, "Vacation Info");
                oKeywords.Add(Others.MailKeyWords.USER_VACATIONSTARTDATE, startdate);
                oKeywords.Add(Others.MailKeyWords.USER_VACATIONENDDATE, endDate);
                oKeywords.Add(Others.MailKeyWords.VACATION_MAILDESCRIPTION, "Please review");

                if (isAdmin == true)
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("ADMIN_VACATION_EMAIL"), string.Empty, mailTo,
                        mailCC, string.Empty, userName + " - " + adminSubject, oKeywords);
                }
                else
                {
                    return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_VACATION_CONFIRMATION_EMAIL"), string.Empty,
                        mailTo, string.Empty, string.Empty, userSubject, oKeywords);
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
        }
        /// <summary>
        /// Cancel Vacation Latter Mail
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="workAuthType"></param>
        /// <param name="mailTo"></param>
        /// <param name="userSubject"></param>
        /// <returns></returns>
        public static bool CancelVacationLetterMail(string userid, string userName, string firstName, string lastName, string workAuthType,
            string mailTo, string userSubject)
        {
            try
            {
                Hashtable oKeywords = new Hashtable();
                oKeywords.Add(Others.MailKeyWords.USERNAME, userName);
                oKeywords.Add(Others.MailKeyWords.USER_ID, userid);
                oKeywords.Add(Others.MailKeyWords.FIRST_NAME, firstName);
                oKeywords.Add(Others.MailKeyWords.LAST_NAME, lastName);
                oKeywords.Add(Others.MailKeyWords.SUPPORTEMAIL, ConfigurationManager.AppSettings["SupportEmail"].ToString());
                oKeywords.Add(Others.MailKeyWords.WORKAUTH_TYPE, workAuthType);
                oKeywords.Add(Others.MailKeyWords.TEMPLATESUBJECT, "Vacation Info");

                return MailFormats.SendMail(MailTemplateCodes.GetMailTemplateCode("USER_VACATION_CANCEL_CONFIRMATION_EMAIL"), string.Empty,
                    mailTo, string.Empty, string.Empty, userSubject, oKeywords);

            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
        }
    }
}