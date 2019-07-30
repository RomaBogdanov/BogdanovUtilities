using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynTests
{
    class TestFile
    {
        public TestFile()
        { }

        public void Meth1()
        {

        }

        public void Meth1(string a, int b)
        {
            string c = "";
            for (int i = 0; i <= b; i++)
            {
                c += a;
            }
        }

        public string Meth1(string a)
        {
            string b = $"Test: {a}";
            return b;
        }
    }
}
