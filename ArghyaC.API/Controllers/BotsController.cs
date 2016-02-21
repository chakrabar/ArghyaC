using ArghyaC.API.Utilities;
using ArghyaC.API.ViewModels;
using ArghyaC.CSharpRunner;
using ArghyaC.CSharpRunner.Types;
using ArghyaC.Infrastructure.Utilities;
using System.Web.Http;

namespace ArghyaC.API.Controllers
{
    public class BotsController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public CodeRunResult<string[]> RunMineMazer(int m, int n, int x, int y, int[][] d, string code)
        {
            var timestamp = System.DateTime.Now.ToString("yyyyMMdd_HH.mm.ss.fffffff");
            var tempFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/SubmittedCode/Temp_" + timestamp);
            //var tempFolder = Server.MapPath("~/SubmittedCode/Temp_" + timestamp);

            var input = new object[] { m, n, x, y, d };
            code = code.Replace("\\r\\n", "" + System.Environment.NewLine);
            var result = FileUtilities.TempCreateDirAndExecute<CodeRunResult<string[]>>(tempFolder,
                () => { return CodeRunner.CompileAndRun<string[]>(code, tempFolder, input); });

            return result;
        }

        [HttpPost]
        public CodeRunResult<string[]> RunMineMazer(MineMazerViewModel context)
        {
            var timestamp = System.DateTime.Now.ToString("yyyyMMdd_HH.mm.ss.fffffff");
            var tempFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/SubmittedCode/Temp_" + timestamp);
            //var tempFolder = Server.MapPath("~/SubmittedCode/Temp_" + timestamp);

            var input = new object[] { context.M, context.N, context.X, context.Y, context.Data };

            var result = FileUtilities.TempCreateDirAndExecute<CodeRunResult<string[]>>(tempFolder,
                () => { return CodeRunner.CompileAndRun<string[]>(context.Code, tempFolder, input); });

            return result;
        }        
    }
}
