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

namespace SealBannedIPDatabase
{
    public partial class Form1 : Form
    {
        private static FileDB _db;
        public Form1()
        {
            InitializeComponent();
            tabControl1.Selected += TabControl1_Selected;
        }

        private void TabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1) 
            {
                dataGridView1.Rows.Clear();
                for (int i = 0; i < _db.Entries.Length; i++) 
                {
                    string line = _db.Entries[i];
                    string[] data = line.Split(" ");
                    string count = data[0];
                    string ip = data[1];
                    object[] row = { count, ip };
                    dataGridView1.Rows.Add(row);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
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
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

            foreach (string line in txtAEntriesSave.Lines) 
            {
                string[] data = line.Split(" ");
                _db.writeEntry(data[0], data[1]);
            }
        }
    }
}
