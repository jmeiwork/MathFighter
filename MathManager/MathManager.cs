using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MathManager
{
    public class MathManager
    {
        private Random _random;

        public MathManager()
        {
            _random = new Random();
        }
        public Equation GetMathEquation()
        {
            var returnEquation = new Equation();

            

            var number1 = _random.Next(0, 30);
            var number2 = _random.Next(0, 30);

            returnEquation.Numbers.Add(number1);
            returnEquation.Numbers.Add(number2);

            returnEquation.Operators.Add("+");

            returnEquation.Answer = number1 + number2;

            return returnEquation;
        }

        public bool Verify(string questionCode, string answer)
        {
            bool result = false;
            answer = answer.Trim();

            if( Regex.IsMatch(answer, "^[0-9]+$") )
            {
                var actualAnswer = questionCode.Split(new string[] { "~" }, StringSplitOptions.None)[1];
                result = actualAnswer == answer;
            }

            return result;
        }
    }
}
