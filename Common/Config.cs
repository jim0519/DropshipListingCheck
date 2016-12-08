using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;

namespace Common
{
    public class Config
    {
        private static Config _instance;
        private Config()
        {
        
        }

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Config();
                }
                return _instance;
            }
        }

        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DefaultContext"].ConnectionString;
            }
        }

        public string LogPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory+"\\Log\\";
            }
        }

        public string SourceFilePath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["SourceFilePath"];
            }
        }

        public string FilePath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["FilePath"];
            }
        }

        public string DelaySecondRange
        {
            get
            {
                return ConfigurationManager.AppSettings["DelaySecondRange"];
            }
        }

        public string FtpHostIP
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpHostIP"];
            }
        }

        public string FtpUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpUserName"];
            }
        }

        public string FtpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpPassword"];
            }
        }

        public string FtpFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpFilePath"];
            }
        }

        public string OODataFeedName
        {
            get
            {
                return ConfigurationManager.AppSettings["OODataFeedName"];
            }
        }

        public string DDDataFeedName
        {
            get
            {
                return ConfigurationManager.AppSettings["DDDataFeedName"];
            }
        }

        public string FtpOOFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpOOFilePath"];
            }
        }

        public string FtpDDFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpDDFilePath"];
            }
        }
        
    }
}
