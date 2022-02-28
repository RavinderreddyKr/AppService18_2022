using AppService18.ExceptionLogger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;

namespace AppService18.Others
{
    public class MailFormats
    {
        /// <summary>
        /// This method replaces the keywords in the given string(sStringToReformat) with values existing in the hash table object(oKeywords) 
        ///  for the corresponding keywords defined as keys in hash table object and returns the formatted string.
        /// </summary>
        /// <param name="sStringToReformat"></param>
        /// <param name="oKeyWords"></param>
        /// <returns></returns>
        public static string GetReFormattedStringByReplacingKeywords(string sStringToReformat, Hashtable oKeyWords)
        {

            string _key = "";
            foreach (object objKey in oKeyWords.Keys)
            {
                sStringToReformat = sStringToReformat.Replace(objKey.ToString(), oKeyWords[objKey].ToString());
                _key = objKey.ToString().Replace('<', '{').Replace('>', '}');
                //if(oKeyWords.ContainsKey((object)_key))
                sStringToReformat = sStringToReformat.Replace(_key, oKeyWords[objKey].ToString());
            }

            return sStringToReformat;
        }

        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="MAIL_FROM"></param>
        /// <param name="MAIL_TO"></param>
        /// <param name="MAIL_CC"></param>
        /// <param name="MAIL_BCC"></param>
        /// <param name="MAIL_SUBJECT"></param>
        /// <param name="MAIL_BODY"></param>
        /// <returns></returns>
        public static bool SendMail(string mailTemplateCode, string mailFrom, string mailTo, string mailCc,string mailBcc,string mailSubject, Hashtable oKeyWords)
        {
            bool bIsMailSentSuccess = true;
            try
            {
                //get mail format for the specified mail type code
                DataSet dsMailFormat = null;

                dsMailFormat = GetMailTemplates(mailTemplateCode);
                //check whether the format exists
                if (dsMailFormat != null && dsMailFormat.Tables.Count > 0 && dsMailFormat.Tables[0].Rows.Count > 0)
                {
                    //Get reformatted string by replacing keywords from mail content an subject
                //    string mailSubject = dsMailFormat.Tables[0].Rows[0]["MAILSUBJECT"].ToString();
                    string mailBody = GetReFormattedStringByReplacingKeywords(dsMailFormat.Tables[0].Rows[0]["MAILFORMAT"].ToString(), oKeyWords);

                    if (mailTo != string.Empty)
                    {
                        Thread mailthreadApp = new Thread(() => Others.SendMail.SendMailContent(mailFrom,mailTo,mailCc,mailBcc, mailSubject, mailBody));
                        mailthreadApp.Name = "Email Thread";
                        mailthreadApp.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Logger.LogInfo(ex);
            }          
          
            return bIsMailSentSuccess;
        }

        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="mailTemplateCode"></param>
        /// <param name="mailFrom"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailCc"></param>
        /// <param name="mailBcc"></param>
        /// <param name="mailSubject"></param>
        /// <param name="oKeyWords"></param>
        /// <param name="sAttachmentFileName"></param>
        /// <returns></returns>
        public static bool SendMail(string mailTemplateCode, string mailFrom, string mailTo, string mailCc, string mailBcc, string mailSubject, Hashtable oKeyWords, DataSet sAttachmentFileName)
        {
            bool bIsMailSentSuccess = true;
            try
            {
                //get mail format for the specified mail type code
                DataSet dsMailFormat = null;

                dsMailFormat = GetMailTemplates(mailTemplateCode);
                //check whether the format exists
                if (dsMailFormat != null && dsMailFormat.Tables.Count > 0 && dsMailFormat.Tables[0].Rows.Count > 0)
                {
                    //Get reformatted string by replacing keywords from mail content an subject
                    //    string mailSubject = dsMailFormat.Tables[0].Rows[0]["MAILSUBJECT"].ToString();
                    string mailBody = GetReFormattedStringByReplacingKeywords(dsMailFormat.Tables[0].Rows[0]["MAILFORMAT"].ToString(), oKeyWords);

                    if (mailTo != string.Empty)
                    {
                        Thread mailthreadApp = new Thread(() => Others.SendMail.SendMailContent(mailFrom, mailTo, mailCc, mailBcc, mailSubject, mailBody, sAttachmentFileName));
                        mailthreadApp.Name = "Email Thread";
                        mailthreadApp.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Logger.LogInfo(ex);
            }

            return bIsMailSentSuccess;
        }

        /// <summary>
        /// Getting Mail Templates based on MailTemplate Code
        /// </summary>
        /// <param name="sMailTemplateCode"></param>
        /// <returns></returns>
        public static DataSet GetMailTemplates(string sMailTemplateCode)
        {
            DataSet dsMailTemplates = new DataSet();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();
            try
            {

                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_GetMailFormatByTypeCode";
                cmdselect.Parameters.AddWithValue("@MailTypeCode", sMailTemplateCode);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dsMailTemplates);
            }
            catch (SqlException ex)
            {
                Logger.LogInfo(ex);
                return null;
            }
            finally
            {
                cmdselect.Connection.Close();
            }

            return dsMailTemplates;
        }
    }
}