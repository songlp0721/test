using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HandheldTerminal_Supervision
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region 全局变量
        //全局变量

        private CardReadManager _cardReadManager = new CardReadManager();
        /// <summary>
        /// 是否需要质检
        /// </summary>
        private bool _IsSampling = false;
        /// <summary>
        /// 是否需要质检确认
        /// </summary>
        private bool _IsIntendance = false;
        /// <summary>
        /// 是否需要卸车
        /// </summary>
        private bool _IsUnloadCar = false;
        //需要显示的信息
        ShowMsgData data = null;
        /// <summary>
        /// 全局键盘钩子
        /// </summary>
        HookKeyClass _hk = new HookKeyClass();
        /// <summary>
        /// 菜单界面
        /// </summary>
        MenuForm _mf = null;
        /// <summary>
        /// 黑屏
        /// </summary>
        BlackScreen _BlackScreen = BlackScreen.CreateBlackScreen();
        #endregion

        #region 界面加载事件
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadHardwareState();
            CreateControl();
            PublicStatic.RollReadCardTimer = cardReadTimer;
            LoadConfig();
            CEHardwareControl.ControlShow_Hide(0);
            LoadWcf(PublicStatic.ServerAddress);
            UserLoginForm ulf = UserLoginForm.CreateUserLoginForm();
            ulf.HideEvent += delegate(object s, EventArgs ex) { LoadUserPermission(); };
            ulf.TopMost = true;
            ulf.Show();
            GlobalKeyRegster();//全局键盘事件注册
        }
        private void GlobalKeyRegster()
        {
            PublicStatic.GlobalHookKey = _hk;
            _hk.KeyEvent += delegate(int keyValue)
            {
                switch (keyValue)
                {
                    case 121://右侧红色,读卡
                        _cardReadManager.ReadCard();
                        break;
                    case 113://F2,打开控制
                        if (_mf != null && _mf.Visible)
                        {
                            _mf.Hide();
                        }
                        else
                        {
                            button40_Click(null, null);
                        }

                        break;
                    case 112://帮助
                        break;
                    case 118: //左软
                        switch (tabControl1.TabPages[tabControl1.SelectedIndex].Text)
                        {
                            case "取样":
                                if (button1.Enabled)
                                {
                                    button1_Click(null, null);
                                }
                                break;
                            case "监督":
                                if (button6.Enabled)
                                {
                                    button6_Click(null, null);
                                }
                                break;
                            case "卸货":
                                if (button17.Enabled)
                                {
                                    button17_Click(null, null);
                                }
                                break;
                            case "出厂":
                                if (button20.Enabled)
                                {
                                    button20_Click(null, null);
                                }
                                break;
                        }

                        break;
                    case 117://右软
                        switch (tabControl1.TabPages[tabControl1.SelectedIndex].Text)
                        {
                            case "取样":
                                if (button2.Enabled)
                                {
                                    button2_Click(null, null);
                                }
                                break;
                            case "监督":
                                if (button5.Enabled)
                                {
                                    button5_Click(null, null);
                                }
                                break;
                            case "卸货":
                                if (button16.Enabled)
                                {
                                    button16_Click(null, null);
                                }
                                break;
                            case "出厂":
                                if (button29.Enabled)
                                {
                                    button29_Click(null, null);
                                }
                                break;
                        }
                        break;
                    case 120://左侧红色键
                        //if (_BlackScreen.Visible)
                        //{
                        //    _BlackScreen.Hide();
                        //    CEHardwareControl.setBackLight(8);
                        //}
                        //else
                        //{
                        //    _BlackScreen.Show();
                        //    CEHardwareControl.CloseBackLight();

                        //}
                        _cardReadManager.ReadCard();
                        break;
                }

            };
        }
        private void LoadConfig()
        {
            DataTable configDt = new DataTable("config");
            if (System.IO.File.Exists(PublicStatic.ConfigPath) && System.IO.File.Exists(PublicStatic.ConfigSchemaPath))
            {
                configDt.ReadXmlSchema(PublicStatic.ConfigSchemaPath);
                configDt.ReadXml(PublicStatic.ConfigPath);
                PublicStatic.ServerAddress = configDt.Rows[0]["WcfAddress"].ToString();
                PublicStatic.Site = configDt.Rows[0]["LocalAddress"].ToString().PadRight(3, ' ');
                PublicStatic.ConfigDataTable = configDt;
            }
            else
            {

                configDt.Columns.Add("WcfAddress");
                configDt.Columns.Add("LocalAddress");
                DataRow dr0 = configDt.NewRow();
                dr0["WcfAddress"] = "http://127.0.0.1:9527"; ; dr0["LocalAddress"] = "K1";
                configDt.Rows.Add(dr0);
                configDt.WriteXmlSchema(PublicStatic.ConfigSchemaPath);
                configDt.WriteXml(PublicStatic.ConfigPath);
                MessageBox.Show("无配置文件,请重新启动,系统将自动添加配置文件!");
                Application.Exit();
            }


        }
        private void LoadUserPermission()
        {
            this.tabPage1.Parent = null;
            this.tabPage2.Parent = null;
            this.tabPage3.Parent = null;
            this.tabPage4.Parent = null;
            this.tabPage5.Parent = null;
            this.tabPage6.Parent = null;
            foreach (DataRow dr in PublicStatic.UserList.Select("User='" + PublicStatic.User + "'"))
            {
                if (dr["ShowSampling"].ToString() == "1") { this.tabPage1.Parent = this.tabControl1; }
                if (dr["ShowIntendance"].ToString() == "1") { this.tabPage3.Parent = this.tabControl1; }
                if (dr["ShowUnloadCar"].ToString() == "1") { this.tabPage2.Parent = this.tabControl1; }
                if (dr["ShowAwayConfirm"].ToString() == "1") { this.tabPage4.Parent = this.tabControl1; }
                if (dr["ShowSpotTest"].ToString() == "1") { this.tabPage5.Parent = this.tabControl1; }
                if (dr["ShowQuery"].ToString() == "1") { this.tabPage6.Parent = this.tabControl1; }
                if (dr["Compel"].ToString() == "1")
                {
                    button42.Visible = true;
                    button43.Visible = true;
                    button44.Visible = true;
                    button45.Visible = true;
                }
                else
                {
                    button42.Visible = false;
                    button43.Visible = false;
                    button44.Visible = false;
                    button45.Visible = false;
                }
            }
            //添加登录后对互联网的连接
            if (PublicStatic.IsNetMode)
            {
                try
                {
                    using (System.Diagnostics.Process.Start(PublicStatic.StartUpPath + "\\GPRS.exe", ""))
                    { }
                }
                catch
                {

                }
            }
        }
        #endregion

        #region 硬件事件

        private void LoadHardwareState()
        {
            ;
            hardwStateTimer.Tick += new EventHandler(hardwStateTimer_Tick);
            hardwStateTimer.Enabled = true;
            cardReadTimer.Tick += new EventHandler(cardReadTimer_Tick);
            _cardReadManager.ReadCardEvent += new CardReadManager.ReadCardEventHandle(_cardReadManager_ReadCardEvent);
        }

        void _cardReadManager_ReadCardEvent(CardReadManager.CardReadStateType cardreadtype, string CardID, Card card, string ErrMsg)
        {//读卡事件
            switch (cardreadtype)
            {
                case CardReadManager.CardReadStateType.ReadNot:
                    if (pictureBox2.Image != null)
                    {
                        pictureBox2.Image = null;
                    }
                    else
                    {
                        pictureBox2.Image = cardImageList.Images[0];
                    }
                    PublicStatic.CardContent = null;
                    ClearTextBox();
                    break;
                case CardReadManager.CardReadStateType.Readed:
                    pictureBox2.Image = cardImageList.Images[1];
                    PublicStatic.CardContent = card.CardDataDic;
                    ShowCardMsg(card);
                    SoundManager.PlaySound(0);
                    break;
                case CardReadManager.CardReadStateType.Reading:
                    pictureBox2.Image = cardImageList.Images[1];
                    break;
                case CardReadManager.CardReadStateType.ReadErr:
                    pictureBox2.Image = cardImageList.Images[2];
                    SoundManager.PlaySound(1);
                    break;
                case CardReadManager.CardReadStateType.WriteSuccess:
                    break;
                case CardReadManager.CardReadStateType.WriteErr:
                    pictureBox2.Image = cardImageList.Images[2];
                    SoundManager.PlaySound(1);
                    break;

            }
        }


        void ClearTextBox()
        {
            foreach (TabPage tp in this.tabControl1.TabPages)
            {
                foreach (Control c in tp.Controls)
                {
                    if (c.GetType() == typeof(TextBox))
                    {
                        c.Text = "";
                    }
                }

            }
            comboBox1.Text = String.Empty;
        }


        void cardReadTimer_Tick(object sender, EventArgs e)
        {//读卡轮询
            _cardReadManager.ReadCard();
        }

        private int image_index = 0;
        private DateTime _DateTimeNow = DateTime.Now;
        private TimeSpan _Hangupinterval = new TimeSpan(0, 0, 15);
        void hardwStateTimer_Tick(object sender, EventArgs e)
        {
            if (PublicStatic.IsNetMode)
            {
                if (DateTime.Now - _DateTimeNow > _Hangupinterval)
                {
                    try
                    {
                        using (System.Diagnostics.Process.Start(PublicStatic.StartUpPath + "\\GPRS.exe", ""))
                        { }
                    }
                    catch
                    { }
                }
                _DateTimeNow = DateTime.Now;
            }
            //显示时间
            label1.Text = DateTime.Now.ToString("HH:mm");
            //电源状态
            //return 1:电量高
            //return 2:电量低
            //return 3:正在充电

            if (CEHardwareControl.GetPowerState() == 3)
            {
                image_index++;
                if (image_index > 16)
                {
                    image_index = 3;
                }
                pictureBox1.Image = powerImageList.Images[image_index];
            }
            else
            {
                int p = CEHardwareControl.GetBattery();
                p /= 6;
                pictureBox1.Image = powerImageList.Images[p];
            }


        }

        #endregion

        #region 网页服务

        WebReference.DBInterfaceRealization _wcf = new HandheldTerminal_Supervision.WebReference.DBInterfaceRealization();
        private void LoadWcf(string url)
        {
            _wcf.Url = url;
            PublicStatic.Wcf = _wcf;
        }

        #endregion

        #region 读取到卡对界面的现实方法及其数据绑定
        void ShowCardMsg(Card card)
        {
            data = new ShowMsgData(card);
            if (PublicStatic.IsNetMode)
            {
                data.GetWcfOverEvent += delegate { this.Invoke(new Action(BindingDataBase)); };
                data.GetWcfData();
            }
            try
            {
                #region 流程控制
                char[] flags = data.flag.ToCharArray();
                switch (data.flow.ToCharArray()[0])
                {
                    case '0':
                        _WgtMsgControl.Data1 = textBox7.Text = "外进";//时间
                        break;
                    case '1':
                        _WgtMsgControl.Data1 = textBox7.Text = "外销";//时间
                        break;
                    case '2':
                        _WgtMsgControl.Data1 = textBox7.Text = "内倒";//时间
                        break;
                }

                foreach (char mychar in data.flow.Substring(1).ToCharArray())
                {
                    switch (mychar)
                    {
                        case '0':
                            _WgtMsgControl.Data1 = textBox7.Text += "[重检]";
                            break;
                        case '1':
                            _WgtMsgControl.Data1 = textBox7.Text += "[空检]";
                            break;
                        case '2':
                            _WgtMsgControl.Data1 = textBox7.Text += "[取样]";
                            break;
                        case '3':
                            _WgtMsgControl.Data1 = textBox7.Text += "[监督]";
                            break;
                        case '4':
                            _WgtMsgControl.Data1 = textBox7.Text += "[卸车]";
                            break;
                    }
                }


                _FlowMsgControl.Data1 = textBox39.Text = textBox11.Text = textBox6.Text = "无取样流程";
                _FlowMsgControl.Data2 = textBox37.Text = textBox9.Text = "无监督流程";
                _FlowMsgControl.Data3 = textBox15.Text = "无卸车流程";

                button1.Enabled = false;
                button2.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button16.Enabled = false;
                button17.Enabled = false;
                _IsSampling = false;
                _IsIntendance = false;
                _IsUnloadCar = false;
                char[] mychararr = card.CardDataDic[2].Substring(1).ToCharArray();
                foreach (char c in mychararr)
                {
                    switch (c)
                    {
                        case '2':
                            _IsSampling = true;
                            button1.Enabled = true;
                            button2.Enabled = true;
                            switch (flags[2])
                            {
                                case '0':
                                case '\0':
                                case ' ':
                                    _FlowMsgControl.Data1 = textBox39.Text = textBox11.Text = textBox6.Text = "尚未取样";
                                    break;
                                case '1':
                                    _FlowMsgControl.Data1 = textBox39.Text = textBox11.Text = textBox6.Text = "取样合格";
                                    break;
                                case '2':
                                    _FlowMsgControl.Data1 = textBox39.Text = textBox11.Text = textBox6.Text = "取样不合格";
                                    break;

                            }
                            break;
                        case '3':
                            _IsIntendance = true;
                            button5.Enabled = true;
                            button6.Enabled = true;
                            switch (flags[3])
                            {
                                case '0':
                                case '\0':
                                case ' ':
                                    _FlowMsgControl.Data2 = textBox37.Text = textBox9.Text = "尚未监督取样";
                                    break;
                                case '1':
                                    _FlowMsgControl.Data2 = textBox37.Text = textBox9.Text = "已监督取样";
                                    break;
                                case '2':
                                    _FlowMsgControl.Data2 = textBox37.Text = textBox9.Text = "否决的监督取样";
                                    break;

                            }
                            break;
                        case '4':
                            _IsUnloadCar = true;
                            button16.Enabled = true;
                            button17.Enabled = true;
                            switch (flags[4])
                            {
                                case '0':
                                case '\0':
                                case ' ':
                                    _FlowMsgControl.Data3 = textBox15.Text = "未卸货";
                                    break;
                                case '1':
                                    _FlowMsgControl.Data3 = textBox15.Text = "已卸货";
                                    break;
                                case '2':
                                    _FlowMsgControl.Data3 = textBox15.Text = textBox15.Text = "否决的卸货";
                                    break;
                            }
                            break;
                    }
                }
                //出厂确认许可
                switch (flags[0])
                {
                    case ' ':
                    case '\0':
                    case '0':
                        textBox18.Text = "尚未出厂确认";
                        break;
                    case '1':
                        textBox18.Text = "已出厂确认";
                        break;
                }
                #endregion

                #region 基本数据绑定
                //卡号
                _BaseMsgControl.Data1 = textBox43.Text = textBox36.Text = textBox8.Text = textBox1.Text = data.incardno;
                //车号
                _BaseMsgControl.Data2 = textBox45.Text = textBox38.Text = textBox10.Text = textBox2.Text = data.carrierno;
                //品名
                _BaseMsgControl.Data3 = textBox47.Text = textBox40.Text = textBox12.Text = textBox3.Text = data.mtrlname;
                //供方
                _BaseMsgControl.Data6 = textBox42.Text = textBox14.Text = textBox4.Text = data.fromdesc;
                //需方
                _BaseMsgControl.Data7 = textBox49.Text = textBox17.Text = textBox16.Text = data.todesc;
                //规格外发
                _BaseMsgControl.Data5 = textBox44.Text = data.modelname;
                //计划单号
                _WgtMsgControl.Data7 = data.planlistno;
                //取样人;
                _FlowMsgControl.Data4 = data.samplingman;
                //监督人
                _FlowMsgControl.Data5 = data.intendanceman;
                //卸车人
                _FlowMsgControl.Data6 = data.unloadcarman;
                //取样时间
                _TimeMsgControl.Data3 = data.samplingdatetime;
                //监督时间
                _TimeMsgControl.Data4 = data.intendancedatetime;
                //卸车时间
                _TimeMsgControl.Data5 = data.unloadcardatetime;
                //杂量
                _FlowMsgControl.Data7 = textBox19.Text = data.otherwgt;
                //扣粉
                _FlowMsgControl.Data8 = textBox20.Text = data.stivewgt;
                //重检磅房
                _WgtMsgControl.Data4 = data.grossstation;
                //空检磅房
                _WgtMsgControl.Data5 = data.tartstation;
                //内部车号
                textBox21.Text = data.incarno;

                //部门
                comboBox1.SelectedItem = data.deptno.Trim();
                #endregion

                #region 其他操作
                button22_Click(null, null);//选择基本面
                #endregion
            }
            catch { }
        }

        private void BindingDataBase()
        {


            #region 查数据库绑定数据
            //重检
            _WgtMsgControl.Data2 = textBox46.Text = textBox41.Text = textBox13.Text = textBox5.Text = data.grosswgt;
            //空检
            _WgtMsgControl.Data3 = textBox48.Text = data.tartwgt;
            //入厂时间
            _TimeMsgControl.Data1 = data.entertime;
            //重检时间
            _TimeMsgControl.Data2 = data.grossdatetime;
            //空检时间
            _TimeMsgControl.Data6 = data.tartdatetime;
            //重检磅房
            _WgtMsgControl.Data4 = data.grossstation;
            //空检磅房
            _WgtMsgControl.Data5 = data.tartstation;
            //车辆信息.
            _CarMsgControl.Data1 = data.drivername;
            _CarMsgControl.Data2 = data.carno;
            _CarMsgControl.Data3 = data.carcolor;
            _CarMsgControl.Data4 = data.retinue;
            _CarMsgControl.Data5 = data.affix;
            #endregion
        }



        #endregion

        #region 确认和取消按钮
        private void button1_Click(object sender, EventArgs e)
        {//取样确认
            Sampling(CommandType.Yes);
        }
        private void button2_Click(object sender, EventArgs e)
        {//取样取消
            Sampling(CommandType.no);
        }
        private void button6_Click(object sender, EventArgs e)
        {//监督确认
            Intendance(CommandType.Yes);
        }
        private void button5_Click(object sender, EventArgs e)
        {//监督取消
            Intendance(CommandType.no);
        }
        private void button17_Click(object sender, EventArgs e)
        {//卸车确认
            UnloadCar(CommandType.Yes);
        }
        private void button16_Click(object sender, EventArgs e)
        {//卸车取消
            UnloadCar(CommandType.no);
        }
        private void button20_Click(object sender, EventArgs e)
        {//出厂确认
            AwayConfirm(CommandType.Yes);
        }
        private void button19_Click(object sender, EventArgs e)
        {//出厂取消
            AwayConfirm(CommandType.no);
        }
        private void button41_Click(object sender, EventArgs e)
        {//退库
            DialogResult result = MessageBox.Show("是否对本车进行退货?", "确认提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            switch (result)
            {
                case DialogResult.Yes:
                    RetrnOfGoods(CommandType.Yes);
                    break;
                case DialogResult.No:
                    RetrnOfGoods(CommandType.no);
                    break;
            }
        }
        #endregion

        #region 强制确认
        private void button44_Click(object sender, EventArgs e)
        {//强制取样
            Sampling(CommandType.compel);
        }

        private void button43_Click(object sender, EventArgs e)
        {//强制监督
            Intendance(CommandType.compel);
        }

        private void button42_Click(object sender, EventArgs e)
        {//强制卸货
            UnloadCar(CommandType.compel);
        }

        private void button45_Click(object sender, EventArgs e)
        {//强制出厂
            AwayConfirm(CommandType.compel);
        }
        #endregion

        #region 抽查按钮
        //基本
        private void button22_Click(object sender, EventArgs e)
        {//基本
            panel1.Controls.Clear();
            panel1.Controls.Add(_BaseMsgControl);
        }
        //检斤
        private void button23_Click(object sender, EventArgs e)
        {//检斤
            panel1.Controls.Clear();
            panel1.Controls.Add(_WgtMsgControl);
        }
        //时间
        private void button24_Click(object sender, EventArgs e)
        {//时间
            panel1.Controls.Clear();
            panel1.Controls.Add(_TimeMsgControl);
        }
        //流程
        private void button34_Click(object sender, EventArgs e)
        {//流程
            panel1.Controls.Clear();
            panel1.Controls.Add(_FlowMsgControl);
        }
        //车辆
        private void button39_Click(object sender, EventArgs e)
        {//车辆
            panel1.Controls.Clear();
            panel1.Controls.Add(_CarMsgControl);
        }
        #endregion

        #region 菜单按钮
        private void button40_Click(object sender, EventArgs e)
        {
            _mf = MenuForm.CreateMenuForm();
            _mf.Show();
        }
        #endregion

        #region 生成抽查控件
        BaseMsgControl _BaseMsgControl = new BaseMsgControl();
        BaseMsgControl _WgtMsgControl = new BaseMsgControl();
        BaseMsgControl _TimeMsgControl = new BaseMsgControl();
        BaseMsgControl _FlowMsgControl = new BaseMsgControl();
        BaseMsgControl _CarMsgControl = new BaseMsgControl();
        private void CreateControl()
        {
            _BaseMsgControl.Name1 = "卡片编号:"; _BaseMsgControl.Data1 = "";//√
            _BaseMsgControl.Name2 = "汽车车号:"; _BaseMsgControl.Data2 = "";//√
            _BaseMsgControl.Name3 = "物料品名:"; _BaseMsgControl.Data3 = "";//√
            _BaseMsgControl.Name4 = "物料等级:"; _BaseMsgControl.Data4 = "";
            _BaseMsgControl.Name5 = "物料规格:"; _BaseMsgControl.Data5 = "";//√
            _BaseMsgControl.Name6 = "供方单位:"; _BaseMsgControl.Data6 = "";//√
            _BaseMsgControl.Name7 = "需方单位:"; _BaseMsgControl.Data7 = "";//√

            _WgtMsgControl.Name1 = "检斤类型:"; _WgtMsgControl.Data1 = "";//√
            _WgtMsgControl.Name2 = "重检重量:"; _WgtMsgControl.Data2 = "";//√
            _WgtMsgControl.Name3 = "空检重量:"; _WgtMsgControl.Data3 = "";//√
            _WgtMsgControl.Name4 = "重检磅房:"; _WgtMsgControl.Data4 = "";//√
            _WgtMsgControl.Name5 = "空检磅房:"; _WgtMsgControl.Data5 = "";//√
            _WgtMsgControl.Name6 = "入厂次数:"; _WgtMsgControl.Data6 = "";
            _WgtMsgControl.Name7 = "计划单号:"; _WgtMsgControl.Data7 = "";//√

            _TimeMsgControl.Name1 = "入厂时间:"; _TimeMsgControl.Data1 = "";
            _TimeMsgControl.Name2 = "重检时间:"; _TimeMsgControl.Data2 = "";//√
            _TimeMsgControl.Name3 = "取样时间:"; _TimeMsgControl.Data3 = "";//√
            _TimeMsgControl.Name4 = "监督时间:"; _TimeMsgControl.Data4 = "";//√
            _TimeMsgControl.Name5 = "卸车时间:"; _TimeMsgControl.Data5 = "";//√
            _TimeMsgControl.Name6 = "二检时间:"; _TimeMsgControl.Data6 = "";//√
            _TimeMsgControl.Name7 = "出厂时间:"; _TimeMsgControl.Data7 = "";

            _FlowMsgControl.Name1 = "取样确认:"; _FlowMsgControl.Data1 = "";//√
            _FlowMsgControl.Name2 = "监督确认:"; _FlowMsgControl.Data2 = "";//√
            _FlowMsgControl.Name3 = "卸车确认:"; _FlowMsgControl.Data3 = "";//√
            _FlowMsgControl.Name4 = "取样人员:"; _FlowMsgControl.Data4 = "";//√
            _FlowMsgControl.Name5 = "监督人员:"; _FlowMsgControl.Data5 = "";//√
            _FlowMsgControl.Name6 = "卸车人员:"; _FlowMsgControl.Data6 = "";//√
            _FlowMsgControl.Name7 = "扣杂数量:"; _FlowMsgControl.Data7 = "";//√
            _FlowMsgControl.Name8 = "扣粉数据:"; _FlowMsgControl.Data8 = "";//

            _CarMsgControl.Name1 = "汽车司机:"; _CarMsgControl.Data1 = "";//√
            _CarMsgControl.Name2 = "车辆编号:"; _CarMsgControl.Data2 = "";
            _CarMsgControl.Name3 = "车辆颜色:"; _CarMsgControl.Data3 = "";//√
            _CarMsgControl.Name4 = "随行人员:"; _CarMsgControl.Data4 = "";//√
            _CarMsgControl.Name5 = "随行物品:"; _CarMsgControl.Data5 = "";//√

            //_CarMsgControl.Name6 = "供方单位:"; _CarMsgControl.Data6 = "";
            //_CarMsgControl.Name7 = "需方单位:"; _CarMsgControl.Data7 = "";
        }
        #endregion

        #region 详细按钮

        #region 车号详细
        BaseMsgControl _CarMsgMore = new BaseMsgControl();
        private void button3_Click(object sender, EventArgs e)
        {//车号详细
            if (data == null) { return; }
            _CarMsgMore.Name1 = "汽车车号:"; _CarMsgMore.Data1 = data.carrierno;
            _CarMsgMore.Name2 = "司机姓名:"; _CarMsgMore.Data2 = data.carno;
            _CarMsgMore.Name3 = "司机姓名:"; _CarMsgMore.Data3 = data.drivername;
            _CarMsgMore.Name4 = "车辆标志:"; _CarMsgMore.Data4 = data.carcolor;
            _CarMsgMore.Name5 = "随行人员:"; _CarMsgMore.Data5 = data.retinue;
            _CarMsgMore.Name6 = "随行物品:"; _CarMsgMore.Data6 = data.affix;
            //_CarMsgMore.Name6 = "供方单位:"; _CarMsgMore.Data6 = "";//√
            //_CarMsgMore.Name7 = "需方单位:"; _CarMsgMore.Data7 = "";//√

            MoreMsgForm mmf = MoreMsgForm.CreateMoreMsgForm();
            mmf.SetInsideControl(_CarMsgMore);
            mmf.Show();

        }
        private void button18_Click(object sender, EventArgs e)
        {//车号详细
            button3_Click(null, null);
        }
        private void button21_Click(object sender, EventArgs e)
        {//车号详细
            button3_Click(null, null);
        }
        private void button4_Click(object sender, EventArgs e)
        {//车号详细
            button3_Click(null, null);
        }
        #endregion

        #region 物料详细
        BaseMsgControl _MtrlMsgMore = new BaseMsgControl();
        private void button30_Click(object sender, EventArgs e)
        {//物料详细
            if (data == null) { return; }
            _MtrlMsgMore.Name1 = "物料品名:"; _MtrlMsgMore.Data1 = data.mtrlname;
            _MtrlMsgMore.Name2 = "供方单位:"; _MtrlMsgMore.Data2 = data.fromdesc;
            _MtrlMsgMore.Name3 = "需方单位:"; _MtrlMsgMore.Data3 = data.todesc;
            MoreMsgForm mmf = MoreMsgForm.CreateMoreMsgForm();
            mmf.SetInsideControl(_MtrlMsgMore);
            mmf.Show();
        }
        private void button31_Click(object sender, EventArgs e)
        {//物料详细
            button30_Click(null, null);
        }
        private void button33_Click(object sender, EventArgs e)
        {//物料详细
            button30_Click(null, null);
        }
        private void button32_Click(object sender, EventArgs e)
        {//物料详细
            button30_Click(null, null);
        }
        #endregion

        #region 重检详细
        BaseMsgControl _GrossMsgMore = new BaseMsgControl();
        private void button26_Click(object sender, EventArgs e)
        {//重检详细
            if (data == null) { return; }
            _GrossMsgMore.Name1 = "重检重量:"; _GrossMsgMore.Data1 = data.grosswgt;
            _GrossMsgMore.Name2 = "重检时间:"; _GrossMsgMore.Data2 = data.grossdatetime;
            _GrossMsgMore.Name3 = "重检磅房:"; _GrossMsgMore.Data3 = data.grossstation;
            _GrossMsgMore.Name4 = "重检人员:"; _GrossMsgMore.Data4 = data.loadgorssman;
            MoreMsgForm mmf = MoreMsgForm.CreateMoreMsgForm();
            mmf.SetInsideControl(_GrossMsgMore);
            mmf.Show();
        }
        private void button27_Click(object sender, EventArgs e)
        {//重检详细
            button26_Click(null, null);
        }

        private void button25_Click(object sender, EventArgs e)
        {//重检详细
            button26_Click(null, null);
        }

        private void button29_Click(object sender, EventArgs e)
        {//外发二检(重检)
            button26_Click(null, null);
        }


        #endregion

        #region 空检详细
        BaseMsgControl _TartMsgMore = new BaseMsgControl();
        private void button28_Click(object sender, EventArgs e)
        {//外发一检详细(空检)
            if (data == null) { return; }
            _TartMsgMore.Name1 = "空检重量:"; _TartMsgMore.Data1 = data.tartwgt;
            _TartMsgMore.Name2 = "空检时间:"; _TartMsgMore.Data2 = data.tartdatetime;
            _TartMsgMore.Name3 = "空检磅房:"; _TartMsgMore.Data3 = data.tartstation;
            _TartMsgMore.Name4 = "空检人员:"; _TartMsgMore.Data4 = data.loadtartman;
            MoreMsgForm mmf = MoreMsgForm.CreateMoreMsgForm();
            mmf.SetInsideControl(_TartMsgMore);
            mmf.Show();
        }
        #endregion

        #region 取样详细
        BaseMsgControl _SamplingMsgMore = new BaseMsgControl();
        private void button8_Click(object sender, EventArgs e)
        {//取样详细
            _SamplingMsgMore.Name1 = "取样时间:"; _SamplingMsgMore.Data1 = data.samplingdatetime;
            _SamplingMsgMore.Name2 = "取样地点:"; _SamplingMsgMore.Data2 = data.samplingstation;
            _SamplingMsgMore.Name3 = "取样人员:"; _SamplingMsgMore.Data3 = data.samplingman;
            MoreMsgForm mmf = MoreMsgForm.CreateMoreMsgForm();
            mmf.SetInsideControl(_SamplingMsgMore);
            mmf.Show();
        }
        private void button9_Click(object sender, EventArgs e)
        {//取样详细
            button8_Click(null, null);
        }
        #endregion

        #region 监督详细
        BaseMsgControl _IntedanceMsgMore = new BaseMsgControl();
        private void button7_Click(object sender, EventArgs e)
        {//监督详细
            if (data == null) { return; }
            _IntedanceMsgMore.Name1 = "监督时间:"; _IntedanceMsgMore.Data1 = data.intendancedatetime;
            _IntedanceMsgMore.Name2 = "监督地点:"; _IntedanceMsgMore.Data2 = data.intedancestation;
            _IntedanceMsgMore.Name3 = "监督人员:"; _IntedanceMsgMore.Data3 = data.intendanceman;
            MoreMsgForm mmf = MoreMsgForm.CreateMoreMsgForm();
            mmf.SetInsideControl(_IntedanceMsgMore);
            mmf.Show();
        }
        #endregion

        #endregion

        #region 查询
        private void button46_Click(object sender, EventArgs e)
        {//一检
            setDataTableToDataGrid(0);
        }
        private void button47_Click(object sender, EventArgs e)
        {//取样
            setDataTableToDataGrid(1);

        }
        private void button48_Click(object sender, EventArgs e)
        {//监督
            setDataTableToDataGrid(2);

        }
        private void button49_Click(object sender, EventArgs e)
        {//卸车
            setDataTableToDataGrid(3);

        }
        private void button50_Click(object sender, EventArgs e)
        {//二检
            setDataTableToDataGrid(4);

        }
        private void button51_Click(object sender, EventArgs e)
        {//出厂
            setDataTableToDataGrid(5);
        }

        private void setDataTableToDataGrid(int param)
        {
            int k = 0;
            if (checkBox1.Checked)
            {
                k = 1;
            }
            try
            {
                PublicStatic.Wcf.BeginGetDataTable(PublicStatic.User, "G04", PublicStatic.CreatePamarDataSet(param, k), new AsyncCallback(CallBack), "AsycState:OK");
                PublicStatic.WaitFormControl.Start();
            }
            catch (Exception ex) { MessageBox.Show("网络服务出现问题,请检查网络...\r\n错误信息:" + ex.Message); }
        }
        private void CallBack(IAsyncResult asyncResult)
        {
            DataSet ds = new DataSet();
            try
            {
                this.Invoke(new Action(() => { PublicStatic.WaitFormControl.Stop(); }));
                ds = PublicStatic.Wcf.EndGetDataTable(asyncResult);

            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => { PublicStatic.WaitFormControl.Stop(); MessageBox.Show("网络服务回调出现问题,请检查网络...\r\n错误信息:" + ex.Message); }));
            }

            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                this.Invoke(new Action(() =>
                {

                    label23.Text = ds.Tables[0].Rows.Count.ToString();
                    PublicStatic.WaitFormControl.Stop();
                    //添加动态修改表格样式
                    DataGridTableStyle dts = new DataGridTableStyle();
                    dts.MappingName = "Table";
                    dataGrid1.TableStyles.Clear();
                    dataGrid1.TableStyles.Add(dts);
                    foreach (DataGridTableStyle d in dataGrid1.TableStyles)
                    {
                        d.GridColumnStyles.Clear();
                    }
                    foreach (DataColumn dc in ds.Tables[0].Columns)
                    {
                        DataGridTextBoxColumn dgtc = new DataGridTextBoxColumn();
                        dgtc.Width = dc.ColumnName.Length * 7;
                        dgtc.HeaderText = dc.ColumnName;
                        dgtc.MappingName = dc.ColumnName;
                        dataGrid1.TableStyles[0].GridColumnStyles.Add(dgtc);
                    }
                    dataGrid1.DataSource = ds.Tables[0];

                }));
            }
            else
            {
                this.Invoke(new Action(() => { dataGrid1.DataSource = null; label23.Text = "0"; }));
            }
        }


        #endregion

        #region  表格设置
        private void InitDataGridColumnHeader()
        {
            DataGridTableStyle dts = new DataGridTableStyle();

            //注意：必须加上这一句，否则自定义列格式无法使用
            dts.MappingName = "Table";
            dataGrid1.TableStyles.Add(dts);
            dataGrid1.TableStyles[0].GridColumnStyles.Clear();
            //--------------------------------
            DataGridTableStyle queryTableStyle = new DataGridTableStyle();

            DataGridTextBoxColumn carriernoColumn = new DataGridTextBoxColumn();
            carriernoColumn.Width = 75;
            carriernoColumn.HeaderText = "车号";
            carriernoColumn.MappingName = "车号";
            dataGrid1.TableStyles[0].GridColumnStyles.Add(carriernoColumn);

            DataGridTextBoxColumn mtrlnameColumn = new DataGridTextBoxColumn();
            mtrlnameColumn.Width = 160;
            mtrlnameColumn.HeaderText = "品名";
            mtrlnameColumn.MappingName = "品名";
            dataGrid1.TableStyles[0].GridColumnStyles.Add(mtrlnameColumn);

            DataGridTextBoxColumn fromdescColumn = new DataGridTextBoxColumn();
            fromdescColumn.Width = 200;
            fromdescColumn.HeaderText = "供方";
            fromdescColumn.MappingName = "供方";
            dataGrid1.TableStyles[0].GridColumnStyles.Add(fromdescColumn);

        }
        #endregion

        #region 选择页面事件
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            PublicStatic.WitchPageShow = tabControl1.SelectedIndex;
        }
        #endregion

        #region 查询详细
        private void dataGrid1_DoubleClick(object sender, EventArgs e)
        {
            //........看需求
        }
        #endregion

        private void textBox50_GotFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = true;
        }

        private void textBox50_LostFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }




    }
}


/*********接口协定************
 * G01:获取该车的所有上层数据
 * G02:获取用户登录信息
 * G03:用户修改密码
 * G04:查询数据
 * S01:取样确认
 * S02:监督确认
 * S03:卸车确认
 * S04:退库确认
 * S05:出厂确认
 * test:测试
 * 
 * 
 * **************************/

/**********版本说明*******
 *1.0.0 :实现基本功能,和总体构架设计
 *未完成的工作:
 *1.添加确认对的参数信息..
 *1.0.1: 添加对自动询卡手动询卡的控制
 *
 ***************************/