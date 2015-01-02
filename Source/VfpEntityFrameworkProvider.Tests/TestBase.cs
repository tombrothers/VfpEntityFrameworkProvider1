using System;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindEFModel;
using VfpClient;

namespace VfpEntityFrameworkProvider.Tests {
    [TestClass]
    public abstract class TestBase {
        public TestContext TestContext { get; set; }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context) {
            VfpProviderFactory.Register();
            File.WriteAllText("NorthwindEFModel.csdl", Properties.Resources.NorthwindEFModelCsdl);
            File.WriteAllText("NorthwindEFModel.msl", Properties.Resources.NorthwindEFModelMsl);
            File.WriteAllText("NorthwindEFModel.ssdl", Properties.Resources.NorthwindEFModelSsdl);
            File.WriteAllBytes("NorthwindVfp.zip", Properties.Resources.NorthwindVfp);
            File.WriteAllBytes("DecimalTable.zip", Properties.Resources.DecimalTable);

            FastZip zip = new FastZip();
            zip.ExtractZip("NorthwindVfp.zip", context.TestDeploymentDir, string.Empty);
            zip.ExtractZip("DecimalTable.zip", Path.Combine(context.TestDeploymentDir, "Decimal"), string.Empty);

            VfpClientTracing.Tracer = new TraceSource("VfpClient", SourceLevels.Information);
            VfpClientTracing.Tracer.Listeners.Add(new TestContextTraceListener(context));
        }

        protected string GetQueryText(IQueryable query) {
            var objectQuery = query as ObjectQuery;

            if (objectQuery != null) {
                return objectQuery.ToTraceString();
            }

            return string.Empty;
        }

        protected IQueryable<Order> GetOrderQuery() {
            return this.GetContext().Orders
                                    .OrderBy(x => x.OrderID)
                                    .Where(x => x.OrderID == 10248);
        }

        protected NorthwindEntities GetContext() {
            var paths = new[] {
                "NorthwindEFModel.csdl",
                "NorthwindEFModel.ssdl",
                "NorthwindEFModel.msl"
            };

            var workspace = new MetadataWorkspace(paths, new[] { Assembly.GetExecutingAssembly() });
            var connection = new EntityConnection(workspace, GetConnection());

            return new NorthwindEntities(connection);
        }

        protected virtual VfpConnection GetConnection() {
            var northwind = Path.Combine(TestContext.TestDeploymentDir, "northwind.dbc");
            var connectionString = @"Provider=VFPOLEDB.1;Data Source=" + northwind + ";ansi=true";
            var connection = new VfpConnection(connectionString);

            EnableTracing(connection);

            return connection;
        }

        private void EnableTracing(VfpConnection connection) {
            if (Debugger.IsAttached) {
                return;
            }

            connection.CommandExecuting = details => TestContext.WriteLine(GetExecutionDetails(details));
            connection.CommandFailed = details => TestContext.WriteLine(GetExecutionDetails(details));
            connection.CommandFinished = details => TestContext.WriteLine(GetExecutionDetails(details));
        }

        private string GetExecutionDetails(VfpClient.VfpCommandExecutionDetails details) {
            return Environment.NewLine + details.ToTraceString() + Environment.NewLine;
        }

        protected void AssertException<T>(Action action) where T : Exception {
            T exception = null;

            try {
                action();
            }
            catch (Exception ex) {
                exception = GetException<T>(ex);
            }

            if (exception == null) {
                throw new Exception(typeof(T).Name + " was not thrown");
            }
        }

        private T GetException<T>(Exception exception) where T : Exception {
            while (exception != null) {
                var specificException = exception as T;

                if (specificException != null) {
                    return specificException;
                }

                exception = exception.InnerException;
            }

            return null;
        }

        private class TestContextTraceListener : TraceListener {
            private readonly TestContext _context;

            public TestContextTraceListener(TestContext context) {
                _context = context;
            }

            public override void Write(string message) {
                _context.WriteLine(message);
            }

            public override void WriteLine(string message) {
                _context.WriteLine(message.Replace("{", "{{").Replace("}", "}}"));
            }
        }
    }
}
