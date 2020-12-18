using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharp_Test
{
    /// <summary>
    /// IO 信息类
    /// </summary>
    public class IOMessageCollection
    {
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        public void ShowDrivesInfo()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach(DriveInfo drive in drives)
            {
                if(drive.IsReady)
                {
                    Console.WriteLine("********************************************************");
                    Console.WriteLine($"Drive Name:                {drive.Name}");
                    Console.WriteLine($"Drive DriveFormat:         {drive.DriveFormat}");
                    Console.WriteLine($"Drive DriveType:           {drive.DriveType}");
                    Console.WriteLine($"Drive RootDirectory:       {drive.RootDirectory}");
                    Console.WriteLine($"Drive VolumeLabel:         {drive.VolumeLabel}");
                    Console.WriteLine($"Drive TotalFreeSpace:      {drive.TotalFreeSpace}");
                    Console.WriteLine($"Drive AvailableFreeSpace:  {drive.AvailableFreeSpace}");
                    Console.WriteLine($"Drive TotalSize:           {drive.TotalSize}");
                }
                Console.WriteLine();
            }    
        }

        /// <summary>
        /// 用户文件夹
        /// </summary>
        /// <returns></returns>
        public string GetDocumentsFolder()
        {
#if NET46
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#else
            string drive = Environment.GetEnvironmentVariable("HOMEDRIVE");
            //Console.WriteLine(drive);
            string path = Environment.GetEnvironmentVariable("HOMEPATH");
            //Console.WriteLine(path);
            return drive + "\\\\" + Path.Combine(path, "document");
#endif
        }
    }

    /// <summary>
    /// 文件信息和操作 默认存储与E盘根目录
    /// </summary>
    public class FileManager
    {
        private IOMessageCollection col = new IOMessageCollection();

        /// <summary>
        /// 获取某个文件的信息
        /// </summary>
        /// <param name="filename"></param>
        public void FileInformation(string filename)
        {
            var file = new FileInfo(filename);
            Console.WriteLine($"Name:{file.Name}");
            Console.WriteLine($"DirectoryName:   {file.DirectoryName}");
            Console.WriteLine($"IsReadOnly:      {file.IsReadOnly}");
            Console.WriteLine($"Extension:       {file.Extension}");
            Console.WriteLine($"Length:          {file.Length}");
            Console.WriteLine($"CreationTime:    {file.CreationTime:F}");
            Console.WriteLine($"LastAccessTime:  {file.LastAccessTime:F}");
            Console.WriteLine($"Attributes:      {file.Attributes}");
        }

        /// <summary>
        /// 创建一个位于E盘根目录的文件
        /// </summary>
        /// <param name="filename"></param>
        public void CreateAFile(string filename)
        {
            string fileFullPath = Path.Combine("E:", filename);
            File.WriteAllText(fileFullPath, "hello world");
            Console.WriteLine("文件创建成功,日期："+DateTime.Now.ToString());
        }

        /// <summary>
        /// 复制文件!
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="targetPath"></param>
        public void CopyAFile(string srcPath,string targetPath)
        {
            if(File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            var file = new FileInfo(srcPath);
            file.CopyTo(targetPath);

            //OR
            //File.Copy(srcPath, targetPath);
            Console.WriteLine("复制成功,日期：" + DateTime.Now.ToString()+$"    文件：{Path.GetFileName(targetPath)}");
        }

        /// <summary>
        /// 注册文件状态监视事件的方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        public void WatchFiles(string folderName= "E:\\\\COPY_TEST", string filter="*.txt")
        {
            var watcher = new FileSystemWatcher(folderName, filter) { IncludeSubdirectories = true };

            watcher.Created += OnFileChanged;
            watcher.Changed += OnFileChanged;
            watcher.Deleted += OnFileChanged;
            watcher.Renamed += OnFileChanged;

            watcher.EnableRaisingEvents = true;
            Console.WriteLine("watching file change");

        }

        /// <summary>
        /// 打印当前文件改变的事件
        /// </summary>
        private void OnFileChanged(object sender,FileSystemEventArgs e)
        {
            Console.WriteLine($"[---------------> WATCHING -->  file {e.Name} {e.ChangeType}");
        }
    }

    /// <summary>
    /// 文件夹信息和操作
    /// </summary>
    public class FolderManager
    {
        /// <summary>
        /// 删除文件夹中的文件副本
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="checkonly"></param>
        public void DeleteDuplicateFile(string directory, bool checkonly)
        {
            IEnumerable<string> fileNames = Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories);
            string previousFileName = string.Empty;

            //Console.WriteLine("文件列表：");
            //foreach (string filename in fileNames)
            //    Console.WriteLine(filename);
            //Console.WriteLine();

            foreach (string fileName in fileNames)
            {
                string previousName = Path.GetFileNameWithoutExtension(previousFileName);
                if(!string.IsNullOrEmpty(previousName))
                {
                    if(previousName.EndsWith("副本") &&fileName.StartsWith(previousFileName.Substring(0, previousFileName.LastIndexOf(" - 副本"))))
                    {
                        var copiedFile = new FileInfo(previousFileName);
                        var originalFile = new FileInfo(fileName);
                        if (copiedFile.Length == originalFile.Length)
                        {
                            Console.WriteLine($"delete {copiedFile.FullName}");
                            if (!checkonly)
                            {
                                copiedFile.Delete();
                            }
                        }
                    }
                }
                previousFileName = fileName;
            }
        }
    }

    /// <summary>
    /// 流操作
    /// </summary>
    public class StreamManager
    {
        const string SampleFilePath = "E:\\samplefile.txt";
        const string SampleOutputFilePath = "E:\\\\COPY_TEST\\outputFile.txt";
        const string BinaryOutPutFilePath = "E:\\\\COPY_TEST\\BinaryOutPutFile.data";

        #region Use FileStream
        public void ReadFileUsingFileStream(string filename)
        {
            const int bufferSize = 4096;
            const int BUFFERSIZE = 256;
            File.OpenRead(filename);  //返回一个FileStream
            using(var stream=new FileStream(filename,FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                ShowStreamInformation(stream);  //显示stream的信息
                Encoding encoding = GetEncoding(stream);

                byte[] buffer = new byte[bufferSize];

                bool completed = false;

                do
                {
                    int nread = stream.Read(buffer, 0, BUFFERSIZE);
                    if (nread == 0) completed = true;
                    if (nread < BUFFERSIZE)
                    {
                        Array.Clear(buffer, nread, BUFFERSIZE - nread);
                    }

                    string s = encoding.GetString(buffer, 0, nread);
                    Console.WriteLine($"read {nread} bytes");
                    Console.WriteLine($"read result:  {s}");
                }
                while (!completed);
            }
        }

        /// <summary>
        /// 显示当前传入stream的状态
        /// </summary>
        /// <param name="stream"></param>
        private void ShowStreamInformation(Stream stream)
        {
            Console.WriteLine($"···>stream can read:{stream.CanRead}"+ $", can write:{stream.CanWrite}"+ $", can seek:{stream.CanSeek}"+ $", can timeout:{stream.CanTimeout}");
            Console.WriteLine($"···>length:{stream.Length},position:{stream.Position}");
            
            if(stream.CanTimeout)
            {
                Console.WriteLine($"···>read timeout:{stream.ReadTimeout} write timeout:{stream.WriteTimeout}");
            }

        }

        private Encoding GetEncoding(Stream stream)
        {
            if (!stream.CanSeek) throw new ArgumentException("require a stream that can seek");

            Encoding encoding = Encoding.ASCII;

            byte[] bom = new byte[5];
            int nRead = stream.Read(bom, offset: 0, count: 5);  //读取五个字节的数据
            if(bom[0]==0xff&&bom[1]==0xfe&&bom[2]==0&&bom[3]==0)
            {
                Console.WriteLine("UTF-32");
                stream.Seek(4, SeekOrigin.Begin);
                return Encoding.UTF32;
            }
            else if(bom[0] == 0xff && bom[1] == 0xfe)
            {
                Console.WriteLine("UTF-16");
                stream.Seek(2, SeekOrigin.Begin);
                return Encoding.UTF32;
            }
            else if(bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
            {
                Console.WriteLine("UTF-8");
                stream.Seek(3, SeekOrigin.Begin);
                return Encoding.UTF32;
            }

            stream.Seek(0, SeekOrigin.Begin);
            return encoding;
        }

        public void WriteTextFile()
        {
            string tempTextFileName = Path.ChangeExtension(Path.GetTempFileName(), "txt");
            using(FileStream stream=File.OpenWrite(tempTextFileName))
            {
                //给流发送三个字节的UTF-8序言
                stream.WriteByte(0xef);
                stream.WriteByte(0xbb);
                stream.WriteByte(0xbf);

                string content = "hello world!";
                byte[] buffer = Encoding.UTF8.GetBytes(content);
                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine($"file{stream.Name} written. filename -> {tempTextFileName}");
            }
        }
    
        /// <summary>
        /// 复制流的实现
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public void CopyUseStreams(string inputFile,string outputFile)
        {
            const int BUFFERSIZE = 4096;
            using (var inputStream=File.OpenRead(inputFile))
            using(var outputStream =File.OpenWrite(outputFile))
            {
                byte[] buffer = new byte[BUFFERSIZE];
                bool completed = false;
                do
                {
                    int nRead = inputStream.Read(buffer, 0, BUFFERSIZE);
                    if (nRead == 0) completed = true;
                    outputStream.Write(buffer, 0, BUFFERSIZE);
                }
                while (!completed);
            }
        }

        /// <summary>
        /// 使用流的CopyTo()方法复制流
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public void CopyUseStreamsAPI(string inputFile, string outputFile)
        {
            using (var inputStream = File.OpenRead(inputFile))
            using (var outputStream = File.OpenWrite(outputFile))
            {
                inputStream.CopyTo(outputStream);
            }
        }

        
        public async Task CreateSampleFile(int nRecords)
        {
            FileStream stream = File.Create(SampleFilePath);
            using(var writer=new StreamWriter(stream))
            {
                var r = new Random();

                var records = Enumerable.Range(0, nRecords).Select(x => new
                {
                    Number = x,
                    Text = $"Sample text {r.Next(200)}",
                    Data = new DateTime(Math.Abs((long)(r.NextDouble() * 2 - 1) * DateTime.MaxValue.Ticks))
                });

                foreach(var rec in records)
                {
                    string date = rec.Data.ToString("d", CultureInfo.InvariantCulture);
                    string s = $"#{rec.Number,8};{rec.Text,-20};{date}#{Environment.NewLine}";
                    await writer.WriteAsync(s);
                }
            }
        }

        /// <summary>
        /// 根据输入值随机读取等长（recordSize）的字符；
        /// 键盘输入变量为起始点
        /// </summary>
        /// <param name="recordSize"></param>
        public void RandomAccessSample(int recordSize)
        {
            try
            {
                using(FileStream stream =File.OpenRead(SampleFilePath))
                {
                    byte[] buffer = new byte[recordSize];
                    do
                    {
                        try
                        {
                            Console.Write("record number (or 'byb' to end):");
                            string line = Console.ReadLine();
                            if (line.ToUpper().CompareTo("BYB") == 0) break;

                            int record;
                            if (int.TryParse(line, out record))
                            {
                                stream.Seek((record - 1) * recordSize, SeekOrigin.Begin);
                                stream.Read(buffer, 0, recordSize);
                                string s = Encoding.UTF8.GetString(buffer);
                                Console.WriteLine($"records:\n{s}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    while (true);
                    Console.WriteLine("finished");
                }
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("Create the sample file use the option -sample first");
            }
        }
        #endregion

        #region Use StreamReader & Stream Writer
        /// <summary>
        /// 使用Streamreader读取文件
        /// </summary>
        /// <param name="filename"></param>
        public void ReadFileUsingReader(string filename=SampleFilePath)
        {
            /*
             * 注意：StreamReader默认使用UTF-8编码，读取特殊编码有两种方法
             * var reader =new StreamReader(stream,detectEncodingFromByteOrderMarks:true) 根据序言定义编码判断，此最万能
             * var reader =new StreamReader(stream,Encoding.UTF8)                         显式定义
             * 
             * 可通过 File.OpenText()获取一个StreamReader实例
             * 
             * 允许使用ReadToEnd()从光标处读入一个字符数组
             * 
             * StreamReader还允许把内容读入一个字符数组(注意：char占两个字节，因此只允许Unicode编码的数据读入，UTF-8读入会乱码)
             * int nChars=100;
             * char[] charArray=new char[nChars];
             * int nCharsRead=reader.Read(charArray,0,nChars)
             */
            var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            //var reader1 = File.OpenText(filename);
            using (var reader =new StreamReader(stream,detectEncodingFromByteOrderMarks:true))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    Console.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// 使用StreamWriter写入文件
        /// </summary>
        /// <param name="filename"></param>
        public void ReadFileUsingWriter(string[] lines,string filename= SampleOutputFilePath)
        {
            var outputStream = File.OpenWrite(filename);
            using (var writer=new StreamWriter(outputStream))
            {
                byte[] preamble = Encoding.UTF8.GetPreamble();
                outputStream.Write(preamble, 0, preamble.Length);
                string data = "";
                foreach (var s in lines)
                    data += s + "\n";
                writer.Write(data);
                Console.WriteLine("写入成功。");
            }
        }
        #endregion

        #region Use BinaryReader & BinaryWriter

        /// <summary>
        /// 使用BinaryWriter写入文件
        /// </summary>
        /// <param name="binFile"></param>
        public void WriteFileUsingBinaryWriter(string binFile= BinaryOutPutFilePath)
        {
            var outputStream = File.Create(binFile);
            using(var writer =new BinaryWriter(outputStream))
            {
                double d = 47.47;
                int i = 42;
                long l = 987654321;
                string s = "sample";
                writer.Write(d);
                writer.Write(i);
                writer.Write(l);
                writer.Write(s);
            }
        }

        /// <summary>
        /// BinaryReader读取文件
        /// </summary>
        /// <param name="binFile"></param>
        public void ReadFileUsingBinaryReader(string binFile=BinaryOutPutFilePath)
        {
            /*
             * 变量的顺序一定要与写入时相对应
             */
            var inputStream = File.Open(binFile,FileMode.Open);
            using(var reader =new BinaryReader(inputStream))
            {
                double d = reader.ReadDouble();
                int i = reader.ReadInt32();
                long l = reader.ReadInt64();
                string s = reader.ReadString();
                Console.WriteLine($"ReadRsult: d:{d}, i:{i}, l:{l}, s:{s}");
            }
        }
        #endregion
    }

    /// <summary>
    /// 压缩文件管理
    /// </summary>
    public class CompressManager
    {
        const string NonCompressFilePath = "E:\\\\COPY_TEST\\压缩测试文件.txt";
        const string CompressedFilePath = "E:\\\\COPY_TEST\\测试文件压缩包.pack";
        const string DecompressedFilePath = "E:\\\\COPY_TEST\\解压缩结果.txt";

        const string CompressFolderPath_ZIP = "E:\\\\COPY_TEST";
        const string CompressedFilePath_ZIP = "E:\\压缩文件.zip";
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="compressedFileName"></param>
        public void CompressFile(string fileName=NonCompressFilePath,string compressedFileName=CompressedFilePath)
        {
            using (FileStream inputStream=File.Open(fileName,FileMode.Open))
            {
                FileStream outputStream = File.OpenWrite(compressedFileName);
                using(var compressStream=new DeflateStream(outputStream,CompressionMode.Compress))
                {
                    inputStream.CopyTo(compressStream);
                }
            }
            Console.WriteLine("压缩成功 | Compress Suc");
        }

        /// <summary>
        /// 解压缩文件并读取其中的内容
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="decompressFileName"></param>
        public void DecompressFile(string filename= CompressedFilePath, string decompressFileName=DecompressedFilePath)
        {
            FileStream inputStream = File.OpenRead(filename);
            using(MemoryStream outputStream=new MemoryStream())
            using(var compressStream=new DeflateStream(inputStream,CompressionMode.Decompress))
            {
                compressStream.CopyTo(outputStream);
                outputStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(outputStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096,leaveOpen:true))
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine($"Decompress data：\n{result}");
                }
            }
        }

        /// <summary>
        /// 将文件夹压缩为ZIP文件
        /// </summary>
        /// <param name="directiory"></param>
        /// <param name="zipFile"></param>
        public void CreateZipFile(string directiory= CompressFolderPath_ZIP, string zipFile=CompressedFilePath_ZIP)
        {
            FileStream zipStream = File.OpenWrite(zipFile);
            using(var archive =new ZipArchive(zipStream,ZipArchiveMode.Create))
            {
                IEnumerable<string> files = Directory.EnumerateFiles(directiory, "*", SearchOption.TopDirectoryOnly);
                foreach(var file in files)
                {
                    ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(file));
                    using (FileStream inputStream=File.OpenRead(file))
                        using(Stream outputStream=entry.Open())
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }
            }
        }
    }

    /*
     * 使用文件地图，快速随机访问大文件
     * 在不同的进程或任务之间共享内存
     * 在不同的进程或任务之间共享文件
     * 使用访问器直接从内存位置进行读写
     * 使用流进行读写
     */

    /// <summary>
    /// 使用访问器创建内存映射文件
    /// </summary>
    public class MemoryMappedFilesCreatedByAccessor
    {
        private ManualResetEventSlim _mapCreated = new ManualResetEventSlim(initialState: false);
        private ManualResetEventSlim _dataWrittenEvent = new ManualResetEventSlim(initialState: false);

        private const string MAPNAME = "SampleMap";

        /// <summary>
        /// 启动内存映射文件示例
        /// </summary>
        public void Run()
        {
            Task.Run(() => WriterAsync());
            Task.Run(() => Reader());
            Console.WriteLine("tasks start");
            Console.ReadLine();
        }

        #region Task
        private async Task WriterAsync()
        {
            try
            {
                using (MemoryMappedFile mappedFile = MemoryMappedFile.CreateOrOpen(MAPNAME, 10000, MemoryMappedFileAccess.ReadWrite))
                {
                    _mapCreated.Set();

                    using(MemoryMappedViewAccessor accessor=mappedFile.CreateViewAccessor(0,10000,MemoryMappedFileAccess.Write))
                    {
                        for(int i=0,pos=0;i<100;i++,pos+=4)
                        {
                            accessor.Write(pos, i);
                            Console.WriteLine($"written {i} at position {pos}");
                            await Task.Delay(10);
                        }
                        _dataWrittenEvent.Set();
                        Console.WriteLine("data written");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"writer {ex.Message}");
            }

        }

        private void Reader()
        {
            try
            {
                Console.WriteLine("reader");
                _mapCreated.Wait();
                Console.WriteLine("reader starting");

                using(MemoryMappedFile mappedFile=MemoryMappedFile.OpenExisting(MAPNAME,MemoryMappedFileRights.Read))
                {
                    using(MemoryMappedViewAccessor accessor=mappedFile.CreateViewAccessor(0,10000,MemoryMappedFileAccess.Read))
                    {
                        _dataWrittenEvent.Wait();
                        Console.WriteLine("reading cna start now");
                        for(int i=0;i<400;i+=4)
                        {
                            int result = accessor.ReadInt32(i);
                            Console.WriteLine($"reading {result} from position {i}");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"reader {ex.Message}");
            }
        }
        #endregion
    }

    /// <summary>
    /// 使用流创建内存映射文件
    /// </summary>
    public class MemoryMappedFilesCreatedByStream
    {
        private ManualResetEventSlim _mapCreated = new ManualResetEventSlim(initialState: false);
        private ManualResetEventSlim _dataWrittenEvent = new ManualResetEventSlim(initialState: false);

        private const string MAPNAME = "SampleMap";

        private async Task WriterUsingStreams()
        {
            try
            {
                using(MemoryMappedFile mappedFile=MemoryMappedFile.CreateOrOpen(MAPNAME,10000,MemoryMappedFileAccess.Write))
                {
                    _mapCreated.Set();
                    Console.WriteLine("shared memory segment created");

                    MemoryMappedViewStream stream = mappedFile.CreateViewStream(0, 10000, MemoryMappedFileAccess.Write);
                    using(var writer=new StreamWriter(stream))
                    {
                        writer.AutoFlush = true;
                        for(int i=0;i<100;i++)
                        {
                            string s = $"some data {i}";
                            Console.WriteLine($"writing {s} at {stream.Position}");
                            await writer.WriteLineAsync(s);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"writer {ex.Message}");
            }
        }

        private async Task ReaderUsingStreams()
        {
            try
            {
                Console.WriteLine("reader");
                _mapCreated.Wait();
                Console.WriteLine("reader starting");

                using (MemoryMappedFile mappedFile = MemoryMappedFile.CreateOrOpen(MAPNAME, 10000, MemoryMappedFileAccess.Read))
                {
                    MemoryMappedViewStream stream = mappedFile.CreateViewStream(0, 10000, MemoryMappedFileAccess.Write);
                    using (var reader = new StreamReader(stream))
                    {
                        _dataWrittenEvent.Wait();
                        Console.WriteLine("reading can start now");

                        for(int i=0;i<100;i++)
                        {
                            long pos = stream.Position;
                            string s = await reader.ReadLineAsync();
                            Console.WriteLine($"read {s} from {pos}");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"reader {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 命名管道通信
    /// </summary>
    public class PiplesNet
    {
        /// <summary>
        /// 创建命名管道服务器->接收消息
        /// </summary>
        /// <param name="pipeName"></param>
        public void PipesReader(string pipeName)
        {
            try
            {
                //简化前Code : using (var pipeReader = new NamedPipeServerStream(pipeName, PipeDirection.In))
                var pipeReader = new NamedPipeServerStream(pipeName, PipeDirection.In);
                using (var reader=new StreamReader(pipeReader))
                {
                    pipeReader.WaitForConnection();
                    Console.WriteLine("reader connected");

                    //简化前Code : const int BUFFERSIZE = 256;

                    bool completed = false;
                    while(!completed)
                    {
                        /*
                         * 简化前Code
                        byte[] buffer = new byte[BUFFERSIZE];
                        int nRead = pipeReader.Read(buffer, 0, BUFFERSIZE);
                        string line = Encoding.UTF8.GetString(buffer, 0, nRead);
                        */

                        //简化后不需要读入数组
                        string line = reader.ReadLine();
                        Console.WriteLine(line);
                        if (line == "bye") completed = true;
                    }
                }
                Console.WriteLine("competed reading.");
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 创建命名管道客户端->发送消息
        /// </summary>
        /// <param name="pipeName"></param>
        public void PipesWriter(string pipeName)
        {
            var pipeWriter = new NamedPipeClientStream("TheRocks", pipeName, PipeDirection.Out);
            using(var writer =new StreamWriter(pipeWriter))
            {
                pipeWriter.Connect();
                Console.WriteLine("writer connected");
                bool completed = false;
                while(!completed)
                {
                    string input = Console.ReadLine();
                    if (input == "bye") completed = true;
                    writer.WriteLine(input);
                    writer.Flush();
                }
            }
            Console.WriteLine("completed writing");
        }
    }
}
