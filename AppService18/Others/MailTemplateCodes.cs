using AppService18.ExceptionLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace AppService18.Others
{
    public class MailTemplateCodes
    {
        /// <summary>
        /// Getting Mail Template Code
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static string GetMailTemplateCode(string searchTerm)
        {
          
            try
            {
                string xmlfile = System.Web.Hosting.HostingEnvironment.MapPath("~/MailTemplateCodes.xml");

                var doc = XDocument.Load(xmlfile);

                return (from c in doc.Descendants("MailTemplate")
                        where ((String)c.Attribute("key")).Equals(searchTerm)
                        select (String)c.Element("Code")).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.LogInfo(ex);
                throw ex;
            }

        }
    }
}