using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Design;
using System.Data.EntityClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VfpEntityFrameworkProvider.Tests {
    [TestClass]
    public class DesignTimeTests : TestBase {
        protected Store.SchemaInformation context;

        [TestInitialize]
        public void TestInitialize() {
            EntityConnectionStringBuilder csb = new EntityConnectionStringBuilder();
            csb.ConnectionString = ConfigurationManager.ConnectionStrings["NorthwindEntities"].ConnectionString;

            var schemaConnection = EntityStoreSchemaGenerator.CreateStoreSchemaConnection(csb.Provider, csb.ProviderConnectionString);
            schemaConnection.Open();

            context = new Store.SchemaInformation(schemaConnection);
        }

        #region SchemaInformation

        [TestMethod]
        [TestCategory("SchemaInformation")]
        //[Title("Query - Tables")]
        [Description("This sample lists all tables in the schema.")]
        public void DesignTime1() {
            using (EntityCommand cmd = new EntityCommand("SELECT t.Name FROM SchemaInformation.Tables AS t", (EntityConnection)context.Connection)) {
                using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess)) {
                    //ObjectDumper.Write(reader);
                }
            }

            var list = context.Tables.ToList();
        }

        [TestMethod]
        [TestCategory("SchemaInformation")]
        //[Title("Query - Views")]
        [Description("This sample lists all views in the schema.")]
        public void DesignTime2() {
            var list = context.Views.ToList();
        }

        [TestMethod]
        [TestCategory("SchemaInformation")]
        //[Title("Query - Functions")]
        [Description("This sample lists all functions in the schema.")]
        public void DesignTime3() {
            var list = context.Functions.ToList();
        }

        [TestMethod]
        [TestCategory("SchemaInformation")]
        //[Title("Query - Function Parameters")]
        [Description("This sample lists all function parameters in the schema.")]
        public void DesignTime4() {
            var list = context.Functions.ToList();
        }

        [TestMethod]
        [TestCategory("SchemaInformation")]
        //[Title("Query - Procedures")]
        [Description("This sample lists all procedures in the schema.")]
        public void DesignTime5() {
            var list = context.Procedures.ToList();
        }

        [TestMethod]
        [TestCategory("SchemaInformation")]
        //[Title("Query - Procedure Parameters")]
        [Description("This sample lists all procedure parameters in the schema.")]
        public void DesignTime6() {
            var list = context.ProcedureParameters.ToList();
        }

        [TestMethod]
        [TestCategory("SchemaInformation")]
        //[Title("Query - Foreign Keys")]
        [Description("This sample lists all foreign keys in the schema.")]
        public void DesignTime7() {
            var tfkQuery = context.TableForeignKeys;
            var tfkList = tfkQuery.ToList();

            var vfkQuery = context.ViewForeignKeys;
            var vfkList = vfkQuery.ToList();

            //var query = tfkQuery.Union(vfkQuery);
            var list = tfkList.Union(vfkList);
        }

        [TestMethod]
        [TestCategory("SchemaInformation")]
        //[Title("Query - Table Constraints")]
        [Description("This sample lists all table constraints.")]
        public void DesignTime8() {
            var list = context.TableConstraints.ToList();
        }

        [TestMethod]
        [TestCategory("SchemaInformation")]
        //[Title("Query - View Constraints")]
        [Description("This sample lists all view constraints.")]
        public void DesignTime9() {
            var list = context.ViewConstraints.ToList();
        }
        #endregion
    }
}