using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Web;

namespace AppService18.ExceptionLogger
{
    public class Logger
    {

        #region Variable Declaration
        private static DirectoryInfo directoryInfo;
        private static FileStream fileStream;
        private static StreamWriter streamWriter;
        private static StackTrace stackTrace;
        private static MethodBase methodBase;
        #endregion

        #region Methods
        /// <summary>
        /// This method is used maintain the error information
        /// </summary>
        /// <param name="info">info of type object</param>
        private static void Info(Object info)
        {

            //Gets folder & file information of the log file

            string LogFolderName = Convert.ToString(ConfigurationManager.AppSettings["LogFolderName"]);

            string LogFileName = Convert.ToString(ConfigurationManager.AppSettings["LogFileName"]);

            string LogFileRoot = Convert.ToString(ConfigurationManager.AppSettings["ErrorLogFilePath"]);

            string ErrorLogFilePath = LogFileRoot + LogFolderName + "\\" + LogFileName;



            //Check for existence of logger file
            if (File.Exists(ErrorLogFilePath))
            {
                try
                {
                    fileStream = new FileStream(ErrorLogFilePath, FileMode.Append, FileAccess.Write);

                    streamWriter = new StreamWriter(fileStream);

                    string val = Convert.ToString(DateTime.Now) + Convert.ToString(Environment.NewLine + info);

                    streamWriter.WriteLine(val);

                }
                catch (ConfigurationErrorsException ex)
                {
                    LogInfo(ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    LogInfo(ex);
                }
                catch (FileNotFoundException ex)
                {
                    LogInfo(ex);
                }
                catch (PathTooLongException ex)
                {
                    LogInfo(ex);
                }
                catch (ArgumentException ex)
                {
                    LogInfo(ex);
                }
                catch (SecurityException ex)
                {
                    LogInfo(ex);
                }
                catch (Exception Ex)
                {
                    LogInfo(Ex);
                }
                finally
                {
                    Dispose();
                }
            }
            else
            {

                //If file doesn't exist create one
                try
                {

                    directoryInfo = Directory.CreateDirectory(LogFileRoot + "\\" + LogFolderName);

                    fileStream = File.Create(ErrorLogFilePath);

                    streamWriter = new StreamWriter(fileStream);

                    String val1 = Convert.ToString(DateTime.Now) + Convert.ToString(Environment.NewLine + info);

                    streamWriter.WriteLine(val1);

                    streamWriter.Close();

                    fileStream.Close();

                }
                catch (FileNotFoundException fileEx)
                {
                    LogInfo(fileEx);
                }
                catch (DirectoryNotFoundException dirEx)
                {
                    LogInfo(dirEx);
                }
                catch (Exception ex)
                {
                    LogInfo(ex);
                }
                finally
                {
                    Dispose();
                }

            }
        }
        /// <summary>
        /// This method is used  to matain the log information
        /// </summary>
        /// <param name="ex">ex of type exception</param>
        public static void LogInfo(Exception ex)
        {
            try
            {

                //Writes error information to the log file including name of the file, line number & error message description

                stackTrace = new StackTrace(ex, true);

                string fileNames = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileName();

                //fileNames = fileNames.Substring(fileNames.LastIndexOf(Application.ProductName));

                Int32 lineNumber = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileLineNumber();

                methodBase = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetMethod();    //These two lines are respnsible to find out name of the method

                String methodName = methodBase.Name;

                // LoggedUserInfo objLoggedUser;
                //String strUserName = string.Empty;

                //if (HttpContext.Current.Session != null)
                //{
                //    if (HttpContext.Current.Session["UserName"] != null)
                //    {
                //        //objLoggedUser = (LoggedUserInfo)HttpContext.Current.Session["LoggedInUserInfo"];
                //        strUserName = Convert.ToString(HttpContext.Current.Session["UserName"]);  //objLoggedUser.EntityID.ToString() + "-" + objLoggedUser.LastName.Trim().ToString() + " " + objLoggedUser.FirstName.Trim().ToString();
                //    }
                //}

                Info(
                        // "EntityID - User Name:" + strUserName + Environment.NewLine +
                        "Source:" + fileNames + Environment.NewLine +
                        "Method:" + methodName + Environment.NewLine +
                        "Line Number:" + Convert.ToString(lineNumber) + Environment.NewLine +
                        "Error Message:" + ex.Message + Environment.NewLine +
                        //  "Stack Trace:" + Convert.ToString(ex.StackTrace) + Environment.NewLine +
                        "-------------------------------------------------------------------------------------------------------------------------------------------------------------"

                    );
            }
            catch (Exception genEx)
            {
                Info(ex.Message);

                Logger.LogInfo(genEx);
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// This method is used to maintain the log information
        /// </summary>
        /// <param name="message">message of type string</param>
        public static void LogInfo(string message)
        {
            try
            {
                //Write general message to the log file
                Info("Message-----" + message + "---------------------------------------------------------------------------------------------------");
            }
            catch (Exception genEx)
            {
                Info(genEx.Message);
            }

        }
        /// <summary>
        /// This method is used to dispose all the  parameters
        /// </summary>
        private static void Dispose()
        {
            if (directoryInfo != null)
                directoryInfo = null;

            if (streamWriter != null)
            {
                streamWriter.Close();
                streamWriter.Dispose();
                streamWriter = null;
            }
            if (fileStream != null)
            {
                fileStream.Dispose();
                fileStream = null;
            }
            if (stackTrace != null)
                stackTrace = null;
            if (methodBase != null)
                methodBase = null;
        }



        public static void successinfo(string methodinfo)
        {
            try
            {

                //Writes error information to the log file including name of the file, line number & error message description

                // stackTrace = new StackTrace();

                // string fileNames = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileName();

                //// Int32 lineNumber = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileLineNumber();

                // methodBase = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetMethod();    //These two lines are respnsible to find out name of the method

                // String methodName = methodBase.Name;


                Info(

                         //"Source:" + fileNames + Environment.NewLine +
                         //"Method:" + methodName + Environment.NewLine +
                         // "Line Number:" + Convert.ToString(lineNumber) + Environment.NewLine +
                         // "Error Message:" + ex.Message + Environment.NewLine +
                         "Success Message:" + methodinfo + Environment.NewLine +
                        // "Stack Trace:" + Convert.ToString(ex.StackTrace) + Environment.NewLine +
                        "-------------------------------------------------------------------------------------------------------------------------------------------"

                    );
            }
            //catch (Exception )
            //{


            //    //clsLogger.LogInfo(genEx);
            //}
            finally
            {
                Dispose();
            }
        }

    }
}
#endregion