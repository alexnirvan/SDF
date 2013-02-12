using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SDreader
{
    public class SDreader
    {
        string _filename;
        StringBuilder _text;

        private List<string> _columns;
        public List<string> Columns
        {
            get { return _columns; }            
        }


        private List<Hashtable> _rows;
        public List<Hashtable> Rows
        {
            get { return _rows; }            
        }
	
	

        public SDreader(string filename)
        {
            _filename = filename;

            _text = new StringBuilder();
            _rows = new List<Hashtable>();
            _columns = new List<string>();

            if (File.Exists(_filename))
            {
                using (StreamReader sr = new StreamReader(_filename))
                {
                    _text.Append(sr.ReadToEnd());
                }

                Parse();
            } 
            
        }

        protected void Parse()
        {
            //Split on separate molecules
            Regex expSplit = new Regex(@"\$\$\$\$\r\n");
            string[] mols = expSplit.Split(_text.ToString());

            int cnt = 0;
            foreach (string mol in mols)
            {
                if (mol.Length > 0)
                {
                    cnt++;
                    Hashtable cells = new Hashtable();

                    //Split on fields/items
                    Regex expItems = new Regex(@"\s>");
                    string[] items = expItems.Split(mol);

                    foreach (string item in items)
                    {
                        //Split for Field Name/ Field Value
                        Regex expDetails = new Regex(@"(\<(/?[^\>]+)\>)");
                        string[] details = expDetails.Split(item.Trim());

                        string columnName = "";
                        string columnValue = "";

                        if (details.Length == 4)
                        {
                            columnName = details[2];
                            columnValue = details[3].Trim();
                        }
                        else
                        {
                            // First column doesn't have a name and it is a structure
                            columnName = "Structure";
                            columnValue = details[0].Trim();
                        }
                        if (!_columns.Contains(columnName))
                            _columns.Add(columnName);


                        cells.Add(columnName, columnValue);
                    }
                    _rows.Add(cells);
                }
            }            
        }
    }
}
