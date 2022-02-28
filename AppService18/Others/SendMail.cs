using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace AppService18.Others
{
    public class SendMail
    {
        /// <summary>
        /// Mail Status Code
        /// </summary>
        public System.Net.Mail.SmtpStatusCode StatusCode { get; set; }

        /// Sender Mail(Ex:Company Mail)
        /// </summary>
        public static string FromEmailID = ConfigurationManager.AppSettings["FromEmailId"].ToString();
        /// <summary>
        /// Default Email Id 
        /// </summary>
        public static string DefaultEmail = ConfigurationManager.AppSettings["MyMailBox"].ToString();

        /// <summary>
        /// Req: subject, body, from email, to email,cc email and bcc email
        /// </summary>
        /// <param name="strSubject"></param>
        /// <param name="strBody"></param>
        /// <param name="FromEmail"></param>
        /// <param name="ToEmail"></param>
        /// <param name="sCCMail"></param>
        /// <param name="sBCCMail"></param>
        /// <returns></returns>
        public static bool SendMailContent(string FromEmail, string ToEmail, string sCCMail, string sBCCMail,string strSubject, string strBody)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient();
            string[] strToAddresses;
            string ToMailAddress = string.Empty;
            try
            {
                SmtpServer = new SmtpClient();
                if (FromEmailID != null && FromEmailID != "")
                    FromEmail = FromEmailID.Trim();
                mail.From = new MailAddress(FromEmail, ConfigurationManager.AppSettings["MailDisplayName"].ToString());

                ToMailAddress = ToEmail + ";" + DefaultEmail;

                if (!string.IsNullOrEmpty(ToMailAddress))
                {
                    strToAddresses = ToMailAddress.Split(';');

                    if (strToAddresses.Length > 0)
                    {
                        for (int intToIndex = 0; intToIndex < strToAddresses.Length; intToIndex++)
                        {
                            if (strToAddresses[intToIndex].Trim() != "")
                            {
                                mail.To.Add(strToAddresses[intToIndex].Trim());
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sCCMail))
                {
                    strToAddresses = sCCMail.Split(';');

                    if (strToAddresses.Length > 0)
                    {
                        for (int intToIndex = 0; intToIndex < strToAddresses.Length; intToIndex++)
                        {
                            if (strToAddresses[intToIndex].Trim() != "")
                            {
                                mail.CC.Add(strToAddresses[intToIndex].Trim());
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sBCCMail))
                {
                    strToAddresses = sBCCMail.Split(';');

                    if (strToAddresses.Length > 0)
                    {
                        for (int intToIndex = 0; intToIndex < strToAddresses.Length; intToIndex++)
                        {
                            if (strToAddresses[intToIndex].Trim() != "")
                            {
                                mail.Bcc.Add(strToAddresses[intToIndex].Trim());
                            }
                        }
                    }
                }
                mail.Subject = strSubject;
                mail.Body = strBody;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                SmtpServer.Host = ConfigurationManager.AppSettings["smptAddress"].ToString();
                SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smptPort"].ToString());
                System.Net.NetworkCredential basicCredential = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smptUid"].ToString(), ConfigurationManager.AppSettings["smptPwd"].ToString());
                SmtpServer.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPEnableSSL"].ToString());
                SmtpServer.UseDefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPDefaultCredentials"].ToString());
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Credentials = basicCredential;
                SmtpServer.Send(mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        ExceptionLogger.Logger.LogInfo("Delivery failed - retrying in 5 seconds." + ex.InnerExceptions[i].StatusCode);
                        System.Threading.Thread.Sleep(5000);
                        SmtpServer.Send(mail);
                    }
                    else
                    {
                        ExceptionLogger.Logger.LogInfo("Failed to deliver message to {0}" +
                            ex.InnerExceptions[i].FailedRecipient);

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Logger.LogInfo("Exception caught in RetryIfBusy(): {0}" + ex.ToString());
            }
            finally
            {
                mail.Dispose();
                mail = null;
                SmtpServer = null;
            }
            return true;
        }
        /// <summary>
        /// Req: subject, body, from email, to email, cc email, bcc email and attachment
        /// </summary>
        /// <param name="FromEmail"></param>
        /// <param name="ToEmail"></param>
        /// <param name="sCCMail"></param>
        /// <param name="sBCCMail"></param>
        /// <param name="strSubject"></param>
        /// <param name="strBody"></param>
        /// <param name="sAttachingFileName"></param>
        /// <returns></returns>
        public static bool SendMailContent(string FromEmail, string ToEmail, string sCCMail, string sBCCMail, string strSubject, string strBody, DataSet sAttachingFileName)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer=new SmtpClient();
            string[] strToAddresses;
            string ToMailAddress = string.Empty;
            try
            {
                SmtpServer = new SmtpClient();
                if (FromEmailID != null && FromEmailID != "")
                    FromEmail = FromEmailID.Trim();
                mail.From = new MailAddress(FromEmail, ConfigurationManager.AppSettings["MailDisplayName"].ToString());

                ToMailAddress = ToEmail + ";" + DefaultEmail;

                if (!string.IsNullOrEmpty(ToMailAddress))
                {
                    strToAddresses = ToMailAddress.Split(';');

                    if (strToAddresses.Length > 0)
                    {
                        for (int intToIndex = 0; intToIndex < strToAddresses.Length; intToIndex++)
                        {
                            if (strToAddresses[intToIndex].Trim() != "")
                            {
                                mail.To.Add(strToAddresses[intToIndex].Trim());
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sCCMail))
                {
                    strToAddresses = sCCMail.Split(';');

                    if (strToAddresses.Length > 0)
                    {
                        for (int intToIndex = 0; intToIndex < strToAddresses.Length; intToIndex++)
                        {
                            if (strToAddresses[intToIndex].Trim() != "")
                            {
                                mail.CC.Add(strToAddresses[intToIndex].Trim());
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sBCCMail))
                {
                    strToAddresses = sBCCMail.Split(';');

                    if (strToAddresses.Length > 0)
                    {
                        for (int intToIndex = 0; intToIndex < strToAddresses.Length; intToIndex++)
                        {
                            if (strToAddresses[intToIndex].Trim() != "")
                            {
                                mail.Bcc.Add(strToAddresses[intToIndex].Trim());
                            }
                        }
                    }
                }
                if (sAttachingFileName.Tables[0].Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in sAttachingFileName.Tables[0].Rows)
                    {
                        if (File.Exists(dr["DOCUMENTURL"].ToString()))
                        {
                            Attachment oAttachment = new Attachment(dr["DOCUMENTURL"].ToString());
                            oAttachment.ContentDisposition.FileName = dr["DOCUMENTNAME"].ToString();
                            mail.Attachments.Add(oAttachment);
                        }
                    }
                }
                mail.Subject = strSubject;
                mail.Body = strBody;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                SmtpServer.Host = ConfigurationManager.AppSettings["smptAddress"].ToString();
                SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smptPort"].ToString());
                System.Net.NetworkCredential basicCredential = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smptUid"].ToString(), ConfigurationManager.AppSettings["smptPwd"].ToString());
                SmtpServer.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPEnableSSL"].ToString());
                SmtpServer.UseDefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPDefaultCredentials"].ToString());
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Credentials = basicCredential;
                SmtpServer.Send(mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        ExceptionLogger.Logger.LogInfo("Delivery failed - retrying in 5 seconds." + ex.InnerExceptions[i].StatusCode);
                        System.Threading.Thread.Sleep(5000);
                        SmtpServer.Send(mail);
                    }
                    else
                    {
                        ExceptionLogger.Logger.LogInfo("Failed to deliver message to {0}" +
                            ex.InnerExceptions[i].FailedRecipient);

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Logger.LogInfo("Exception caught in RetryIfBusy(): {0}" + ex.ToString());
            }
            finally
            {
                mail.Dispose();
                mail = null;
                SmtpServer = null;
            }
            return true;
        }
    }
}