
namespace ArghyaC.CSharpRunner.Types
{
    public class TestCaseResult
    {
        public long Milliseconds { get; set; }
        public bool IsCorrect { get; set; }
        public string Message { get; set; }
        public bool HasException { get; set; }
    }

    public class TestCaseResult<TResult> : TestCaseResult
    {
        public TResult Output { get; set; }        
    }
}
