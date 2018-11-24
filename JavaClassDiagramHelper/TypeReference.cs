using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class TypeReference
    {
        public string Name { get; set; }
        public List<string> GenericTypes { get; } = new List<string>();
        public bool IsArray { get; set; } = false;

        public override string ToString()
        {
            string ret = Name;

            if (GenericTypes.Count > 0)
            {
                ret += "<";
                foreach (string type in GenericTypes)
                {
                    ret += type;
                }
                ret += ">";
            }

            if (IsArray)
            {
                ret += "[]";
            }

            return ret;
        }
    }
}
