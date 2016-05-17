using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpManagerToOwnMoney.Models
{
    public class LvOperation
    {
        public Int32 Id { get; set; }

        public DateTime? Date { get; set; }

        public String Type { get; set; }

        public String Summa { get; set; }

        public String Account { get; set; }

        public String Category { get; set; }

        public String Comment { get; set; }

        public bool IsSelected { get; set; }
    }
}
