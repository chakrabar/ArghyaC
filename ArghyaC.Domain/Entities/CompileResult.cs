using System;
using System.Collections.Generic;

namespace ArghyaC.Domain.Entities
{
    public class CompileResult
    {
        public bool IsCompiled { get; set; }
        public string AssemblyPath { get; set; }
        public string FilePath { get; set; }
        public Exception CompileException { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
