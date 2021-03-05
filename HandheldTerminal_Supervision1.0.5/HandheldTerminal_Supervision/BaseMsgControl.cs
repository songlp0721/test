using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace HandheldTerminal_Supervision
{
    public partial class BaseMsgControl : UserControl
    {
        public BaseMsgControl()
        {
            InitializeComponent();
        }
        public string Name1
        {
            set 
            {
                label58.Text = value;
                label58.Visible = true;
            }
        }
        public string Name2
        {
            set
            {
                label57.Text = value;
                label57.Visible = true;
            }
        }
        public string Name3
        {
            set
            {
                label56.Text = value;
                label56.Visible = true;
            }
        }
        public string Name4
        {
            set
            {
                label55.Text = value;
                label55.Visible = true;
            }
        }
        public string Name5
        {
            set
            {
                label54.Text = value;
                label54.Visible = true;
            }
        }
        public string Name6
        {
            set
            {
                label53.Text = value;
                label53.Visible = true;
            }
        }
        public string Name7
        {
            set
            {
                label52.Text = value;
                label52.Visible = true;
            }
        }

        public string Name8
        {
            set
            {
                label1.Text = value;
                label1.Visible = true;
            }
        }

        public string Data1
        {
            set
            {
                textBox50.Text = value;
                textBox50.Visible = true;
            }
        }
        public string Data2
        {
            set
            {
                textBox52.Text = value;
                textBox52.Visible = true;
            }
        }
        public string Data3
        {
            set
            {
                textBox54.Text = value;
                textBox54.Visible = true;
            }

        }
        public string Data4
        {
            set
            {
                textBox56.Text = value;
                textBox56.Visible = true;
            }
        }
        public string Data5
        {
            set
            {
                textBox55.Text = value;
                textBox55.Visible = true;
            }
        }
        public string Data6
        {
            set
            {
                textBox53.Text = value;
                textBox53.Visible = true;
            }
        }
        public string Data7
        {
            set
            {
                textBox51.Text = value;
                textBox51.Visible = true;
            }
        }
        public string Data8
        {
            set
            {
                textBox1.Text = value;
                textBox1.Visible = true;
            }
        }

    }

}
