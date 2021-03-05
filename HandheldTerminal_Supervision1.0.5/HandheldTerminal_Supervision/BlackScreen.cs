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
    public partial class BlackScreen : Form
    {
        private BlackScreen()
        {
            InitializeComponent();
        }
        private static BlackScreen _BlackSecreen;
        public static BlackScreen CreateBlackScreen()
        {
            if (_BlackSecreen == null || _BlackSecreen.IsDisposed)
            {
                _BlackSecreen = new BlackScreen();
            }
            return _BlackSecreen;
        }
    }
}