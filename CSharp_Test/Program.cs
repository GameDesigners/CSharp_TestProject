using System;
using System.Collections;
using System.Collections.Generic;

namespace CSharp_Test
{
    class Program
    {

        private Program _instance;
        public Program Instance
        {
            get
            {
                return _instance ?? (_instance = new Program());
            }
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string FullName => $"{FirstName} {LastName}";

        int? canNull = null;
        static void Main(string[] args)
        {
            Console.WriteLine("x*y=" + Function(y: 10).ToString());
            DisplayArray(1, 2, 3, 4, 5, 6);

            DayOfWeekend dow = (DayOfWeekend)0x3;
            Console.WriteLine(dow.ToString());

            string s = "Hello World";
            Console.WriteLine("使用拓展方法调用得到字符串"+s+"的长度，为："+s.GetStringLength());

            Console.WriteLine("\n.......泛型类部分的代码....................................................");
            var list = new LinkList<int>();
            list.AddLast(2);
            list.AddLast(4);
            list.AddLast(6);

            foreach(var i in list)
            {
                Console.Write(i+" ");
            }
        }

        public static int Function(int x=1, int y=1) => x * y;

        public static void DisplayArray(params int[] data)
        {
            foreach (int i in data)
                Console.Write(i + " ");
        }
    }
}

public struct Vector2
{
    public double X { get; set; }
    public double Y { get; set; }
}


[Flags]
public enum DayOfWeekend
{
    Monday=0x1,
    Tuesday=0x2,
    Wednesday=0x4,
    Thursday=0x8,
    Friday=0x10,
    Saturday=0x20,
    Sunday=0x40,

    Weekend=Saturday | Sunday,
    Workday=0x1f,
    AllWeek=Weekend | Workday
}

/// <summary>
/// 拓展方法类
/// </summary>
public static class StringContruct
{
    public static int GetStringLength(this string s) => s.Length;
}

#region 利用链表讲解泛型
public class LinkedListNode<T>
{
    public LinkedListNode(T value)
    {
        Value = value;
    }

    public T Value { get; private set; }
    public LinkedListNode<T> Next { get; internal set; }
    public LinkedListNode<T> Prev { get; internal set; }
}

public class LinkList<T> : IEnumerable<T>
{
    public LinkedListNode<T> First { get; private set; }
    public LinkedListNode<T> Last { get; private set; }

    /// <summary>
    /// 将新元素查到链表尾部
    /// </summary>
    /// <param name="_node"></param>
    /// <returns></returns>
    public LinkedListNode<T> AddLast(T _node)
    {
        var newNode = new LinkedListNode<T>(_node);
        if (First == null)  //当前的链表为空链表
        {
            First = Last = newNode;
        }
        else
        {
            //插到last的后面
            Last.Next = newNode;
            newNode.Prev = Last;
            Last = newNode;
        }
        return newNode;
    }

    public IEnumerator<T> GetEnumerator()
    {
        LinkedListNode<T> current = First;
        while (current != null)
        {
            yield return current.Value;    //创建一个枚举器的状态机（目前先不详细讲）
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
#endregion