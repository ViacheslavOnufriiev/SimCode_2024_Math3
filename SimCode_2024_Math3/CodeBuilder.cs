using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimCode_2024_Math3
{
    internal class CodeBuilder
    {
        IEnumerable<string> expressions;        

        string[] references = { "System.dll", "mscorlib.dll" };

        string classPattern = @"
                          using System;
                          
                          namespace {0}
                          {{
                            internal class {1}
                            {{
                                  public static void {2}()
                                  {{
                                     double result;
                                     {3}
                                  }}
                            }}
                          }}
                          ";
        public string NameSpace { get; private set; }
        public string ClassName { get; private set; }
        public string MethodName { get; private set; }

        public CodeBuilder AddNameSpace(string nameSpace)
        {
            NameSpace = nameSpace;
            return this;
        }

        public CodeBuilder AddClassName(string className)
        {
            ClassName = className;
            return this;
        }

        public CodeBuilder AddMethodName(string methodName)
        {
            MethodName = methodName;
            return this;
        }

        public CodeBuilder AddExpresionCollection(IEnumerable<string> expressions)
        {
            this.expressions = expressions;
            return this;
        }

        public Type Build()
        {
            var sb = new StringBuilder();
            foreach (var expression in expressions)
            {
                sb.Append("Console.WriteLine(").Append(expression).AppendLine(");");
            }
            var classText = String.Format(classPattern, NameSpace, ClassName, MethodName, sb.ToString());

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters compilerParams = new CompilerParameters()
            {
                GenerateInMemory = true,
                TreatWarningsAsErrors = false,
                GenerateExecutable = false
            };
            compilerParams.ReferencedAssemblies.AddRange(references);

            var compileRsult = provider.CompileAssemblyFromSource(compilerParams, classText);

            if (compileRsult.Errors.HasErrors)
            {
                foreach (CompilerError error in compileRsult.Errors)
                {
                    Console.WriteLine(error.ToString());
                }
                return null;
            }
            var module = compileRsult.CompiledAssembly.GetModules()[0];

            return module.GetType($"{NameSpace}.{ClassName}");

        }
    }
}
