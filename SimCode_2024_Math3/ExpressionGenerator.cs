using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimCode_2024_Math3
{
    internal class ExpressionGenerator
    {
        private static List<string> operators = new List<string> { "+", "-", "*", "/" };
        private static int significantDigits = 10;

        public IEnumerable<string> GenerateExpressions()
        {
            var numbers = new List<string> { "1", "2", "3" };
            return GenerateAllExpressions(numbers, 0, "").Take(300);
        }

        private static List<string> GenerateAllExpressions(List<string> numbers, int index, string current, bool parenthesesUsed = false)
        {
            var expressions = new List<string>();

            if (index == numbers.Count)
            {
                if (!string.IsNullOrEmpty(current))
                {
                    expressions.Add(current); // Add the current expression if it's not empty
                }
                return expressions; // Return the current list of expressions
            }

            // Generate expressions by continuing with the next number
            expressions.AddRange(GenerateAllExpressions(numbers, index + 1, current + numbers[index]));

            // Attempt to add a decimal point and the next number if allowed
            if (index < numbers.Count)
            {
                string lastNumber = GetLastNumber(current);
                bool canAddDecimal = (!lastNumber.Contains('.'));
                if (canAddDecimal)
                {
                    expressions.AddRange(GenerateAllExpressions(numbers, index + 1, current + "." + numbers[index]));
                    expressions.AddRange(GenerateAllExpressions(numbers, index + 1, current + "-." + numbers[index]));
                }
                expressions.AddRange(GenerateAllExpressions(numbers, index + 1, current + "-" + numbers[index]));
            }

            // Insert operators between numbers, ensuring valid decimal handling
            if (index > 0)
            { // Ensure there's a number to attach operators to
                foreach (var op in operators)
                {
                    expressions.AddRange(GenerateAllExpressions(numbers, index + 1, current + op + numbers[index]));
                    expressions.AddRange(GenerateAllExpressions(numbers, index + 1, current + op + "." + numbers[index]));
                    int indexOfMinus = current.IndexOf("-");
                    bool canAddMinus = (op != "-") && (op != "+") && (indexOfMinus != 0) && ((indexOfMinus == -1) || !operators.Contains(current[indexOfMinus].ToString()));
                    if (canAddMinus)
                    {
                        expressions.AddRange(GenerateAllExpressions(numbers, index + 1, current + op + "-." + numbers[index]));
                        expressions.AddRange(GenerateAllExpressions(numbers, index + 1, current + op + "-" + numbers[index]));
                    }

                    if (!parenthesesUsed && string.Join("", operators).Intersect(current).Any())
                    {
                        expressions.AddRange(GenerateAllExpressions(numbers, index + 1, "(" + current + ")" + op + numbers[index], true));
                        expressions.AddRange(GenerateAllExpressions(numbers, index + 1, "(" + current + ")" + op + "." + numbers[index], true));
                        if (canAddMinus)
                        {
                            expressions.AddRange(GenerateAllExpressions(numbers, index + 1, "(" + current + ")" + op + "-." + numbers[index], true));
                            expressions.AddRange(GenerateAllExpressions(numbers, index + 1, "(" + current + ")" + op + "-" + numbers[index], true));
                            expressions.AddRange(GenerateAllExpressions(numbers, index + 1, "(-" + current + ")" + op + "-." + numbers[index], true));
                            expressions.AddRange(GenerateAllExpressions(numbers, index + 1, "(-" + current + ")" + op + "-" + numbers[index], true));

                        }
                        var lastNumber = GetLastNumber(current);

                        var currentWithoutLastNumber = current.Substring(0, current.Length - lastNumber.Length);
                        expressions.AddRange(GenerateAllExpressions(numbers, index + 1, currentWithoutLastNumber + "(" + lastNumber + op + numbers[index] + ")", true));
                        expressions.AddRange(GenerateAllExpressions(numbers, index + 1, currentWithoutLastNumber + "(" + lastNumber + op + "." + numbers[index] + ")", true));
                        if (canAddMinus)
                        {
                            expressions.AddRange(GenerateAllExpressions(numbers, index + 1, currentWithoutLastNumber + "(" + lastNumber + op + "-" + numbers[index] + ")", true));
                            expressions.AddRange(GenerateAllExpressions(numbers, index + 1, currentWithoutLastNumber + "(" + lastNumber + op + "-." + numbers[index] + ")", true));

                        }
                        char lastOp = currentWithoutLastNumber.Last();
                        if ((lastOp != '+') && (lastOp != '-'))
                        {
                            expressions.AddRange(GenerateAllExpressions(numbers, index + 1, currentWithoutLastNumber + "-(" + lastNumber + op + numbers[index] + ")", true));
                            expressions.AddRange(GenerateAllExpressions(numbers, index + 1, currentWithoutLastNumber + "-(" + lastNumber + op + "." + numbers[index] + ")", true));
                            if (canAddMinus)
                            {
                                expressions.AddRange(GenerateAllExpressions(numbers, index + 1, currentWithoutLastNumber + "-(" + lastNumber + op + "-" + numbers[index] + ")", true));
                                expressions.AddRange(GenerateAllExpressions(numbers, index + 1, currentWithoutLastNumber + "-(" + lastNumber + op + "-." + numbers[index] + ")", true));
                            }
                        }
                    }
                }
            }
            return expressions.Distinct().ToList(); // Ensure uniqueness
        }

        private static string GetLastNumber(string expression)
        {
            int i = expression.Length - 1;
            while (i >= 0 && (char.IsDigit(expression[i]) || expression[i] == '.'))
            {
                i--; // Move backwards through the expression to find the start of the last number
            }
            return expression.Substring(i + 1); // Return the last number segment of the expression
        }
    }
}
