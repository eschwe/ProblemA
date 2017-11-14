using System;
using System.Collections.Generic;
using System.IO;

namespace ModelService
{
    public class TestClass
    {
        public MemoryStream ms { get; set; }
        public Dictionary<string, string> dic { get; set; }
        public List<int> list { get; set; }
        public List<List<string>> nested { get; set; }
        public string str { get; set; }
        public string str2 { get; set; }
        public int i { get; set; }
        public DateTime dt { get; set; }

        public TestClass()
        {
            dt = DateTime.Now;
        }
    }
}
