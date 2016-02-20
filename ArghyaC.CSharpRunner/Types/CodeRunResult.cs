using ArghyaC.CSharpRunner.Types.Enums;
using System.Collections.Generic;

namespace ArghyaC.CSharpRunner.Types
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
        public virtual IEnumerable<TestCaseResult> TestCaseResults { get; set; }
        public IEnumerable<string> CompileErrors { get; set; }
    }

    public class CodeRunResult<TOutcome> : CodeRunResult
    {
        public IEnumerable<TestCaseResult<TOutcome>> TestCaseResultsWithOutput { get; set; }
    }
}
