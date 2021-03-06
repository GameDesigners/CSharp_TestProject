﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        //int? canNull = null;

        static void Main(string[] args)
        {


            //BaseKnow_Test();
            //Generic_Test();
            //Array_And_Tuple_Test();

            //Operators_AndForceTypeConvert_Test();

            //Delegate_Lambda_Event();

            //String_Pattern();
            //FileAndStream_Test();

            //Network_Test();

            //XML_JSON_Test();
            Children c = new Children();

            Console.WriteLine(GetChildrenTypeByParentParam(c));

        }

        public static string GetChildrenTypeByParentParam(Parent _parent) => _parent.GetType().ToString();

        public static int Function(int x=1, int y=1) => x * y;

        public static void DisplayArray(params int[] data)
        {
            foreach (int i in data)
                Console.Write(i + " ");
        }

        /// <summary>
        /// 基础测试
        /// </summary>
        private static void BaseKnow_Test()
        {
            Console.WriteLine("\n.......C#基础部分的代码....................................................");
            Console.WriteLine("x*y=" + Function(y: 10).ToString());
            DisplayArray(1, 2, 3, 4, 5, 6);

            DayOfWeekend dow = (DayOfWeekend)0x3;
            Console.WriteLine(dow.ToString());

            string s = "Hello World";
            Console.WriteLine("使用拓展方法调用得到字符串" + s + "的长度，为：" + s.GetStringLength());
        }
        /// <summary>
        /// 测试泛型的方法
        /// </summary>
        private static void Generic_Test()
        {
            Console.WriteLine("\n\n.......泛型类部分的代码....................................................");
            var list = new LinkList<int>();
            list.AddLast(2);
            list.AddLast(4);
            list.AddLast(6);

            foreach (var i in list)
            {
                Console.Write(i + " ");
            }

            //测试泛型
            DocumentManager<Document> docManager = new DocumentManager<Document>();  //声明一个管理类
            docManager.AddDocument(new Document("Titile A", "Sample A"));
            docManager.AddDocument(new Document("Titile B", "Sample B"));

            //显示管理器队列中的元素
            docManager.DisplayAllDocuments();


            //测试泛型的协变
            IIndex<Rectangle> rectangles = RectangleCollection.GetRectangles;
            IIndex<Shape> shapes = rectangles;  //体现了泛型的协变，返回的IIndex<Rectangle>值可以赋值给IIndex<Shape>
            for (int index = 0; index < shapes.Count; index++)
                Console.WriteLine(shapes[index].ToString());

            //测试泛型的抗变
            ShapeDisplay display = new ShapeDisplay();
            Console.WriteLine($"shapes[0] 's type : {shapes[0].GetType().BaseType}");
            //!ERROR :编译器报错，因为shape[0]是 Shape,不能使用Rectangle的泛型 -> display.show(shapes[0]);


            //测试Nullable
            Nullable<int> x = 4;
            Console.WriteLine($"x.Value={x.Value}");

            //测试泛型方法和泛型委托
            var accounts = new List<Account>()
            {
                new Account("A",1500),
                new Account("B",2200),
                new Account("C",1800),
                new Account("D",2400)
            };

            Console.WriteLine($"使用泛型方法计算出的{accounts.Count}个账户的总金额为:{Alorithms.AccumulateSimple(accounts)}");
            Console.WriteLine($"使用泛型方法&泛型计算出的{accounts.Count}个账户的总金额为:{Alorithms.AccumulateSimple<Account, decimal>(accounts, (item, sum) => sum += item.Balance)}");
        }

        /// <summary>
        /// 数组和元组测试函数
        /// </summary>
        private static void Array_And_Tuple_Test()
        {
            //数组和元组
            Console.WriteLine("\n\n.......数组与元组部分的代码....................................................");
            Array intArray1 = Array.CreateInstance(typeof(int), 5);
            for (int index = 0; index < intArray1.Length; index++)
                intArray1.SetValue(index * 10, index);
            for (int index = 0; index < intArray1.Length; index++)
                Console.WriteLine($"intArray1[{index}]={intArray1.GetValue(index)}");

            int[] intArray2 = (int[])intArray1; //强制类型转换，OK

            int[] intArrayCopyVersion = (int[])intArray2.Clone();  //复制数组

            string[] names = { "Christina Aguilera", "Shakira", "Beyonce", "Lady Gaga" };
            Array.Sort(names);  //抽象类中实现的排序算法（利用默认的规则）,继承自IComparable接口
            Console.WriteLine("\n> 排序后的names");
            foreach (var name in names)
                Console.WriteLine(name);

            Person[] persons =
            {
                new Person{ FirstName="Damon",LastName="Hill"},
                new Person{ FirstName="Niki",LastName="Lauda"},
                new Person{ FirstName="Ayrton",LastName="Senna"},
                new Person{ FirstName="Graham",LastName="Hill"}
            };
            Array.Sort(persons);  //排序，person已经重载了比较的规则
            Console.WriteLine("\n> 排序后的persons");
            foreach (var p in persons)
                Console.WriteLine(p.ToString());

            Array.Sort(persons, new PersonComparer(PersonCompareType.FirstName));
            Console.WriteLine("\n> 根据FirstName排序后的persons");
            foreach (var p in persons)
                Console.WriteLine(p.ToString());

            ArraySegment<Person> part = new ArraySegment<Person>(persons, 2, 2);  //从索引2开始，引用两个元素
            Console.WriteLine("\n> 截取Person后所得的ArraySegment数组");
            foreach (var p in part)
                Console.WriteLine(p.ToString());

            Console.WriteLine("\n> 使用yield生成的一个简易的集合");
            var helloCollections = new HelloCollection();
            foreach (var s in helloCollections)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("\n> 测试yeild实现的GameMove");
            var game = new GameMoves();
            IEnumerator enumerator = game.Cross();
            while (enumerator.MoveNext())
                enumerator = enumerator.Current as IEnumerator;
            Console.WriteLine("\n> 测试Person实现的IEquatable接口");
            var janet = new Person { FirstName = "Janet", LastName = "Jackson" };
            Person[] persons1 =
            {
                new Person{FirstName="Michael",LastName="Jackson"},
                janet
            };

            Person[] persons2 =
            {
                new Person{FirstName="Michael",LastName="Jackson"},
                janet
            };

            if (persons1 != persons2) { Console.WriteLine("not the same reference"); }

            var t1 = Tuple.Create(1, "Stephanie");
            var t2 = Tuple.Create(1, "Stephanie");
            if (t1 != t2) Console.WriteLine("not the same reference to the tuple");
            if (t1.Equals(t2)) Console.WriteLine("the same content");
        }

        /// <summary>
        /// 操作符
        /// </summary>
        private static void Operators_AndForceTypeConvert_Test()
        {
            Console.WriteLine("\n> check和uncheck运算符");
            Operators.CheckAndUnCheck();
            Console.WriteLine("\n> is运算符");
            Operators.Is_Operator();
            Console.WriteLine("\n> as运算符");
            Operators.As_Operator();
            Console.WriteLine("\n> sizeof()运算符");
            Operators.SizeOf_Operator();
            Console.WriteLine("\n> typeof()运算符");
            Operators.TypeOf_Operator();
            Console.WriteLine("\n> nameof()运算符");
            Operators.NameOf_Operator();
            //自定义强制类型转换
            Console.WriteLine("\n> 自定义强制类型转换");
            Currency currency = new Currency(50, 20);
            Console.WriteLine($"currency :{currency}");
            float money = currency;
            Console.WriteLine($"转换后获得的金额数值为(强制转换为float)：{money}");
            money += 1;
            Currency reconvertValue = (Currency)money;
            Console.WriteLine($"{money}转化为Currency:{reconvertValue}");

            //Console.ReadLine();
        }

        private static void Delegate_Lambda_Event()
        {
            Console.WriteLine("\n> 委托的基本使用");
            string str = "this is lower char";
            Delegates.UpperChar upperchar = new Delegates.UpperChar(str.ToUpper);
            Delegates.UpperChar upperChar1 = str.ToUpper;
            Console.WriteLine($"{str} 使用委托实例：upperchar 返回的结果为：{upperchar()}");
            Console.WriteLine($"{str} 用upperchar.Invoke()返回的结果一致：{upperchar.Invoke()}");  //是直接调用的编译器版本（即即使你用上面的调用方式，编译器也会帮你改成此行的方式）
            Console.WriteLine($"{str} 用委托推断声明的委托变量返回结果     ：{upperChar1.Invoke()}");


            Console.WriteLine("\n> 使用委托数组");
            Delegates.DoubleOp[] doubleOps =
            {
                MathOperations.MultiplyByTwo,
                MathOperations.Square
            };

            for(int index=0;index<doubleOps.Length;index++)
            {
                Console.WriteLine($"使用方法{index}");
                Console.WriteLine($"Value is 2.0,使用方法的返回结果为：{doubleOps[index](2.0)}");
                Console.WriteLine($"Value is 7.94,使用方法的返回结果为：{doubleOps[index](7.94)}");
                Console.WriteLine($"Value is 1.414,使用方法的返回结果为：{doubleOps[index](1.414)}");
            }


            Console.WriteLine("\n> 使用Func<>做函数的参数");
            Employee[] employees =
            {
                new Employee("Bugs Bunny",20000),
                new Employee("Elmer Fudd",10000),
                new Employee("Daffy Duck",25000),
                new Employee("Wile Coyote",1000000.38m),
                new Employee("Fogtorn Leghorn",23000),
                new Employee("Road Runner",50000),
            };
            BubbleSorter.Sort(employees, Employee.CompareSalary);
            foreach (var em in employees)
                Console.WriteLine(em);


            Console.WriteLine("\n> 多播委托");
            Action<double> operations = MathOperations.MultiplayByTwoResult;
            operations += MathOperations.SquareResult;
            operations.Invoke(10.2);


            Console.WriteLine("\n> 匿名方法");
            string mid = ", middle part";
            //Func<string, string> anonDel = delegate (string param)
            Func<string,string> anonDel = param=>  //Lambda表达式的写法
            {
                param += mid;
                param += " add this was added to the string.";
                return param;
            };
            Console.WriteLine(anonDel("Start of string"));


            Console.WriteLine("\n> 闭包");
            int someVal = 5;
            Func<int, int> f = x => { someVal += x; x = x + someVal;return x; };
            Console.WriteLine($"lambda实现的委托f返回的结果为{f(3)}");
            Console.WriteLine($"现在someVal的值为:{someVal}");

            Console.WriteLine("\n> 事件的使用");
            var dealer = new CarDealer();
            var daniel = new Consumer("Daniel");
            dealer.NewCarInfo += daniel.NewCarIsHere;
            dealer.NewCar("Mercedes");

            var sebastian = new Consumer("Sebastian");
            dealer.NewCarInfo += sebastian.NewCarIsHere;
            dealer.NewCar("Sebastian");
        }

        private static void String_Pattern()
        {
            Console.WriteLine("\n> FormattableString的使用");
            int param1 = 1;
            int param2 = 2;
            int param3 = 3;
            FormattableString s = $"The data {param1},{param2},{param3}";
            Console.WriteLine($"Format:{s.Format}");
            for (int index = 0; index < s.ArgumentCount; index++)
                Console.WriteLine($"argument {index}:{s.GetArgument(index)}");

            Console.WriteLine("\n> 给字符串插值使用[其他区域:中国、美国等]值");
            var day = new DateTime(2025, 02, 12);
            Console.WriteLine($"{day:d}");
            Console.WriteLine(String_Class.Invariant($"{day:d}"));

            Console.WriteLine("\n> 日期和时间的数据格式");
            var now = DateTime.Now;
            Console.WriteLine($"{now:D}");
            Console.WriteLine($"{now:d}");

            Console.WriteLine("\n> 正则表达式");
            StringPattern.Find1(StringPattern.input,@"\bn");
        }
        private static void FileAndStream_Test()
        {
            Console.WriteLine("\n\n.......C#文件和流测试的代码....................................................");

            IOMessageCollection col = new IOMessageCollection();
            FileManager fileManager = new FileManager();
            FolderManager folderManager = new FolderManager();
            StreamManager streamManager = new StreamManager();
            CompressManager compressManager = new CompressManager();

            string rootPath = @"TestResource\";
            string rootModelFile = rootPath + "CODE.txt";                         //根目录下的CODE.txt文件
            string secondFolderPath = rootPath + @"COPY_TEST\";                   //COPY_TEST目录
            string COPY_TEST_ModelName = rootPath + @"COPY_TEST\CODE.txt";        //COPY_TEST目录下的CODE.txt文件
            string COPY_TEST_CopyName = rootPath + @"COPY_TEST\CODE - 副本.txt";   //COPY_TEST目录下的CODE.txt副本文件
            string FileStream_WriteFileName = secondFolderPath + "WriteFile.txt"; //使用FileStream写入的文件路径
            string BigDataFilePath = rootPath + "BigDataFile.txt";                //生成多数据文件路径
            string ArticleFilePath = secondFolderPath + @"诗.txt";
            string BinaryOutPutFile = secondFolderPath + "BinaryOutPutFile.data"; //二进制文件
            string NonCompressFilePath = secondFolderPath+ "诗.txt";
            string CompressedFilePath = secondFolderPath+"测试文件压缩包.pack";
            string DecompressedFilePath = secondFolderPath+"解压缩结果.txt";
            string CompressFolderPath_ZIP = secondFolderPath;
            string CompressedFilePath_ZIP = rootPath +"压缩文件.zip";

            Console.WriteLine("\n> 此计算机的磁盘信息");
            col.ShowDrivesInfo();

            Console.WriteLine("\n> 计算机用户文件夹");
            Console.WriteLine(col.GetDocumentsFolder());

            Console.WriteLine("\n> 注册文件状态监视事件的方法");
            fileManager.WatchFiles(rootPath, "*.*");

            Console.WriteLine($"\n> 获取文件夹 [{rootPath}]信息");
            folderManager.FolderInfomation(rootPath);

            Console.WriteLine($"\n> 创建文件[CODE.txt]");
            fileManager.CreateAFile(rootModelFile);

            Console.WriteLine($"\n> 获取文件[CODE.txt]信息");
            fileManager.FileInformation(rootModelFile);

            Console.WriteLine("\n> 复制文件[CODE.txt]");
            fileManager.CopyAFile(rootModelFile, COPY_TEST_ModelName);
            fileManager.CopyAFile(COPY_TEST_ModelName, COPY_TEST_CopyName);

            Console.WriteLine($"\n> 删除文件夹[{secondFolderPath}]的副本文件");
            folderManager.DeleteDuplicateFile(secondFolderPath, true);

            Console.WriteLine($"\n> 使用FileStream流读取[{rootModelFile}]");
            streamManager.ReadFileUsingFileStream(rootModelFile);

            Console.WriteLine("\n> 使用FileStream流写入txt");
            streamManager.WriteTextFile(FileStream_WriteFileName);

            

            //Console.WriteLine("\n> 生成一个文件");
            //int recordSize = 1000000000;
            //Task task = streamManager.CreateSampleFile(recordSize, BigDataFilePath);

            //Console.WriteLine($"\n> 从大数据文件[ {Path.GetFileName(BigDataFilePath)}]中定点查找：");
            //streamManager.RandomAccessSample(BigDataFilePath);  //有点问题

            

            Console.WriteLine("\n> 使用StreamWriter写入文件");
            streamManager.WriteFileUsingWriter(ArticleFilePath,lines: new string[] {
                "我所见的世界太小，而真实的世界太大。",
                "所以，我希望可以去看到更远的地方，看看那里的风景。",
                "我始终这么觉得，却发现现实中，自己的脚却不愿意挪动半步。",
                "是啊，有太多太多可以束缚着自己的事情，或许这只是借口，或许，更多的是自甘束缚。",
                "只能心中不断地提醒自己，外面地世界，陌生而又可怕。",
                "陌生，却想要去熟悉，但却心悸，真是一个复杂的矛盾体。",
                "\n",
                "发现自己就像一个早熟的老头，",
                "划定了自己的活动范围，",
                "然后，用失神的眼睛，",
                "一日复一日不变地，",
                "窥视着那一方，吸引着每一个年轻人去探索的天空。"
            });

            Console.WriteLine("\n> 使用StreamReader读取文件");
            streamManager.ReadFileUsingReader(ArticleFilePath);

            Console.WriteLine("\n> 使用BinaryWriter写入文件");
            streamManager.WriteFileUsingBinaryWriter(BinaryOutPutFile);

            Console.WriteLine("\n> 使用BinaryReader读取文件");
            streamManager.ReadFileUsingBinaryReader(BinaryOutPutFile);

            Console.WriteLine("\n> 使用DeflateStream压缩文件");
            compressManager.CompressFile(NonCompressFilePath,CompressedFilePath);
            Console.WriteLine("\n> 使用DeflateStream解压缩文件");
            compressManager.DecompressFile(CompressedFilePath,DecompressedFilePath);

            Console.WriteLine("\n> 使用ZipArchive压缩ZIP文件");
            compressManager.CreateZipFile(CompressFolderPath_ZIP,CompressedFilePath_ZIP);
        }

        private static void Network_Test()
        {
            Console.WriteLine("\n\n.......C#网络测试的代码....................................................");
            
        }

        private static void XML_JSON_Test()
        {
            //XML_Test.ReadTextNodes();
            //XML_Test.ReadElementContent();
            XML_Test.ReadElementContent2();
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