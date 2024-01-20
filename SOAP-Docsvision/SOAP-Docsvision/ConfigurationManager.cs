using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace SOAP_Docsvision
{
    public class ConfigurationManager
    {
        private readonly string _urlDocsvision;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _sessionLoginAction;
        private readonly string _newCardAction;
        private readonly string _sessionCloseAction;
        private readonly string _baseName;
        private readonly string _documentId;
        private readonly string _sessionLoginXml;
        private readonly string _newCardXml;
        private readonly string _sessionCloseXml;

        public ConfigurationManager() 
        {
            _urlDocsvision = ConfigurationSettings.AppSettings["UrlDocsvision"];
            _userName = ConfigurationSettings.AppSettings["UserName"];
            _password = ConfigurationSettings.AppSettings["Password"];
            _sessionLoginAction = ConfigurationSettings.AppSettings["SessionLoginAction"];
            _newCardAction = ConfigurationSettings.AppSettings["NewCardAction"];
            _sessionCloseAction = ConfigurationSettings.AppSettings["SessionCloseAction"];
            _baseName = ConfigurationSettings.AppSettings["BaseName"];
            _documentId = ConfigurationSettings.AppSettings["DocumentId"];
            _sessionLoginXml = File.ReadAllText(ConfigurationSettings.AppSettings["SessionXml"]).Replace("{$baseName$}", _baseName);
            _newCardXml = File.ReadAllText(ConfigurationSettings.AppSettings["NewCardXml"]).Replace("{$cardId$}", _documentId);
            _sessionCloseXml = File.ReadAllText(ConfigurationSettings.AppSettings["SessionCloseXml"]);
        }

        public string UrlDocsvision => _urlDocsvision;
        public string UserName => _userName;
        public string Password => _password;
        public string SessionLoginAction => _sessionLoginAction;
        public string NewCardAction => _newCardAction;
        public string SessionCloseAction => _sessionCloseAction;
        public string SessionLoginXml => _sessionLoginXml;
        public string NewCardXml => _newCardXml;
        public string SessionCloseXml => _sessionCloseXml;
    }
}
