using System.Collections.Generic;
using Microsoft.Win32;
namespace WincePda
{
    public class Win32
    {
        public static List<string> AllNames()
        {
            List<string> names = new List<string>();
            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(@"Comm\RasBook");
            string[] subNames=RegKey.GetSubKeyNames();
            for (int i = 0; i < subNames.Length;i++ )
            {
                RegistryKey subKey= RegKey.OpenSubKey(subNames[i]);
                if (subKey.ValueCount==3)
                {
                    names.Add(subNames[i]);
                } subKey.Close();

            }
            RegKey.Close();
            return names;
        }
    }

    public class pdaNet
    {
        /// <summary>
        /// 读取DNS信息，判断当前的dns的ip信息。如果是127.0.0.1表示没有网络，如果都是192.168打头的，表示当前连接不是gprs
        /// </summary>
        /// <returns></returns>
        public static bool isGPRSNet()
        {
            string hostName = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry myIp = System.Net.Dns.GetHostEntry(hostName);

            System.Net.IPAddress[] ipList = myIp.AddressList;

            if (ipList.Length == 0)
            {
                return false;
            }
            else
            {
                if (ipList[0].ToString() == "127.0.0.1")
                {
                    //表示没有网络连接
                    return false;
                }


                //计数器，判断有几个192.168打头的ip
                int num = 0;
                for (int i = 0; i < ipList.Length; i++)
                {
                    if (ipList[i].ToString().StartsWith("192.168"))
                    {
                        num += 1;
                    }
                }
                if (num == ipList.Length)
                {
                    //全部都是192.168打头的
                    return false;
                }
                else
                {
                    //有不是192.168打头的ip，要进一步查询是否是GPRS连接
                    return true;
                }

            }

        }
    }

}
