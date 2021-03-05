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
    public partial class MoreMsgForm : Form
    {
        private Button button1;
        private Panel panel1;
    
        private MoreMsgForm()
        {
            InitializeComponent();
        }
        private static MoreMsgForm _moreMsgForm = new MoreMsgForm();
        public static MoreMsgForm CreateMoreMsgForm()
        {
            if (_moreMsgForm == null || _moreMsgForm.IsDisposed)
            {
                _moreMsgForm = new MoreMsgForm();
            }
            return _moreMsgForm;
        }

        public void SetInsideControl(Control c)
        {
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(c);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(74, 285);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 20);
            this.button1.TabIndex = 0;
            this.button1.Text = "确定";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(232, 276);
            // 
            // MoreMsgForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(238, 320);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MoreMsgForm";
            this.ResumeLayout(false);

        }


    }
}