using System;
using System.Collections.Generic;
using System.Text;

/*
 *   此代码文件主要用于测试泛型方法和泛型委托
 */

namespace CSharp_Test
{
    public interface IAccount
    {
        string Name { get; }
        decimal Balance { get;}
    }
    public class Account : IAccount
    {
        public string  Name    { get; }
        public decimal Balance { get; private set; }

        public Account(string name,decimal balance)
        {
            Name = name;
            Balance = balance;
        }
    }

    public static class Alorithms
    {
        /// <summary>
        /// 泛型方法
        /// </summary>
        /// <typeparam name="TAccount"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal AccumulateSimple<TAccount>(IEnumerable<TAccount> source) where TAccount : IAccount
        {
            decimal sum = 0;
            foreach(TAccount a in source)
            {
                sum += a.Balance;
            }
            return sum;
        }

        /// <summary>
        /// 泛型方法
        /// 泛型委托作为函方法参数
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T2 AccumulateSimple<T1,T2>(IEnumerable<T1> source,Func<T1,T2,T2> action)
        {
            T2 sum = default(T2);
            foreach(T1 item in source)
            {
                sum = action(item, sum);
            }
            return sum;
        }
    }
}
