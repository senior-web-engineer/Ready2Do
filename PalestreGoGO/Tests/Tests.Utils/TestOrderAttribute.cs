using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Utils
{
    public class TestOrderAttribute: Attribute
    {
        public int I { get; }
        public TestOrderAttribute(int i)
        {
            I = i;
        }
    }
}
