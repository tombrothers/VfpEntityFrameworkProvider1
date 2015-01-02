using System;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VfpClient;

namespace VfpEntityFrameworkProvider.CodeFirst.Tests {
    [TestClass]
    public abstract class TestBase {
        public TestContext TestContext { get; set; }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context) {
            VfpProviderFactory.Register();
            Database.SetInitializer(new DataInitializer());

            File.WriteAllBytes("AutoGenId.zip", Properties.Resources.AutoGenId);
            File.WriteAllBytes("AllTypes.zip", Properties.Resources.AllTypes);

            var zip = new FastZip();
            zip.ExtractZip("AutoGenId.zip", Path.Combine(context.TestDeploymentDir, @"AutoGenId\Data"), string.Empty);
            zip.ExtractZip("AllTypes.zip", Path.Combine(context.TestDeploymentDir, @"AllTypes"), string.Empty);

            VfpClientTracing.Tracer = new TraceSource("VfpClient", SourceLevels.Information);
            VfpClientTracing.Tracer.Listeners.Add(new TestContextTraceListener(context));
        }

        protected CodeFirstContext GetContext() {
            var connection = new VfpConnection(ConfigurationManager.ConnectionStrings["CodeFirstContext"].ConnectionString);

            EnableTracing(connection);

            return new CodeFirstContext(connection);
        }

        protected void EnableTracing(VfpConnection connection) {
            if (Debugger.IsAttached) {
                return;
            }

            connection.CommandExecuting = details => TestContext.WriteLine(details.ToTraceString() + Environment.NewLine);
            connection.CommandFailed = details => TestContext.WriteLine(details.ToTraceString() + Environment.NewLine);
            connection.CommandFinished = details => TestContext.WriteLine(details.ToTraceString() + Environment.NewLine);
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