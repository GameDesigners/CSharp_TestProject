using System;
using System.Collections.Generic;
using System.Text;

/*
 *   此代码文件主要实现了一个简易版的泛型结构Nullable<T>
 */

namespace CSharp_Test
{
    //约束为值类型，因为引用类型可以为空，使用Nullable类无意义
    public struct Nullable<T> where T : struct
    {
        public Nullable(T value)
        {
            _hasValue = true;  //把有值定义为默认true
            _value = value;
        }

        private bool _hasValue;
        private T    _value;
        public bool HasValue => _hasValue;

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("no value");
                return _value;
            }
        }

        public static explicit operator T(Nullable<T> value) =>value.Value;
        public static implicit operator Nullable<T>(T value) =>new Nullable<T>(value);
        public override string ToString() => !HasValue ? string.Empty : _value.ToString();
    }
}
