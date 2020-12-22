using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp_Test
{
    public class Operators
    {
        /// <summary>
        /// Check和Uncheck运算符
        /// </summary>
        public static void CheckAndUnCheck()
        {
            byte b = 255;
            //unchecked
            {
                b++;
            }
            Console.WriteLine(b);
        }

        public static void Is_Operator()
        {
#pragma warning disable CS0183
#pragma warning disable CS0184
            int i = 10;
            if (i is object)
                Console.WriteLine($"i 与object是兼容的");
            if (i is int)
                Console.WriteLine("i 与int是兼容的");
            if (!(i is double))
                Console.WriteLine("i 与double不兼容");
        }

        public static void As_Operator()
        {
            object o1 = "hello world";
            object o2 = 5;

            //as强制类型转换
            string str1 = o1 as string;
            string result1 = str1 == null ? ("为空") : ("不为空");
            Console.WriteLine(o1.ToString() +"转换成string"+ result1);

            string str2 = o2 as string;
            string result2 = str2 == null ? ("为空") : ("不为空");
            Console.WriteLine(o2.ToString() + "转换成string" + result2);
        }

        public static void SizeOf_Operator()
        {
            Console.WriteLine($"int在栈中需要的长度为   {sizeof(int)}");
            Console.WriteLine($"long在栈中需要的长度为  {sizeof(long)}");
            Console.WriteLine($"float在栈中需要的长度为 {sizeof(float)}");
            Console.WriteLine($"double在栈中需要的长度为{sizeof(double)}");
        }

        public static void TypeOf_Operator()
        {
            Console.WriteLine($"int的System.T为   {typeof(int)}");
            Console.WriteLine($"long在栈中需要的长度为  {typeof(long)}");
            Console.WriteLine($"float在栈中需要的长度为 {typeof(float)}");
            Console.WriteLine($"double在栈中需要的长度为{typeof(double)}");
        }

        public static void NameOf_Operator()
        {
            int num = 14;
            Console.WriteLine($"num的变量名称是：{nameof(num)}");
        }
    }


    public struct Currency
    {
        public uint Dollars { get; }
        public ushort Cents { get; }

        public Currency(uint _dollar,ushort _cents)
        {
            Dollars = _dollar;
            Cents = _cents;
        }

        /// <summary>
        /// 由Currency强制转换为float
        /// </summary>
        /// <param name="currency"></param>
        public static implicit operator float(Currency currency) =>currency.Dollars+(currency.Cents/100.0f);

        public static implicit operator Currency(float value)
        {
            uint dollars = Convert.ToUInt32(value);
            ushort cents = Convert.ToUInt16((value - dollars) * 100);
            return new Currency(dollars, cents);
        }
        public override string ToString() => $"${Dollars}.{Cents,-2:00}";
    }
}
