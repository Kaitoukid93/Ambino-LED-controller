using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Settings
{
    public class IEffect
    {
        //int _id { get; set; }
        //string _name { get; set; }
        //bool isDisplay { get; set; }        

        private int _id;

        public int Id {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;

        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        //private bool isDisplay;
        //public bool IsDisplay {
        //    get { return isDisplay; }
        //    set { isDisplay = value; }
        //}

    }
}
