using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HandheldTerminal_Supervision
{
    public partial class UserLoginForm : Form
    {
        private UserLoginForm()
        {
            InitializeComponent();
        }
        private static UserLoginForm _userLoginForm;
        public static UserLoginForm CreateUserLoginForm()
        {
            if (_userLoginForm == null || _userLoginForm.IsDisposed)
            {
                _userLoginForm = new UserLoginForm();
            }
            
            return _userLoginForm;

        }

        /// <summary>
        /// 该窗体隐藏事件
        /// </summary>
        public event EventHandler HideEvent;


       

        private void UserLoginForm_Load(object sender, EventArgs e)
        {//界面加载
            //加载用户列表
            if (!System.IO.File.Exists(PublicStatic.StartUpPath + @"\UserData.XML"))
            {
                PublicStatic.CreateDefultUserData();
            }

             comboBox1.DataSource = PublicStatic.GetLocalUserList();
             comboBox1.DisplayMember = "User";
             comboBox1.ValueMember = "Password";

             comboBox1.Focus();
             try
             {
                 comboBox1.SelectedIndex = 0;
             }
             catch { }
             
        }
        
        private void button1_Click(object sender, EventArgs e)
        {//登录
            if (comboBox1.SelectedValue.ToString() == PublicStatic.Encrypt(textBox1.Text))
            {//登录成功 
                PublicStatic.User = comboBox1.Text;
                if (HideEvent != null) { HideEvent(null, null); }
                this.Hide();
            }
            else
            {//登录失败 

                MessageBox.Show("用户登录失败!\r\n如果您的账户有异动请刷新列表!", "用户登录", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            { //断网模式
                PublicStatic.IsNetMode = false;
            }
            else
            {//联网模式 
                PublicStatic.IsNetMode = true;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {//刷新用户列表
            try
            {
                PublicStatic.Wcf.BeginGetDataTable("system", "G02", PublicStatic.CreatePamarDataSet(PublicStatic.Site), new AsyncCallback(CallBack), "AsycState:OK");
                PublicStatic.WaitFormControl.Start();
            }
            catch (Exception ex) {
                PublicStatic.WaitFormControl.Stop();
                MessageBox.Show("刷新用户列表失败,错误消息:"+ex.Message); }
            
        }
        private void CallBack(IAsyncResult asyncResult)
        {
            try
            {
                PublicStatic.SaveLocalUserData(PublicStatic.Wcf.EndGetDataTable(asyncResult).Tables[0]);
                this.Invoke(new Action(() => { UserLoginForm_Load(null, null); PublicStatic.WaitFormControl.Stop(); }));
                MessageBox.Show("刷新用户列表成功!","用户登录", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("刷新用户列表回调失败!\r\n1.请检查网络\r\n2.是否数据服务器未开启\r\n调试消息:" + ex.Message);
                PublicStatic.WaitFormControl.Stop();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {//退出程序
            Application.Exit();
        }
        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = true;
        }
        private void textBox1_LostFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(null, null);
            }
        }

        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        {//自动询卡模式
            if (checkBox2.Checked)
            {
                PublicStatic.IsRollReadCard = true;
            }
            else
            {
                PublicStatic.IsRollReadCard = false;
            }
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {//按键模式
            switch (e.KeyValue)
            { 
                //case 38://上翻
                //    try
                //    {
                //        comboBox1.SelectedIndex--;
                //    }
                //    catch (Exception) { }
                //    break;
                //case 40://下翻
                //    try
                //    {
                //    comboBox1.SelectedIndex++;
                //    }
                //    catch (Exception) { }
                //    break;
                case 13://回车
                    textBox1.Focus();

                    break;
            
            }

        }

    }
}