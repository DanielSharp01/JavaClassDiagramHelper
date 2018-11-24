using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Field
    {
        public Annotation Annotation { get; set; }
        public TypeReference Type { get; set; }
        public string Name { get; set; }
        public List<Qualifier> Qualifiers { get; } = new List<Qualifier>();

        public override string ToString()
        {
            string ret = "";
            foreach (Qualifier qualifier in Qualifiers)
            {
                ret += Enum.GetName(typeof(Qualifier), qualifier).ToLower() + " ";
            }
            ret += Type.ToString() + " " + Name;

            return ret;
        }
    }
}
