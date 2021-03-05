using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using Microsoft.WindowsCE.Forms;

namespace HandheldTerminal_Supervision
{
    public partial class MenuForm : Form
    {
        private MenuForm()
        {
            InitializeComponent();
        }
        private static MenuForm _MenuForm;
        public static MenuForm CreateMenuForm()
        {
            if (_MenuForm == null || _MenuForm.IsDisposed)
            {
                _MenuForm = new MenuForm();
            }
            _MenuForm.Height = 227;
            PublicStatic.WitchPageShow = 9;
            return _MenuForm;
        }

        private void MenuForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = PublicStatic.ServerAddress;
            textBox2.Text = PublicStatic.Site;
            textBox3.Text = PublicStatic.User;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserLoginForm.CreateUserLoginForm().Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {//任务栏开
            CEHardwareControl.ControlShow_Hide(1);
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {//任务栏关
            CEHardwareControl.ControlShow_Hide(0);
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {//退出

                PublicStatic.Wcf.Abort();
                PublicStatic.GlobalHookKey.Stop();
                CEHardwareControl.ControlShow_Hide(1);
                Application.Exit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startdatetime = DateTime.Now;
                string startmm = DateTime.Now.Millisecond.ToString();
                DataSet ds = PublicStatic.Wcf.GetDataTable(PublicStatic.User, "test", PublicStatic.CreatePamarDataSet("planlistno", "carrierno"));
                DateTime rundatetime = DateTime.Now;
                string runmm = DateTime.Now.Millisecond.ToString();
                MessageBox.Show(string.Format("返回成功:\r\n返回消息:{0}\r\n服务器信息:{1}\r\n开始执行时间:{2}\r\n结束执行时间:{3}\r\n服务器时间 :{4}",
                    ds.Tables[0].Rows[0][0].ToString(),
                    ds.Tables[0].Rows[0][1].ToString(),
                    startdatetime.ToString("yyyy-MM-dd HH:mm:ss"+" "+startmm),
                    rundatetime.ToString("yyyy-MM-dd HH:mm:ss" + " " +runmm),
                    ds.Tables[0].Rows[0][2].ToString()
                    ));
                    

            }
            catch (Exception ex)
            {
                MessageBox.Show("WCF服务连接失败,错误代码:"+ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            PublicStatic.ConfigDataTable.Rows[0]["WcfAddress"] = textBox1.Text.Trim();
            PublicStatic.Wcf.Url = textBox1.Text.Trim();
            WriteConfig();
            this.Hide();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            PublicStatic.Site = textBox2.Text.PadRight(3,' ').Substring(0, 3);
            PublicStatic.ConfigDataTable.Rows[0]["LocalAddress"] = textBox2.Text.Trim();
            WriteConfig();
            this.Hide();
        }

        private void WriteConfig()
        {
            PublicStatic.ConfigDataTable.WriteXml(PublicStatic.ConfigPath);
            MessageBox.Show("保存完毕","提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
        }

        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = true;
        }

        private void textBox2_GotFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = true;
        }


        private void button12_Click(object sender, EventArgs e)
        {//重设密码:
            if (PublicStatic.UserList.Select("User='" + PublicStatic.User + "'")[0]["Password"].ToString() == PublicStatic.Encrypt(textBox4.Text))
            {

                string userid = PublicStatic.UserList.Select("User='" + PublicStatic.User + "'")[0]["UserId"].ToString();
                try
                {
                    if (PublicStatic.Wcf.GetDataTable(PublicStatic.User, "G03", PublicStatic.CreatePamarDataSet(userid, textBox4.Text, textBox5.Text)) != null)
                    {
                        PublicStatic.UserList.Select("User='" + PublicStatic.User + "'")[0]["Password"] =PublicStatic.Encrypt(textBox5.Text);
                        PublicStatic.SaveLocalUserData(PublicStatic.UserList);
                        textBox4.Text = "";
                        textBox5.Text = "";
                        MessageBox.Show("修改密码成功!");
                        this.Hide();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("修改密码失败!\r\n无网网络不能修改密码.\r\n错误信息:"+ex.Message);
                }
            }
            else
            {
                MessageBox.Show("旧密码不正确,请重新输入!");
            }

        }

        private void textBox1_LostFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }

        private void textBox2_LostFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button11_Click(object sender, EventArgs e)
        {//自动询卡
            DialogResult result = MessageBox.Show("自动询卡开关", "设置", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            switch (result)
            { 
                case DialogResult.Yes:
                    PublicStatic.IsRollReadCard = true;
                    break;
                case DialogResult.No:
                    PublicStatic.IsRollReadCard = false;
                    break;
                case DialogResult.Cancel:
                    break;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try {
                DateTime mydt = DateTime.Parse(PublicStatic.Wcf.GetDataTable(PublicStatic.User, "test", PublicStatic.CreatePamarDataSet("planlistno", "carrierno")).Tables[0].Rows[0][2].ToString());
                if (CEHardwareControl.setSystemTime(mydt))
                {
                    MessageBox.Show("校时成功!");
                }
            
            }
            catch {
                MessageBox.Show("校时失败!");
            }
        }
        public void button4_Click(object sender, EventArgs e)
        {//grps开
            System.Diagnostics.Process.Start(PublicStatic.StartUpPath + "\\GPRS.exe", "");           

        }

        private void button8_Click(object sender, EventArgs e)
        {//进入
            if (textBox6.Text == "980508")
            {
                this.Height = 320;
                textBox6.Text = "";
            }
        }

        private void textBox4_GotFocus(object sender, EventArgs e)
        {//所有文本框得到焦点
            inputPanel1.Enabled = true;
        }

        private void textBox4_LostFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }

    }
}