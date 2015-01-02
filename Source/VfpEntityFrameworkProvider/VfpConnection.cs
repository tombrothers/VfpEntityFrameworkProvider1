using System;
using System.Data;
using System.Data.Common;
using VfpEntityFrameworkProvider.VfpOleDb;

namespace VfpEntityFrameworkProvider {
    public class VfpConnection : VfpClient.VfpConnection {
        public VfpConnection() {
        }

        public VfpConnection(string connectionString)
            : base(connectionString) {
        }

        protected override DbProviderFactory DbProviderFactory {
            get {
                return VfpProviderFactory.Instance;
            }
        }

        public new VfpCommand CreateCommand() {
            return (VfpCommand)CreateDbCommand();
        }

        protected override DbCommand CreateDbCommand() {
            return new VfpCommand(OleDbConnection.CreateCommand(), this);
        }

        public override DataTable GetSchema(string collectionName) {
            return FixViewSchema(collectionName, base.GetSchema(collectionName));
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues) {
            return FixViewSchema(collectionName, base.GetSchema(collectionName, restrictionValues));
        }

        private static DataTable FixViewSchema(string collectionName, DataTable schema) {
            if (!SchemaNames.Views.Equals(collectionName, StringComparison.InvariantCultureIgnoreCase) || schema.Rows.Count == 0) {
                return schema;
            }

            var newSchema = schema.AsEnumerable()
                                  .Where(x => !x.Field<string>(SchemaColumnNames.View.Sql).Contains("?"))
                                  .CopyToDataTable();

            newSchema.TableName = schema.TableName;

            return newSchema;
        }
    }
}