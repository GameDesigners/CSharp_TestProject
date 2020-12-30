using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharp_Test
{
    class String_Class
    {
        public static string Invariant(FormattableString s) => s.ToString(CultureInfo.InvariantCulture);
    }

    public class Racer : IComparable<Racer>,IFormattable
    {
        public int Id { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public int Wins { get; set; }

        public Racer(int id, string firstname, string lastname, string country) : this(id, firstname, lastname, country, wins: 0) { }
        public Racer(int id, string firstname, string lastname, string country, int wins)
        {
            Id = id;
            FirstName = firstname;
            LastName = lastname;
            Country = country;
            Wins = wins;
        }

        public override string ToString() => $"{FirstName} {LastName}";
        public string ToString(string format,IFormatProvider formatProvider)
        {
            if (format == null) format = "N";
            switch(format.ToUpper())
            {
                case "N":
                    return ToString();
                case "F":
                    return FirstName;
                case "L":
                    return LastName;
                case "W":
                    return $"{ToString()},Wins: {Wins}";
                case "C":
                    return $"{ToString()},Country: {Country}";
                case "A":
                    return $"{ToString()},Country: {Country} Wins: {Wins}";
                default:
                    throw new FormatException(string.Format(formatProvider, $"Format {format} is not support"));
            }
        }

        public string ToString(string format) => ToString(format, null);

        public int CompareTo(Racer other)
        {
            int compare = LastName?.CompareTo(other?.LastName) ?? -1;
            if(compare==0)
            {
                return FirstName?.CompareTo(other?.FirstName) ?? -1;
            }
            return compare;
        }
    }


    class StringPattern
    {
        public static string input= @"This book is perfect for both experienced C# programmers looking to "+
                           "the first time. The authors deliver unparalleled coverage of "+
                           "Visual Studio 2013 and .NET Framework 4.5.1 additions, as woll as "+
                           "new test-dr1ven development and concur rent programming reatures. "+
                           "Source code for all the. examples are available for download, so you " +
                           "can start writing Windows desktop， Windows Store apps， and ASP.NET "+
                           "web applications immediately.";

        public static void Find1(string text,string pattern)
        {
            MatchCollection matches = Regex.Matches(text,pattern,RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            foreach(Match nextMatch in matches)
            {
                Console.WriteLine(nextMatch.Index);
            }
        }
    }
}
