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
    public partial class WaitForm : Form
    {
        private WaitForm()
        {
            InitializeComponent();
        }

        public static WaitForm _waitForm = null;

        public static WaitForm CreateWaitForm()
        {
            if (_waitForm == null || _waitForm.IsDisposed)
            {
                _waitForm = new WaitForm();
            }
            return _waitForm;
        }

        private void WaitForm_Load(object sender, EventArgs e)
        {
            timer1.Tick += new EventHandler(timer1_Tick);
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value += 5;
            if (progressBar1.Value == 100) { progressBar1.Value = 0; }
        }
        public void Start()
        {
            this.Top = 100;
            this.Left = 10;
            this.TopMost = true;
            this.Show();
            timer1.Enabled = true;
        }
        public void Stop()
        {
            this.TopMost = false;
            this.Hide();
            timer1.Enabled = false;
        }

    }
}