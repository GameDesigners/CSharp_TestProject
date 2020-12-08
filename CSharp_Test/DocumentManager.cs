using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp_Test
{
    /// <summary>
    /// 文档管理泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class DocumentManager<T>
    {
        private readonly Queue<T> documentQueue = new Queue<T>();  //文档队列

        /// <summary>
        /// 向文档管理队列添加文档
        /// </summary>
        /// <param name="doc"></param>
        public void AddDocument(T doc)
        {
            lock(this)  //与线程有关，暂且不讲
            {
                documentQueue.Enqueue(doc);
            }
        }

        /// <summary>
        /// 当前文档是否可用
        /// </summary>
        public bool IsDocumentAvailable => documentQueue.Count > 0;
    }
}
