﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
   public  class GetAnalizator
   {
        public string patient {  get; set; }
        public List<Services> services { get; set; }
        public int progress { get; set; }
   }
}
