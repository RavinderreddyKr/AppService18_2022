using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AppService18.ExceptionLogger;
using AppService18.Models;
using AppService18.Others;

namespace AppService18.Controllers
{
    [RoutePrefix("api/Common")]
    public class CommonController : ApiController
    {
        CommonFunction commonFunction = new CommonFunction();

        /// <summary>
        /// Getting APP Version
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("POST", "GET")]
        [Route("GetAPPVersion")]
        public IHttpActionResult GetAPPVersion()
        {
            APPVersionModel versionMdl = new APPVersionModel();
            List<APPVersionModel> versionList = new List<APPVersionModel>();
            ResponseModel objResponseVersionMdl = new ResponseModel();

            try
            {
                var baseUrl = HttpContext.Current.Request.Url.Query.ToString();

                if (baseUrl == "" || baseUrl == null)
                {
                    versionMdl.APPVersion = ConfigurationManager.AppSettings["APPVersion"].ToString();

                    return Json(versionMdl);
                }
                else
                {
                    versionList.Add(new APPVersionModel
                    {
                        APPVersion = ConfigurationManager.AppSettings["APPVersion"].ToString()
                    });

                    if (versionList.Count == 0)
                    {
                        objResponseVersionMdl.Data = versionList;
                        objResponseVersionMdl.Status = false;
                        objResponseVersionMdl.Message = "Data Not Received successfully";
                    }
                    else
                    {
                        objResponseVersionMdl.Data = versionList;
                        objResponseVersionMdl.Status = true;
                        objResponseVersionMdl.Message = "Data Received successfully";
                    }
                    return Json(objResponseVersionMdl);
                }



            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw;
            }

        }
        /// <summary>
        /// Getting Country Details
        /// </summary>
        /// <param name="commonMdl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCountries")]
        public IHttpActionResult GetCountries()
        {
            List<CommonModel> countryList = new List<CommonModel>();
            ResponseModel objResponseCountryMdl = new ResponseModel();
            DataTable dtCountries = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_GetCountriesAndStates";               
                cmdselect.Parameters.AddWithValue("@CountryId", 0);
                cmdselect.Parameters.AddWithValue("@Action", "COUNTRIES");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtCountries);

                countryList = (from DataRow dr in dtCountries.Rows
                                    select new CommonModel()
                                    {
                                        countryId=Convert.ToInt64(dr["COUNTRYID"]),
                                        countryName = dr["COUNTRYNAME"].ToString()

                                    }).ToList();


                if (countryList.Count == 0)
                {
                    objResponseCountryMdl.Data = countryList;
                    objResponseCountryMdl.Status = false;
                    objResponseCountryMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseCountryMdl.Data = countryList;
                    objResponseCountryMdl.Status = true;
                    objResponseCountryMdl.Message = "Data Received successfully";
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
            return Json(objResponseCountryMdl);
        }
        /// <summary>
        /// Getting States
        /// </summary>
        /// <param name="commonStatesMdl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStates/{countryId}")]
        public IHttpActionResult GetStates(Int64 countryId)
        {
            List<CommonModel> stateList = new List<CommonModel>();
            ResponseModel objResponseStateMdl = new ResponseModel();
            DataTable dtStates = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_GetCountriesAndStates";
                cmdselect.Parameters.AddWithValue("@CountryId", countryId);
                cmdselect.Parameters.AddWithValue("@Action", "STATES");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtStates);

                stateList = (from DataRow dr in dtStates.Rows
                               select new CommonModel()
                               {
                                   stateId = Convert.ToInt64(dr["STATEID"]),
                                   stateName = dr["STATENAME"].ToString(),
                                   note=dr["NOTE"].ToString()

                               }).ToList();


                if (stateList.Count == 0)
                {
                    objResponseStateMdl.Data = stateList;
                    objResponseStateMdl.Status = false;
                    objResponseStateMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseStateMdl.Data = stateList;
                    objResponseStateMdl.Status = true;
                    objResponseStateMdl.Message = "Data Received successfully";
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
            return Json(objResponseStateMdl);
        }
        /// <summary>
        /// Getting Previous Address Details based on UID
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPreviouAddress/{UID}")]
        public IHttpActionResult GetPreviouAddress(Int64 UID)
        {
            List<AddressModel> previousAddressList = new List<AddressModel>();
            ResponseModel objResponsePreAddressMdl = new ResponseModel();
            DataTable dtPreviousAddress = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_ManageAddressChangeRequest";
                cmdselect.Parameters.AddWithValue("@SearchName", null);
                cmdselect.Parameters.AddWithValue("@UID", UID);
                cmdselect.Parameters.AddWithValue("@AddressId", 0);
                cmdselect.Parameters.AddWithValue("@Action", "GetCurrentAddressById");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtPreviousAddress); 

                 previousAddressList = (from DataRow dr in dtPreviousAddress.Rows
                             select new AddressModel()
                             {
                                UID = Convert.ToInt64(dr["UID"].ToString()),
                                previousCountry = dr["COUNTRYNAME"].ToString(),
                                previousState = dr["CURRENTSTATE"].ToString(),
                                previousCity = dr["CURRENTCITY"].ToString(),
                                previousStreet = dr["CURRENTSTREET_1"].ToString(),
                                previousSuite = dr["CURRENTSTREET_2"].ToString(),
                                previousZipCode = dr["CURRENTZIPCODE"].ToString(),
                                EffectiveDate = dr["EFFECTIVEDATE"].ToString()

                             }).ToList();


                if (previousAddressList.Count == 0)
                {
                    objResponsePreAddressMdl.Data = previousAddressList;
                    objResponsePreAddressMdl.Status = false;
                    objResponsePreAddressMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponsePreAddressMdl.Data = previousAddressList;
                    objResponsePreAddressMdl.Status = true;
                    objResponsePreAddressMdl.Message = "Data Received successfully";
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
            return Json(objResponsePreAddressMdl);
        }
        /// <summary>
        /// Updating current address details based on UID
        /// </summary>
        /// <param name="addressChangeMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateCurrentAddress")]
        public IHttpActionResult UpdateCurrentAddress(AddressModel addressChangeMdl)
        {
            List<AddressModel> currentAddressList = new List<AddressModel>();
            ResponseModel objResponseCurrentAddressMdl = new ResponseModel();
            DataSet dsUserEmail = new DataSet();
            DataTable dtCurrentAddress = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_SaveAddressChangeRequest";
                cmdselect.Parameters.AddWithValue("@Uid", addressChangeMdl.UID);
                cmdselect.Parameters.AddWithValue("@CurrentCountry", addressChangeMdl.currentCountry);
                cmdselect.Parameters.AddWithValue("@CurrentState", addressChangeMdl.currentState);
                cmdselect.Parameters.AddWithValue("@CurrentCity", addressChangeMdl.currentCity);
                cmdselect.Parameters.AddWithValue("@CurrentZipCode", addressChangeMdl.currentZipCode);
                cmdselect.Parameters.AddWithValue("@CurrentStreet1", addressChangeMdl.currentStreet);
                cmdselect.Parameters.AddWithValue("@CurrentStreet2", addressChangeMdl.currentSuite);
                cmdselect.Parameters.AddWithValue("@CurrentHomePhone", addressChangeMdl.currentHomePhoneNo);
                cmdselect.Parameters.AddWithValue("@CurrentCellNo", addressChangeMdl.currentMobileNo);
                cmdselect.Parameters.AddWithValue("@EffectiveDate", addressChangeMdl.EffectiveDate);
                cmdselect.Parameters.AddWithValue("@Status", addressChangeMdl.status);
                cmdselect.Parameters.AddWithValue("@LogginedUserId", addressChangeMdl.loggedInUserId);
                cmdselect.Parameters.AddWithValue("@Action", "SaveCurrentAddressMobileApp");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtCurrentAddress);

                currentAddressList = (from DataRow dr in dtCurrentAddress.Rows
                                      select new AddressModel()
                                      {
                                          returnStatus = dr["Msg"].ToString(),
                                          returnMsg = dr["ReturnMsg"].ToString(),
                                          AdminTOMail = dr["TOMAILID"].ToString(),
                                          AdminCCMail = dr["CCMAILID"].ToString(),
                                          AdminBCCMail = dr["BCCMAILID"].ToString(),
                                          AdminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                          AdminMailBody = dr["ADMINMAILBODY"].ToString(),
                                          UserMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                          UserMailBody = dr["USERMAILBODY"].ToString(),
                                          PreviousResidenceAddress = dr["PreviousResidenceAddress"].ToString(),
                                          CurrentResidenceAddress = dr["CurrentResidenceAddress"].ToString(),
                                          CurrentWLAddress = dr["CurrentWLAddress"].ToString(),
                                          EffectiveDate = dr["EffectiveDate"].ToString()

                                      }).ToList();


                if (currentAddressList.Count == 0)
                {
                    objResponseCurrentAddressMdl.Data = currentAddressList;
                    objResponseCurrentAddressMdl.Status = false;
                    objResponseCurrentAddressMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    if (currentAddressList[0].returnStatus == "1")
                    {
                        dsUserEmail = commonFunction.GetUserEmail(addressChangeMdl.UID, null);


                        if (dsUserEmail.Tables[0].Rows.Count > 0)
                        {
                            if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                            {
                                // User Mail
                                MailTemplates.ResidenceAddressMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(),
                                      dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                     currentAddressList[0].PreviousResidenceAddress, currentAddressList[0].CurrentResidenceAddress, currentAddressList[0].CurrentWLAddress, addressChangeMdl.EffectiveDate,
                                       dsUserEmail.Tables[0].Rows[0]["PAYCHECKID"].ToString(), dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), string.Empty, currentAddressList[0].UserMailSubject, string.Empty, false, addressChangeMdl.currentState);

                                // Admin Mail

                                MailTemplates.ResidenceAddressMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(),
                                     dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                     currentAddressList[0].PreviousResidenceAddress, currentAddressList[0].CurrentResidenceAddress, currentAddressList[0].CurrentWLAddress, addressChangeMdl.EffectiveDate,
                                       dsUserEmail.Tables[0].Rows[0]["PAYCHECKID"].ToString(), currentAddressList[0].AdminTOMail, currentAddressList[0].AdminCCMail, currentAddressList[0].UserMailSubject, currentAddressList[0].AdminMailSubject, true, addressChangeMdl.currentState);
                            }

                        }
                      
                    }
                    objResponseCurrentAddressMdl.Data = currentAddressList;
                    objResponseCurrentAddressMdl.Status = true;
                    objResponseCurrentAddressMdl.Message = "Data Received successfully";
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
            return Json(objResponseCurrentAddressMdl);
        }
        /// <summary>
        /// Getting Projects based on UID BusinessRules:SUBMITSTATUS='SubmitPoint' and PROJECTSTATUS=1 and PROJECTOPENSTATUS='Open'
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProjects/{UID}")]
        public IHttpActionResult GetProjects(Int64 UID)
        {
            List<ProjectModel> projectList = new List<ProjectModel>();
            ResponseModel objResponseprojectMdl = new ResponseModel();
            DataTable dtProjects = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_ManageWorkLocationAddressHistory";
                cmdselect.Parameters.AddWithValue("@UID", UID);
                cmdselect.Parameters.AddWithValue("@Action", "GETPROJECTSBYUID");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtProjects);

                projectList = (from DataRow dr in dtProjects.Rows
                             select new ProjectModel()
                             {
                                 projectId = Convert.ToInt64(dr["PROJECTID"]),
                                 clientName = dr["ENDCLIENTNAME"].ToString()

                             }).ToList();


                if (projectList.Count == 0)
                {
                    objResponseprojectMdl.Data = projectList;
                    objResponseprojectMdl.Status = false;
                    objResponseprojectMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseprojectMdl.Data = projectList;
                    objResponseprojectMdl.Status = true;
                    objResponseprojectMdl.Message = "Data Received successfully";
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
            return Json(objResponseprojectMdl);
        }
        /// <summary>
        /// Getting Previous Work Location Address Details based on ProjectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPreviousWorkLocationAddress/{projectId}")]
        public IHttpActionResult GetPreviousWorkLocationAddress(Int64 projectId)
        {
            List<WorkLocationAddressModel> preWorkLocationAddressList = new List<WorkLocationAddressModel> ();
            ResponseModel objResponseWorkAddressMdl = new ResponseModel();
            DataTable dtPreviousWorkAddress = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_ManageWorkLocationAddressHistory";
                cmdselect.Parameters.AddWithValue("@PROJECTID", projectId);
                cmdselect.Parameters.AddWithValue("@Action", "GETWORKLOCATIONDETAILS");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtPreviousWorkAddress);

                preWorkLocationAddressList = (from DataRow dr in dtPreviousWorkAddress.Rows
                               select new WorkLocationAddressModel()
                               {
                                   UID= Convert.ToInt64(dr["UID"]),
                                   projectId = Convert.ToInt64(dr["PROJECTID"]),
                                   address1= dr["ADDRESS1"].ToString(),
                                   state1 = dr["STATE1"].ToString(),
                                   otherState1 = dr["OTHERSTATE1"].ToString(),
                                   city1 = dr["CITY1"].ToString(),
                                   zipCode1 = dr["ZIPCODE1"].ToString(),
                                   address2 = dr["ADDRESS2"].ToString(),
                                   state2 = dr["STATE2"].ToString(),
                                   otherState2 = dr["OTHERSTATE2"].ToString(),
                                   city2 = dr["CITY2"].ToString(),
                                   zipCode2 = dr["ZIPCODE2"].ToString(),
                                   address3 = dr["ADDRESS3"].ToString(),
                                   state3 = dr["STATE3"].ToString(),
                                   otherState3 = dr["OTHERSTATE3"].ToString(),
                                   city3 = dr["CITY3"].ToString(),
                                   zipCode3 = dr["ZIPCODE3"].ToString(),
                                   EffectiveDate = dr["EffectiveDate"].ToString()

                               }).ToList();


                if (preWorkLocationAddressList.Count == 0)
                {
                    objResponseWorkAddressMdl.Data = preWorkLocationAddressList;
                    objResponseWorkAddressMdl.Status = false;
                    objResponseWorkAddressMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseWorkAddressMdl.Data = preWorkLocationAddressList;
                    objResponseWorkAddressMdl.Status = true;
                    objResponseWorkAddressMdl.Message = "Data Received successfully";
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
            return Json(objResponseWorkAddressMdl);
        }
        /// <summary>
        /// Updating Current Work Location Address based on ProjectId
        /// </summary>
        /// <param name="worklocationAddressMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateWorkLocationAddress")]
        public IHttpActionResult UpdateWorkLocationAddress(WorkLocationAddressModel worklocationAddressMdl)
        {
            List<WorkLocationAddressModel> updateWorkAddressList = new List<WorkLocationAddressModel>();
            ResponseModel objResponseUpdateWorkAddressMdl = new ResponseModel();
            DataSet dsUserEmail = new DataSet();
            DataTable dtUpdateWorkAddress = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_ManageWorkLocationAddressHistory";
                cmdselect.Parameters.AddWithValue("@UID", worklocationAddressMdl.UID);
                cmdselect.Parameters.AddWithValue("@PROJECTID", worklocationAddressMdl.projectId);
                cmdselect.Parameters.AddWithValue("@ADDRESS1", worklocationAddressMdl.address1);
                cmdselect.Parameters.AddWithValue("@STATE1", worklocationAddressMdl.state1);
                cmdselect.Parameters.AddWithValue("@OTHERSTATE1", worklocationAddressMdl.otherState1);
                cmdselect.Parameters.AddWithValue("@CITY1", worklocationAddressMdl.city1);
                cmdselect.Parameters.AddWithValue("@ZIPCODE1", worklocationAddressMdl.zipCode1);
                cmdselect.Parameters.AddWithValue("@ADDRESS2", worklocationAddressMdl.address2);
                cmdselect.Parameters.AddWithValue("@STATE2", worklocationAddressMdl.state2);
                cmdselect.Parameters.AddWithValue("@OTHERSTATE2", worklocationAddressMdl.otherState2);
                cmdselect.Parameters.AddWithValue("@CITY2", worklocationAddressMdl.city2);
                cmdselect.Parameters.AddWithValue("@ZIPCODE2", worklocationAddressMdl.zipCode2);
                cmdselect.Parameters.AddWithValue("@ADDRESS3", worklocationAddressMdl.address3);
                cmdselect.Parameters.AddWithValue("@STATE3", worklocationAddressMdl.state3);
                cmdselect.Parameters.AddWithValue("@OTHERSTATE3", worklocationAddressMdl.otherState3);
                cmdselect.Parameters.AddWithValue("@CITY3", worklocationAddressMdl.city3);
                cmdselect.Parameters.AddWithValue("@ZIPCODE3", worklocationAddressMdl.zipCode3);
                cmdselect.Parameters.AddWithValue("@EFFECTIVEDATE", worklocationAddressMdl.EffectiveDate);
                cmdselect.Parameters.AddWithValue("@LOGGEDINUSERID", worklocationAddressMdl.loggedInUserId);
                cmdselect.Parameters.AddWithValue("@ACTION", "UPDATEWORKLOCATIONDETAILSMOBILEAPP");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtUpdateWorkAddress);

                updateWorkAddressList = (from DataRow dr in dtUpdateWorkAddress.Rows
                                         select new WorkLocationAddressModel()
                                         {
                                             returnStatus = dr["MSG"].ToString(),
                                             returnMsg = dr["ReturnMsg"].ToString(),
                                             AdminTOMail = dr["TOMAILID"].ToString(),
                                             AdminCCMail = dr["CCMAILID"].ToString(),
                                             AdminBCCMail = dr["BCCMAILID"].ToString(),
                                             AdminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                             AdminMailBody = dr["ADMINMAILBODY"].ToString(),
                                             UserMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                             UserMailBody = dr["USERMAILBODY"].ToString(),
                                             CurrentResidenceAddress = dr["CurrentResidenceAddress"].ToString(),
                                             CurrentWLAddress = dr["CurrentWLAddress"].ToString(),
                                             PreviousWLAddress = dr["PreviousWLAddress"].ToString(),
                                             EffectiveDate = dr["EffectiveDate"].ToString()

                                         }).ToList();


                if (updateWorkAddressList.Count == 0)
                {
                    objResponseUpdateWorkAddressMdl.Data = updateWorkAddressList;
                    objResponseUpdateWorkAddressMdl.Status = false;
                    objResponseUpdateWorkAddressMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    if (updateWorkAddressList[0].returnStatus == "1")
                    {
                        dsUserEmail = commonFunction.GetUserEmail(worklocationAddressMdl.UID, null);


                        if (dsUserEmail.Tables[0].Rows.Count > 0)
                        {
                            if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                            {
                                // User Mail
                                MailTemplates.WorkLocationAddressMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(),
                                  dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                  updateWorkAddressList[0].CurrentWLAddress, updateWorkAddressList[0].PreviousWLAddress, updateWorkAddressList[0].CurrentResidenceAddress, worklocationAddressMdl.EffectiveDate,
                                  dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), string.Empty, updateWorkAddressList[0].UserMailSubject, updateWorkAddressList[0].AdminMailSubject, false, worklocationAddressMdl.state1);


                                // Admin Mail

                                MailTemplates.WorkLocationAddressMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(),
                                   dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                   updateWorkAddressList[0].CurrentWLAddress, updateWorkAddressList[0].PreviousWLAddress, updateWorkAddressList[0].CurrentResidenceAddress, worklocationAddressMdl.EffectiveDate,
                                   updateWorkAddressList[0].AdminTOMail, updateWorkAddressList[0].AdminCCMail, updateWorkAddressList[0].UserMailSubject, updateWorkAddressList[0].AdminMailSubject, true, worklocationAddressMdl.state1);
                            }

                        }
                    }
                    objResponseUpdateWorkAddressMdl.Data = updateWorkAddressList;
                    objResponseUpdateWorkAddressMdl.Status = true;
                    objResponseUpdateWorkAddressMdl.Message = "Data Received successfully";
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
            return Json(objResponseUpdateWorkAddressMdl);
        }
        /// <summary>
        /// Inserting Status log Process
        /// </summary>
        /// <param name="statusLogMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StatusLog")]
        public IHttpActionResult StatusLog(StatusLogModel statusLogMdl)
        {
            List<StatusLogModel> statusLogMdlList = new List<StatusLogModel>();
            ResponseModel objResponseStatusLogMdl= new ResponseModel();
            DataTable dtStatusLogMdl = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_StatusHistory";
                cmdselect.Parameters.AddWithValue("@UserId", statusLogMdl.userId);
                cmdselect.Parameters.AddWithValue("@CURRENTSTATE", statusLogMdl.currentStatus);
                cmdselect.Parameters.AddWithValue("@STATEDESCRIPTION", statusLogMdl.statusDescription);
                cmdselect.Parameters.AddWithValue("@LoggedinUserId", statusLogMdl.loggedInUserId);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtStatusLogMdl);

                statusLogMdlList = (from DataRow dr in dtStatusLogMdl.Rows
                                    select new StatusLogModel()
                                    {
                                        returnStatus = dr["Msg"].ToString()

                                    }).ToList();
            
                if (statusLogMdlList.Count == 0)
                {
                    objResponseStatusLogMdl.Data = statusLogMdlList;
                    objResponseStatusLogMdl.Status = false;
                    objResponseStatusLogMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseStatusLogMdl.Data = statusLogMdlList;
                    objResponseStatusLogMdl.Status = true;
                    objResponseStatusLogMdl.Message = "Data Received successfully";
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
            return Json(objResponseStatusLogMdl);
        }
        /// <summary>
        /// Getting Weekly Status Report based on UID and Action
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="action"></param>
        /// GET GetWeeklyStatusReport?UID=90560&action=WSRVIEWBYUID
        /// <returns></returns>
        [HttpGet]
        [Route("GetWeeklyStatusReport")]
        public IHttpActionResult GetWeeklyStatusReport(Int64 UID, string action)
        {
            List<WeeklyStatusReportModel> WSRList = new List<WeeklyStatusReportModel>();
            ResponseModel objResponseWSRMdl = new ResponseModel();
            DataTable dtWSReport = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_EmployeeWeeklyStatusReport";
                cmdselect.Parameters.AddWithValue("@UID", UID);
                cmdselect.Parameters.AddWithValue("@Action", action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtWSReport);

                WSRList = (from DataRow dr in dtWSReport.Rows
                           select new WeeklyStatusReportModel()
                           {
                               UID = Convert.ToInt64(dr["UID"]),
                               weeklyReportId = Convert.ToInt64(dr["WEEKLYREPORTID"]),
                               title = dr["TITLE"].ToString(),
                               superVisor = dr["SUPERVISOR"].ToString(),
                               fromDate = dr["FROMDATE"].ToString(),
                               toDate = dr["TODATE"].ToString(),
                               activitiesPlanned = dr["ACTIVITIESPLANNED"].ToString(),
                               activitiesAccomplished = dr["ACTIVITIESACCOMPLISHED"].ToString(),
                               activitiesPlannedNextWeek = dr["ACTIVITIESPLANNEDNEXTWEEK"].ToString(),
                               projectProgress = dr["PROJECTPROGRESS"].ToString(),
                               specificQuestions = dr["SPECIFICQUESTIONS"].ToString(),
                               activeStatus = Convert.ToBoolean(dr["ACTIVESTATUS"].ToString()),
                               approvalStatus = Convert.ToBoolean(dr["APPROVALSTATUS"].ToString())

                           }).ToList();


                if (WSRList.Count == 0)
                {
                    objResponseWSRMdl.Data = WSRList;
                    objResponseWSRMdl.Status = false;
                    objResponseWSRMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseWSRMdl.Data = WSRList;
                    objResponseWSRMdl.Status = true;
                    objResponseWSRMdl.Message = "Data Received successfully";
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
            return Json(objResponseWSRMdl);
        }
        /// <summary>
        /// Save Weekly Status Report Details
        /// </summary>
        /// <param name="weeklyStatusReportMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveWeeklyStatusReport")]
        public IHttpActionResult SaveWeeklyStatusReport(WeeklyStatusReportModel weeklyStatusReportMdl)
        {
            List<WeeklyStatusReportModel> WSReportList = new List<WeeklyStatusReportModel>();
            ResponseModel objResponseSaveWSRMdl = new ResponseModel();
            DataSet dsUserEmail = new DataSet();
            DataTable dtSaveWSReport = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_EmployeeWeeklyStatusReport";
                cmdselect.Parameters.AddWithValue("@UID", weeklyStatusReportMdl.UID);
                cmdselect.Parameters.AddWithValue("@WeeklyReportId", weeklyStatusReportMdl.weeklyReportId);
                cmdselect.Parameters.AddWithValue("@Title", weeklyStatusReportMdl.title);
                cmdselect.Parameters.AddWithValue("@SuperVisor", weeklyStatusReportMdl.superVisor);
                cmdselect.Parameters.AddWithValue("@Fromdate", weeklyStatusReportMdl.fromDate);
                cmdselect.Parameters.AddWithValue("@Todate", weeklyStatusReportMdl.toDate);
                cmdselect.Parameters.AddWithValue("@ActivitiesPlanned", weeklyStatusReportMdl.activitiesPlanned);
                cmdselect.Parameters.AddWithValue("@ActivitiesAccomplished", weeklyStatusReportMdl.activitiesAccomplished);
                cmdselect.Parameters.AddWithValue("@ActivitiesPlannedNextWeek", weeklyStatusReportMdl.activitiesPlannedNextWeek);
                cmdselect.Parameters.AddWithValue("@ProjectProgress", weeklyStatusReportMdl.projectProgress);
                cmdselect.Parameters.AddWithValue("@SpecificQuestions", weeklyStatusReportMdl.specificQuestions);
                cmdselect.Parameters.AddWithValue("@ActiveStatus", weeklyStatusReportMdl.activeStatus);
                cmdselect.Parameters.AddWithValue("@ApprovalStatus", weeklyStatusReportMdl.approvalStatus);
                cmdselect.Parameters.AddWithValue("@Description", weeklyStatusReportMdl.description);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", weeklyStatusReportMdl.loggedInUserId);
                cmdselect.Parameters.AddWithValue("@Action", weeklyStatusReportMdl.action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtSaveWSReport);

                WSReportList = (from DataRow dr in dtSaveWSReport.Rows
                                select new WeeklyStatusReportModel()
                                {
                                    returnStatus = dr["ReturnStatus"].ToString(),
                                    returnMsg = dr["ReturnMsg"].ToString(),
                                    AdminTOMail = dr["TOMAILID"].ToString(),
                                    AdminCCMail = dr["CCMAILID"].ToString(),
                                    AdminBCCMail = dr["BCCMAILID"].ToString(),
                                    AdminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                    AdminMailBody = dr["ADMINMAILBODY"].ToString(),
                                    UserMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                    UserMailBody = dr["USERMAILBODY"].ToString()

                                }).ToList();


                if (WSReportList.Count == 0)
                {
                    objResponseSaveWSRMdl.Data = WSReportList;
                    objResponseSaveWSRMdl.Status = false;
                    objResponseSaveWSRMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    dsUserEmail = commonFunction.GetUserEmail(weeklyStatusReportMdl.UID, null);


                    if (dsUserEmail.Tables[0].Rows.Count > 0)
                    {
                        if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                        {
                            // User Mail
                            MailTemplates.WeeklyStatusReportMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(),
                                dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(), weeklyStatusReportMdl.fromDate, weeklyStatusReportMdl.toDate,
                                weeklyStatusReportMdl.activitiesAccomplished, weeklyStatusReportMdl.activitiesPlannedNextWeek, weeklyStatusReportMdl.projectProgress, weeklyStatusReportMdl.specificQuestions,
                                dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), string.Empty,
                                WSReportList[0].UserMailSubject, WSReportList[0].AdminMailSubject, false);


                            // Admin Mail

                            MailTemplates.WeeklyStatusReportMail(dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(), dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(),
                             dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(), weeklyStatusReportMdl.fromDate, weeklyStatusReportMdl.toDate,
                             weeklyStatusReportMdl.activitiesAccomplished, weeklyStatusReportMdl.activitiesPlannedNextWeek, weeklyStatusReportMdl.projectProgress, weeklyStatusReportMdl.specificQuestions,
                             WSReportList[0].AdminTOMail, WSReportList[0].AdminCCMail, WSReportList[0].UserMailSubject, WSReportList[0].AdminMailSubject, true);
                        }

                    }
                    objResponseSaveWSRMdl.Data = WSReportList;
                    objResponseSaveWSRMdl.Status = true;
                    objResponseSaveWSRMdl.Message = "Data Received successfully";
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
            return Json(objResponseSaveWSRMdl);
        }
        /// <summary>
        /// Save Mobile Infromation
        /// </summary>
        /// <param name="mobileInfoMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveMobileInformation")]
        public IHttpActionResult SaveMobileInformation(MobileInfoModel mobileInfoMdl)
        {
            List<MobileInfoModel> MIList = new List<MobileInfoModel>();
            ResponseModel objResponseSaveMIMdl = new ResponseModel();
            DataTable dtSaveMIReport = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_ManageMobileInformation";
                cmdselect.Parameters.AddWithValue("@UID", mobileInfoMdl.UID);
                cmdselect.Parameters.AddWithValue("@CurrentVersion", mobileInfoMdl.currentVersion);
                cmdselect.Parameters.AddWithValue("@PreviousVersion", mobileInfoMdl.previousVersion);
                cmdselect.Parameters.AddWithValue("@DeviceName", mobileInfoMdl.deviceName);
                cmdselect.Parameters.AddWithValue("@DeviceId", mobileInfoMdl.deviceId);
                cmdselect.Parameters.AddWithValue("@Location", mobileInfoMdl.location);
                cmdselect.Parameters.AddWithValue("@OSVersion", mobileInfoMdl.osVersion);
                cmdselect.Parameters.AddWithValue("@OSType", mobileInfoMdl.osType);
                cmdselect.Parameters.AddWithValue("@LastUpdatedDate", mobileInfoMdl.lastUpdatedDate);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", mobileInfoMdl.loggedInUserId);
                cmdselect.Parameters.AddWithValue("@SearchTxtVal", mobileInfoMdl.searchTxtVal);
                cmdselect.Parameters.AddWithValue("@Action", mobileInfoMdl.action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtSaveMIReport);

                MIList = (from DataRow dr in dtSaveMIReport.Rows
                          select new MobileInfoModel()
                          {
                              returnStatus = dr["Msg"].ToString(),


                          }).ToList();


                if (MIList.Count == 0)
                {
                    objResponseSaveMIMdl.Data = MIList;
                    objResponseSaveMIMdl.Status = false;
                    objResponseSaveMIMdl.Message = "Data Not Received successfully";
                }
                else
                {

                    objResponseSaveMIMdl.Data = MIList;
                    objResponseSaveMIMdl.Status = true;
                    objResponseSaveMIMdl.Message = "Data Received successfully";
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
            return Json(objResponseSaveMIMdl);
        }
        /// <summary>
        /// Getting Mobile Information Based on Search Text Value
        /// </summary>
        /// <param name="searchTxtVal"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMobileInformation")]
        public IHttpActionResult GetMobileInformation(MobileInfoModel mobileInfoModel)
        {
            List<MobileInfoModel> MIReportList = new List<MobileInfoModel>();
            ResponseModel objResponseMIMdl = new ResponseModel();
            DataTable dtMIReport = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();


            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_ManageMobileInformation";
                cmdselect.Parameters.AddWithValue("@UID", mobileInfoModel.UID);
                cmdselect.Parameters.AddWithValue("@SearchTxtVal", mobileInfoModel.searchTxtVal);
                cmdselect.Parameters.AddWithValue("@Action", mobileInfoModel.action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtMIReport);
                if (dtMIReport != null && dtMIReport.Rows.Count > 0)
                {
                    if (mobileInfoModel.action == "BINDUSERS")
                    {
                        MIReportList = (from DataRow dr in dtMIReport.Rows
                                        select new MobileInfoModel()
                                        {
                                            UID = Convert.ToInt64(dr["UID"]),
                                            fullUserName = dr["FullUserName"].ToString(),

                                        }).ToList();
                    }
                    else if (mobileInfoModel.action == "MICOUNTS")
                    {
                        MIReportList = (from DataRow dr in dtMIReport.Rows
                                        select new MobileInfoModel()
                                        {
                                            iOSCount = Convert.ToInt32(dr["IOSCOUNT"]),
                                            androidCount = Convert.ToInt32(dr["ANDROIDCOUNT"]),
                                            miPendingCount = Convert.ToInt32(dr["MIPENDINGCOUNT"])

                                        }).ToList();

                    }
                    else
                    {
                        MIReportList = (from DataRow dr in dtMIReport.Rows
                                        select new MobileInfoModel()
                                        {
                                            UID = Convert.ToInt64(dr["UID"]),
                                            fullUserName = dr["FullUserName"].ToString(),
                                            firstName = dr["FNAME"].ToString(),
                                            lastName = dr["LNAME"].ToString(),
                                            emailId = dr["EMAILID"].ToString(),
                                            phoneNo = dr["CURRENTPHNO"].ToString(),
                                            workAuthType = dr["WORKAUTHORIZATIONTYPE"].ToString(),
                                            currentVersion = dr["CURRENTVERSION"].ToString(),
                                            previousVersion = dr["PREVIOUSVERSION"].ToString(),
                                            deviceName = dr["DEVICENAME"].ToString(),
                                            deviceId = dr["DEVICEID"].ToString(),
                                            location = dr["LOCATION"].ToString(),
                                            osVersion = dr["OSVERSION"].ToString(),
                                            osType = dr["OSTYPE"].ToString(),
                                            lastUpdatedDate = dr["LASTUPDATEDDATE"].ToString(),
                                            createdDate = dr["CREATEDDATE"].ToString(),

                                        }).ToList();
                    }
                }
                if (MIReportList.Count == 0)
                {
                    objResponseMIMdl.Data = MIReportList;
                    objResponseMIMdl.Status = false;
                    objResponseMIMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseMIMdl.Data = MIReportList;
                    objResponseMIMdl.Status = true;
                    objResponseMIMdl.Message = "Data Received successfully";
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
            return Json(objResponseMIMdl);
        }
    }
    
}
