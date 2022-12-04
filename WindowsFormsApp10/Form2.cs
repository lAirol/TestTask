using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp10
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var log = textBox1.Text.ToString();
            var pass = textBox2.Text.ToString();
            string connectionstring = @"Data Source=DESKTOP-A0SU57N;Initial Catalog = Test; Integrated Security = True";
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            string sqlExpression = string.Format("select * from Pass where Login = '{0}' and Password ='{1}'", log, pass);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader sqlreader = command.ExecuteReader();
            if (sqlreader.Read() == false)
                MessageBox.Show("Данные введены неверно");
            else
                this.Close();
            connection.Close();

        }
    }
}

