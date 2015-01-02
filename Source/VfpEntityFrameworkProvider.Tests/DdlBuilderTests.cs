using System;
using System.Data.Metadata.Edm;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VfpEntityFrameworkProvider.Tests.Properties;

namespace VfpEntityFrameworkProvider.Tests {
    [TestClass]
    public class DdlBuilderTests : TestBase {
        [TestMethod]
        public void DdlBuilderTests_Test() {
            var  script = DdlBuilder.CreateObjectsScript(GetStoreItemCollection());

            Console.WriteLine(script);
        }

        private StoreItemCollection GetStoreItemCollection() {
            var ssdl = Resources.NorthwindEFModelSsdl;
            var xmlReaders = new XmlReader[1];

            xmlReaders[0] = XmlReader.Create(new StringReader(ssdl));

            return new StoreItemCollection(xmlReaders);
        }
    }
}