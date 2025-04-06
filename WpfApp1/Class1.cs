using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
   public  class Class1
    {
        public int IDYslugi { get; set; }
        public string Service { get; set; }
        public double Price { get; set; }
        public int SrokVapol { get; set; }
        public double Ot { get; set; }
        public double Do { get; set; }
        public bool qs { get; set; }

        public Class1 (int IDYslugi, string Service, double Price, int SrokVapol, double Ot, double Do, bool qs)
        {
            this.IDYslugi = IDYslugi;
            this.Service = Service;
            this.Price = Price;
            this.SrokVapol = SrokVapol;
            this.Ot = Ot;
            this.Do = Do;
            this.qs = qs;
        }
    }
}
