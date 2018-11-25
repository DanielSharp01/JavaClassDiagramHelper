using System;
using System.Collections.Generic;
using System.Text;

namespace JavaClassDiagramHelper
{
    public abstract class Classifier : Qualified
    {
        public Classifier(List<Annotation> annotations, List<Qualifier> qualifiers)
            : base(annotations, qualifiers)
        { }
    }
}
