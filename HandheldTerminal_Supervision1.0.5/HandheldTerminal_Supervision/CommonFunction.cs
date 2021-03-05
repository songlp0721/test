using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HandheldTerminal_Supervision
{
    public static class CommonFunction
    {
        public static string[] CutString(string s, int bytecount)
        {
            List<string> mylist = new List<string>();
            char[] mychar = s.ToCharArray();
            while (true)
            {
                if (Encoding.Default.GetBytes(s).Length <= bytecount)
                {
                    mylist.Add(s);
                    break;
                }
                int n = 0;
                int i = 0;
                foreach (char c in mychar)
                {
                    n += Encoding.Default.GetBytes(c.ToString()).Length;
                    if (n <= bytecount)
                    {
                        i++;
                    }
                    else
                    {
                        mylist.Add(s.Substring(0, i));
                        s = s.Remove(0, i);
                        mychar = s.ToCharArray();
                        break;
                    }
                }

            }
            return mylist.ToArray();
        }
    }
}
