using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ModelService;

namespace compobj
{
    class Program
    {
        static void Main(string[] args)
        {
            TestClass test1 = BuildTestClass1();
            TestClass test2 = BuildTestClass2();
            IModelService modelService = new ModelServiceProvider();

            PressAnyKey("Printing TestClass1 object model...");
            StringBuilder message = modelService.PrintObject(test1);
            Console.Write(message.ToString() + "\n\n");

            PressAnyKey("Showing differences between test1 and test2...");
            List<Difference> diff = modelService.GetDifferingProperties(test1, test2);
            foreach(Difference d in diff)
            {
                Console.Write("{0} : {1} , {2}\n", d.PropertyName, d.Object1, d.Object2);
            }

            PressAnyKey("Showing equal properties in test1 and test2...");
            List<string> equals = modelService.GetEqualProperties(test1, test2);
            foreach(string str in equals)
            {
                Console.WriteLine(str);
            }

            PressAnyKey("Computing MD5 hash of test1 and test2...");
            string hash = modelService.ComputeHash(test1, new MD5CryptoServiceProvider());
            Console.WriteLine(hash);
            hash = modelService.ComputeHash(test2, new MD5CryptoServiceProvider());
            Console.WriteLine(hash);

            PressAnyKey("Done");
        }

        static void PressAnyKey(string message)
        {
            Console.Write(message + "\nPress any key to continue...\n");
            Console.ReadKey();
        }

        static TestClass BuildTestClass1()
        {
            MemoryStream ms = new MemoryStream();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("foo", "bar");
            dic.Add("bar", null);

            List<int> list = new List<int>();
            list.Add(1);
            list.Add(2);

            List<List<string>> nested = new List<List<string>>();
            List<string> strlist1 = new List<string>();
            List<string> strlist2 = new List<string>();
            strlist1.Add("one");
            strlist1.Add("two");
            strlist2.Add("three");
            strlist2.Add("four");

            nested.Add(strlist1);
            nested.Add(strlist2);
            
            TestClass test = new TestClass();
            test.ms = ms;
            test.dic = dic;
            test.list = list;
            test.nested = nested;
            test.str = "string";
            test.str2 = "foo";
            test.i = 42;

            return test;
        }

        static TestClass BuildTestClass2()
        {
            MemoryStream ms = new MemoryStream();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("foo", "bar");
            dic.Add("bar", "foo");

            List<int> list = new List<int>();
            list.Add(1);

            List<List<string>> nested = new List<List<string>>();
            List<string> strlist1 = new List<string>();
            List<string> strlist2 = new List<string>();
            strlist1.Add("one");
            strlist1.Add("two");
            strlist2.Add("three");

            nested.Add(strlist1);
            nested.Add(strlist2);

            TestClass test = new TestClass();
            test.ms = ms;
            test.dic = dic;
            test.list = list;
            test.nested = nested;
            test.str = "string";
            test.str2 = "foobar";
            test.i = 42;

            return test;
        }
    }
}
