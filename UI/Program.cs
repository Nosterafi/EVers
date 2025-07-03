using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;

namespace UI
{
    internal class Program
    {
        class Dfg : HashVerifiable
        {
            public string Content;

            protected override string ComputeId() =>
                base.ComputeId() + ComputeHash(Encoding.UTF8.GetBytes(Content));

            public Dfg(string content)
            {
                Content = content;
                SetCurrentId();
            }
        }
        static void Main(string[] args)
        {
            var a = new Dfg("aaa");
            Console.WriteLine(a.IsNotChanged());

            a.Content = "asd";
            Console.WriteLine(a.IsNotChanged());
        }
    }
}
