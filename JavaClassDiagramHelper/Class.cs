using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Class
    {
        public Annotation Annotation { get; set; }
        public TypeReference Name { get; set; }
        public List<Qualifier> Qualifiers { get; } = new List<Qualifier>();
        public List<Class> InternalClasses { get; } = new List<Class>();
        public List<Method> Methods { get; } = new List<Method>();
        public List<Field> Fields { get; } = new List<Field>();
        public TypeReference Extends { get; set; }
        public List<TypeReference> Implements { get; } = new List<TypeReference>();

        public override string ToString()
        {
            string ret = "";
            foreach (Qualifier qualifier in Qualifiers)
            {
                ret += Enum.GetName(typeof(Qualifier), qualifier).ToLower() + " ";
            }
            ret += "class " + Name.ToString();

            if (Extends != null)
            {
                ret += " extends " + Extends.ToString();
            }

            if (Implements.Count > 0)
            {
                ret += " implements";
                foreach (TypeReference type in Implements)
                {
                    ret += type.ToString() + ", ";
                }
                ret = ret.Substring(0, ret.Length - 2);
            }

            return ret;
        }
    }
}
