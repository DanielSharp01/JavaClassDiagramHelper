using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Parameter
    {
        public TypeReference Type { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Type.ToString() + " " + Name;
        }
    }
}
