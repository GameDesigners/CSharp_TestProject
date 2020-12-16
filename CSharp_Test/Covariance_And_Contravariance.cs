using System;
using System.Collections.Generic;
using System.Text;
/*
 *   此代码文件主要测试
 *   Covariance      协变
 *   Contravariance  抗变
 *   
 *   C#4以后支持了泛型接口和泛型委托的协变与抗变
 */
namespace CSharp_Test
{
    /// <summary>
    /// 泛型类型用out来标记，此泛型接口就是协变的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IIndex<out T>
    {
        T this[int index] { get; }
        int Count { get; }
    }

    public interface IDisplay<in T>
    {
        void show(T item);
    }

    public class Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public override string ToString() => $"Width:{Width},Height:{Height}";

        
    }

    public class ShapeDisplay : IDisplay<Rectangle>
    {
        public void show(Rectangle item)
        {
            Console.WriteLine($"Width:{item.Width},Height:{item.Height}");
        }
    }
    public class Rectangle : Shape
    {

    }



    public class RectangleCollection : IIndex<Rectangle>
    {
        private Rectangle[] data = new Rectangle[3]  //定义三个定长数组
        {
            new Rectangle{Width=2,Height=5},
            new Rectangle{Width=3,Height=7},
            new Rectangle{Width=4.5,Height=2.9}
        };

        private static RectangleCollection _coll;
        public static RectangleCollection GetRectangles => _coll ?? (_coll = new RectangleCollection());

        public Rectangle this[int index]
        {
            get
            {
                if (index < 0 || index >= data.Length)
                    throw new ArgumentException("index");
                return data[index];
            }
        }

        public int Count => data.Length;

    }
}
