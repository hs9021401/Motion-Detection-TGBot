using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EMGU_Example
{
    public class Settings
    {
        private IniUtility _iniReader;

        protected string SECNAME_TG = "TGInfo";
        public string TGtoken;
        public string TGSendToGroup;
        public string TGchatID;

        protected string SECNAME_IMG = "ImgInfo";
        public int UpperBound;
        public int LowerBound;
        public int FrameCount;
        public string SaveFolder;

        public Settings()
        {
            string ini_path = System.IO.Directory.GetCurrentDirectory() + "\\setting.ini";
            
            if (!File.Exists(ini_path))
            {
                System.Windows.Forms.MessageBox.Show("No ini file found.\r\n\r\nWe'll create a new ini setting for you.\r\n\r\nPlease set Telegram Bot API token and chat ID later.", 
                    "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);

                using (StreamWriter sw = new StreamWriter(ini_path))
                {
                    sw.WriteLine("[" + SECNAME_TG + "]");
                    sw.WriteLine("token=");
                    sw.WriteLine("send_to_group=1");
                    sw.WriteLine("chatid=");
                    sw.WriteLine("[" + SECNAME_IMG + "]");
                    sw.WriteLine("upper_bound=100");
                    sw.WriteLine("lower_bound=80");
                    sw.WriteLine("framecount=100");
                    sw.WriteLine("folder=" + System.IO.Directory.GetCurrentDirectory() + "\\SaveImg");
                }                                       
            }
            _iniReader = new IniUtility(ini_path);
            readAll();
        }

        public void writeAll()
        {
            _iniReader.WriteValue(SECNAME_TG, "token", TGtoken);
            _iniReader.WriteValue(SECNAME_TG, "send_to_group", TGSendToGroup);
            _iniReader.WriteValue(SECNAME_TG, "chatid", TGchatID);
            _iniReader.WriteValue(SECNAME_IMG, "upper_bound", UpperBound.ToString());
            _iniReader.WriteValue(SECNAME_IMG, "lower_bound", LowerBound.ToString());
            _iniReader.WriteValue(SECNAME_IMG, "framecount", FrameCount.ToString());
            _iniReader.WriteValue(SECNAME_IMG, "folder", SaveFolder);
        }

        public void readAll()
        {           
            TGtoken = _iniReader.ReadValue(SECNAME_TG, "token");
            TGSendToGroup = _iniReader.ReadValue(SECNAME_TG, "send_to_group");
            TGchatID = _iniReader.ReadValue(SECNAME_TG, "chatid");
            UpperBound = Int32.Parse(_iniReader.ReadValue(SECNAME_IMG, "upper_bound"));
            LowerBound = Int32.Parse(_iniReader.ReadValue(SECNAME_IMG, "lower_bound"));
            FrameCount = Int32.Parse(_iniReader.ReadValue(SECNAME_IMG, "framecount"));
            SaveFolder = _iniReader.ReadValue(SECNAME_IMG, "folder");
        }

    }
}
