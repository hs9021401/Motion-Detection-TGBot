using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace EMGU_Example
{
    public class IniUtility
    {
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString", SetLastError = true)]
        private static extern uint WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString", SetLastError = true)]
        private static extern uint GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private string _sPath;

        public IniUtility(string IniPath)
        {
            _sPath = IniPath;
        }

        public void WriteValue(string Section, string Key, string Value)
        {
            long ret = WritePrivateProfileString(Section, Key, Value, _sPath);
        }

        public string ReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);

            GetPrivateProfileString(Section, Key, "", temp, 255, _sPath);

            return temp.ToString();
        }
    }
}
