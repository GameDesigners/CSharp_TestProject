using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp_Test
{
    #region Delegate
    public class Delegates
    {
        public delegate string UpperChar();

        public delegate double DoubleOp(double x);

    }

    public class MathOperations
    {
        public static double MultiplyByTwo(double value) => value * 2;
        public static double Square(double value) => value * value;

        public static void MultiplayByTwoResult(double value)
        {
            Console.WriteLine($"{value}×2={value * 2}");
        }

        public static void SquareResult(double value)
        {
            Console.WriteLine($"{value}*{value}={value * value}");
        }
    }

    public class BubbleSorter
    {
        static public void Sort<T>(IList<T> sortArray, Func<T, T, bool> comparion)
        {
            bool swapped = true;
            do
            {
                swapped = false;
                for (int i = 0; i < sortArray.Count - 1; i++)
                {
                    if (comparion(sortArray[i + 1], sortArray[i]))
                    {
                        T temp = sortArray[i];
                        sortArray[i] = sortArray[i + 1];
                        sortArray[i + 1] = temp;
                        swapped = true;
                    }
                }
            } while (swapped);
        }
    }

    public class Employee
    {
        public Employee(string name,decimal salary)
        {
            Name = name;
            Salary = salary;
        }

        public string Name { get; }
        public decimal Salary { get; private set; }

        public override string ToString() => $"{Name} , {Salary}";

        public static bool CompareSalary(Employee e1, Employee e2) => e1.Salary < e2.Salary;
    }
    #endregion

    #region Event
    public class CarInfoEventArgs : EventArgs
    {
        public CarInfoEventArgs(string car)
        {
            Car = car;
        }

        public string Car { get; }
    }

    public class CarDealer
    {
        private event EventHandler<CarInfoEventArgs> newCarInfo;
        public event EventHandler<CarInfoEventArgs>NewCarInfo
        {
            add
            {
                newCarInfo += value;
            }
            remove
            {
                newCarInfo -= value;
            }
        }
        public void NewCar(string car)
        {
            Console.WriteLine($"CarDealer,new car {car}");
            newCarInfo?.Invoke(this, new CarInfoEventArgs(car));
        }
    }

    public class Consumer
    {
        private string _name;
        public Consumer(string name)
        {
            _name = name;
        }

        public void NewCarIsHere(object sender,CarInfoEventArgs e)
        {
            Console.WriteLine($"{_name} : car ({e.Car}) is new");
        }
    }
    #endregion
}
