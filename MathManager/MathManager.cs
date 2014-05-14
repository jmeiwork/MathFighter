using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathManager
{
    public class MathManager
    {
        public Equation GetMathEquation()
        {
            var returnEquation = new Equation();

            var random = new Random();

            var number1 = random.Next(0, 30);
            var number2 = random.Next(0, 30);

            returnEquation.Numbers.Add(number1);
            returnEquation.Numbers.Add(number2);

            returnEquation.Operators.Add("+");

            returnEquation.Answer = number1 + number2;

            return returnEquation;
        }
    }
}
