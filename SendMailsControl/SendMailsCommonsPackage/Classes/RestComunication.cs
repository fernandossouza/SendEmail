using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Net;
using System.IO;
using System.Configuration;
using System.Reflection;

namespace SendMailsCommonsPackage.Classes
{
    
    public class RestComunication
    {
         

        public string RequestOtherAPI(string method, string url, string authHeader, string contentType, string data = null)
        {
            ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 

            Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            string content = null;

            WebRequest webRequest = null;
            WebResponse webResponse = null;
            HttpWebResponse httpResponse = null;
            Stream dataStream = null;
            StreamReader streamReader = null;

            try
            {
                webRequest = WebRequest.Create(url);
                webRequest.Method = method;
                webRequest.ContentType = contentType;
                webRequest.Headers.Add(HttpRequestHeader.Authorization, authHeader);

                // If there is data to send,
                // do appropriate logic
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(data);
                    webRequest.ContentLength = byteArray.Length;
                    dataStream = webRequest.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                webResponse = webRequest.GetResponse();

                httpResponse = (HttpWebResponse)webResponse;

                dataStream = webResponse.GetResponseStream();

                streamReader = new StreamReader(dataStream);

                content = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().Name + " - " + ex.Message);
            }
            finally
            {
                if (streamReader != null) streamReader.Close();
                if (dataStream != null) dataStream.Close();
                if (httpResponse != null) httpResponse.Close();
                if (webResponse != null) webResponse.Close();
                if (webRequest != null) webRequest.Abort();
            }

            return content;
        }

    }
}
