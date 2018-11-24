using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaClassDiagramHelper
{
    public class Parser
    {
        private static readonly string[] primitiveTypes = new string[] { "char", "boolean", "byte", "double", "float", "int", "long", "short", "void" };

        private static readonly string[] qualifierWords = new string[] { "public", "protected", "private", "static", "abstract", "final", "transient",
            "synchronized", "volatile" };

        private int currentIndex = 0;
        private List<Token> tokens;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens.Where(t => t.Type != TokenType.Comment).ToList();
        }

        public object Parse()
        {
            // Skip until the first class declaration
            while ((!qualifierWords.Contains(tokens[currentIndex].Value) || tokens[currentIndex - 1].Value == "import")
                && tokens[currentIndex].Value != "class" && tokens[currentIndex].Value != "interface" && tokens[currentIndex].Value != "@") currentIndex++;
            Annotation annotation = NextAnnotation();
            List<Qualifier> qualifiers = NextQualifiers();
            if (tokens[currentIndex].Value == "class")
            {
                return NextClass(qualifiers, annotation);
            }
            else if (tokens[currentIndex].Value == "interface")
            {
                return NextInterface(qualifiers, annotation);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private List<Qualifier> NextQualifiers()
        {
            List<Qualifier> qualifiers = new List<Qualifier>();
            for (; currentIndex < tokens.Count && qualifierWords.Contains(tokens[currentIndex].Value); currentIndex++)
            {
                qualifiers.Add(Enum.Parse<Qualifier>(UpperFirstChar(tokens[currentIndex].Value)));
            }

            return qualifiers;
        }

        private Class NextClass(List<Qualifier> qualifiers, Annotation annotation)
        {
            currentIndex++;
            TypeReference className = NextTypeReference();
            Class classObj = new Class() { Name = className, Annotation = annotation };
            classObj.Qualifiers.AddRange(qualifiers);

            // Extends
            if (tokens[currentIndex].Value == "extends")
            {
                currentIndex++;
                classObj.Extends = NextTypeReference();
            }

            // Implements
            if (tokens[currentIndex].Value == "extends")
            {
                currentIndex++;
                while (tokens[currentIndex].Value == ",")
                {
                    currentIndex++;
                    classObj.Implements.Add(NextTypeReference());
                }
            }

            currentIndex++; // Skipping {

            object member = null;
            while ((member = NextMember()) != null)
            {
                if (member is Method method)
                {
                    classObj.Methods.Add(method);
                }
                else if (member is Field field)
                {
                    classObj.Fields.Add(field);
                }
                else if (member is Class internalClass)
                {
                    classObj.InternalClasses.Add(internalClass);
                }
            }

            currentIndex++; // Skipping }

            return classObj;
        }

        private Interface NextInterface(List<Qualifier> qualifiers, Annotation annotation)
        {
            currentIndex++;
            TypeReference className = NextTypeReference();
            Interface interfaceObj = new Interface() { Name = className, Annotation = annotation };
            interfaceObj.Qualifiers.AddRange(qualifiers);

            // Extends
            if (tokens[currentIndex].Value == "extends")
            {
                currentIndex++;
                interfaceObj.Extends = NextTypeReference();
            }

            currentIndex++; // Skipping {

            object member = null;
            while ((member = NextMember()) != null)
            {
                if (member is Method method)
                {
                    interfaceObj.Methods.Add(method);
                }
            }

            currentIndex++; // Skipping }

            return interfaceObj;
        }

        private TypeReference NextTypeReference()
        {
            TypeReference typeReference = new TypeReference { Name = tokens[currentIndex++].Value };

            while (tokens[currentIndex].Value == "." && tokens[currentIndex + 1].Type == TokenType.Identifier)
            {
                currentIndex++;
                typeReference.Name += "." + tokens[currentIndex++].Value;
            }

            if (tokens[currentIndex].Value == "<")
            {
                currentIndex++;
                typeReference.GenericTypes.Add(tokens[currentIndex++].Value);
                while (tokens[currentIndex].Value == ",")
                {
                    currentIndex++;
                    typeReference.GenericTypes.Add(tokens[currentIndex++].Value);
                }
                currentIndex++; // Skipping >
            }
            
            if (tokens[currentIndex].Value == "[")
            {
                currentIndex += 2; // Skipping []
                typeReference.IsArray = true;
            }
            else if (tokens[currentIndex].Value == ".")
            {
                currentIndex += 3; // Skipping ...
            }

            return typeReference;
        }

        private Parameter NextParameter()
        {
            TypeReference typeReference = NextTypeReference();
            return new Parameter() { Type = typeReference, Name = tokens[currentIndex++].Value };
        }

        private object NextMember()
        {
            if (tokens[currentIndex].Value == "}") return null;

            Annotation annotation = NextAnnotation();
            List<Qualifier> qualifiers = NextQualifiers();

            if (tokens[currentIndex].Value == "class")
            {
                return NextClass(qualifiers, annotation);
            }
            else if (tokens[currentIndex].Value == "interface")
            {
                return NextInterface(qualifiers, annotation);
            }

            TypeReference typeRef = NextTypeReference();
            string name;
            if (tokens[currentIndex].Value == "(")
            {
                name = typeRef.Name;
                typeRef = null;
            }
            else
            {
                name = tokens[currentIndex++].Value;
            }

            if (tokens[currentIndex].Value == "(")
            {
                currentIndex++;
                Method method = new Method() { ReturnType = typeRef, Name = name, Annotation = annotation };
                method.Qualifiers.AddRange(qualifiers);
                if (tokens[currentIndex].Value != ")")
                {
                    method.Parameters.Add(NextParameter());
                    while (tokens[currentIndex].Value == ",")
                    {
                        currentIndex++;
                        method.Parameters.Add(NextParameter());
                    }
                }

                currentIndex++; // Skipping )

                SkipMethodBody();

                return method;
            }
            else if (tokens[currentIndex].Value == "[")
            {
                // We have a field
                currentIndex++;
                typeRef.IsArray = true;
            }

            Field field = new Field() { Type = typeRef, Name = name, Annotation = annotation };
            field.Qualifiers.AddRange(qualifiers);
            while (tokens[currentIndex].Value != ";") currentIndex++;
            currentIndex++; // Skipping ;
            return field;
        }

        private void SkipMethodBody()
        {
            while (tokens[currentIndex].Value != "{" && tokens[currentIndex].Value != ";") currentIndex++;

            if (tokens[currentIndex].Value == ";")
            {
                currentIndex++;
                return;
            }

            currentIndex++; // Skipping {
            int braceLevel = 1;
            while (braceLevel > 0)
            {
                if (tokens[currentIndex].Value == "{")
                {
                    braceLevel++;
                }
                else if (tokens[currentIndex].Value == "}")
                {
                    braceLevel--;
                }

                currentIndex++;
            }
        }

        private Annotation NextAnnotation()
        {
            if (tokens[currentIndex].Value == "@")
            {
                currentIndex++;
                return new Annotation { Name = tokens[currentIndex++].Value };
            }

            return null;
        }

        private string UpperFirstChar(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
