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

        public Classifier Parse()
        {
            // Skip until the first class declaration
            while ((!qualifierWords.Contains(tokens[currentIndex].Value) || tokens[currentIndex - 1].Value == "import")
                && tokens[currentIndex].Value != "class" && tokens[currentIndex].Value != "interface" && tokens[currentIndex].Value != "@") currentIndex++;
            List<Annotation> annotations = NextAnnotations();
            List<Qualifier> qualifiers = NextQualifiers();
            if (tokens[currentIndex].Value == "class")
            {
                return NextClass(annotations, qualifiers);
            }
            else if (tokens[currentIndex].Value == "interface")
            {
                return NextInterface(annotations, qualifiers);
            }
            else if (tokens[currentIndex].Value == "enum")
            {
                return NextEnum(annotations, qualifiers);
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

        private Class NextClass(List<Annotation> annotations, List<Qualifier> qualifiers)
        {
            currentIndex++;
            TypeReference className = NextTypeReference();
            Class classObj = new Class(annotations, qualifiers) { Name = className };

            // Extends
            if (tokens[currentIndex].Value == "extends")
            {
                currentIndex++;
                classObj.Extends = NextTypeReference();
            }

            // Implements
            if (tokens[currentIndex].Value == "implements")
            {
                currentIndex++;
                classObj.Implements.Add(NextTypeReference());
                while (tokens[currentIndex].Value == ",")
                {
                    currentIndex++;
                    classObj.Implements.Add(NextTypeReference());
                }
            }

            currentIndex++; // Skipping {

            Qualified member = null;
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
                else if (member is Classifier classifier)
                {
                    classObj.InternalClassifiers.Add(classifier);
                }
            }

            currentIndex++; // Skipping }

            return classObj;
        }

        private Interface NextInterface(List<Annotation> annotations, List<Qualifier> qualifiers)
        {
            currentIndex++;
            TypeReference name = NextTypeReference();
            Interface interfaceObj = new Interface(annotations, qualifiers) { Name = name };

            // Extends
            if (tokens[currentIndex].Value == "extends")
            {
                currentIndex++;
                interfaceObj.Extends = NextTypeReference();
            }

            currentIndex++; // Skipping {

            Qualified member = null;
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

        private Enumeration NextEnum(List<Annotation> annotations, List<Qualifier> qualifiers)
        {
            currentIndex++;
            TypeReference name = NextTypeReference();
            Enumeration enumObj = new Enumeration(annotations, qualifiers) { Name = name };

            while (tokens[currentIndex].Value != "}")
            {
                currentIndex++;
                enumObj.Values.Add(tokens[currentIndex].Value);
                while (tokens[currentIndex].Value != "}" && tokens[currentIndex].Value != ",") currentIndex++;
            }

            currentIndex++; // Skipping }

            return enumObj;
        }

        private TypeReference NextTypeReference(bool generic = false)
        {
            TypeReference typeReference = new TypeReference { Name = tokens[currentIndex++].Value };
            if (tokens[currentIndex].Value == "extends" && generic)
            {
                currentIndex++;
                typeReference.Extends = tokens[currentIndex++].Value;
            }

            while (tokens[currentIndex].Value == "." && tokens[currentIndex + 1].Type == TokenType.Identifier)
            {
                currentIndex++;
                typeReference.Name += "." + tokens[currentIndex++].Value;
            }

            if (tokens[currentIndex].Value == "<")
            {
                currentIndex++;
                if (tokens[currentIndex].Value == "?") currentIndex++;
                if (tokens[currentIndex].Value == "extends") currentIndex++;
                typeReference.GenericTypes.Add(NextTypeReference(true));
                while (tokens[currentIndex].Value == ",")
                {
                    currentIndex++;
                    if (tokens[currentIndex].Value == "?") currentIndex++;
                    if (tokens[currentIndex].Value == "extends") currentIndex++;
                    typeReference.GenericTypes.Add(NextTypeReference(true));
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

        private Qualified NextMember()
        {
            if (tokens[currentIndex].Value == "}") return null;

            List<Annotation> annotations = NextAnnotations();
            List<Qualifier> qualifiers = NextQualifiers();

            if (tokens[currentIndex].Value == "{")
            {
                Method method = new Method(annotations, qualifiers) { Name = null };
                SkipMethodBody(method);
                return method;
            }

            if (tokens[currentIndex].Value == "class")
            {
                return NextClass(annotations, qualifiers);
            }
            else if (tokens[currentIndex].Value == "interface")
            {
                return NextInterface(annotations, qualifiers);
            }
            else if (tokens[currentIndex].Value == "enum")
            {
                return NextEnum(annotations, qualifiers);
            }

            TypeReference typeRef = NextTypeReference();
            TypeReference name;
            if (tokens[currentIndex].Value == "(")
            {
                name = typeRef;
                typeRef = null;
            }
            else
            {
                name = NextTypeReference();
            }

            if (tokens[currentIndex].Value == "(")
            {
                currentIndex++;
                Method method = new Method(annotations, qualifiers) { ReturnType = typeRef, Name = name };
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

                if (tokens[currentIndex].Value == "throws")
                {
                    currentIndex++;
                    method.Throws.Add(NextTypeReference());
                    while (tokens[currentIndex].Value == ",")
                    {
                        currentIndex++;
                        method.Throws.Add(NextTypeReference());
                    }
                }

                SkipMethodBody(method);

                return method;
            }
            else if (tokens[currentIndex].Value == "[")
            {
                // We have a field
                currentIndex++;
                typeRef.IsArray = true;
            }

            if (name.IsArray)
            {
                typeRef.IsArray = true;
                name.IsArray = false;
            }

            Field field = new Field(annotations, qualifiers) { Type = typeRef, Name = name };
            while (tokens[currentIndex].Value != ";") currentIndex++;
            currentIndex++; // Skipping ;
            return field;
        }

        private void SkipMethodBody(Method method)
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
                else if (tokens[currentIndex].Type == TokenType.Identifier)
                {
                    method.BodyIdentifiers.Add(tokens[currentIndex].Value);
                }

                currentIndex++;
            }
        }

        private List<Annotation> NextAnnotations()
        {
            List<Annotation> ret = new List<Annotation>();
            if (tokens[currentIndex].Value == "@")
            {
                currentIndex++;
                ret.Add(new Annotation { Name = tokens[currentIndex++].Value });
                if (tokens[currentIndex].Value == "(")
                {
                    currentIndex++;
                    while (tokens[currentIndex].Value != ")") currentIndex++;
                    currentIndex++; //Skipping )
                }
                if (tokens[currentIndex].Value == ",") currentIndex++;
            }

            return ret;
        }

        public static string UpperFirstChar(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
