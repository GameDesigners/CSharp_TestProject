using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp_Test
{
    /// <summary>
    /// 实现了Sort比较规则的类
    /// 继承自IComparer<T>
    /// </summary>
    public class Person : IComparable<Person>,IEquatable<Person>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CompareTo(Person other)
        {
            if (other == null)
                return 1;

            int result = string.Compare(this.LastName, other.LastName);
            if(result==0)
            {
                result = string.Compare(this.FirstName, other.FirstName);
            }
            return result;
        }

        public override string ToString() => $"{Id} : {FirstName} {LastName}";

        #region IEquatable相关实现
        public override bool Equals(object obj)
        {
            if (obj == null) return base.Equals(obj);
            return Equals(obj as Person);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public bool Equals(Person other)
        {
            if (other == null)
                return base.Equals(other);

            return Id == other.Id && FirstName == other.FirstName && LastName == other.LastName;
        }
        #endregion
    }

    public enum PersonCompareType { FirstName,LastName}


    /// <summary>
    /// 独立于Person的比较器实现方式
    /// </summary>
    public class PersonComparer : IComparer<Person>
    {
        private PersonCompareType _compareType;

        public PersonComparer(PersonCompareType compareType)
        {
            _compareType = compareType;
        }
        public int Compare(Person x,Person y)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return 1;
            if (x != null && y == null) return -1;
            switch(_compareType)
            {
                case PersonCompareType.FirstName:
                    return string.Compare(x.FirstName, y.FirstName);
                case PersonCompareType.LastName:
                    return string.Compare(x.LastName, y.LastName);
                default:
                    throw new ArgumentException("unexpected compare type");
            }
        }
    }

    /// <summary>
    /// 使用yield生成一个简单集合的代码，可用foreach来访问
    /// </summary>
    public class HelloCollection
    {
        /// <summary>
        /// 注意这里如何书写，这种方法个人认为没有工程意义
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            yield return "hello";
            yield return "world";
        }
    }

    public class YieldCollection
    {
        #region 这里使用yield的原理(编译器自动生成 且 自己经过优化的)
        public IEnumerator<string> GetEnumerator() => new Enumerator(0);

        public class Enumerator : IEnumerator<string>, System.Collections.IEnumerator, IDisposable
        {
            private int _state;
            private string _current;

            public Enumerator(int state)
            {
                _state = state;
            }

            bool System.Collections.IEnumerator.MoveNext()
            {
                switch(_state)
                {
                    case 0:
                        _current = "hello";
                        _state = 1;
                        return true;
                    case 1:
                        _current = "world";
                        _state = 2;
                        return true;
                    case 2:
                        break;
                }
                return false;
            }

            void System.Collections.IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            string System.Collections.Generic.IEnumerator<string>.Current => _current;

            object System.Collections.IEnumerator.Current => _current;

            void IDisposable.Dispose() { }
        }
        #endregion
    }

    /// <summary>
    /// 使用yield进行两个迭代器的相互迭代（巧妙的构思）
    /// </summary>
    public class GameMoves
    {
        private System.Collections.IEnumerator _cross;
        private System.Collections.IEnumerator _circle;

        public GameMoves()
        {
            _cross = Cross();
            _circle = Circle();
        }

        private int _move = 0;
        const int MaxMoves = 9;

        public System.Collections.IEnumerator Cross()
        {
            while(true)
            {
                Console.WriteLine($"Cross,move{_move}");
                if (++_move >= MaxMoves)
                    yield break;  //停止迭代器继续迭代

                yield return _circle;
            }
        }

        public System.Collections.IEnumerator Circle()
        {
            while (true)
            {
                Console.WriteLine($"Circle,move{_move}");
                if (++_move >= MaxMoves)
                    yield break;

                yield return _cross;
            }
        }
    }
}
