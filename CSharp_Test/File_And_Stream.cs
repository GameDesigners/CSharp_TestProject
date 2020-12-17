using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

    public class StreamManager
    {
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
    }
}
