using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Interface
    {
        public Annotation Annotation { get; set; }
        public TypeReference Name { get; set; }
        public List<Qualifier> Qualifiers { get; } = new List<Qualifier>();
        public List<Method> Methods { get; } = new List<Method>();
        public TypeReference Extends { get; set; }

        public override string ToString()
        {
            string ret = "";
            foreach (Qualifier qualifier in Qualifiers)
            {
                ret += Enum.GetName(typeof(Qualifier), qualifier).ToLower() + " ";
            }
            ret += "interface " + Name.ToString();

            if (Extends != null)
            {
                ret += " extends " + Extends.ToString();
            }

            return ret;
        }
    }
}
