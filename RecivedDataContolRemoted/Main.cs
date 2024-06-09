using Microsoft.Extensions.Configuration;
using RecivedDataContolRemoted.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecivedDataContolRemoted
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            
        }

        private void remoteTVButton_Click(object sender, EventArgs e)
        {
            try
            {
                RemoteTVForm remoteTVForm = new RemoteTVForm();
                this.Hide();
                remoteTVForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
            
        }

        private void RemoteAirButton_Click(object sender, EventArgs e)
        {

        }
    }
}
