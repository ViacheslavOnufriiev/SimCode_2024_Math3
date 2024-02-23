using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimCode_2024_Math3
{
    internal class Program
    {
        static void Main(string[] args)        {
            
            var expressions = new ExpressionGenerator().GenerateExpressions();

            var builder = new CodeBuilder();
            var type = builder
                .AddNameSpace("SimCode_2024_Math3")
                .AddClassName("Calculator")
                .AddMethodName("ShowResults")
                .AddExpresionCollection(expressions)
                .Build();
            if (type is null) { return; }

            type.GetMethod(builder.MethodName).Invoke(null, new object[] { });

            Console.ReadKey();
        }
    }
}
