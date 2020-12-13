using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp_Test
{
    /// <summary>
    /// 文档管理泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class DocumentManager<TDocument> where TDocument : IDocument  //TDocument强制要求实现IDocument的接口
    {
        private readonly Queue<TDocument> documentQueue = new Queue<TDocument>();  //文档队列

        /// <summary>
        /// 向文档管理队列添加文档
        /// </summary>
        /// <param name="doc"></param>
        public void AddDocument(TDocument doc)
        {
            lock(this)  //与线程有关，暂且不讲
            {
                documentQueue.Enqueue(doc);
            }
        }

        /// <summary>
        /// 获取当前文档
        /// </summary>
        /// <returns></returns>
        public TDocument GetDocument()
        {
            TDocument doc = default; 
            lock(this)
            {
                doc = documentQueue.Dequeue();
            }
            return doc;
        }
        /// <summary>
        /// 当前文档是否可用
        /// </summary>
        public bool IsDocumentAvailable => documentQueue.Count > 0;

        public void DisplayAllDocuments()
        {
            foreach(TDocument t in documentQueue)
            {
                Console.WriteLine(t.Title);
            }
        }
    }

    public interface IDocument
    {
        string Title { get; set; }
        string Content { get; set; }
    }

    public class Document : IDocument
    {
        public Document() { }

        public Document(string _title,string _content)
        {
            Title = _title;
            Content = _content;
        }

        public string Title { get; set; }
        public string Content { get; set; }
    }

}
