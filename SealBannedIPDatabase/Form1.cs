using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AutoUpdaterDotNET;
using System.Diagnostics;
using System.Configuration;

namespace SealBannedIPDatabase
{
    public partial class Form1 : Form
    {
        private static FileDB _db;
        private readonly string urlticket = ConfigurationManager.AppSettings["urlticket"];
        public Form1()
        {
            InitializeComponent();
            tabControl1.Selected += TabControl1_Selected;
            txtFilterCount.KeyPress += TxtFilterCount_KeyPress;
        }

        private void TxtFilterCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void TabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                ListAllData();
            }
            else if (tabControl1.SelectedIndex == 2) 
            {
                ListRecent();
            }
        }

        private void ListAllData() 
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < _db.Entries.Length; i++)
            {
                string line = _db.Entries[i];
                addLineToGrid(line);
            }

        }

        private void ListRecent() 
        {
            dataGridView2.Rows.Clear();
            for (int i = 0; i < _db.Recent.Count; i++)
            {
                string line = _db.Recent[i];
                addLineToGridRecent(line);
            }
        }

        private void addLineToGridRecent(string line)
        {
            string[] data = line.Split(" ");
            string count = data[0];
            string ip = data[1];
            object[] row = { count, ip };
            dataGridView2.Rows.Add(row);
        }

        private void addLineToGrid(string line) 
        {
            string[] data = line.Split(" ");
            string count = data[0];
            string ip = data[1];
            object[] row = { count, ip };
            dataGridView1.Rows.Add(row);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                AutoUpdater.Mandatory = true;
                AutoUpdater.UpdateMode = Mode.Forced;
                AutoUpdater.DownloadPath = Environment.CurrentDirectory;
                var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
                AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600);
                if (currentDirectory.Parent != null)
                {
                    AutoUpdater.InstallationPath = currentDirectory.FullName;
                }


                AutoUpdater.Start("https://raw.githubusercontent.com/David12301/SealBannedIPDatabase/master/config.xml");


                _db = new FileDB();
            }
            catch (Exception ex) 
            {
                if (!File.Exists(FileDB.dbname)) 
                {
                    if ((MessageBox.Show("The database file doesn't exist. Do you wish to create it?", "System", MessageBoxButtons.YesNo) == DialogResult.Yes))
                    {
                        var _file = File.Create(FileDB.dbname);
                        _file.Close();
                        _db = new FileDB();
                    }
                    else 
                    {
                        MessageBox.Show("Exiting...");
                        this.Close();
                    }
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Save list
            StringBuilder sb = new StringBuilder();
            int before = _db.Entries.Length;
            foreach (string line in txtAEntriesSave.Lines) 
            {
                string[] data = line.Split(" ");
                string err = _db.writeEntry(data[0], data[1]);
                if (err != string.Empty) {
                    sb.Append(err);
                }
            }
            int after = _db.Entries.Length - before;
            txtAMessage.Text = "";
            txtAMessage.Text = sb.ToString();
            txtAMessage.Text += "New entries: " + after;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Filter IP
            string _ip = txtFilterIP.Text;
            int i = _db.ipExistsinDB(_ip);
            if (i >= 0) {
                dataGridView1.Rows.Clear();
                string line = _db.Entries[i];
                addLineToGrid(line);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            ListAllData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Over count 
            int val = Int32.Parse(txtFilterCount.Value.ToString());
            dataGridView1.Rows.Clear();
            for (int i = 0; i < _db.Entries.Length; i++)
            {
                string line = _db.Entries[i];
                string[] data = line.Split(" ");
                string count = data[0];
                int c = Int32.Parse(count);
                if (c >= val) 
                {
                    addLineToGrid(line);
                }
                
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sc1 = txtCompare1.Text;
            string[] lst1 = sc1.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries
            );
            string sc2 = txtCompare2.Text;
            string[] lst2 = sc2.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries
            );


            List<string> different = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (string n1 in lst1) 
            {
                bool isInlst2 = false;
                foreach (string n2 in lst2) 
                {
                    if (n2.Contains(n1)) {
                        isInlst2 = true;
                        break;
                    }
                }

                bool exists = false;
                foreach (string d in different)
                {
                    if (d.Equals(n1))
                    {
                        exists = true;
                        break;
                    }
                }

                if (!isInlst2 && !exists)
                {
                    different.Add(n1);
                }

            }

            foreach (string d in different)
            {
                sb.Append(d + Environment.NewLine);
            }
            txtResultCompare.Text = sb.ToString();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string n = txtGenerateURL.Text.Trim();
            string[] nl = n.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries
            );
            foreach (string l in nl) {
                sb.Append(urlticket + l + Environment.NewLine);
            }

            txtGenerateURL.Text = sb.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string n = txtGenerateURL.Text.Trim();
            string[] nl = n.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries
            );
            foreach (string l in nl)
            {
                sb.Append(urlticket + l + " ");
            }


            string cmdCommand = "/C start chrome --new-tab " + sb.ToString();
            System.Diagnostics.Process.Start("CMD.exe", cmdCommand);

        }
    }
}
