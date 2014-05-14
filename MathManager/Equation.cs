using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathManager
{
    public class Equation
    {
        public Equation()
        {
            Numbers = new List<decimal>();
            Operators = new List<string>();
        }
        public List<decimal> Numbers { get; set; }
        public List<string> Operators { get; set; }
        public decimal Answer { get; set; }

    }
}
