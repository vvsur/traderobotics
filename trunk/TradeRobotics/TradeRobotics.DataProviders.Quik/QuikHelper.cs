using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using WealthLab;

namespace TradeRobotics.DataProviders.Quik
{
    /// <summary>
    /// Dataset configuration
    /// </summary>
    [Serializable]
    class QuikSettings
    {

        public string Symbols {get;set;}

        public string SerializeToString()
        {
            SoapFormatter soap = new SoapFormatter();
            string soapString = String.Empty;

            using (MemoryStream mStream = new MemoryStream())
            {
                soap.Serialize(mStream, this);

                using (StreamReader reader = new StreamReader(mStream))
                {
                    mStream.Seek(0, SeekOrigin.Begin);
                    soapString = reader.ReadToEnd();
                }
            }

            soapString = soapString.Replace('<', '{').Replace('>', '}');

            return soapString;
        }

        public static object DeserializeFromString(string soapString)
        {
            soapString = soapString.Replace('{', '<').Replace('}', '>');

            SoapFormatter soap = new SoapFormatter();
            object obj = null;

            using (MemoryStream mStream = new MemoryStream())
            {

                using (StreamWriter writer = new StreamWriter(mStream))
                {
                    writer.Write(soapString);
                    writer.Flush();

                    mStream.Seek(0, SeekOrigin.Begin);
                    obj = soap.Deserialize(mStream);
                }
            }

            return obj;
        }
    }

    /// <summary>
    /// SymbolList helper class
    /// </summary>
    public class SymbolList
    {
        public bool Sort = true;
        public char Delimeter = ',';
        public List<string> Items = new List<string>();

        public string this[int index]
        {
            get { return Items[index]; }
        }

        public string Text
        {
            get
            {
                return ToString();
            }
            set
            {
                Items.Clear();
                AddText(value);
            }
        }

        /*public int Count
        {
            get { return Items.Count; }
        }*/

        public SymbolList()
        {
        }

        public SymbolList(string text)
        {
            this.Text = text;
        }

        public SymbolList(List<string> symbols)
        {
            Items = symbols;
        }

        public void AddText(string str)
        {
            if (str != null)
            {
                string[] t = str.Split(new char[] { ' ', ',', '\n', '\r' });

                foreach (string s in t)
                {
                    Add(s.Trim(new char[] { ' ', '\n', '\r' }));
                }
            }
        }

        /*public void Clear()
        {
            Items.Clear();
        }*/

        private void Add(string symbol)
        {
            if ((!Items.Contains(symbol)) && (symbol != ""))
                Items.Add(symbol);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Sort)
            {
                Items.Sort();
            }

            foreach (string s in Items)
            {
                sb.Append(s);
                sb.Append(Delimeter);
            }


            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }
    }
}