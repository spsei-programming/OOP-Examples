using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirStats.Extensions
{
    public class Calculator
    {
        public delegate double Operation(double x, double y);

        public static double Add(double x, double y)
        {
            return x + y;
        }

        public static double Substract(double x, double y)
        {
            return x - y;
        }
    }


    public static class DateTimeExtensionsTest
    {
        delegate DateTime AddDaysFunc(DateTime date, int daysToAdd);

        private static Func<DateTime, int, DateTime> addFunctor = (date, daysToAdd) =>
        {
            return date.AddDays(daysToAdd);
        };

        static DateTimeExtensionsTest()
        {
            //AddDaysFunc f = DateTimeExtensions.AddDay;

            //f(DateTime.Now, 2);

            //DateTime.Now.AddDay(2);

            //DateTimeExtensions.AddDay(DateTime.Now, 2);

            addFunctor(DateTime.Now, 2);

            List<Calculator.Operation> operations = new List<Calculator.Operation>(5);

            operations.Add(Calculator.Add);
            operations.Add(Calculator.Substract);
            operations.Add(Calculator.Add);
            operations.Add(Calculator.Add);

            var x = 3;
            var y = 2.1;
            double res = 0;

            foreach (var op in operations)
            {
                res += op(res, y);
            }
;        }
    }
    public static class DateTimeExtensions
    {
        // Add one day to date time
        
        public static DateTime AddDay(this DateTime date, int daysToAdd)
        {
            return date.AddDays(daysToAdd);
        }
    }
}
