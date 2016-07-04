using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wintogo
{
    public class TestClass
    {
        public TestClass()
        {
            
        }
        public TestClass(int n)
        {
            MyProperty = n;
        }
        public int MyProperty { get; set; }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return MyProperty.ToString();
        }
    }
}
