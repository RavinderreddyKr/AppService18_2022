using AppService18.ExceptionLogger;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace AppService18.Others
{
    public class CommonFunction
    {
        /// <summary>
        /// EnCryption DeCryption
        /// </summary>
        public class EnCryptionDeCryption
        {
            static MACTripleDES mac3des = new MACTripleDES();
            static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            /// <summary>
            /// Encode
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static string Encode(string value)
            {
                string sRetval = "";
                try
                {
                    sRetval = System.Web.HttpUtility.UrlEncode(md5Encode(value, TamperProofKey));
                }
                catch
                {
                }
                return sRetval;
            }
            /// <summary>
            /// Decode
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static string Decode(string value)
            {
                string sRetval = "";
                try
                {
                    sRetval = System.Web.HttpUtility.UrlDecode(value);
                    sRetval = md5Decode(sRetval, TamperProofKey);
                }
                catch
                {
                }
                return sRetval;
            }

            //Function to encode the string
            public static string md5Encode(string value, string key)
            {
                string sRetval = "";
                try
                {
                    mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
                    sRetval = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value)) + System.Convert.ToChar("-") + System.Convert.ToBase64String(mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
                }
                catch { }
                return sRetval;
            }

            //Function to decode the string
            //Throws an exception if the data is corrupt
            public static string md5Decode(string value, string key)
            {
                String dataValue = "";

                try
                {
                    mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
                    dataValue = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(value.Split(System.Convert.ToChar("-"))[0]));
                }
                catch
                {
                }

                return dataValue;
            }
            public static string TamperProofKey
            {
                get
                {
                    string sTamperProofKey = ConfigurationManager.AppSettings["TamperProofKey"].ToString();
                    try
                    {
                        if (sTamperProofKey != null || sTamperProofKey != "")
                            return sTamperProofKey;
                    }
                    catch
                    {
                        return "NSIKEY";
                    }
                    return "NSIKEY";
                }
            }

        }
        /// <summary>
        /// Convert Word to Pdf
        /// </summary>
        #region PdfConversion
        public Microsoft.Office.Interop.Word.Application MSdoc;

        //Use for the parameter whose type are not known or say Missing
        object Unknown = Type.Missing;
        public  void WordtoPDF(object Source, object Target)
        {   //Creating the instance of Word Application
            if (MSdoc == null) MSdoc = new Microsoft.Office.Interop.Word.Application();

            try
            {
                MSdoc.Visible = false;
                MSdoc.Documents.Open(ref Source, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                MSdoc.Application.Visible = false;
                MSdoc.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMinimize;

                object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

                MSdoc.ActiveDocument.SaveAs(ref Target, ref format,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                       ref Unknown, ref Unknown);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Logger.LogInfo(ex);
            }
            finally
            {
                if (MSdoc != null)
                {
                    MSdoc.Documents.Close(ref Unknown, ref Unknown, ref Unknown);
                    MSdoc.Application.Quit(ref Unknown, ref Unknown, ref Unknown);
                }
                // for closing the application
                MSdoc.Quit(ref Unknown, ref Unknown, ref Unknown);
            }
        }
        #endregion
        /// <summary>
        /// Convert Excel to Pdf
        /// </summary>
        /// <param name="path"></param>
        /// <param name="TargetFile"></param>
        public void ExceltoPDF(string path, string TargetFile)
        {
            Microsoft.Office.Interop.Excel.Application excelApplication = new Microsoft.Office.Interop.Excel.Application();
            Workbook excelWorkBook = null;
            string paramSourceBookPath = path;
            object paramMissing = Type.Missing;
            string paramExportFilePath = TargetFile;
            XlFixedFormatType paramExportFormat = XlFixedFormatType.xlTypePDF;
            XlFixedFormatQuality paramExportQuality =
                XlFixedFormatQuality.xlQualityStandard;
            bool paramOpenAfterPublish = false;
            bool paramIncludeDocProps = true;
            bool paramIgnorePrintAreas = true;
            object paramFromPage = Type.Missing;
            object paramToPage = Type.Missing;

            try
            {
                // Open the source workbook.
                excelWorkBook = excelApplication.Workbooks.Open(paramSourceBookPath,
                    paramMissing, paramMissing, paramMissing, paramMissing,
                    paramMissing, paramMissing, paramMissing, paramMissing,
                    paramMissing, paramMissing, paramMissing, paramMissing,
                    paramMissing, paramMissing);

                // Save it in the target format.
                if (excelWorkBook != null)
                    excelWorkBook.ExportAsFixedFormat(paramExportFormat,
                        paramExportFilePath, paramExportQuality,
                        paramIncludeDocProps, paramIgnorePrintAreas, paramFromPage,
                        paramToPage, paramOpenAfterPublish,
                        paramMissing);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Logger.LogInfo(ex);
            }
            finally
            {
                // Close the workbook object.
                if (excelWorkBook != null)
                {
                    excelWorkBook.Close(false, paramMissing, paramMissing);
                    excelWorkBook = null;
                }

                // Quit Excel and release the ApplicationClass object.
                if (excelApplication != null)
                {
                    excelApplication.Quit();
                    excelApplication = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

            }




        }
        /// <summary>
        /// Convert Image to Pdf
        /// </summary>
        /// <param name="srcFilename"></param>
        /// <param name="dstFilename"></param>
        public void ConvertImageToPdf(string srcFilename, string dstFilename)
        {
            iTextSharp.text.Rectangle pageSize = null;

            using (var srcImage = new Bitmap(srcFilename))
            {
                pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
            }
            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();
                var image = iTextSharp.text.Image.GetInstance(srcFilename);
                document.Add(image);
                document.Close();

                System.IO.File.WriteAllBytes(dstFilename, ms.ToArray());
            }
        }
        /// <summary>
        /// Convert PPT to Pdf
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Target"></param>
        public void PPTtoPDF(object Source, object Target)
        {
            Microsoft.Office.Interop.PowerPoint.Application pptApplication = null;
            Microsoft.Office.Interop.PowerPoint.Presentation pptPresentation = null;
            object unknownType = Type.Missing;
            try
            {
                pptApplication = new Microsoft.Office.Interop.PowerPoint.Application();

                pptPresentation = pptApplication.Presentations.Open((string)Source,
                                                                 Microsoft.Office.Core.MsoTriState.msoTrue,
                                                                  Microsoft.Office.Core.MsoTriState.msoTrue,
                                                                  Microsoft.Office.Core.MsoTriState.msoFalse);
                if (pptPresentation != null)
                {
                    pptPresentation.ExportAsFixedFormat((string)Target,
                                                         Microsoft.Office.Interop.PowerPoint.PpFixedFormatType.ppFixedFormatTypePDF,
                                                         Microsoft.Office.Interop.PowerPoint.PpFixedFormatIntent.ppFixedFormatIntentPrint,
                                                         MsoTriState.msoFalse,
                                                         Microsoft.Office.Interop.PowerPoint.PpPrintHandoutOrder.ppPrintHandoutVerticalFirst,
                                                         Microsoft.Office.Interop.PowerPoint.PpPrintOutputType.ppPrintOutputSlides,
                                                         MsoTriState.msoFalse, null,
                                                         Microsoft.Office.Interop.PowerPoint.PpPrintRangeType.ppPrintAll, string.Empty,
                                                         true, true, true, true, false, unknownType);

                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Logger.LogInfo(ex);
            }
            finally
            {
                // Close and release the Document object.
                if (pptPresentation != null)
                {
                    pptPresentation.Close();
                    pptPresentation = null;
                }

                // Quit Word and release the ApplicationClass object.
                pptApplication.Quit();
                pptApplication = null;
                GC.Collect();
            }
        }
        /// <summary>
        /// Getting Employee Information based on UserId Or UID
        /// </summary>
        /// <param name="iUID"></param>
        /// <param name="sUserId"></param>
        /// <returns></returns>
        public DataSet GetUserEmail(Int64 iUID, string sUserId)
        {
            DataSet dsuserEmail = new DataSet();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();

            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_GetEmployeeEmailIdFromUserId";
                cmdselect.Parameters.AddWithValue("@UID", iUID);
                cmdselect.Parameters.AddWithValue("@UserId", sUserId);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dsuserEmail);

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

            return dsuserEmail;
        }
        /// <summary>
        /// Getting Timesheet Details
        /// </summary>
        /// <param name="UId"></param>
        /// <param name="TimeSheetDocId"></param>
        /// <param name="DocumentFileName"></param>
        /// <param name="LoggedInUserId"></param>
        /// <param name="SearchValue"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public DataSet ManageTimeSheetsFileView(Int64 UId, Int64 TimeSheetDocId, string DocumentFileName, string LoggedInUserId, string SearchValue, string Action)
        {
            DataSet dsManageTSFileView = new DataSet();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();
            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_ManageTimeSheetFileView";
                cmdselect.Parameters.AddWithValue("@UID", UId);
                cmdselect.Parameters.AddWithValue("@TimeSheetDocId", TimeSheetDocId);
                cmdselect.Parameters.AddWithValue("@DocumentFileName", DocumentFileName);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                cmdselect.Parameters.AddWithValue("@SearchValue", SearchValue);
                cmdselect.Parameters.AddWithValue("@Action", Action);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dsManageTSFileView);

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

            return dsManageTSFileView;
        }
        /// <summary>
        /// Email Documents Attachments Success and failure status updating
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="description"></param>
        /// <param name="status"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns></returns>
        public DataSet EmailDocumentStatus(Int64 UID, string description, string status, string loggedInUserId, string mailedFrom)
        {
            DataSet dsEmailDocumentStatus = new DataSet();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();
            try
            {

                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_EmailAttachmentStatus";
                cmdselect.Parameters.AddWithValue("@UID", UID);
                cmdselect.Parameters.AddWithValue("@Description", description);
                cmdselect.Parameters.AddWithValue("@Status", status);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", loggedInUserId);
                cmdselect.Parameters.AddWithValue("@MailedFrom", mailedFrom);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dsEmailDocumentStatus);
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

            return dsEmailDocumentStatus;
        }
        /// <summary>
        /// Get Vacation Letter Id to Save Document BY Vacation ID
        /// </summary>
        /// <param name="VacationUid"></param>
        /// <param name="LoggedInUserId"></param>
        /// <returns></returns>
        public Int64 GetVacationLetterId(Int64 VacationUid, string LoggedInUserId)
        {
            DataSet dsVacationId = new DataSet();
            SqlDataAdapter da;
            SqlCommand cmdselect = new SqlCommand();
            Int64 vacationId = 0;
            try
            {
                cmdselect.CommandType = CommandType.StoredProcedure;
                cmdselect.CommandText = "sp_GetVacationLetterId";
                cmdselect.Parameters.AddWithValue("@VacationUid", VacationUid);
                cmdselect.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                cmdselect.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["APPServiceConstring"].ToString());
                cmdselect.Connection.Open();
                da = new SqlDataAdapter(cmdselect);
                da.Fill(dsVacationId);
                if (dsVacationId.Tables[0].Rows.Count > 0)
                {
                    if (dsVacationId.Tables[0].Rows[0]["Msg"].ToString() == "1")
                    {
                        vacationId = Convert.ToInt64(dsVacationId.Tables[1].Rows[0]["VACATIONID"]);

                    }
                }

            }
            catch (SqlException ex)
            {
                Logger.LogInfo(ex);
                // return null;
            }
            finally
            {
                cmdselect.Connection.Close();
            }

            return vacationId;
        }

    }
}