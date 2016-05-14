using System;
using Entitas.CodeGenerator;
namespace Entitas
{
    public class RoslynComponentInfoProvider : ICodeGeneratorDataProvider
    {
        Type [] type;
        string [] _poolNames;
        string [] _blueprintNames;

        public string [] blueprintNames {
            get {
                return _blueprintNames;
            }
        }

        public ComponentInfo [] componentInfos {
            get {
                throw new NotImplementedException ();
            }
        }

        public string [] poolNames {
            get {
                return _poolNames;
            }
        }


        public RoslynComponentInfoProvider ()
        {
        }

        public RoslynComponentInfoProvider (Type [] type, string [] poolNames, string [] blueprintNames)
        {
            this.type = type;
            _poolNames = poolNames;
            _blueprintNames = blueprintNames;
        }
    }
}

