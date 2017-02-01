using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DudeMugen;

namespace DudeMugen
{
    public partial class BufferConvertForm : Form
    {
        public BufferConvertForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Converts the command in the textbox to the corresponding buffering system variable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void convertButton_Click(object sender, EventArgs e)
        {
            try
            {
                outputBox.Text = CommandInterpreter.CMDToBufferingSystem(commandBox.Text, commandBox.Text, Convert.ToByte(buttonBufferTimeBox.Value), Convert.ToByte(directionBufferTimeBox.Value), 9, Convert.ToByte(commandVarBox.Value));
            }
            catch (ArgumentException x)
            {
                MessageBox.Show(x.Message, "Error");
            }
        }
    }
}
