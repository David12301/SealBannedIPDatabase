using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;

namespace SealBannedIPDatabase
{
    public class FileDB
    {
        public static string dbname = ConfigurationManager.AppSettings["dbfilename"];
        private static string[] dbstrcontent;
        public FileDB()
        {
            dbstrcontent = File.ReadAllLines(dbname);
        }

        public string Name
        {
            get { return dbname; }
        }

        public string[] Entries 
        {
            get { return dbstrcontent; }
        }

        public string writeEntry(string count, string ip) 
        {
            string err = string.Empty;
            try
            {
                int number = -1;
                if (Int32.TryParse(count, out number)) 
                {
                    int index = -1;
                    if ((index = ipExistsinDB(ip)) >= 0)
                    {
                        string[] data = dbstrcontent[index].Split(" ");
                        int _n = Int32.Parse(data[0]);
                        string _ip = data[1];
                        _n += number;
                        dbstrcontent[index] = _n + " " + _ip;
                    }
                    else
                    {
                        string new_entry = count + " " + ip;
                        List<string> temp = new List<string>(dbstrcontent);
                        temp.Add(new_entry);
                        dbstrcontent = temp.ToArray();

                    }
                }

                File.WriteAllLines(dbname, dbstrcontent);
                
            }
            catch (Exception ex) 
            {
                err += ex.Message;
            }
            return err;

        }



        public int ipExistsinDB(string ip) 
        {
            
            for(int i=0;i<dbstrcontent.Length;i++)
            {
                string line = dbstrcontent[i];
                string[] data = line.Split(" ");
                string _ip = data[1];
                if (ip == _ip) {
                    return i;
                }
            }
            return -1;
        }
    }
}
