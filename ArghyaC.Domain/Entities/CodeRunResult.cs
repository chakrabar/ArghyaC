using ArghyaC.Domain.Enums;
using System.Collections.Generic;

namespace ArghyaC.Domain.Entities
{
    public class CodeRunResult
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public CodeCompState State { get; set; }
        public IEnumerable<string> TestCaseResults { get; set; }
    }
}
