using System;
using Entitas.CodeGenerator;
namespace Entitas
{
    public class RoslynComponentInfoProvider : ICodeGeneratorDataProvider
    {
        Type [] _type;
        string [] _poolNames;
        string [] _blueprintNames;

        public string [] blueprintNames {
            get {
                return _blueprintNames;
            }
        }

        public ComponentInfo [] componentInfos {
            get {
                return null;
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

        public RoslynComponentInfoProvider (Type [] type, string [] poolNames, string [] blueprintNames1)
        {
            this._type = type;
            this._poolNames = poolNames;
            this._blueprintNames = blueprintNames1;
        }
    }
}

