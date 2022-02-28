using AppService18.ExceptionLogger;
using AppService18.Models;
using AppService18.Others;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AppService18.Controllers
{
    [RoutePrefix("api/Vacation")]
    public class VacationController : ApiController
    {
        CommonFunction commonFunction = new CommonFunction();

        /// <summary>
        /// Vacation File Upload
        /// </summary>
        /// <returns></returns>
        [Route("VacationFileUpload")]
        [HttpPost]
        public IHttpActionResult VacationFileUpload()
        {
            VacationModel uploadvacationMdl = new VacationModel();
            List<VacationModel> vacationFileList = new List<VacationModel>();
            ResponseModel _objResponsevacationMdl = new ResponseModel();
            DataSet dsUserEmail = new DataSet();
            DataTable dtvacationUploadFiles = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();
            string UserName = string.Empty;
            string OriginalFileName = string.Empty;
            string PDFConvertFilePath = string.Empty;


            NameValueCollection vacationForm = HttpContext.Current.Request.Form;
            try
            {
                foreach (string key in vacationForm.AllKeys)
                {
                    if (key == "UID")
                    {
                        uploadvacationMdl.UID = Convert.ToInt64(vacationForm[key]);
                    }
                    else if (key == "vacationId")
                    {
                        uploadvacationMdl.vacationId = Convert.ToInt64(vacationForm[key]);
                    }
                    else if (key == "UserFullName")
                    {
                        uploadvacationMdl.UserFullName = vacationForm[key];
                    }
                    else if (key == "vacationStartdate")
                    {
                        uploadvacationMdl.vacationStartdate = vacationForm[key];
                    }
                    else if (key == "vacationEnddate")
                    {
                        uploadvacationMdl.vacationEnddate = vacationForm[key];
                    }
                    else if (key == "userId")
                    {
                        uploadvacationMdl.userId = vacationForm[key];
                    }
                    else if (key == "vacationComments")
                    {
                        uploadvacationMdl.vacationComments = vacationForm[key];
                    }
                    else if (key == "vacationAction")
                    {
                        uploadvacationMdl.vacationAction = vacationForm[key];
                    }
                    else if(key== "vacationCountry")
                    {
                        uploadvacationMdl.vacationCountry = vacationForm[key];
                    }
                }


                string _UploadTimerVal = string.Empty;
                string SaveFileToFolder = string.Empty;
                var message = string.Empty;
                string folderPath = System.Configuration.ConfigurationManager.AppSettings["VacationPath"] + uploadvacationMdl.UID + "_" + uploadvacationMdl.userId;
                Dictionary<string, object> dict = new Dictionary<string, object>();

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];

                    var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();

                    if (!Directory.Exists(folderPath))
                    {
                        //If Directory (Folder) does not exists. Create it.
                        Directory.CreateDirectory(folderPath);
                    }
                    if (uploadvacationMdl.vacationId == 0)
                    {
                        uploadvacationMdl.vacationId = commonFunction.GetVacationLetterId(uploadvacationMdl.UID, uploadvacationMdl.userId);
                    }

                    if (uploadvacationMdl.vacationId != 0)
                    {
                        _UploadTimerVal = DateTime.Now.ToString("yyyyMMddHHmmss");

                        OriginalFileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                        var OriginalfilePath = (folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + postedFile.FileName);

                        postedFile.SaveAs(OriginalfilePath); // Saving file in Sever
                                                             // Pdf Files
                        if (extension == ".pdf")
                        {
                            uploadvacationMdl.vacationDocumentName = postedFile.FileName;
                            uploadvacationMdl.vactionDocumentFileName = "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + postedFile.FileName;
                            uploadvacationMdl.vacationDocumentUrl = folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + postedFile.FileName;
                        }
                        // Convert Word to Pdf
                        else if (extension == ".docx" || extension == ".doc")
                        {
                            PDFConvertFilePath = folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            commonFunction.WordtoPDF(OriginalfilePath, PDFConvertFilePath);
                            uploadvacationMdl.vacationDocumentName = OriginalFileName + ".pdf";
                            uploadvacationMdl.vactionDocumentFileName = "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            uploadvacationMdl.vacationDocumentUrl = folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            System.IO.File.Delete(OriginalfilePath);
                        }
                        // Convert Excel to Pdf
                        else if (extension == ".xls" || extension == ".csv" || extension == ".xlsx")
                        {
                            PDFConvertFilePath = folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            commonFunction.ExceltoPDF(OriginalfilePath, PDFConvertFilePath);
                            uploadvacationMdl.vacationDocumentName = OriginalFileName + ".pdf";
                            uploadvacationMdl.vactionDocumentFileName = "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            uploadvacationMdl.vacationDocumentUrl = folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            System.IO.File.Delete(OriginalfilePath);
                        }
                        // Convert PPT to Pdf
                        else if (extension == ".pptx" || extension == ".ppt")
                        {
                            PDFConvertFilePath = folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            commonFunction.PPTtoPDF(OriginalfilePath, PDFConvertFilePath);
                            uploadvacationMdl.vacationDocumentName = OriginalFileName + ".pdf";
                            uploadvacationMdl.vactionDocumentFileName = "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            uploadvacationMdl.vacationDocumentUrl = folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            System.IO.File.Delete(OriginalfilePath);
                        }
                        // Convert Image to Pdf
                        else if (extension == ".jpg" || extension == ".gif" || extension == ".png" || extension == ".jpeg")
                        {
                            PDFConvertFilePath = folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            commonFunction.ConvertImageToPdf(OriginalfilePath, PDFConvertFilePath);
                            uploadvacationMdl.vacationDocumentName = OriginalFileName + ".pdf";
                            uploadvacationMdl.vactionDocumentFileName = "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            uploadvacationMdl.vacationDocumentUrl = folderPath + "\\" + "Vacation" + "_" + uploadvacationMdl.vacationId + "_" + _UploadTimerVal + "_" + OriginalFileName + ".pdf";
                            System.IO.File.Delete(OriginalfilePath);
                        }
                    }
                    // Saving filedata in Database
                    cmdselect.CommandType = CommandType.StoredProcedure;
                    cmdselect.CommandText = "Sp_SaveVacationLetterProcess";
                    cmdselect.Parameters.AddWithValue("@UID", uploadvacationMdl.UID);
                    cmdselect.Parameters.AddWithValue("@VacationId", uploadvacationMdl.vacationId);
                    cmdselect.Parameters.AddWithValue("@VacationStdate", uploadvacationMdl.vacationStartdate);
                    cmdselect.Parameters.AddWithValue("@VacationEnddate", uploadvacationMdl.vacationEnddate);
                    cmdselect.Parameters.AddWithValue("@DocumentUrl", uploadvacationMdl.vacationDocumentUrl);
                    cmdselect.Parameters.AddWithValue("@DocumentName", uploadvacationMdl.vacationDocumentName);
                    cmdselect.Parameters.AddWithValue("@DocumentFileName", uploadvacationMdl.vactionDocumentFileName);
                    cmdselect.Parameters.AddWithValue("@ApprovalStatus", "Submit");
                    cmdselect.Parameters.AddWithValue("@VacationStatus", true);
                    cmdselect.Parameters.AddWithValue("@Comments", uploadvacationMdl.vacationComments);
                    cmdselect.Parameters.AddWithValue("@Action", uploadvacationMdl.vacationAction);
                    cmdselect.Parameters.AddWithValue("@logginUserId", uploadvacationMdl.userId);
                    cmdselect.Parameters.AddWithValue("@VacationCountry", uploadvacationMdl.vacationCountry);
                    cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                    cmdselect.Connection.Open();
                    da = new SqlDataAdapter(cmdselect);
                    da.Fill(dtvacationUploadFiles);

                    if (dtvacationUploadFiles != null && dtvacationUploadFiles.Rows.Count > 0)
                    {
                        vacationFileList = (from DataRow dr in dtvacationUploadFiles.Rows
                                            select new VacationModel()
                                            {
                                                vacationStatusMsg = dr["ResultMsg"].ToString(),
                                                vacationStatusFlag = Convert.ToInt32(dr["Msg"].ToString()),
                                                adminTOMail = dr["TOMAILID"].ToString(),
                                                adminCCMail = dr["CCMAILID"].ToString(),
                                                adminBCCMail = dr["BCCMAILID"].ToString(),
                                                adminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                                adminMailBody = dr["ADMINMAILBODY"].ToString(),
                                                userMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                                userMailBody = dr["USERMAILBODY"].ToString(),


                                            }).ToList();
                    }
                    if (vacationFileList.Count == 0)
                    {
                        _objResponsevacationMdl.Data = vacationFileList;
                        _objResponsevacationMdl.Status = false;
                        _objResponsevacationMdl.Message = "Data Not Received successfully";

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
                        if (vacationFileList[0].vacationStatusFlag != 3)
                        {
                            dsUserEmail = commonFunction.GetUserEmail(uploadvacationMdl.UID, null);


                            if (dsUserEmail.Tables[0].Rows.Count > 0)
                            {
                                if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                                {
                                    // User Mail
                                    MailTemplates.VacationLetterMail(dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(), dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(),
                                        dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                        uploadvacationMdl.vacationStartdate, uploadvacationMdl.vacationEnddate, uploadvacationMdl.vacationComments, null,
                                       dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), string.Empty, vacationFileList[0].userMailSubject, vacationFileList[0].userMailBody, false);

                                    // Admin Mail

                                    MailTemplates.VacationLetterMail(dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(), dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(),
                                        dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                        uploadvacationMdl.vacationStartdate, uploadvacationMdl.vacationEnddate, uploadvacationMdl.vacationComments, null,
                                          vacationFileList[0].adminTOMail, vacationFileList[0].adminCCMail, vacationFileList[0].adminMailSubject, vacationFileList[0].adminMailBody, true);
                                }

                            }
                        }

                        // postedFile.SaveAs(filePath); // Saving file in Sever
                        _objResponsevacationMdl.Data = vacationFileList;
                        _objResponsevacationMdl.Status = true;
                        _objResponsevacationMdl.Message = "Data Received successfully";

                        dict.Add("Message", "Your new vacation applied successfully");
                        dict.Add("AdminTOMail", vacationFileList[0].adminTOMail);
                        dict.Add("AdminCCMail", vacationFileList[0].adminCCMail);
                        dict.Add("AdminBCCMail", vacationFileList[0].adminBCCMail);
                        dict.Add("AdminMailSubject", vacationFileList[0].adminMailSubject);
                        dict.Add("AdminMailBody", vacationFileList[0].adminMailBody);
                        dict.Add("UserMailSubject", vacationFileList[0].userMailSubject);
                        dict.Add("UserMailBody", vacationFileList[0].userMailBody);
                    }




                }

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
            return Json(_objResponsevacationMdl);
        }
        /// <summary>
        /// Getting Vacation Letter File
        /// </summary>
        /// <param name="vacationViewMdl"></param>
        /// <returns></returns>
        [Route("ViewVacationLetter")]
        [HttpPost]
        public IHttpActionResult ViewVacationLetter(VacationModel vacationViewMdl)
        {
            DataTable dtVacationFileViewData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();
            ResponseModel _objResponseVacationDocsMdl = new ResponseModel();

            try
            {

                byte[] _Buffer = null;

                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_VacationFileView";
                cmdselect.Parameters.AddWithValue("@VLUID", vacationViewMdl.UID);
                cmdselect.Parameters.AddWithValue("@VacationId", vacationViewMdl.vacationId);
                cmdselect.Parameters.AddWithValue("@VLDocFileName", vacationViewMdl.vacationDocumentName);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtVacationFileViewData);
                if (dtVacationFileViewData != null && dtVacationFileViewData.Rows.Count > 0)
                {

                    //Getting Vacation Letter Details based on UID                    

                    if (dtVacationFileViewData.Rows[0]["DOCUMENTURL"].ToString() != "")
                    {

                        // Open file for reading
                        System.IO.FileStream _FileStream = new System.IO.FileStream(dtVacationFileViewData.Rows[0]["DOCUMENTURL"].ToString(), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                        // attach filestream to binary reader
                        System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);

                        // get total byte length of the file
                        long _TotalBytes = new System.IO.FileInfo(dtVacationFileViewData.Rows[0]["DOCUMENTURL"].ToString()).Length;

                        // read entire file into buffer
                        _Buffer = _BinaryReader.ReadBytes((Int32)_TotalBytes);

                        // close file reader
                        _FileStream.Close();
                        _FileStream.Dispose();
                        _BinaryReader.Close();
                        vacationViewMdl.vacationDocument = _Buffer;
                        vacationViewMdl.UID = Convert.ToInt64(dtVacationFileViewData.Rows[0]["UID"].ToString());
                        vacationViewMdl.vacationId = Convert.ToInt64(dtVacationFileViewData.Rows[0]["VACATIONID"].ToString());
                        vacationViewMdl.vacationDocumentName = dtVacationFileViewData.Rows[0]["DOCUMENTNAME"].ToString();
                        vacationViewMdl.vactionDocumentFileName = dtVacationFileViewData.Rows[0]["DOCUMENTFILENAME"].ToString();
                        vacationViewMdl.vacationDocumentUrl = dtVacationFileViewData.Rows[0]["DOCUMENTURL"].ToString();
                        vacationViewMdl.vacationStatusFlag = Convert.ToInt32(dtVacationFileViewData.Rows[0]["StatusMsg"].ToString());
                        vacationViewMdl.vacationStatusMsg = dtVacationFileViewData.Rows[0]["ResultMsg"].ToString();

                        _objResponseVacationDocsMdl.Data = vacationViewMdl;
                        _objResponseVacationDocsMdl.Status = true;
                        _objResponseVacationDocsMdl.Message = "Data Received successfully";
                    }
                    else
                    {
                        _objResponseVacationDocsMdl.Data = "[]";
                        _objResponseVacationDocsMdl.Status = false;
                        _objResponseVacationDocsMdl.Message = "Data Not Received successfully";
                    }

                }

                else
                {
                    _objResponseVacationDocsMdl.Data = "[]";
                    _objResponseVacationDocsMdl.Status = false;
                    _objResponseVacationDocsMdl.Message = "Data Not Received successfully";
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
            return Json(_objResponseVacationDocsMdl);
        }
        /// <summary>
        /// Getting Vacation Letter Details
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetVacationDetails/{UID}")]
        public IHttpActionResult GetVacationDetails(Int64 UID)
        {
            List<VacationModel> vacationList = new List<VacationModel>();
            ResponseModel objResponsevacationMdl = new ResponseModel();
            DataTable dtvacationData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_ManageVacationProcess";
                cmdselect.Parameters.AddWithValue("@VacationUID", UID);
                cmdselect.Parameters.AddWithValue("@VacationVisaId", 0);
                cmdselect.Parameters.AddWithValue("@VacationId", 0);
                cmdselect.Parameters.AddWithValue("@VacationComments", "");
                cmdselect.Parameters.AddWithValue("@VacationAction", "GETVACATIONDETAILS");
                cmdselect.Parameters.AddWithValue("@VacationLoggedUserId", "");
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtvacationData);

                vacationList = (from DataRow dr in dtvacationData.Rows
                                select new VacationModel()
                                {
                                    UID = Convert.ToInt64(dr["UID"]),
                                    vacationId = Convert.ToInt64(dr["VACATIONID"]),
                                    vacationStartdate = dr["VACATIONSTDATE"].ToString(),
                                    vacationEnddate = dr["VACATIONENDDATE"].ToString(),
                                    vacationApprovalStatus = dr["VACATIONAPPROVALSTATUS"].ToString(),
                                    previousComments = dr["VACATIONCOMMENTS"].ToString(),
                                    vacationDocumentName = dr["DOCUMENTNAME"].ToString(),
                                    vactionDocumentFileName = dr["DOCUMENTFILENAME"].ToString(),
                                    vacationDocumentUrl = dr["DOCUMENTURL"].ToString(),
                                    vacationCountry=dr["VACATIONCOUNTRY"].ToString()

                                }).ToList();


                if (vacationList.Count == 0)
                {
                    objResponsevacationMdl.Data = vacationList;
                    objResponsevacationMdl.Status = false;
                    objResponsevacationMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponsevacationMdl.Data = vacationList;
                    objResponsevacationMdl.Status = true;
                    objResponsevacationMdl.Message = "Data Received successfully";
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
            return Json(objResponsevacationMdl);
        }

        /// <summary>
        /// Update Vacation Letter Details
        /// </summary>
        /// <param name="updateVacationMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateVacationDates")]
        public IHttpActionResult UpdateVacationDates(VacationModel updateVacationMdl)
        {


            List<VacationModel> updateVLList = new List<VacationModel>();
            ResponseModel _objResponseUpdateVLMdl = new ResponseModel();
            DataSet dsUserEmail = new DataSet();
            DataTable dtUpdateVLData = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "Sp_SaveVacationLetterProcess";
                cmdselect.Parameters.AddWithValue("@UID", updateVacationMdl.UID);
                cmdselect.Parameters.AddWithValue("@VacationId", updateVacationMdl.vacationId);
                cmdselect.Parameters.AddWithValue("@VacationStdate", updateVacationMdl.vacationStartdate);
                cmdselect.Parameters.AddWithValue("@VacationEnddate", updateVacationMdl.vacationEnddate);
                cmdselect.Parameters.AddWithValue("@DocumentUrl", updateVacationMdl.vacationDocumentUrl);
                cmdselect.Parameters.AddWithValue("@DocumentName", updateVacationMdl.vacationDocumentName);
                cmdselect.Parameters.AddWithValue("@DocumentFileName", updateVacationMdl.vactionDocumentFileName);
                cmdselect.Parameters.AddWithValue("@ApprovalStatus", "Submit");
                cmdselect.Parameters.AddWithValue("@VacationStatus", true);
                cmdselect.Parameters.AddWithValue("@Comments", updateVacationMdl.vacationComments);
                cmdselect.Parameters.AddWithValue("@Action", updateVacationMdl.vacationAction);
                cmdselect.Parameters.AddWithValue("@logginUserId", updateVacationMdl.userId);
                cmdselect.Parameters.AddWithValue("@VacationCountry", updateVacationMdl.vacationCountry);                
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtUpdateVLData);
                if (dtUpdateVLData != null && dtUpdateVLData.Rows.Count > 0)
                {

                    updateVLList = (from DataRow dr in dtUpdateVLData.Rows
                                    select new VacationModel()
                                    {
                                        vacationStatusMsg = dr["ResultMsg"].ToString(),
                                        vacationStatusFlag = Convert.ToInt32(dr["Msg"].ToString()),
                                        adminTOMail = dr["TOMAILID"].ToString(),
                                        adminCCMail = dr["CCMAILID"].ToString(),
                                        adminBCCMail = dr["BCCMAILID"].ToString(),
                                        adminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                        adminMailBody = dr["ADMINMAILBODY"].ToString(),
                                        userMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                        userMailBody = dr["USERMAILBODY"].ToString(),

                                    }).ToList();



                }
                if (updateVLList.Count == 0)
                {
                    _objResponseUpdateVLMdl.Data = updateVLList;
                    _objResponseUpdateVLMdl.Status = false;
                    _objResponseUpdateVLMdl.Message = "Data Not Received successfully";
                }
                else
                {

                    dsUserEmail = commonFunction.GetUserEmail(updateVacationMdl.UID, null);


                    if (dsUserEmail.Tables[0].Rows.Count > 0)
                    {
                        if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                        {
                            // User Mail
                            MailTemplates.VacationLetterMail(dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(), dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(),
                                dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                updateVacationMdl.vacationStartdate, updateVacationMdl.vacationEnddate, updateVacationMdl.vacationComments, null,
                               dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), string.Empty, updateVLList[0].userMailSubject, updateVLList[0].userMailBody, false);

                            // Admin Mail

                            MailTemplates.VacationLetterMail(dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(), dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(),
                                dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                                updateVacationMdl.vacationStartdate, updateVacationMdl.vacationEnddate, updateVacationMdl.vacationComments, null,
                                  updateVLList[0].adminTOMail, updateVLList[0].adminCCMail, updateVLList[0].adminMailSubject, updateVLList[0].adminMailBody, true);
                        }

                    }


                    _objResponseUpdateVLMdl.Data = updateVLList;
                    _objResponseUpdateVLMdl.Status = true;
                    _objResponseUpdateVLMdl.Message = "Data Received successfully";
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
            return Json(_objResponseUpdateVLMdl);
        }
        /// <summary>
        /// Update Vacation Letter Document Status
        /// </summary>
        /// <param name="VLDocStatusMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateVacationDocumentStatus")]
        public IHttpActionResult UpdateVacationDocumentStatus(VacationModel VLDocStatusMdl)
        {
            List<VacationModel> VLDocStatusList = new List<VacationModel>();
            ResponseModel objResponseVLDocStatusMdl = new ResponseModel();
            DataTable dtVLDocStatus = new DataTable();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_UpdateVacationDocStatus";
                cmdselect.Parameters.AddWithValue("@VacationUid", VLDocStatusMdl.UID);
                cmdselect.Parameters.AddWithValue("@VacationId", VLDocStatusMdl.vacationId);
                cmdselect.Parameters.AddWithValue("@DocFileName", VLDocStatusMdl.vacationDocumentName);
                cmdselect.Parameters.AddWithValue("@LoggedinUserId", VLDocStatusMdl.userId);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtVLDocStatus);

                VLDocStatusList = (from DataRow dr in dtVLDocStatus.Rows
                                   select new VacationModel()
                                   {
                                       vacationStatusFlag = Convert.ToInt32(dr["MSG"].ToString()),
                                       vacationStatusMsg = dr["ResultMsg"].ToString()

                                   }).ToList();


                if (VLDocStatusList.Count == 0)
                {
                    objResponseVLDocStatusMdl.Data = VLDocStatusList;
                    objResponseVLDocStatusMdl.Status = false;
                    objResponseVLDocStatusMdl.Message = "Data Not Received successfully";
                }
                else
                {
                    objResponseVLDocStatusMdl.Data = VLDocStatusList;
                    objResponseVLDocStatusMdl.Status = true;
                    objResponseVLDocStatusMdl.Message = "Data Received successfully";
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
            return Json(objResponseVLDocStatusMdl);
        }
        /// <summary>
        /// Cancel Vacation Letter
        /// </summary>
        /// <param name="cancelVLMdl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelVacation")]
        public IHttpActionResult CancelVacation(VacationModel cancelVLMdl)
        {
            List<VacationModel> cancelVacationList = new List<VacationModel>();
            ResponseModel objResponseCancelVLMdl = new ResponseModel();
            DataTable dtCancelVactionData = new DataTable();
            DataSet dsUserEmail = new DataSet();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_ManageVacationProcess";
                cmdselect.Parameters.AddWithValue("@VacationUID", cancelVLMdl.UID);
                cmdselect.Parameters.AddWithValue("@VacationVisaId", 0);
                cmdselect.Parameters.AddWithValue("@VacationId", cancelVLMdl.vacationId);
                cmdselect.Parameters.AddWithValue("@VacationComments", "Vacation was canceled");
                cmdselect.Parameters.AddWithValue("@VacationAction", "VacationCancel");
                cmdselect.Parameters.AddWithValue("@VacationLoggedUserId", cancelVLMdl.userId);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dtCancelVactionData);

                cancelVacationList = (from DataRow dr in dtCancelVactionData.Rows
                                      select new VacationModel()
                                      {
                                          vacationStatusFlag = Convert.ToInt32(dr["Msg"].ToString()),
                                          vacationStatusMsg = dr["ResultMsg"].ToString(),
                                          vacationStatus = dr["VACATIONSTATUS"].ToString(),
                                          adminTOMail = dr["TOMAILID"].ToString(),
                                          adminCCMail = dr["CCMAILID"].ToString(),
                                          adminBCCMail = dr["BCCMAILID"].ToString(),
                                          adminMailSubject = dr["ADMINMAILSUBJECT"].ToString(),
                                          adminMailBody = dr["ADMINMAILBODY"].ToString(),
                                          userMailSubject = dr["USERMAILSUBJECT"].ToString(),
                                          userMailBody = dr["USERMAILBODY"].ToString(),

                                      }).ToList();


                if (cancelVacationList.Count == 0)
                {
                    objResponseCancelVLMdl.Data = cancelVacationList;
                    objResponseCancelVLMdl.Status = false;
                    objResponseCancelVLMdl.Message = "Data Not Received successfully";
                }
                else
                {

                    dsUserEmail = commonFunction.GetUserEmail(cancelVLMdl.UID, null);


                    if (dsUserEmail.Tables[0].Rows.Count > 0)
                    {
                        if (dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString() != "")
                        {
                            // User Mail
                            MailTemplates.CancelVacationLetterMail(dsUserEmail.Tables[0].Rows[0]["USERID"].ToString(), dsUserEmail.Tables[0].Rows[0]["UserName"].ToString(),
                                dsUserEmail.Tables[0].Rows[0]["FNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["LNAME"].ToString(), dsUserEmail.Tables[0].Rows[0]["WORKAUTHORIZATIONTYPE"].ToString(),
                               dsUserEmail.Tables[0].Rows[0]["EMAILID"].ToString(), cancelVacationList[0].userMailSubject);

                        }

                    }

                    objResponseCancelVLMdl.Data = cancelVacationList;
                    objResponseCancelVLMdl.Status = true;
                    objResponseCancelVLMdl.Message = "Data Received successfully";
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
            return Json(objResponseCancelVLMdl);
        }
    }
}
