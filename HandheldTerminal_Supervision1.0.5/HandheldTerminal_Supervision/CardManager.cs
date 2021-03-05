using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data;

namespace HandheldTerminal_Supervision
{

    class CLR_ISO14443A
    {

        #region API

        [DllImport("DeviceAPI.dll", EntryPoint = "SerialPortFunctionSwitch_Ex")]
        private static extern bool SerialPortFunctionSwitch_Ex(int iModule);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_init")]
        private static extern bool RF_ISO14443A_init();

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_free")]
        private static extern void RF_ISO14443A_free();

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_request_Ex")]
        private static extern int RF_ISO14443A_request_Ex(int iMode, byte[] pszData);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_authentication")]
        private static extern int RF_ISO14443A_authentication(int iMode, int iBlock, byte[] pszKey, int iLenKey);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_read")]
        private static extern int RF_ISO14443A_read(int iBlock, byte[] pszData);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_write")]
        private static extern int RF_ISO14443A_write(int iBlock, byte[] pszData, int iLenData);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_initval")]
        private static extern int RF_ISO14443A_initval(int iBlock, int iValue);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_increment")]
        private static extern int RF_ISO14443A_increment(int iBlockValue, int iBlockResult, int iValue);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_decrement")]
        private static extern int RF_ISO14443A_decrement(int iBlockValue, int iBlockResult, int iValue);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_readval")]
        private static extern int RF_ISO14443A_readval(int iBlock, byte[] pszValue);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ModeSwitch")]
        private static extern int RF_ModeSwitch(int iMode);

        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_halt")]
        private static extern int RF_ISO14443A_halt();


        [DllImport("DeviceAPI.dll", EntryPoint = "RF_ISO14443A_ul_write")]
        private static extern int RF_ISO14443A_ul_write(int iBlock, byte[] pszData, int iLenData);


        private const int ISO14443A = 0;
        #endregion

        public static string UID = string.Empty;
        public static string TagType = string.Empty;
        DataSet dt = new DataSet();
        private static bool _PowerOn = false;

        #region 公用RFID操作的方法
        /// <summary>
        /// 模块初始化
        /// </summary>
        /// <returns></returns>
        public static bool initMode()
        {

            bool iRes = RF_ISO14443A_init();

            RF_ModeSwitch(ISO14443A);

            return iRes;

        }

        /// <summary>
        /// halt 标签休眠
        /// </summary>
        /// <returns></returns>
        public static bool halt()
        {
            if (RF_ISO14443A_halt() == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public static void freeMode()
        {
            RF_ISO14443A_free();
        }


        /// <summary>
        /// 寻卡  成功返回true
        /// </summary>
        /// <returns></returns>
        public static bool ReadID(byte[] pszData)
        {
            //寻卡指令 
            int iRes = RF_ISO14443A_request_Ex(1, pszData);

            if (iRes == 0x00)
                return true;
            else
                return false;
            //return BitConverter.ToString(pszData, 1, pszData[0]).Replace("-", "");
        }

        /// <summary>
        /// 读取指定扇区的所有数据
        /// </summary>
        /// <param name="sector"></param>
        /// <returns></returns>
        public static string[] ReadData(int sector)
        {
            

            string str = string.Empty;
            byte[] pszData = new byte[255];

            int iRes = -1;
            for (int i = 0; i < 4; i++)
            {
                iRes = RF_ISO14443A_read(sector * 4 + i, pszData);

                if (iRes == 0x00)
                {
                    str += BitConverter.ToString(pszData, 1, pszData[0]).Replace("-", "");
                    if (i < 3)
                    {
                        str += ",";
                    }
                }
                else
                {
                    return null;
                }
            }
            string[] strData = str.Split(',');
            return strData;
        }


        /// <summary>
        /// Ult标签写数
        /// </summary>
        /// <param name="iPage"></param>
        /// <param name="pszData"></param>
        /// <returns></returns>
        public static bool data_ul_write(int iPage, byte[] pszData)
        {
            int res = RF_ISO14443A_ul_write(iPage, pszData, pszData.Length);
            if (res == 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// 密钥认证 sector为扇区号，key为密码,成功返回值为true
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool readQ(int sector, string key)
        {

            byte[] pszKey = new byte[6];
            //string strKey = "FFFFFFFFFFFF";
            try
            {
                for (int i = 0; i < pszKey.Length; i++)
                {
                    if ((2 * i + 2) > key.Length)
                    {
                        break;
                    }
                    pszKey[i] = Convert.ToByte(key.Substring(2 * i, 2), 16);
                }
            }
            catch (System.Exception)
            {
                return false;
            }
            int ires = RF_ISO14443A_authentication(0, sector * 4, pszKey, 6);
            if (ires == 0x00)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 寻卡操作，成功则返回字符串， 失败返回空
        /// </summary>
        /// <returns></returns>
        public static bool getRFID()
        {
            if (!_PowerOn)
            { _PowerOn = initMode(); }
            
            byte[] pszData = new byte[255];
            string strRFID = string.Empty;
            UID = string.Empty;
            TagType = string.Empty;
            //电子标签  寻卡操作 look for cards
            int iRes = RF_ISO14443A_request_Ex(1, pszData);

            if (iRes == 0x00)
            {
                //寻卡成功  返回数组 0字节数据长度 1，2字节ATQA 3字节UID长度 4字节后为UID信息
                //Search card successfully, 0 bytes  return the length of the array, 1,2 byte return ATQA, 3 bytes return UID length, remain bytes return UID.
                strRFID = BitConverter.ToString(pszData, 4, pszData[3]).Replace("-", "");
                UID = strRFID;


                if ((pszData[1] == 0x44) && (pszData[2] == 0x00))
                {
                    TagType = "ultra_light";
                }
                else if ((pszData[1] == 0x04) && (pszData[2] == 0x00))
                {
                    TagType = "Mifare_One(S50)";
                }
                else if ((pszData[1] == 0x02) && (pszData[2] == 0x00))
                {
                    TagType = "Mifare_One(S70)";
                }
                else if ((pszData[1] == 0x44) && (pszData[2] == 0x03))
                {
                    TagType = "Mifare_DESFire";
                }
                else if ((pszData[1] == 0x08) && (pszData[2] == 0x00))
                {
                    TagType = "Mifare_Pro";
                }
                else if ((pszData[1] == 0x04) && (pszData[2] == 0x03))
                {
                    TagType = "Mifare_ProX";
                }

                return true;
            }
            else
                return false;


        }

        /// <summary>
        /// 验证密钥  白卡的默认密钥为FFFFFFFFFFFF
        /// </summary>
        /// <param name="sector">扇区号</param>
        /// <returns>true表示密钥正确，false表示密钥错误</returns>
        public static bool authentication(int sector)
        {
            byte[] pszKey = new byte[6] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            int ires = RF_ISO14443A_authentication(0, sector * 4 + 3, pszKey, 6);
            if (ires == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 写数操作
        /// </summary>
        /// <param name="sector">扇区号</param>
        /// <param name="block">块序号</param>
        /// <param name="dataT">要写入的z字符</param>
        /// <returns>true为成功，false为失败</returns>
        public static bool writeData(int sector, int block, string dataT)
        {

            byte[] bWrite = System.Text.Encoding.GetEncoding("GB2312").GetBytes(dataT);
            byte[] pszData = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                pszData[i] = 0x20;      //  空格的ACSII码值为0x20
            }
            int Leng = bWrite.Length > 16 ? 16 : bWrite.Length;
            for (int j = 0; j < Leng; j++)
            {
                pszData[j] = bWrite[j];
            }

            int iRes = RF_ISO14443A_write(sector * 4 + block, pszData, pszData.Length);
            if (iRes == 0)
                return true;
            else
                return false;


        }


        /// <summary>
        /// 读数操作
        /// </summary>
        /// <param name="sector">扇区号</param>
        /// <param name="block">块序号</param>
        /// <returns>返回读取到的字符，读数失败返回空</returns>
        public static string readData(int sector, int block)
        {
            byte[] pszData = new byte[30];
            int iRes = RF_ISO14443A_read(sector * 4 + block, pszData);
            if (iRes == 0)
                return System.Text.Encoding.GetEncoding("GB2312").GetString(pszData, 1, pszData[0]);
            else
                return string.Empty;
        }


        /// <summary>
        /// 读取Ult的数据
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="block"></param>
        /// <param name="dataInBlcok"></param>
        /// <returns></returns>
        public static bool readUltData(int i, out string dataInBlcok)
        {
            byte[] pszData = new byte[30];
            int iRes = RF_ISO14443A_read(i, pszData);
            if (iRes == 0)
            {
                dataInBlcok = BitConverter.ToString(pszData, 1, 4).Replace("-", "");
                return true;
            }
            else
            {
                dataInBlcok = "Failure";
                return false;
            }

        }


        /// <summary>
        /// 将字符转换成byte类型。字符长度长于8的，取低8位，字符不满8位的，高位补零
        /// </summary>
        /// <param name="dataToWrite"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool getUltData(string dataToWrite, out byte[] data)
        {
            try
            {
                data = new byte[4];
                string[] dataStr = new string[] { "00", "00", "00", "00" };
                int L = dataToWrite.Length;
                if (L > 8)
                {
                    dataToWrite = dataToWrite.Substring(L - 8, 8);
                }
                if (L < 8)
                {
                    for (int i = L; i < 8; i++)
                    {
                        dataToWrite = "0" + dataToWrite;
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    dataStr[i] = dataToWrite.Substring(2 * i, 2);
                    data[i] = Convert.ToByte(dataStr[i], 16);

                }


                return true;
            }
            catch (Exception)
            {
                data = new byte[] { 0, 0, 0, 0 };
                return false;
            }


        }



        /// <summary>
        /// 往指定扇区和block写入字符，按GB2312编码
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="block"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool writeGB2312(int sector ,int block, string msg)
        {
            byte[] dataA = System.Text.Encoding.GetEncoding("GB2312").GetBytes(msg);
            byte[] pszData = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                //初始化0x20，即全" "字符。
                pszData[i] = 0x20;
            }
            for (int j = 0; j < dataA.Length; j++)
            {
                pszData[j] = dataA[j];
            }
            int iRes = RF_ISO14443A_write(sector*4+ block, pszData, pszData.Length);
            if (iRes == 0)
                return true;
            else
                return false;
        }


        /// <summary>
        /// 读取指定block的数据，按GB2312译码
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool readDataGB2312(int sector, int block, ref string dataMsg)
        {
            try
            {
                dataMsg = string.Empty;
                byte[] pszData = new byte[30];
                int iRes = RF_ISO14443A_read(sector * 4 + block, pszData);
                if (iRes == 0)
                {
                    dataMsg = System.Text.Encoding.GetEncoding("GB2312").GetString(pszData, 1, pszData[0]);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void PowerOff()
        {
            if (_PowerOn )
            {
                //ISO14443A 资源释放（下电操作） ISO14443A Resources released（Power Off）
                RF_ISO14443A_free();
                _PowerOn = false;
            }
        }

        #endregion




    }
    public class Card
    {
        private Card()
        { }

        private static Card _card;
        public static Card CreateCard()
        {
            if (_card == null)
            {
                _card = new Card();
            }
            return _card;

        }

        public Dictionary<int, string> CardDataDic = new Dictionary<int, string>();
        public string err;
        public string id;


        public void Clear()
        {
            id = "";
            CardDataDic.Clear();
        }

    }

    public class CardReadManager
    {

        public enum CardReadStateType
        { 
            /// <summary>
            /// 读同一卡片中
            /// </summary>
            Reading,
            /// <summary>
            /// 读取到卡片
            /// </summary>
            Readed,
            /// <summary>
            /// 未读到
            /// </summary>
            ReadNot,
            /// <summary>
            /// 读取错误
            /// </summary>
            ReadErr,
            /// <summary>
            /// 写成功
            /// </summary>
            WriteSuccess,
            /// <summary>
            /// 写失败
            /// </summary>
            WriteErr,
        }


        private string _key = "FFFFFFFFFFFF";

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        } 
        public delegate void ReadCardEventHandle(CardReadStateType cardreadtype, string CardID,Card card,string ErrMsg);
        public event ReadCardEventHandle ReadCardEvent;
        private Card _Card = Card.CreateCard();
        private string _Uid = string.Empty;
        public void ReadCard()
        {
            if (CLR_ISO14443A.getRFID())//读取卡片成功
            {
                
                if (_Uid == CLR_ISO14443A.UID) 
                {
                    if (ReadCardEvent != null) { ReadCardEvent(CardReadStateType.Reading, CLR_ISO14443A.UID, _Card, ""); }//读同一卡片中
                    return; 
                }
                else 
                { _Uid = CLR_ISO14443A.UID; _Card.Clear(); }
                for (int s = 0; s < 16; s++)
                {
                    if (CLR_ISO14443A.readQ(s, _key))
                    {
                        for (int b = 0; b < 3; b++)
                        {
                            string msg = string.Empty;
                            if (s == 0 && b == 0) { continue; }
                            if (CLR_ISO14443A.readDataGB2312(s, b, ref msg))
                            {
                                _Card.CardDataDic.Add(s * 4 + b, msg);
                            }
                            else
                            {
                                _Card.err = "读取错误";
                                if (ReadCardEvent != null) { ReadCardEvent(CardReadStateType.ReadErr, CLR_ISO14443A.UID, _Card, "读取卡片块时发成错误,扇区:"+s.ToString()+" 块:"+b.ToString()); }//读块成功
                            }
                        }
                    }
                    else
                    {
                        _Card.err = "验证错误";
                        _Card.id = CLR_ISO14443A.UID;
                        if (ReadCardEvent != null) { ReadCardEvent(CardReadStateType.ReadErr, CLR_ISO14443A.UID, _Card, "读取卡片块时验证错误,扇区:" + s.ToString()); }//读块成功
                    }
                }

                if (ReadCardEvent != null) { _Card.id = CLR_ISO14443A.UID; ; ReadCardEvent(CardReadStateType.Readed, CLR_ISO14443A.UID, _Card, ""); }//读卡成功
            }
            else
            {
                _Uid = string.Empty;
                if (ReadCardEvent != null) { ReadCardEvent(CardReadStateType.ReadNot, CLR_ISO14443A.UID, _Card, ""); }//未读到卡
                _Card.Clear();
            }
        }
        public bool WriteCard(int absolutelyBlock, string msg)
        {
            if (CLR_ISO14443A.readQ(absolutelyBlock / 4, _key))
            {
                if (CLR_ISO14443A.writeGB2312(absolutelyBlock / 4, absolutelyBlock % 4, msg))
                {
                    if (ReadCardEvent != null) { ReadCardEvent(CardReadStateType.WriteSuccess, CLR_ISO14443A.UID, _Card, ""); }//写块成功
                    return true;
                }
                else
                {
                    if (ReadCardEvent != null) { ReadCardEvent(CardReadStateType.WriteErr, CLR_ISO14443A.UID, _Card, "写块时失败,绝对块为:" + absolutelyBlock.ToString()); }//写块失败
                    return false;
                }
            }
            else
            {
                if (ReadCardEvent != null) { ReadCardEvent(CardReadStateType.WriteErr, CLR_ISO14443A.UID, _Card, "写块时验证失败,扇区为:" + (absolutelyBlock / 4).ToString()); }//验证失败
                return false;
            }
        }
        public void Rest()
        {
            _Uid = string.Empty;
        }
        public bool TestCard()
        {
            return CLR_ISO14443A.getRFID();
        }
    }

}
