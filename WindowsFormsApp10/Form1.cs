using ExcelDataReader;
using ExcelDataReader.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp10
{

    public partial class Form1 : Form
    {
        public SqlCommand command;
        public SqlConnection connection;
        static string connectionstring = @"Data Source=DESKTOP-A0SU57N;Initial Catalog = Test; Integrated Security = True";
        public DataTable dt;
        public Form1()
        {
            InitializeComponent();
            Show();
        }
        DataTableCollection tableCollection;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            { 
                using (OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Excel Workbook|*xlsx|Excel Workbook|*xls" })
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtFilename.Text = openFileDialog.FileName;
                        using (var stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                });
                                tableCollection = result.Tables;
                                DataTable dt = tableCollection[0];
                                dataGridView1.DataSource = dt;
                            }
                        }
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            connection = new SqlConnection(connectionstring);
            command = new SqlCommand("DELETE FROM DataTable", connection);
            connection.Open();
            try
            {  
                command.ExecuteNonQuery();
                foreach (DataTable table in tableCollection)
                {
                    foreach (DataRow dataColumn in table.Rows)
                    {
                        string sqlExpression = string.Format("INSERT INTO DataTable (A,B,C,D,E,F,G,H) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", dataColumn.ItemArray[0], dataColumn.ItemArray[1], dataColumn.ItemArray[2], dataColumn.ItemArray[3], dataColumn.ItemArray[4], dataColumn.ItemArray[5], dataColumn.ItemArray[6], dataColumn.ItemArray[7]);
                        command.CommandText = sqlExpression;
                        command.ExecuteNonQuery();
                    }
                }
                dt = new DataTable();
                string script = "Select * from DataTable ";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(script, connection);
                dataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                MessageBox.Show("Успешно импортированно"); 
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            connection.Close();
        }

        private void Show()
        {
            Form2 form = new Form2();
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            dt = new DataTable();
            connection = new SqlConnection(connectionstring);
            connection.Open();
            try
            {
                switch (comboBox1.SelectedIndex)
                {

                    case 0:
                        {
                            string script = "Select A,F from DataTable where A like'%01'";
                            SqlDataAdapter dataAdapter = new SqlDataAdapter(script, connection);
                            dataAdapter.Fill(dt);

                            dataGridView1.DataSource = dt;
                        }
                        break;
                    case 1:
                        {
                            List<string> matrixnames = new List<string>();
                            List<double> matrix = new List<double>();
                            List<decimal> st = new List<decimal>();
                            string sqlExpression = string.Format("Select A,F from DataTable");
                            SqlCommand command = new SqlCommand(sqlExpression, connection);
                            SqlDataReader sqlreader = command.ExecuteReader();
                            while (sqlreader.Read())
                            {
                                st.Add(sqlreader.GetDecimal(1));
                                matrixnames.Add(sqlreader.GetString(0));
                            }
                            foreach (var aret in st)
                            {
                                foreach (var nret in st)
                                {
                                    matrix.Add(Math.Abs(decimal.ToDouble((aret - nret) * (aret - nret))));
                                }
                            }
                            int k = 0;
                            matrixnames.Insert(0, " ");
                            foreach (string columns in matrixnames)
                            {
                                dt.Columns.Add(columns);
                            }
                            DataRow workRow;
                            matrixnames.Remove(" ");
                            foreach (string rows in matrixnames)
                            {
                                workRow = dt.NewRow();
                                workRow[0] = rows;
                                for (int j = 1; j < st.Count + 1; j++)
                                {
                                    workRow[j] = matrix[k];
                                    k++;
                                }
                                dt.Rows.Add(workRow);
                            }
                            dataGridView1.DataSource = dt;
                        }
                        break;
                    case 2:
                        {
                            List<string> matrixnames = new List<string>();
                            List<double> matrix = new List<double>();
                            List<decimal> st = new List<decimal>();
                            string sqlExpression = string.Format("Select A,F from DataTable");
                            SqlCommand command = new SqlCommand(sqlExpression, connection);
                            SqlDataReader sqlreader = command.ExecuteReader();
                            while (sqlreader.Read())
                            {
                                st.Add(sqlreader.GetDecimal(1));
                                matrixnames.Add(sqlreader.GetString(0));
                            }
                            foreach (var aret in st)
                            {
                                foreach (var nret in st)
                                {
                                    matrix.Add(Math.Abs(decimal.ToDouble((aret - nret) * (aret - nret))));
                                }
                            } 
                            sqlreader.Close();
                            int i = 0;
                            foreach (string start in matrixnames)
                                foreach (string end in matrixnames)
                                {
                                    string sqlExpression1 = string.Format("INSERT INTO MatRez (Cells,RezValue) VALUES ('{0}','{1}')", start + "_" + end, matrix[i] );
                                    command.CommandText = sqlExpression1;
                                    command.ExecuteNonQuery();
                                    i++;
                                  
                                }
                            string script = "Select * from MatRez ";
                            SqlDataAdapter dataAdapter = new SqlDataAdapter(script, connection);
                            dataAdapter.Fill(dt);
                            dataGridView1.DataSource = dt;
                            MessageBox.Show("Успешно импортированно");
                        }
                        break;
                }



            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            connection.Close();
        }
    }
}
