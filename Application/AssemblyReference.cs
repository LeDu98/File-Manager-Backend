using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(System.Reflection.Metadata.AssemblyReference).Assembly;
    }
}
