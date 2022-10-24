using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerSharedTests
{
    static class UnitTestHelper
    {
        public static bool AlmostEquals(float a, float b, float tolerance=0.001f)
        {
            return Math.Abs(a - b) < tolerance;
        }
    }
}
