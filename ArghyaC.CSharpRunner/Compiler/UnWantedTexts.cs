using System.Collections.Generic;
using System.Linq;

namespace ArghyaC.CSharpRunner.Compiler
{
    internal class UnWantedTexts //TODO: static
    {
        const string _using1 = "Microsoft.CSharp";
        const string _using2 = "System.CodeDom.Compiler"; //compiler
        const string _using3 = "System.Reglection";
        const string _using4 = "System.Reflection.Emit"; //compile runtime types
        const string _using5 = "System.Runtime";
        const string _using6 = "System.Runtime.InteropServices"; //pInvoke, com calls
        const string _using7 = "System.Runtime.Remoting"; //also redundant, still....
        const string _using8 = "System.Security";
        const string _using9 = "System.IO";
        const string _using10 = "System.Net";

        const string _dll = ".dll";

        const string _attribute1 = "DllImport"; //pInvoke
        const string _attribute2 = "ComImport"; //comInvoke
        const string _attribute3 = "assembly";

        const string _keyword1 = "// compile";
        const string _keyword2 = "unsafe"; //no pinters please
        const string _keyword3 = "byte*";

        List<string> _texts = null;

        public IEnumerable<string> Texts
        {
            get
            {
                if (_texts == null)
                    _texts = this.GetType()
                        .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Select(f => (string)f.GetValue(this))
                        .ToList();
                return _texts;
            }
        }
    }
}
