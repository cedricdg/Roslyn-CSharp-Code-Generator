using System;
using Entitas.CodeGenerator;
namespace Entitas
{
    public class RoslynComponentInfoProvider : ICodeGeneratorDataProvider
    {
        Type [] type;
        string [] v;
        string [] blueprintNames1;

        public string [] blueprintNames {
            get {
                throw new NotImplementedException ();
            }
        }

        public ComponentInfo [] componentInfos {
            get {
                throw new NotImplementedException ();
            }
        }

        public string [] poolNames {
            get {
                throw new NotImplementedException ();
            }
        }


        public RoslynComponentInfoProvider ()
        {
        }

        public RoslynComponentInfoProvider (Type [] type, string [] v, string [] blueprintNames1)
        {
            this.type = type;
            this.v = v;
            this.blueprintNames1 = blueprintNames1;
        }
    }
}

