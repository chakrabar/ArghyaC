using ArghyaC.Domain.Enums;
using System.Collections.Generic;

namespace ArghyaC.Domain.Entities
{
    public class CodeRunResult
    {
        public CodeRunResult()
        {
            TestCaseResults = new List<TestCaseResult>();
            Instructions = new List<string>();
        }

        public IEnumerable<string> Instructions { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public CodeCompState State { get; set; }
        public IEnumerable<TestCaseResult> TestCaseResults { get; set; }
        public IEnumerable<string> CompileErrors { get; set; }
    }

    public class TestCaseResult //TODO: move out
    {
        public long Milliseconds { get; set; }
        public bool IsCorrect { get; set; }
        public string Message { get; set; }
    }
}
