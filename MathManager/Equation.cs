﻿using System;
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
            QuestionId = Guid.NewGuid().ToString();
        }
        public List<decimal> Numbers { get; set; }
        public List<string> Operators { get; set; }
        public decimal Answer { get; set; }

        public string QuestionId
        {
            get;
            private set;
        }

        public string QuestionCode
        {
            get
            {
                //I'm sending the answer to the client. I know it's bad...
                return string.Join(",", Numbers.ToArray()) + string.Join(",", Operators.ToArray()) + "~" + Answer;
            }
            
        }
        public string EquationString
        {
            get
            {
                var builder = new StringBuilder();
                int operatorIndex = 0;

                foreach(var number in Numbers)
                {
                    builder.Append(number);
                    if( Operators.Count > operatorIndex )
                    {
                        builder.Append(Operators[operatorIndex++]);
                    }
                }
                return builder.ToString();
            }
        }
    }
}
