using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppService18.Models;
using AppService18.ExceptionLogger;
using AppService18.Others;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.IO;
using System.Web.Configuration;
using System.Web;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace AppService18.Controllers
{
    [RoutePrefix("api/TimeSheet")]
    public class TimeSheetController : ApiController
    {
        CommonFunction commonFunction = new CommonFunction();

        /// <summary>
        /// CEMS MobileAPP Activation/Registration
        /// </summary>
        /// <param name="timeSheetRegisterMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public IHttpActionResult Register(TimeSheetModel timeSheetRegisterMdl)
        {
            List<TimeSheetModel> timeSheetRegList = new List<TimeSheetModel>();
            ResponseModel _objResponseRegModel = new ResponseModel();
            DataTable dtAPPUserLogin = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                string EncryptPassword = string.Empty;

                if (timeSheetRegisterMdl.APPPassword != null && timeSheetRegisterMdl.APPPassword != "")
                {
                    EncryptPassword = CommonFunction.EnCryptionDeCryption.Encode(timeSheetRegisterMdl.APPPassword);
                }


                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_UserActivationForMobileApp";
                cmdselect.Parameters.AddWithValue("@USERID", timeSheetRegisterMdl.UserId);
                // cmdselect.Parameters.AddWithValue("@EMAILID", timeSheetRegisterMdl.EMailId);
                cmdselect.Parameters.AddWithValue("@PASSWORD", EncryptPassword);
                cmdselect.Parameters.AddWithValue("@ACTION", timeSheetRegisterMdl.Action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtAPPUserLogin);

                timeSheetRegList = (from DataRow dr in dtAPPUserLogin.Rows
                                    select new TimeSheetModel()
                                    {

                                        UserId = dr["USERID"].ToString(),
                                        UserFullName = dr["UserFullName"].ToString(),
                                        EMailId = dr["EMAILID"].ToString(),
                                        AdminTOMail = dr["TOMAILID"].ToString(),
                                        AdminCCMail = dr["CCMAILID"].ToString(),
                                        AdminBCCMail = dr["BCCMAILID"].ToString(),
                                        AdminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                        AdminMailBody = dr["ADMINMAILBODY"].ToString(),
                                        UserMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                        UserMailBody = dr["USERMAILBODY"].ToString(),
                                        StatusFlag = Convert.ToInt32(dr["UserFlag"].ToString()),
                                        ReturnMsg = dr["ReturnMsg"].ToString()

                                    }).ToList();


                if (timeSheetRegList.Count == 0)
                {
                    _objResponseRegModel.Data = timeSheetRegList;
                    _objResponseRegModel.Status = false;
                    _objResponseRegModel.Message = "Data Not Received successfully";
                }
                else
                {
                    if (timeSheetRegList[0].EMailId != "")
                    {
                        MailTemplates.RegistrationMail(timeSheetRegList[0].UserFullName, timeSheetRegList[0].EMailId, timeSheetRegList[0].UserMailSubject, timeSheetRegList[0].UserMailBody);
                    }
                    _objResponseRegModel.Data = timeSheetRegList;
                    _objResponseRegModel.Status = true;
                    _objResponseRegModel.Message = "Data Received successfully";
                }


            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
            finally
            {
                cmdselect.Connection.Close();
            }
            return Json(_objResponseRegModel);
        }
        /// <summary>
        /// Change and Forgot Password
        /// </summary>
        /// <param name="timeSheetChangePwdMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(TimeSheetModel timeSheetChangePwdMdl)
        {
            List<TimeSheetModel> timeSheetChangePwdList = new List<TimeSheetModel>();
            ResponseModel _objResponseChangePwdModel = new ResponseModel();
            DataTable dtChangePwdData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                string EncryptOldPassword = string.Empty;
                string EncryptNewPassword = string.Empty;
                if (timeSheetChangePwdMdl.Action == "FORGOTPWD") // If Forgot password only set to default password
                {
                    timeSheetChangePwdMdl.NewAPPPassword = System.Configuration.ConfigurationManager.AppSettings["DefaultAPPPassword"].ToString();
                }

                if (timeSheetChangePwdMdl.APPPassword != null && timeSheetChangePwdMdl.APPPassword != "")// Old Password Encryption
                {
                    EncryptOldPassword = CommonFunction.EnCryptionDeCryption.Encode(timeSheetChangePwdMdl.APPPassword);
                }
                if (timeSheetChangePwdMdl.NewAPPPassword != null && timeSheetChangePwdMdl.NewAPPPassword != "")// New Password Encryption
                {
                    EncryptNewPassword = CommonFunction.EnCryptionDeCryption.Encode(timeSheetChangePwdMdl.NewAPPPassword);
                }


                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_UserChangePasswordForMobileApp";
                cmdselect.Parameters.AddWithValue("@USERID", timeSheetChangePwdMdl.UserId);
                cmdselect.Parameters.AddWithValue("@OLDPASSWORD", EncryptOldPassword);
                cmdselect.Parameters.AddWithValue("@NEWPASSWORD", EncryptNewPassword);
                cmdselect.Parameters.AddWithValue("@ACTION", timeSheetChangePwdMdl.Action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtChangePwdData);

                timeSheetChangePwdList = (from DataRow dr in dtChangePwdData.Rows
                                          select new TimeSheetModel()
                                          {
                                              UID = Convert.ToInt64(dr["UID"].ToString()),
                                              UserId = dr["USERID"].ToString(),
                                              UserFullName = dr["UserFullName"].ToString(),
                                              EMailId = dr["EMAILID"].ToString(),
                                              DefaultAPPPassword = System.Configuration.ConfigurationManager.AppSettings["DefaultAPPPassword"].ToString(),
                                              AdminTOMail = dr["TOMAILID"].ToString(),
                                              AdminCCMail = dr["CCMAILID"].ToString(),
                                              AdminBCCMail = dr["BCCMAILID"].ToString(),
                                              AdminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                              AdminMailBody = dr["ADMINMAILBODY"].ToString(),
                                              UserMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                              UserMailBody = dr["USERMAILBODY"].ToString(),
                                              StatusFlag = Convert.ToInt32(dr["UserFlag"].ToString()),
                                              ReturnMsg = dr["ReturnMsg"].ToString()

                                          }).ToList();


                if (timeSheetChangePwdList.Count == 0)
                {
                    _objResponseChangePwdModel.Data = timeSheetChangePwdList;
                    _objResponseChangePwdModel.Status = false;
                    _objResponseChangePwdModel.Message = "Data Not Received successfully";
                }
                else
                {
                    if(timeSheetChangePwdMdl.Action == "FORGOTPWD")
                    {
                        if (timeSheetChangePwdList[0].EMailId != "")
                        {
                            MailTemplates.ForgotMail(timeSheetChangePwdList[0].UserFullName, timeSheetChangePwdList[0].EMailId,
                            timeSheetChangePwdList[0].UserMailSubject, timeSheetChangePwdList[0].UserMailBody, timeSheetChangePwdList[0].DefaultAPPPassword);
                        }
                    }
                    _objResponseChangePwdModel.Data = timeSheetChangePwdList;
                    _objResponseChangePwdModel.Status = true;
                    _objResponseChangePwdModel.Message = "Data Received successfully";
                }


            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
            finally
            {
                cmdselect.Connection.Close();
            }
            return Json(_objResponseChangePwdModel);
        }
        /// <summary>
        /// CEMS MobileAPP Login
        /// </summary>
        /// <param name="userPwdStatusMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login(TimeSheetModel timeSheetLoginMdl)
        {

            List<TimeSheetModel> timeSheetLoginList = new List<TimeSheetModel>();
            ResponseModel _objResponseLoginModel = new ResponseModel();
            DataTable dtUserPwdStatus = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                string EncryptPassword = string.Empty;

                if (timeSheetLoginMdl.APPPassword != null && timeSheetLoginMdl.APPPassword != "")
                {
                    EncryptPassword = CommonFunction.EnCryptionDeCryption.Encode(timeSheetLoginMdl.APPPassword);
                }

                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_LoginForMobileApp";
                cmdselect.Parameters.AddWithValue("@USERID", timeSheetLoginMdl.UserId);
                cmdselect.Parameters.AddWithValue("@PASSWORD", EncryptPassword);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtUserPwdStatus);

                timeSheetLoginList = (from DataRow dr in dtUserPwdStatus.Rows
                                      select new TimeSheetModel()
                                      {
                                          StatusFlag = Convert.ToInt32(dr["PWDStatusFlag"].ToString()),
                                          ReturnMsg = dr["ReturnMsg"].ToString(),
                                          UID = Convert.ToInt64(dr["UID"]),
                                          RoleId = Convert.ToInt64(dr["ROLEID"]),
                                          UserId = dr["USERID"].ToString(),
                                          UserFullName = dr["UserFullName"].ToString(),
                                          FirstName=dr["FNAME"].ToString(),
                                          LastName = dr["LNAME"].ToString(),
                                          EMailId = dr["EMAILID"].ToString(),
                                          WorkAuthType = dr["WORKAUTHORIZATIONTYPE"].ToString()

                                      }).ToList();

                if (timeSheetLoginList.Count == 0)
                {
                   
                    _objResponseLoginModel.Data = timeSheetLoginList;
                    _objResponseLoginModel.Status = false;
                    _objResponseLoginModel.Message = "Data Not Received successfully";
                    
                }
                else
                {
                    _objResponseLoginModel.Data = timeSheetLoginList;
                    _objResponseLoginModel.Status = true;
                    _objResponseLoginModel.Message = "Data Received successfully";

                }


            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
            finally
            {
                cmdselect.Connection.Close();
            }
            return Json(_objResponseLoginModel);
        }
        /// <summary>
        /// Save TimeSheet Details
        /// </summary>
        /// <param name="saveTimeSheetMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveTimeSheetDetails")]
        public IHttpActionResult SaveTimeSheetDetails(TimeSheetModel saveTimeSheetMdl)
        {

            List<TimeSheetModel> saveTimeSheetList = new List<TimeSheetModel>();
            ResponseModel _objResponseSaveTSMdl = new ResponseModel();
            DataSet dsUserEmail = new DataSet();
            DataTable dtSaveTimeSheetData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {

                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_SaveTimeSheetDetailsForMobileApp";
                cmdselect.Parameters.AddWithValue("@PayHoursId", saveTimeSheetMdl.PayHoursId);
                cmdselect.Parameters.AddWithValue("@UID", saveTimeSheetMdl.UID);
                cmdselect.Parameters.AddWithValue("@PayHoursMonth", saveTimeSheetMdl.PayHoursMonth);
                cmdselect.Parameters.AddWithValue("@PayPeriod", saveTimeSheetMdl.PayPeriod);
                cmdselect.Parameters.AddWithValue("@PayHours", saveTimeSheetMdl.PayHours);
                cmdselect.Parameters.AddWithValue("@SubmitStatus", saveTimeSheetMdl.SubmitStatus);
                cmdselect.Parameters.AddWithValue("@ApprovalStatus", saveTimeSheetMdl.ApprovalStatus);
                cmdselect.Parameters.AddWithValue("@Comments", saveTimeSheetMdl.Comments);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", saveTimeSheetMdl.UserId);
                cmdselect.Parameters.AddWithValue("@Action", saveTimeSheetMdl.Action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtSaveTimeSheetData);

                saveTimeSheetList = (from DataRow dr in dtSaveTimeSheetData.Rows
                                     select new TimeSheetModel()
                                     {

                                         StatusFlag = Convert.ToInt32(dr["MSG"].ToString()),
                                         ReturnMsg = dr["ResultMsg"].ToString(),
                                         PayHoursYear = dr["PayHoursYear"].ToString(),
                                         AdminTOMail = dr["TOMAILID"].ToString(),
                                         AdminCCMail = dr["CCMAILID"].ToString(),
                                         AdminBCCMail = dr["BCCMAILID"].ToString(),
                                         AdminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                         AdminMailBody = dr["ADMINMAILBODY"].ToString(),
                                         UserMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                         UserMailBody = dr["USERMAILBODY"].ToString()

                                     }).ToList();

                if (saveTimeSheetList.Count == 0)
                {
                    _objResponseSaveTSMdl.Data = saveTimeSheetList;
                    _objResponseSaveTSMdl.Status = false;
                    _objResponseSaveTSMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    if (dtSaveTimeSheetData.Rows[0]["MSG"].ToString() == "1" || dtSaveTimeSheetData.Rows[0]["MSG"].ToString() == "3")
                    {
                        dsUserEmail = commonFunction.GetUserEmail(saveTimeSheetMdl.UID, null);


                        if (dsUserEmail.Tables[0].Rows.Count > 0)
                        {
                            if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                            {
                                if (dtSaveTimeSheetData.Rows.Count > 0)
                                {
                                    if (dtSaveTimeSheetData.Rows[0]["MSG"].ToString() == "3")
                                    {
                                        saveTimeSheetMdl.PayHoursMonth = dtSaveTimeSheetData.Rows[0]["PayHoursMonth"].ToString();
                                        saveTimeSheetMdl.PayPeriod = dtSaveTimeSheetData.Rows[0]["PayPeriod"].ToString();
                                        saveTimeSheetMdl.PayHours = dtSaveTimeSheetData.Rows[0]["PayHours"].ToString();
                                    }
                                }

                                // User Mail
                                MailTemplates.PayPeriodHoursMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(), string.Empty, string.Empty,
                                   saveTimeSheetList[0].PayHoursYear, saveTimeSheetMdl.PayHoursMonth, saveTimeSheetMdl.PayPeriod, saveTimeSheetMdl.PayHours, dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                   dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), string.Empty, saveTimeSheetList[0].UserMailSubject, saveTimeSheetList[0].UserMailBody, string.Empty,
                                   string.Empty, string.Empty, false);

                                // Admin Mail

                                MailTemplates.PayPeriodHoursMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(), dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(),
                                    saveTimeSheetList[0].PayHoursYear, saveTimeSheetMdl.PayHoursMonth, saveTimeSheetMdl.PayPeriod, saveTimeSheetMdl.PayHours, dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                   saveTimeSheetList[0].AdminTOMail, saveTimeSheetList[0].AdminCCMail, string.Empty, string.Empty,
                                   saveTimeSheetList[0].AdminMailSubject, saveTimeSheetList[0].AdminMailBody, string.Empty, true);
                            }

                        }
                    }
                    _objResponseSaveTSMdl.Data = saveTimeSheetList;
                    _objResponseSaveTSMdl.Status = true;
                    _objResponseSaveTSMdl.Message = "Data Received successfully";
                }


            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
            finally
            {
                cmdselect.Connection.Close();
            }
            return Json(_objResponseSaveTSMdl);
        }

        /// <summary>
        /// Check TimeSheet Exists or not based on UID,Year and Month
        /// </summary>
        /// <param name="checkTimeSheetMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CheckTimeSheetExists")]
        public IHttpActionResult CheckTimeSheetExists(TimeSheetModel checkTimeSheetMdl)
        {


            List<TimeSheetModel> timeSheetExistsList = new List<TimeSheetModel>();
            ResponseModel _objResponseTSExistsMdl = new ResponseModel();
            DataTable dtCheckTimeSheetData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_CheckTimeSheetExistsForMobileApp";
                cmdselect.Parameters.AddWithValue("@UID", checkTimeSheetMdl.UID);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtCheckTimeSheetData);

                timeSheetExistsList = (from DataRow dr in dtCheckTimeSheetData.Rows
                                       select new TimeSheetModel()
                                       {
                                           UID = Convert.ToInt64(dr["UID"]),
                                           StatusFlag = Convert.ToInt32(dr["MSG"].ToString()),
                                           PayPeriod = dr["DateRange"].ToString(),
                                           PayHoursMonth = dr["CurrentMonth"].ToString()

                                       }).ToList();

                if (timeSheetExistsList.Count == 0)
                {
                    _objResponseTSExistsMdl.Data = timeSheetExistsList;
                    _objResponseTSExistsMdl.Status = false;
                    _objResponseTSExistsMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    _objResponseTSExistsMdl.Data = timeSheetExistsList;
                    _objResponseTSExistsMdl.Status = true;
                    _objResponseTSExistsMdl.Message = "Data Received successfully";
                }

            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
            finally
            {
                cmdselect.Connection.Close();
            }

            return Json(_objResponseTSExistsMdl);
        }
        /// <summary>
        /// Getting TimeSheet Details
        /// </summary>
        /// <param name="timeSheetDetailsMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetTimeSheetDetails")]
        public IHttpActionResult GetTimeSheetDetails(TimeSheetModel timeSheetDetailsMdl)
        {


            List<TimeSheetModel> timeSheetDetailsList = new List<TimeSheetModel>();
            ResponseModel _objResponseTSDetailMdl = new ResponseModel();
            DataTable dtgetTimeSheetData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_GetTimeSheetDetailsForMobileApp";
                cmdselect.Parameters.AddWithValue("@UID", timeSheetDetailsMdl.UID);
                cmdselect.Parameters.AddWithValue("@PayHoursId", timeSheetDetailsMdl.PayHoursId);
                cmdselect.Parameters.AddWithValue("@Action", timeSheetDetailsMdl.Action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtgetTimeSheetData);
                if (dtgetTimeSheetData != null && dtgetTimeSheetData.Rows.Count > 0)
                {
                    //Getting TimeSheet Details based on UID
                    if (dtgetTimeSheetData.Rows[0]["Action"].ToString() == "GETALLTIMESHEETSBYUID")
                    {
                        timeSheetDetailsList = (from DataRow dr in dtgetTimeSheetData.Rows
                                                select new TimeSheetModel()
                                                {

                                                    UID = Convert.ToInt32(dr["UID"].ToString()),
                                                    PayHoursId = Convert.ToInt32(dr["PAYHOURSID"].ToString()),
                                                    UserFullName = dr["FULLNAME"].ToString(),
                                                    WorkAuthType = dr["WORKAUTHORIZATIONTYPE"].ToString(),
                                                    PayHoursYear = dr["PAYHOURSYEAR"].ToString(),
                                                    PayHoursMonth = dr["PAYHOURSMONTH"].ToString(),
                                                    PayPeriod = dr["PAYPERIOD"].ToString(),
                                                    PayHours = dr["PAYHOURS"].ToString(),
                                                    SubmitStatus = dr["SUBMITSTATUS"].ToString(),
                                                    ApprovalStatus = dr["APPROVALSTATUS"].ToString(),
                                                    ExpiredStatus = Convert.ToInt32(dr["EXPIREDSTATUS"].ToString()),
                                                    Action = dr["Action"].ToString()

                                                }).ToList();
                    }
                    //Getting TimeSheet UserId's
                    else if (dtgetTimeSheetData.Rows[0]["Action"].ToString() == "BINDSEARCHTSUSERS")
                    {
                        timeSheetDetailsList = (from DataRow dr in dtgetTimeSheetData.Rows
                                                select new TimeSheetModel()
                                                {
                                                    UID = Convert.ToInt64(dr["UID"]),
                                                    UserFullName = dr["FULLNAME"].ToString(),
                                                    Action = dr["Action"].ToString()

                                                }).ToList();
                    }
                }
                if (timeSheetDetailsList.Count == 0)
                {
                    _objResponseTSDetailMdl.Data = timeSheetDetailsList;
                    _objResponseTSDetailMdl.Status = false;
                    _objResponseTSDetailMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    _objResponseTSDetailMdl.Data = timeSheetDetailsList;
                    _objResponseTSDetailMdl.Status = true;
                    _objResponseTSDetailMdl.Message = "Data Received successfully";
                }


            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
            finally
            {
                cmdselect.Connection.Close();
            }
            return Json(_objResponseTSDetailMdl);
        }
        /// <summary>
        /// Getting TimeSheet Document Details
        /// </summary>
        /// <param name="timeSheetDetailsMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetTimeSheetDocumentDetails")]
        public IHttpActionResult GetTimeSheetDocumentDetails(TimeSheetDocModel TSDocDetailsMdl)
        {


            List<TimeSheetDocModel> timeSheetDocsList = new List<TimeSheetDocModel>();
            ResponseModel _objResponseTSDocsMdl = new ResponseModel();
            DataTable dtgetTimeSheetDocs = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_GetTimeSheetDocumentsForMobileApp";
                cmdselect.Parameters.AddWithValue("@UID", TSDocDetailsMdl.UID);
                cmdselect.Parameters.AddWithValue("@TimeSheetDocId", TSDocDetailsMdl.TimeSheetDocId);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", TSDocDetailsMdl.UserId);
                cmdselect.Parameters.AddWithValue("@Action", TSDocDetailsMdl.DocAction);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtgetTimeSheetDocs);
                if (dtgetTimeSheetDocs != null && dtgetTimeSheetDocs.Rows.Count > 0)
                {
                    //Getting TimeSheet Documents Details based on UID
                    if (dtgetTimeSheetDocs.Rows[0]["Action"].ToString() == "GETTSDOCUMENTS")
                    {
                        timeSheetDocsList = (from DataRow dr in dtgetTimeSheetDocs.Rows
                                             select new TimeSheetDocModel()
                                             {

                                                 UID = Convert.ToInt32(dr["UID"].ToString()),
                                                 TimeSheetDocId = Convert.ToInt32(dr["TIMESHEETDOCID"].ToString()),
                                                 DocumentFileName = dr["DOCUMENTFILENAME"].ToString(),
                                                 DocumentName = dr["DOCUMENTNAME"].ToString(),
                                                 DocumentUrl = dr["DOCUMENTURL"].ToString(),
                                                 StartDate = dr["STARTDATE"].ToString(),
                                                 EndDate = dr["ENDDATE"].ToString(),
                                                 Month = dr["MONTH"].ToString(),
                                                 DocSubmitStatus = dr["SUBMITSTATUS"].ToString(),
                                                 DocReturnMsg = dr["ResultMsg"].ToString(),
                                                 DocStatusFlag = Convert.ToInt32(dr["MSG"].ToString()),
                                                 DocCreatedDate = dr["CREATEDONDATE"].ToString()

                                             }).ToList();
                    }
                    //Getting TimeSheet Documents Timesheet DocumentId
                    else if (dtgetTimeSheetDocs.Rows[0]["Action"].ToString() == "EDITTSDOCUMENT")
                    {
                        timeSheetDocsList = (from DataRow dr in dtgetTimeSheetDocs.Rows
                                             select new TimeSheetDocModel()
                                             {
                                                 UID = Convert.ToInt32(dr["UID"].ToString()),
                                                 TimeSheetDocId = Convert.ToInt32(dr["TIMESHEETDOCID"].ToString()),
                                                 DocumentFileName = dr["DOCUMENTFILENAME"].ToString(),
                                                 DocumentName = dr["DOCUMENTNAME"].ToString(),
                                                 DocumentUrl = dr["DOCUMENTURL"].ToString(),
                                                 StartDate = dr["STARTDATE"].ToString(),
                                                 EndDate = dr["ENDDATE"].ToString(),
                                                 Month = dr["MONTH"].ToString(),
                                                 DocSubmitStatus = dr["SUBMITSTATUS"].ToString(),
                                                 DocReturnMsg = dr["ResultMsg"].ToString(),
                                                 DocStatusFlag = Convert.ToInt32(dr["MSG"].ToString()),
                                                 DocCreatedDate = dr["CREATEDONDATE"].ToString()
                                             }).ToList();
                    }
                    //deactivate TimeSheet Documents based on UID
                    else if (dtgetTimeSheetDocs.Rows[0]["Action"].ToString() == "DELETETSDOCUMENT")
                    {
                        timeSheetDocsList = (from DataRow dr in dtgetTimeSheetDocs.Rows
                                             select new TimeSheetDocModel()
                                             {
                                                 DocReturnMsg = dr["ResultMsg"].ToString(),
                                                 DocStatusFlag = Convert.ToInt32(dr["MSG"].ToString())

                                             }).ToList();
                    }
                    //Delete TimeSheet Document based Timesheet DocumentId
                    else if (dtgetTimeSheetDocs.Rows[0]["Action"].ToString() == "DELETEFILE")
                    {
                        timeSheetDocsList = (from DataRow dr in dtgetTimeSheetDocs.Rows
                                             select new TimeSheetDocModel()
                                             {
                                                 DocReturnMsg = dr["ResultMsg"].ToString(),
                                                 DocStatusFlag = Convert.ToInt32(dr["MSG"].ToString())

                                             }).ToList();
                    }
                }
                if (timeSheetDocsList.Count == 0)
                {
                    _objResponseTSDocsMdl.Data = timeSheetDocsList;
                    _objResponseTSDocsMdl.Status = false;
                    _objResponseTSDocsMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    _objResponseTSDocsMdl.Data = timeSheetDocsList;
                    _objResponseTSDocsMdl.Status = true;
                    _objResponseTSDocsMdl.Message = "Data Received successfully";
                }


            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
            finally
            {
                cmdselect.Connection.Close();
            }
            return Json(_objResponseTSDocsMdl);
        }
        /// <summary>
        /// Update TimeSheet Document Dates
        /// </summary>
        /// <param name="timeSheetDatesMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateTimeSheetDates")]
        public IHttpActionResult UpdateTimeSheetDates(TimeSheetDocModel timeSheetDatesMdl)
        {


            List<TimeSheetDocModel> timeSheetDatesList = new List<TimeSheetDocModel>();
            ResponseModel _objResponseTSDatesMdl = new ResponseModel();
            DataSet dsUserEmail = new DataSet();
            DataTable dtTimeSheetDatesData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_SaveTimeSheetDocumentForMobileApp";
                cmdselect.Parameters.AddWithValue("@UID", timeSheetDatesMdl.UID);
                cmdselect.Parameters.AddWithValue("@TimeSheetDocId", timeSheetDatesMdl.TimeSheetDocId);
                cmdselect.Parameters.AddWithValue("@TSDocStartDate", timeSheetDatesMdl.StartDate);
                cmdselect.Parameters.AddWithValue("@TSDocEndDate", timeSheetDatesMdl.EndDate);
                cmdselect.Parameters.AddWithValue("@TSMonth", timeSheetDatesMdl.Month);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", timeSheetDatesMdl.UserId);
                cmdselect.Parameters.AddWithValue("@Action", timeSheetDatesMdl.DocAction);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtTimeSheetDatesData);
                if (dtTimeSheetDatesData != null && dtTimeSheetDatesData.Rows.Count > 0)
                {

                    timeSheetDatesList = (from DataRow dr in dtTimeSheetDatesData.Rows
                                          select new TimeSheetDocModel()
                                          {
                                              DocReturnMsg = dr["ResultMsg"].ToString(),
                                              DocStatusFlag = Convert.ToInt32(dr["MSG"].ToString()),
                                              AdminTOMail = dr["TOMAILID"].ToString(),
                                              AdminCCMail = dr["CCMAILID"].ToString(),
                                              AdminBCCMail = dr["BCCMAILID"].ToString(),
                                              AdminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                              AdminMailBody = dr["ADMINMAILBODY"].ToString(),
                                              UserMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                              UserMailBody = dr["USERMAILBODY"].ToString(),
                                              TimeSheetDocId = Convert.ToInt64(dr["TSDOCUMENTID"])

                                          }).ToList();



                }
                if (timeSheetDatesList.Count == 0)
                {
                    _objResponseTSDatesMdl.Data = timeSheetDatesList;
                    _objResponseTSDatesMdl.Status = false;
                    _objResponseTSDatesMdl.Message = "Data Not Received successfully";
                }
                else
                {

                    dsUserEmail = commonFunction.GetUserEmail(timeSheetDatesMdl.UID, null);


                    if (dsUserEmail.Tables[0].Rows.Count > 0)
                    {
                        if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                        {
                            // User Mail
                            MailTemplates.TimeSheetDocumentMail(timeSheetDatesMdl.UID, timeSheetDatesList[0].TimeSheetDocId,dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(),
                                dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), timeSheetDatesMdl.StartDate, timeSheetDatesMdl.EndDate, dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                               dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), string.Empty, timeSheetDatesList[0].UserMailSubject, timeSheetDatesList[0].UserMailBody, string.Empty,
                               string.Empty, string.Empty, false);

                            // Admin Mail

                            MailTemplates.TimeSheetDocumentMail(timeSheetDatesMdl.UID, timeSheetDatesList[0].TimeSheetDocId,dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(),
                                 dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), timeSheetDatesMdl.StartDate, timeSheetDatesMdl.EndDate, dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                  timeSheetDatesList[0].AdminTOMail, timeSheetDatesList[0].AdminCCMail, string.Empty, string.Empty, timeSheetDatesList[0].AdminMailSubject, timeSheetDatesList[0].AdminMailBody, string.Empty, true);
                        }

                    }


                    _objResponseTSDatesMdl.Data = timeSheetDatesList;
                    _objResponseTSDatesMdl.Status = true;
                    _objResponseTSDatesMdl.Message = "Data Received successfully";
                }


            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }
            finally
            {
                cmdselect.Connection.Close();
            }
            return Json(_objResponseTSDatesMdl);
        }
        /// <summary>  
        /// Upload Document.....  
        /// </summary>        
        /// <returns></returns>  
        [Route("UploadFile")]
        [HttpPost]
        public HttpResponseMessage UploadFile()
        {
            TimeSheetDocModel uploadTimesheetDocMdl = new TimeSheetDocModel();
            List<TimeSheetDocModel> TSUploadDocsList = new List<TimeSheetDocModel>();
            ResponseModel _objResponseTSUploadMdl = new ResponseModel();
            DataSet dsUserEmail = new DataSet();
            DataTable dtTimeSheetUploadFiles = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();          
            string UserName = string.Empty;
            string OriginalFileName = string.Empty;
            string PDFConvertFilePath = string.Empty;


            NameValueCollection form = HttpContext.Current.Request.Form;
            try
            {
                foreach (string key in form.AllKeys)
                {
                    if (key == "UID")
                    {
                        uploadTimesheetDocMdl.UID = Convert.ToInt64(form[key]);
                    }
                    else if (key == "TimeSheetDocId")
                    {
                        uploadTimesheetDocMdl.TimeSheetDocId = Convert.ToInt64(form[key]);
                    }
                    else if (key == "UserName")
                    {
                        UserName = form[key];
                    }
                    else if (key == "Month")
                    {
                        uploadTimesheetDocMdl.Month = form[key];
                    }
                    else if (key == "StartDate")
                    {
                        uploadTimesheetDocMdl.StartDate = form[key];
                    }
                    else if (key == "EndDate")
                    {
                        uploadTimesheetDocMdl.EndDate = form[key];
                    }
                    else if (key == "UserId")
                    {
                        uploadTimesheetDocMdl.UserId = form[key];
                    }
                    else if (key == "DocAction")
                    {
                        uploadTimesheetDocMdl.DocAction = form[key];
                    }
                }


                string _UploadTimerVal = string.Empty;
                string SaveFileToFolder = string.Empty;
                var message = string.Empty;
                string folderPath = System.Configuration.ConfigurationManager.AppSettings["TimeSheetFileUploadPath"] + uploadTimesheetDocMdl.UID + "_" + UserName;
                Dictionary<string, object> dict = new Dictionary<string, object>();

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 25; //Size = 25 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png", ".jpeg", ".pdf", ".docx", ".doc", ".xls", ".csv", ".xlsx", ".pptx", ".ppt" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            message = string.Format("Please Upload image of type .jpg", ".gif", ".png", ".jpeg", ".pdf", ".docx", ".doc", ".xls", ".csv", ".xlsx", ".pptx", ".ppt");

                            dict.Add("Message", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            message = string.Format("Please Upload a file upto 25 mb.");

                            dict.Add("Message", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            if (!Directory.Exists(folderPath))
                            {
                                //If Directory (Folder) does not exists. Create it.
                                Directory.CreateDirectory(folderPath);
                            }

                            _UploadTimerVal = DateTime.Now.ToString("yyyyMMddHHmmss");

                            OriginalFileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                            var OriginalfilePath = (folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + postedFile.FileName);

                            postedFile.SaveAs(OriginalfilePath); // Saving file in Sever
                            // Pdf Files
                            if (extension == ".pdf")
                            {
                                uploadTimesheetDocMdl.DocumentName = postedFile.FileName;
                                uploadTimesheetDocMdl.DocumentFileName = "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + postedFile.FileName;
                                uploadTimesheetDocMdl.DocumentUrl = folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + postedFile.FileName;
                            }
                            // Convert Word to Pdf
                            else if (extension == ".docx" || extension == ".doc")
                            {
                                PDFConvertFilePath = folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                commonFunction.WordtoPDF(OriginalfilePath, PDFConvertFilePath);
                                uploadTimesheetDocMdl.DocumentName = OriginalFileName + ".pdf";
                                uploadTimesheetDocMdl.DocumentFileName = "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                uploadTimesheetDocMdl.DocumentUrl = folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                System.IO.File.Delete(OriginalfilePath);
                            }
                            // Convert Excel to Pdf
                            else if (extension == ".xls" || extension == ".csv" || extension == ".xlsx")
                            {
                                PDFConvertFilePath = folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                commonFunction.ExceltoPDF(OriginalfilePath, PDFConvertFilePath);
                                uploadTimesheetDocMdl.DocumentName = OriginalFileName + ".pdf";
                                uploadTimesheetDocMdl.DocumentFileName = "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                uploadTimesheetDocMdl.DocumentUrl = folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                System.IO.File.Delete(OriginalfilePath);
                            }
                            // Convert PPT to Pdf
                            else if (extension == ".pptx" || extension == ".ppt")
                            {
                                PDFConvertFilePath = folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                commonFunction.PPTtoPDF(OriginalfilePath, PDFConvertFilePath);
                                uploadTimesheetDocMdl.DocumentName = OriginalFileName + ".pdf";
                                uploadTimesheetDocMdl.DocumentFileName = "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                uploadTimesheetDocMdl.DocumentUrl = folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                System.IO.File.Delete(OriginalfilePath);
                            }
                            // Convert Image to Pdf
                            else if (extension == ".jpg" || extension == ".gif" || extension == ".png" || extension == ".jpeg")
                            {
                                PDFConvertFilePath = folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                commonFunction.ConvertImageToPdf(OriginalfilePath, PDFConvertFilePath);
                                uploadTimesheetDocMdl.DocumentName = OriginalFileName + ".pdf";
                                uploadTimesheetDocMdl.DocumentFileName = "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                uploadTimesheetDocMdl.DocumentUrl = folderPath + "\\" + "TimeSheet" + "_" + uploadTimesheetDocMdl.UID + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                                System.IO.File.Delete(OriginalfilePath);
                            }

                            // Saving filedata in Database
                            cmdselect.CommandType = CommandType.StoredProcedure;
                            cmdselect.CommandText = "sp_SaveTimeSheetDocumentForMobileApp";
                            cmdselect.Parameters.AddWithValue("@UID", uploadTimesheetDocMdl.UID);
                            cmdselect.Parameters.AddWithValue("@TimeSheetDocId", uploadTimesheetDocMdl.TimeSheetDocId);
                            cmdselect.Parameters.AddWithValue("@TSDocStartDate", uploadTimesheetDocMdl.StartDate);
                            cmdselect.Parameters.AddWithValue("@TSDocEndDate", uploadTimesheetDocMdl.EndDate);
                            cmdselect.Parameters.AddWithValue("@TSMonth", uploadTimesheetDocMdl.Month);
                            cmdselect.Parameters.AddWithValue("@DocumentUrl", uploadTimesheetDocMdl.DocumentUrl);
                            cmdselect.Parameters.AddWithValue("@DocumentName", uploadTimesheetDocMdl.DocumentName);
                            cmdselect.Parameters.AddWithValue("@DocumentFileName", uploadTimesheetDocMdl.DocumentFileName);
                            cmdselect.Parameters.AddWithValue("@LoggedInUserId", uploadTimesheetDocMdl.UserId);
                            cmdselect.Parameters.AddWithValue("@Action", uploadTimesheetDocMdl.DocAction);
                            cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                            cmdselect.Connection.Open();
                            da = new SqlDataAdapter(cmdselect);
                            da.Fill(dtTimeSheetUploadFiles);

                            if (dtTimeSheetUploadFiles != null && dtTimeSheetUploadFiles.Rows.Count > 0)
                            {
                                TSUploadDocsList = (from DataRow dr in dtTimeSheetUploadFiles.Rows
                                                    select new TimeSheetDocModel()
                                                    {
                                                        DocReturnMsg = dr["ResultMsg"].ToString(),
                                                        DocStatusFlag = Convert.ToInt32(dr["MSG"].ToString()),
                                                        AdminTOMail = dr["TOMAILID"].ToString(),
                                                        AdminCCMail = dr["CCMAILID"].ToString(),
                                                        AdminBCCMail = dr["BCCMAILID"].ToString(),
                                                        AdminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                                        AdminMailBody = dr["ADMINMAILBODY"].ToString(),
                                                        UserMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                                        UserMailBody = dr["USERMAILBODY"].ToString(),
                                                        TimeSheetDocId = Convert.ToInt64(dr["TSDOCUMENTID"])

                                                    }).ToList();
                            }
                            if (TSUploadDocsList.Count == 0)
                            {
                                _objResponseTSUploadMdl.Data = TSUploadDocsList;
                                _objResponseTSUploadMdl.Status = false;
                                _objResponseTSUploadMdl.Message = "Data Not Received successfully";

                                dict.Add("Message", "File not uploaded successfully");
                                dict.Add("AdminTOMail", "");
                                dict.Add("AdminCCMail", "");
                                dict.Add("AdminBCCMail", "");
                                dict.Add("AdminMailSubject", "");
                                dict.Add("AdminMailBody", "");
                                dict.Add("UserMailSubject", "");
                                dict.Add("UserMailBody", "");

                            }
                            else
                            {

                                dsUserEmail = commonFunction.GetUserEmail(uploadTimesheetDocMdl.UID, null);


                                if (dsUserEmail.Tables[0].Rows.Count > 0)
                                {
                                    if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                                    {
                                        // User Mail
                                        MailTemplates.TimeSheetDocumentMail(uploadTimesheetDocMdl.UID, TSUploadDocsList[0].TimeSheetDocId, dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(),
                                            dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), uploadTimesheetDocMdl.StartDate, uploadTimesheetDocMdl.EndDate, dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                           dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), string.Empty, TSUploadDocsList[0].UserMailSubject, TSUploadDocsList[0].UserMailBody, string.Empty,
                                           string.Empty, string.Empty, false);

                                        // Admin Mail

                                        MailTemplates.TimeSheetDocumentMail(uploadTimesheetDocMdl.UID, TSUploadDocsList[0].TimeSheetDocId, dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(),
                                             dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), uploadTimesheetDocMdl.StartDate, uploadTimesheetDocMdl.EndDate, dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                              TSUploadDocsList[0].AdminTOMail, TSUploadDocsList[0].AdminCCMail, string.Empty,string.Empty, TSUploadDocsList[0].AdminMailSubject, TSUploadDocsList[0].AdminMailBody,string.Empty, true);
                                    }

                                }

                                // postedFile.SaveAs(filePath); // Saving file in Sever
                                _objResponseTSUploadMdl.Data = TSUploadDocsList;
                                _objResponseTSUploadMdl.Status = true;
                                _objResponseTSUploadMdl.Message = "Data Received successfully";

                                dict.Add("Message", "File uploaded successfully");
                                dict.Add("AdminTOMail", TSUploadDocsList[0].AdminTOMail);
                                dict.Add("AdminCCMail", TSUploadDocsList[0].AdminCCMail);
                                dict.Add("AdminBCCMail", TSUploadDocsList[0].AdminBCCMail);
                                dict.Add("AdminMailSubject", TSUploadDocsList[0].AdminMailSubject);
                                dict.Add("AdminMailBody", TSUploadDocsList[0].AdminMailBody);
                                dict.Add("UserMailSubject", TSUploadDocsList[0].UserMailSubject);
                                dict.Add("UserMailBody", TSUploadDocsList[0].UserMailBody);
                            }




                        }
                    }


                    //  message = string.Format("File uploaded successfully.");

                    return Request.CreateResponse(HttpStatusCode.Created, dict);
                }
                var res = string.Format("Please upload a file.");
                dict.Add("message", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
                //var res = string.Format("some Message");
                //dict.Add("error", res);
                //return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            finally
            {
                cmdselect.Connection.Close();
            }
        }
        [Route("GetFileName")]
        [HttpPost]
        /// <summary>
        /// Function to get byte array from a file
        /// </summary>
        /// <param name="_FileName">File name to get byte array</param>
        /// <returns>Byte Array</returns>
        public IHttpActionResult FileToByteArray(TimeSheetDocModel TSFileViewMdl)
        {
            DataTable dtFileViewData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();
            ResponseModel _objResponseTSViewDocsMdl = new ResponseModel();

            try
            {

                byte[] _Buffer = null;

                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_GetTimeSheetDocumentsForMobileApp";
                cmdselect.Parameters.AddWithValue("@UID", TSFileViewMdl.UID);
                cmdselect.Parameters.AddWithValue("@TimeSheetDocId", TSFileViewMdl.TimeSheetDocId);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", TSFileViewMdl.UserId);
                cmdselect.Parameters.AddWithValue("@Action", TSFileViewMdl.DocAction);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtFileViewData);
                if (dtFileViewData != null && dtFileViewData.Rows.Count > 0)
                {

                    //Getting TimeSheet Documents Details based on UID
                    if (dtFileViewData.Rows[0]["Action"].ToString() == "FILEVIEW")
                    {
                        if (dtFileViewData.Rows[0]["DOCUMENTURL"].ToString() != "")
                        {

                            // Open file for reading
                            System.IO.FileStream _FileStream = new System.IO.FileStream(dtFileViewData.Rows[0]["DOCUMENTURL"].ToString(), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                            // attach filestream to binary reader
                            System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);

                            // get total byte length of the file
                            long _TotalBytes = new System.IO.FileInfo(dtFileViewData.Rows[0]["DOCUMENTURL"].ToString()).Length;

                            // read entire file into buffer
                            _Buffer = _BinaryReader.ReadBytes((Int32)_TotalBytes);

                            // close file reader
                            _FileStream.Close();
                            _FileStream.Dispose();
                            _BinaryReader.Close();
                            TSFileViewMdl.Document = _Buffer;
                            TSFileViewMdl.UID = Convert.ToInt64(dtFileViewData.Rows[0]["UID"].ToString());
                            TSFileViewMdl.TimeSheetDocId = Convert.ToInt64(dtFileViewData.Rows[0]["TIMESHEETDOCID"].ToString());
                            TSFileViewMdl.DocumentName = dtFileViewData.Rows[0]["DOCUMENTNAME"].ToString();
                            TSFileViewMdl.DocumentFileName = dtFileViewData.Rows[0]["DOCUMENTFILENAME"].ToString();
                            TSFileViewMdl.DocumentUrl = dtFileViewData.Rows[0]["DOCUMENTURL"].ToString();
                            TSFileViewMdl.DocStatusFlag = Convert.ToInt32(dtFileViewData.Rows[0]["MSG"].ToString());
                            TSFileViewMdl.DocReturnMsg = dtFileViewData.Rows[0]["ResultMsg"].ToString();
                            _objResponseTSViewDocsMdl.Data = TSFileViewMdl;
                            _objResponseTSViewDocsMdl.Status = true;
                            _objResponseTSViewDocsMdl.Message = "Data Received successfully";
                        }
                        else
                        {
                            _objResponseTSViewDocsMdl.Data = "[]";
                            _objResponseTSViewDocsMdl.Status = false;
                            _objResponseTSViewDocsMdl.Message = "Data Not Received successfully";
                        }
                    }

                    else
                    {
                        _objResponseTSViewDocsMdl.Data = "[]";
                        _objResponseTSViewDocsMdl.Status = false;
                        _objResponseTSViewDocsMdl.Message = "Data Not Received successfully";
                    }
                }

                else
                {
                    _objResponseTSViewDocsMdl.Data = "[]";
                    _objResponseTSViewDocsMdl.Status = false;
                    _objResponseTSViewDocsMdl.Message = "Data Not Received successfully";
                }



            }
            catch (Exception ex)
            {
                // Error
                Logger.LogInfo(ex);
                throw;
            }
            finally
            {
                cmdselect.Connection.Close();
            }
            return Json(_objResponseTSViewDocsMdl);
        }
        ///// <summary>
        ///// Mail
        ///// </summary>
        ///// <param name="mailModel"></param>
        ///// <returns></returns>
        //[Route("Mail")]
        //[HttpPost]
        //public HttpResponseMessage Mail(MailModel mailModel)
        //{
        //    bool _IsMailSuccess = false;
        //    Dictionary<string, object> mailDict = new Dictionary<string, object>();
           
        //    try
        //    {

        //        _IsMailSuccess = MailFormats.SendMail(mailModel.mailFrom, mailModel.mailTo, mailModel.mailCC, mailModel.mailBcc, mailModel.mailSubject, 
        //                         mailModel.mailBody);
               

        //        if (_IsMailSuccess==true)
        //        {

        //            mailDict.Add("MailSuccessFlag", "1");
        //            return Request.CreateResponse(HttpStatusCode.OK, mailDict);
        //        }
        //        else
        //        {
        //            mailDict.Add("MailSuccessFlag", "0");
        //            return Request.CreateResponse(HttpStatusCode.NotFound, mailDict);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogInfo(ex);
        //        throw;
        //    }
           
        //}
    }
}

