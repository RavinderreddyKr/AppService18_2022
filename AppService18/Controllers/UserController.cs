using AppService18.ExceptionLogger;
using AppService18.Models;
using AppService18.Others;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppService18.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        CommonFunction commonFunction = new CommonFunction();
        /// <summary>
        /// Getting User Information based on UID
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserInfo/{UID}")]
        public IHttpActionResult GetUserInfo(Int64 UID)
        {
            List<UserModel> userList = new List<UserModel>();
            ResponseModel objResponseuserMdl = new ResponseModel();
            DataTable dtUserInfo = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_UserInformation";
                cmdselect.Parameters.AddWithValue("@UID", UID);
                cmdselect.Parameters.AddWithValue("@Action", "USERINFO");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtUserInfo);

                userList = (from DataRow dr in dtUserInfo.Rows
                            select new UserModel()
                            {
                                UID = Convert.ToInt64(dr["UID"].ToString()),
                                userName = dr["USERFULLNAME"].ToString(),
                                companyStartDate = dr["COMPANYSTDATE"].ToString(),
                                jobTitle = dr["JOBTITLE"].ToString(),
                                clientName = dr["CLIENTNAME"].ToString(),
                                vendorName = dr["VENDORNAME"].ToString(),
                                implementorName = dr["IMPLEMENTORNAME"].ToString(),
                                startDateClient = dr["CLIENTSTDATE"].ToString(),
                                lCASalary = dr["LCASALARY"].ToString(),
                                supervisorName = dr["SUPERVISORNAME"].ToString(),
                                supervisorPhone = dr["SUPERVISORPHNO"].ToString(),
                                supervisorEmailId = dr["SUPERVISOREMAIL"].ToString(),
                                techSupervisorName = dr["TECHSUPERVISORNAME"].ToString(),
                                techSupervisorPhone = dr["TECHSUPERVISORPHNO"].ToString(),
                                techSupervisorEmailId = dr["TECHSUPERVISOREMAIL"].ToString(),
                                clientLocation = dr["CLIENTLOCATION"].ToString(),
                                residenceAddress = dr["RESIDENCEADDRESS"].ToString(),
                                jobChangeLCAfileddate = dr["JOBCHANGELCAFILEDDATE"].ToString(),
                                jobChangeAddress = dr["JOBCHANGEADDRESS"].ToString(),
                                projectId = Convert.ToInt64(dr["PROJECTID"].ToString()),
                                contractId = Convert.ToInt64(dr["CONTRACTID"].ToString())

                            }).ToList();


                if (userList.Count == 0)
                {
                    objResponseuserMdl.Data = userList;
                    objResponseuserMdl.Status = false;
                    objResponseuserMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseuserMdl.Data = userList;
                    objResponseuserMdl.Status = true;
                    objResponseuserMdl.Message = "Data Received successfully";
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
            return Json(objResponseuserMdl);
        }
        /// <summary>
        /// User Question And Answers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserQuestionAndAnswers")]
        public IHttpActionResult GetUserQuestionAndAnswers()
        {
            List<QAModel> userQuestionAnswersList = new List<QAModel>();
            ResponseModel objResponseQAMdl = new ResponseModel();
            DataTable dtuserQuestionAnswers = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_UserInformation";
                cmdselect.Parameters.AddWithValue("@UID", 0);
                cmdselect.Parameters.AddWithValue("@Action", "QA");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtuserQuestionAnswers);

                userQuestionAnswersList = (from DataRow dr in dtuserQuestionAnswers.Rows
                                           select new QAModel()
                                           {
                                               QAAID = Convert.ToInt64(dr["QAAID"].ToString()),
                                               question = dr["QUESTION"].ToString(),
                                               answer = dr["ANSWER"].ToString(),

                                           }).ToList();


                if (userQuestionAnswersList.Count == 0)
                {
                    objResponseQAMdl.Data = userQuestionAnswersList;
                    objResponseQAMdl.Status = false;
                    objResponseQAMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseQAMdl.Data = userQuestionAnswersList;
                    objResponseQAMdl.Status = true;
                    objResponseQAMdl.Message = "Data Received successfully";
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
            return Json(objResponseQAMdl);
        }
        /// <summary>
        /// Update User Information
        /// </summary>
        /// <param name="userInfoMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateUserInformation")]
        public IHttpActionResult UpdateUserInformation(UserModel userInfoMdl)
        {

            List<UserModel> updateUserInfolist = new List<UserModel>();
            ResponseModel _objResponseUpdateUIMdl = new ResponseModel();
            DataSet dsUserEmail = new DataSet();
            DataTable dtUpdateUserInfoData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_UpdateUserInformation";
                cmdselect.Parameters.AddWithValue("@UID", userInfoMdl.UID);
                cmdselect.Parameters.AddWithValue("@ContractId", userInfoMdl.contractId);
                cmdselect.Parameters.AddWithValue("@ProjectId", userInfoMdl.projectId);
                cmdselect.Parameters.AddWithValue("@JobTitle", userInfoMdl.jobTitle);
                cmdselect.Parameters.AddWithValue("@ClientName", userInfoMdl.clientName);
                cmdselect.Parameters.AddWithValue("@VendorName", userInfoMdl.vendorName);
                cmdselect.Parameters.AddWithValue("@ImplementorName", userInfoMdl.implementorName);
                cmdselect.Parameters.AddWithValue("@StartDateClient", userInfoMdl.startDateClient);
                cmdselect.Parameters.AddWithValue("@JobChangeLCAfileddate", userInfoMdl.jobChangeLCAfileddate);
                cmdselect.Parameters.AddWithValue("@Address", userInfoMdl.address);
                cmdselect.Parameters.AddWithValue("@City", userInfoMdl.city);
                cmdselect.Parameters.AddWithValue("@State", userInfoMdl.state);
                cmdselect.Parameters.AddWithValue("@ZipCode", userInfoMdl.zipCode);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", userInfoMdl.loggedInUserId);
                cmdselect.Parameters.AddWithValue("@Action", userInfoMdl.action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtUpdateUserInfoData);

                updateUserInfolist = (from DataRow dr in dtUpdateUserInfoData.Rows
                                      select new UserModel()
                                      {

                                          statusFlag = Convert.ToInt32(dr["ReturnStatus"].ToString()),
                                          returnMsg = dr["ReturnMsg"].ToString(),
                                          userInfoTitle = dr["UserInfoTitle"].ToString(),
                                          previousData = dr["PreviousData"].ToString(),
                                          updateData = dr["UpdateData"].ToString(),
                                          AdminTOMail = dr["TOMAILID"].ToString(),
                                          AdminCCMail = dr["CCMAILID"].ToString(),
                                          AdminBCCMail = dr["BCCMAILID"].ToString(),
                                          AdminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                          AdminMailBody = dr["ADMINMAILBODY"].ToString(),
                                          UserMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                          UserMailBody = dr["USERMAILBODY"].ToString()

                                      }).ToList();

                if (updateUserInfolist.Count == 0)
                {
                    _objResponseUpdateUIMdl.Data = updateUserInfolist;
                    _objResponseUpdateUIMdl.Status = false;
                    _objResponseUpdateUIMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    dsUserEmail = commonFunction.GetUserEmail(userInfoMdl.UID, null);


                    if (dsUserEmail.Tables[0].Rows.Count > 0)
                    {
                        if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                        {
                            // User Mail
                            MailTemplates.UserInfoMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(),
                                dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(), updateUserInfolist[0].userInfoTitle, updateUserInfolist[0].previousData, updateUserInfolist[0].updateData,
                                dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), string.Empty, updateUserInfolist[0].UserMailSubject, updateUserInfolist[0].AdminMailSubject, false);


                            // Admin Mail

                            MailTemplates.UserInfoMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(),
                             dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(), updateUserInfolist[0].userInfoTitle, updateUserInfolist[0].previousData, updateUserInfolist[0].updateData,
                             updateUserInfolist[0].AdminTOMail, updateUserInfolist[0].AdminCCMail, updateUserInfolist[0].UserMailSubject, updateUserInfolist[0].AdminMailSubject, true);
                        }

                    }
                    _objResponseUpdateUIMdl.Data = updateUserInfolist;
                    _objResponseUpdateUIMdl.Status = true;
                    _objResponseUpdateUIMdl.Message = "Data Received successfully";
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
            return Json(_objResponseUpdateUIMdl);
        }
    }
}
