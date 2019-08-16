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
            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public void Meth1(string a, int b)
        {
            string c = "";
            for (int i = 0; i <= b; i++)
            {
                c += a;
            }
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public string Meth1(string a)
        {
            try
            {

            }
            catch
            {

                throw;
            }
            string b = $"Test: {a}";
            return b;
        }
    }
}
