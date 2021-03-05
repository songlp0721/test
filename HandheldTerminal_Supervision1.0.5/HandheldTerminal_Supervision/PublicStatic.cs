using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Security.Cryptography;

namespace HandheldTerminal_Supervision
{
    public static class PublicStatic
    {

        static PublicStatic()//静态初始化
        { 
            //初始化标志位字典表
            _Flow2FlagDic.Add("0", 5); //重检--一检标志
            _Flow2FlagDic.Add("1", 15);//空检--二检标志
            _Flow2FlagDic.Add("2", 2); //取样--取样标志
            _Flow2FlagDic.Add("3", 3); //监督--监督标志
            _Flow2FlagDic.Add("4", 4); //卸车--卸车标志

            _FlagExplainDic.Add(0, "外销出厂许可");
            _FlagExplainDic.Add(1, "返回方式");
            _FlagExplainDic.Add(2, "取样标识");
            _FlagExplainDic.Add(3, "监督标识");
            _FlagExplainDic.Add(4, "卸车标识");
            _FlagExplainDic.Add(5, "一检标识");
            _FlagExplainDic.Add(6, "上抛标识");
            _FlagExplainDic.Add(7, "一车多货");
            _FlagExplainDic.Add(8, "一检票据");
            _FlagExplainDic.Add(9, "二检票据");
            _FlagExplainDic.Add(10, "回皮类型");
            _FlagExplainDic.Add(11, "是否有后抛计划");
            _FlagExplainDic.Add(12, "是否与ERP互交");
            _FlagExplainDic.Add(13, "下站");
            _FlagExplainDic.Add(14, "退货标识");
            _FlagExplainDic.Add(15, "二检标识");

        }


        private static Dictionary<int, string> _CardContent;
        public static Dictionary<int, string> CardContent
        {
            set { _CardContent = value; }
            get { return _CardContent; }
        }
        private static WebReference.DBInterfaceRealization _wcf;

        public static WebReference.DBInterfaceRealization Wcf
        {
            get { return PublicStatic._wcf; }
            set { PublicStatic._wcf = value; }
        }

        private static int _WitchPageShow;
        /// <summary>
        /// 当前选择的页面
        /// </summary>
        public static int WitchPageShow
        {
            get { return PublicStatic._WitchPageShow; }
            set { PublicStatic._WitchPageShow = value; }
        }
        private static Dictionary<string, int> _Flow2FlagDic = new Dictionary<string, int>();
        /// <summary>
        /// 流程对标志位字典
        /// </summary>
        public static Dictionary<string, int> Flow2FlagDic
        {
            get { return PublicStatic._Flow2FlagDic; }
            set { PublicStatic._Flow2FlagDic = value; }
        }
        private static Dictionary<int, string> _FlagExplainDic = new Dictionary<int, string>();
        /// <summary>
        /// 标志说明字典
        /// </summary>
        public static Dictionary<int, string> FlagExplainDic
        {
            get { return PublicStatic._FlagExplainDic; }
            set { PublicStatic._FlagExplainDic = value; }
        }
        private static WaitForm _WaitFormControl = WaitForm.CreateWaitForm();

        public static WaitForm WaitFormControl
        {
            get { return PublicStatic._WaitFormControl; }
            set { PublicStatic._WaitFormControl = value; }
        }


        private static HookKeyClass _GlobalHookKey;
        /// <summary>
        /// 全局钩子
        /// </summary>
        public static HookKeyClass GlobalHookKey
        {
            get { return PublicStatic._GlobalHookKey; }
            set { PublicStatic._GlobalHookKey = value; }
        }
        /// <summary>
        /// 产生数据字典
        /// </summary>
        /// <param name="questid">询问号0:入厂,1:一检,2:取样,3:监督,4:卸车,5:二检,6:出厂</param>
        /// <param name="isthiscrew">是否当班:0:全部1:当班</param>
        /// <returns></returns>
        

public static DataSet CreatePamarDataSet(int questid, int isthiscrew)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("quest");
            dt.Columns.Add("questid");
            dt.Columns.Add("isthiscrew");
            DataRow dr = dt.NewRow();
            dr["questid"] = questid.ToString();
            dr["isthiscrew"] = isthiscrew.ToString();
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            return ds;
        }
        /// <summary>
        /// 产生地址参数字典
        /// </summary>
        /// <param name="site">本机地址</param>
        /// <returns>数据字典</returns>
        public static DataSet CreatePamarDataSet(string site)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Site");
            dt.Columns.Add("Key");
            DataRow dr = dt.NewRow();
            dr["Key"] = site.Trim();
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            return ds;
        }
        /// <summary>
        /// 产生参数字典
        /// </summary>
        /// <param name="planlistno">计划单号</param>
        /// <param name="carrierno">车号</param>
        /// <returns>参数字典</returns>
        public static DataSet CreatePamarDataSet(string planlistno, string carrierno)
        { 
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);
            dt.Columns.Add("planlistno");
            dt.Columns.Add("carrierno");
            DataRow dr1 = dt.NewRow();
            dr1["planlistno"] = planlistno;
            dr1["carrierno"] = carrierno;          
            dt.Rows.Add(dr1);
            return ds;

        }
        public static DataSet CreatePamarDataSet(string username, string oldpassword, string newpassword)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);
            dt.Columns.Add("username");
            dt.Columns.Add("oldpassword");
            dt.Columns.Add("newpassword");
            DataRow dr1 = dt.NewRow();
            dr1["username"] = username;
            dr1["oldpassword"] = oldpassword;
            dr1["newpassword"] = newpassword;
            dt.Rows.Add(dr1);
            return ds;
        }
        /// <summary>
        /// 向数据库写确认信息
        /// </summary>
        /// <param name="planlistno">计划单号</param>
        /// <param name="carrierno">车号</param>
        /// <param name="result">结果</param>
        /// <param name="datetime">时间</param>
        /// <param name="place">地点</param>
        /// <param name="op">操作员</param>
        /// <returns></returns>
        public static DataSet CreateCommandDataSet(string planlistno, string carrierno,string cardid,string cardno ,string result, string datetime, string place, string op)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);
            dt.Columns.Add("planlistno");
            dt.Columns.Add("carrierno");
            dt.Columns.Add("cardid");
            dt.Columns.Add("incardno");
            dt.Columns.Add("result");
            dt.Columns.Add("datetime");
            dt.Columns.Add("place");
            dt.Columns.Add("op");
            DataRow dr1 = dt.NewRow();
            dr1["planlistno"] = planlistno;
            dr1["carrierno"] = carrierno;
            dr1["cardid"] = cardid;
            dr1["incardno"] = cardno;
            dr1["result"] = result;
            dr1["datetime"] = datetime;
            dr1["place"] = place;
            dr1["op"] = op;
            dt.Rows.Add(dr1);
            
            return ds;

        }
        /// <summary>
        /// 向数据库写确认信息
        /// </summary>
        /// <param name="planlistno">计划单号</param>
        /// <param name="carrierno">车号</param>
        /// <param name="result">结果</param>
        /// <param name="datetime">时间</param>
        /// <param name="place">地点</param>
        /// <param name="op">操作员</param>
        /// <param name="op">数据</param>
        /// <returns></returns>
        public static DataSet CreateCommandDataSet(string planlistno, string carrierno, string cardid, string cardno, string result, string datetime, string place, string op,string data,string data2)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);
            dt.Columns.Add("planlistno");
            dt.Columns.Add("carrierno");
            dt.Columns.Add("cardid");
            dt.Columns.Add("incardno");
            dt.Columns.Add("result");
            dt.Columns.Add("datetime");
            dt.Columns.Add("place");
            dt.Columns.Add("op");
            dt.Columns.Add("data");
            dt.Columns.Add("data2");
            DataRow dr1 = dt.NewRow();
            dr1["planlistno"] = planlistno;
            dr1["carrierno"] = carrierno;
            dr1["cardid"] = cardid;
            dr1["incardno"] = cardno;
            dr1["result"] = result;
            dr1["datetime"] = datetime;
            dr1["place"] = place;
            dr1["op"] = op;
            dr1["data"] = data;
            dr1["data2"] = data2;
            dt.Rows.Add(dr1);

            return ds;

        }
        private static string _User;
        /// <summary>
        /// 用户
        /// </summary>
        public static string User
        {
            get { return PublicStatic._User; }
            set { PublicStatic._User = value; }
        }
        private static string _Site;
        /// <summary>
        /// 本机位置
        /// </summary>
        public static string Site
        {
            get { return PublicStatic._Site; }
            set { PublicStatic._Site = value; }
        }
        private static string _ServerAddress;
        /// <summary>
        /// 服务地址
        /// </summary>
        public static string ServerAddress
        {
            get { return PublicStatic._ServerAddress; }
            set { PublicStatic._ServerAddress = value; }
        }

        private static string _StartUpPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

        public static string StartUpPath
        {
            get { return PublicStatic._StartUpPath; }
            set { PublicStatic._StartUpPath = value; }
        }

        private static string _ConfigPath = _StartUpPath + @"\Set.XML";
        /// <summary>
        /// 配置字典地址
        /// </summary>
        public static string ConfigPath
        {
            get { return PublicStatic._ConfigPath; }
           // set { PublicStatic._ConfigPath = value; }
        }
        private static string _ConfigSchemaPath = _StartUpPath + @"\SetSchema.XML";
        /// <summary>
        /// 配置字典框架地址
        /// </summary>
        public static string ConfigSchemaPath
        {
            get { return PublicStatic._ConfigSchemaPath; }
           // set { PublicStatic._ConfigSchemaPath = value; }
        }
        private static DataTable _ConfigDataTable;
        /// <summary>
        /// 配置字典
        /// </summary>
        public static DataTable ConfigDataTable
        {
            get { return PublicStatic._ConfigDataTable; }
            set { PublicStatic._ConfigDataTable = value; }
        }
        
        private static bool _IsNetMode = true;
        /// <summary>
        /// 网络模式
        /// </summary>
        public static bool IsNetMode
        {
            get { return PublicStatic._IsNetMode; }
            set { PublicStatic._IsNetMode = value; }
        }
        private static bool _IsRollReadCard = false;
        /// <summary>
        /// 是否轮询读卡
        /// </summary>
        public static bool IsRollReadCard
        {
            get { return PublicStatic._IsRollReadCard; }
            set { PublicStatic._IsRollReadCard = value;

            _RollReadCardTimer.Enabled = value;
            }
        }
        private static System.Windows.Forms.Timer _RollReadCardTimer;

        public static System.Windows.Forms.Timer RollReadCardTimer
        {
           // get { return PublicStatic._RollReadCardTimer; }
            set { PublicStatic._RollReadCardTimer = value; }
        }


        private static DataTable _UserList;

        public static DataTable UserList
        {
            get { return PublicStatic._UserList; }
            set { PublicStatic._UserList = value; }
        }
        public static void SaveLocalUserData(DataTable dt)
        {
            dt.WriteXmlSchema(StartUpPath + @"\UserDataSchema.XML");
            dt.WriteXml(StartUpPath + @"\UserData.XML");
        }
        public static DataTable GetLocalUserList()
        {
            DataTable userdataDataTable = new DataTable();
            userdataDataTable.ReadXmlSchema(StartUpPath + @"\UserDataSchema.XML");
            userdataDataTable.ReadXml(StartUpPath + @"\UserData.XML");
            _UserList = userdataDataTable;
            return userdataDataTable;
        }
        public static void CreateDefultUserData()
        {
            DataTable dt = new DataTable("userList");
            dt.Columns.Add("UserId");
            dt.Columns.Add("User");
            dt.Columns.Add("Password");
            dt.Columns.Add("ShowSampling");
            dt.Columns.Add("ShowIntendance");
            dt.Columns.Add("ShowUnloadCar");
            dt.Columns.Add("ShowAwayConfirm");
            dt.Columns.Add("ShowSpotTest");
            dt.Columns.Add("ShowQuery");
            dt.Columns.Add("Compel");
            DataRow dr = dt.NewRow();
            dr["UserId"] = "1";
            dr["User"] = "system";
            dr["Password"] = "61ACB9AA";//明文为980508
            dr["ShowSampling"] = "1";
            dr["ShowIntendance"] = "1";
            dr["ShowUnloadCar"] = "1";
            dr["ShowAwayConfirm"] = "1";
            dr["ShowSpotTest"] = "1";
            dr["ShowQuery"] = "1";
            dr["Compel"] = "1";
            dt.Rows.Add(dr);
            //UserList.PrimaryKey = new DataColumn[] {UserList.Columns["User"] };
            dt.WriteXmlSchema(StartUpPath + @"\UserDataSchema.XML");
            dt.WriteXml(StartUpPath + @"\UserData.XML");

        }
        //MD5加密函数
        public static string Encrypt(string pwd)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(pwd);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string t_str = BitConverter.ToString(hashedBytes);
            return t_str.Replace("-", "").Substring(0, 8);
        }
        public static string EncryptMD5(string str)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(str);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string t_str = BitConverter.ToString(hashedBytes);
            return t_str.Replace("-", "").Substring(0, 8);
        }


    }
}
