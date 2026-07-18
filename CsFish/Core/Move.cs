using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public readonly struct Move(int f, int t)
    {
        public readonly int From = f;
        public readonly int To = t;
    }
}
