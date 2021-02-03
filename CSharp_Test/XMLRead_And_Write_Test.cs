using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CSharp_Test
{
    class XMLRead_And_Write_Test
    {
        private const string BooksFileName = "books.xml";
        private const string NewBookFileName = "newbooks.xml";

        
    }

    class XML_Test
    {
        private const string BooksFileName = "XML/books.xml";
        private const string NewBookFileName = "newbooks.xml";
        public static void ReadTextNodes()
        {
            using(XmlReader reader=XmlReader.Create(BooksFileName))
            {
                while(reader.Read())
                {
                    if(reader.NodeType==XmlNodeType.Text)
                    {
                        Console.WriteLine(reader.Value);
                    }
                }
            }
        }

        public static void ReadElementContent()
        {
            using(XmlReader reader =XmlReader.Create(BooksFileName))
            {
                while(!reader.EOF)
                {
                    if(reader.MoveToContent()==XmlNodeType.Element&&reader.Name== "title")
                    {
                        Console.WriteLine(reader.ReadElementContentAsString());
                    }
                    else
                    {
                        //move on
                        reader.Read();
                    }
                }
            }
        }

        public static void ReadElementContent2()
        {
            using (XmlReader reader = XmlReader.Create(BooksFileName))
            {
                while (!reader.EOF)
                {
                    if (reader.MoveToContent() == XmlNodeType.Element)
                    {
                        try
                        {
                            Console.WriteLine(reader.ReadElementContentAsString());
                        }
                        catch (XmlException)
                        {
                            reader.Read();
                        }
                    }
                    else
                    {
                        //move on
                        reader.Read();
                    }
                }
            }
        }

        public static void ReadDecimal()
        {

        }

        public static void ReadAttributes()
        {

        }
        
        public static void ShowUsage()
        {

        }
    }
}
