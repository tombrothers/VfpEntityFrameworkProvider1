using System.Reflection;
using SampleQueries.Harness;

namespace SampleQueries.Runner {
    public class Sample : SampleBase {
        private readonly MethodInfo _method;
        private readonly string _sourceCode;

        public Sample(SampleSuite sampleSuite, MethodInfo method, string title, string description, string sourceCode)
            : base(sampleSuite, title, description) {
            _method = method;
            _sourceCode = sourceCode;
        }

        public MethodInfo Method {
            get { return _method; }
        }

        public string SourceCode {
            get { return _sourceCode; }
        }
    }
}