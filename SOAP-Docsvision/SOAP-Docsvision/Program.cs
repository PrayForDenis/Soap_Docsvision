using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SOAP_Docsvision
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationManager();
            string sessionOpenResult = CallWebService(config, config.SessionLoginXml, config.SessionLoginAction);

            if (string.IsNullOrEmpty(sessionOpenResult)) {
                Console.WriteLine("Error Session Create");
                Console.ReadKey();
                return;
            }

            string sessionId = ParseSessionIdFromResponse(sessionOpenResult);
            string createCardResult = CallWebService(config, config.NewCardXml.Replace("{$sessionId$}", sessionId), config.NewCardAction);

            if (string.IsNullOrEmpty(createCardResult)) {
                Console.WriteLine("Error Card Create");
                Console.ReadKey();
                return;
            }

            string sessionCloseResult = CallWebService(config, config.SessionCloseXml.Replace("{$sessionId$}", sessionId), config.SessionCloseAction);

            if (string.IsNullOrEmpty(sessionCloseResult)) {
                Console.WriteLine("Error Session Close");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("------------------------");
            Console.WriteLine("Success! Press any key to stop...");
            Console.ReadKey();
        }

        private static string CallWebService(ConfigurationManager config, string xml, string action)
        {
            var url = config.UrlDocsvision;
            var username = config.UserName;
            var password = config.Password;

            string soapResult = "";

            try {
                XmlDocument soapEnvelopeXml = CreateSoapEnvelope(xml);
                HttpWebRequest webRequest = CreateWebRequest(url, action, username, password);
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                asyncResult.AsyncWaitHandle.WaitOne();

                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                    Console.Write(soapResult);
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace); 
            }

            return soapResult;
        }

        private static HttpWebRequest CreateWebRequest(string url, string action, string username, string password)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(
                new Uri(url),
                "Basic",
                new NetworkCredential(username, password)
            );

            webRequest.Credentials = credentialCache;
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(string xml)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            try { 
                soapEnvelopeDocument.LoadXml(xml);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
            }
            return soapEnvelopeDocument;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        private static string ParseSessionIdFromResponse(string response)
        {
            XDocument tst = XDocument.Parse(response);
            XNamespace xmlns = "http://schemas.docsvision.com/Platform/2009-02-03/StorageServer/";
            var tstr = tst.Descendants(xmlns + "sessionId");
            return tstr.FirstOrDefault().Value;
        }
    }
}
