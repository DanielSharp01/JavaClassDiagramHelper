using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class TypeReference
    {
        public string Name { get; set; }
        public string Extends { get; set; }
        public List<TypeReference> GenericTypes { get; } = new List<TypeReference>();
        public bool IsArray { get; set; } = false;

        public override string ToString()
        {
            string ret = Name;

            if (GenericTypes.Count > 0)
            {
                ret += "<";
                foreach (TypeReference type in GenericTypes)
                {
                    ret += type.ToString();
                }
                ret += ">";
            }

            if (IsArray)
            {
                ret += "[]";
            }

            return ret;
        }

        public string ToUMLString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Name);

            if (GenericTypes.Count > 0)
            {
                stringBuilder.Append("<");
                foreach (TypeReference type in GenericTypes)
                {
                    if (type.Name == "?" && type.Extends != null)
                    {
                        stringBuilder.Append(type.Extends);
                    }
                    else
                    {
                        stringBuilder.Append(type.Name);
                    }
                    stringBuilder.Append(", ");
                }
                stringBuilder.Length -= 2;
                stringBuilder.Append(">");
            }

            if (IsArray)
            {
                stringBuilder.Append("[0..*]");
            }

            return stringBuilder.ToString();
        }
    }
}
