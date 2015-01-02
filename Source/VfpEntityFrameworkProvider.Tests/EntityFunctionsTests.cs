using System;
using System.Data.Objects;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VfpEntityFrameworkProvider.Tests {
    [TestClass]
    public class EntityFunctionsTests : TestBase {
        private const int YEAR = 2011;
        private const int MONTH = 5;
        private const int DAY = 29;
        private const int HOUR = 4;
        private const int MINUTE = 37;
        private const int SECOND = 58;
        private readonly DateTime TestDateTime;
        private readonly DateTimeOffset TestDateTimeOffset;
        private readonly TimeSpan TestTimeSpan;
        private readonly int TotalDays;
 
        public EntityFunctionsTests() {
            this.TestDateTime = new DateTime(YEAR, MONTH, DAY, HOUR, MINUTE, SECOND);
            this.TestDateTimeOffset = new DateTimeOffset(this.TestDateTime);
            var timespan = this.TestDateTime.Subtract(new DateTime());
            this.TotalDays = (int)timespan.TotalDays;
            this.TestTimeSpan = new TimeSpan(this.TotalDays, HOUR, MINUTE, SECOND);
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffYears_DateTime_Test() {
            DateTime testDateTime2 = this.TestDateTime.AddYears(1);
            var result = this.GetOrderQuery().Select(x => EntityFunctions.DiffYears(this.TestDateTime, testDateTime2)).First();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMonths_DateTime_Test() {
            DateTime testDateTime2 = this.TestDateTime.AddMonths(1);
            var result = this.GetOrderQuery().Select(x => EntityFunctions.DiffMonths(this.TestDateTime, testDateTime2)).First();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffDays_DateTime_Test() {
            DateTime testDateTime2 = this.TestDateTime.AddDays(1);
            var result = this.GetOrderQuery().Select(x => EntityFunctions.DiffDays(this.TestDateTime, testDateTime2)).First();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffHours_DateTime_Test() {
            DateTime testDateTime2 = this.TestDateTime.AddHours(1);

            var result = this.GetOrderQuery().Select(x => EntityFunctions.DiffHours(this.TestDateTime, testDateTime2)).First();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMinutes_DateTime_Test() {
            DateTime testDateTime2 = this.TestDateTime.AddMinutes(1);
            var result = this.GetOrderQuery().Select(x => EntityFunctions.DiffMinutes(this.TestDateTime, testDateTime2)).First();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffSeconds_DateTime_Test() {
            DateTime testDateTime2 = this.TestDateTime.AddSeconds(1);
            var result = this.GetOrderQuery().Select(x => EntityFunctions.DiffSeconds(this.TestDateTime, testDateTime2)).First();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void EntityFunctionsTests_TruncateTime_DateTime_Test() {
            var result = this.GetOrderQuery().Select(x => (DateTime)EntityFunctions.TruncateTime(EntityFunctions.CreateDateTime(YEAR, MONTH, DAY, HOUR, MINUTE, SECOND))).First();

            Assert.AreEqual(YEAR, result.Year);
            Assert.AreEqual(MONTH, result.Month);
            Assert.AreEqual(DAY, result.Day);
        }

        [TestMethod]
        public void EntityFunctionsTests_Right_Test() {
            var result = this.GetOrderQuery().Select(x => EntityFunctions.Right(x.Customer.CustomerID, 2)).First();
            Assert.AreEqual("ET", result);
        }

        [TestMethod]
        public void EntityFunctionsTests_Left_Test() {
            var result = this.GetOrderQuery().Select(x => EntityFunctions.Left(x.Customer.CustomerID, 2)).First();
            Assert.AreEqual("VI", result);
        }    

        [TestMethod]
        public void EntityFunctionsTests_CreateDateTime_Test() {
            var result = this.GetOrderQuery().Select(x => (DateTime) EntityFunctions.CreateDateTime(YEAR, MONTH, DAY, HOUR, MINUTE, SECOND)).First();

            Assert.AreEqual(YEAR, result.Year);
            Assert.AreEqual(MONTH, result.Month);
            Assert.AreEqual(DAY, result.Day);
            Assert.AreEqual(HOUR, result.Hour);
            Assert.AreEqual(MINUTE, result.Minute);
            Assert.AreEqual(SECOND, result.Second);
        }
        
        [TestMethod]
        public void EntityFunctionsTests_AddYears_DateTime_Test() {
            var result = this.GetOrderQuery().Select(x => (DateTime)EntityFunctions.AddYears(this.TestDateTime, 1)).First();

            Assert.AreEqual(YEAR + 1, result.Year);
            Assert.AreEqual(MONTH, result.Month);
            Assert.AreEqual(DAY, result.Day);
            Assert.AreEqual(HOUR, result.Hour);
            Assert.AreEqual(MINUTE, result.Minute);
            Assert.AreEqual(SECOND, result.Second);
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMonths_DateTime_Test() {
            var result = this.GetOrderQuery().Select(x => (DateTime)EntityFunctions.AddMonths(this.TestDateTime, 1)).First();

            Assert.AreEqual(YEAR, result.Year);
            Assert.AreEqual(MONTH + 1, result.Month);
            Assert.AreEqual(DAY, result.Day);
            Assert.AreEqual(HOUR, result.Hour);
            Assert.AreEqual(MINUTE, result.Minute);
            Assert.AreEqual(SECOND, result.Second);
        }
        
        [TestMethod]
        public void EntityFunctionsTests_AddDays_DateTime_Test() {
            var result = this.GetOrderQuery().Select(x => (DateTime)EntityFunctions.AddDays(this.TestDateTime, 1)).First();

            Assert.AreEqual(YEAR, result.Year);
            Assert.AreEqual(MONTH, result.Month);
            Assert.AreEqual(DAY + 1, result.Day);
            Assert.AreEqual(HOUR, result.Hour);
            Assert.AreEqual(MINUTE, result.Minute);
            Assert.AreEqual(SECOND, result.Second);
        }

        [TestMethod]
        public void EntityFunctionsTests_AddHours_DateTime_Test() {
            var result = this.GetOrderQuery().Select(x => (DateTime)EntityFunctions.AddHours(this.TestDateTime, 1)).First();

            Assert.AreEqual(YEAR, result.Year);
            Assert.AreEqual(MONTH, result.Month);
            Assert.AreEqual(DAY, result.Day);
            Assert.AreEqual(HOUR + 1, result.Hour);
            Assert.AreEqual(MINUTE, result.Minute);
            Assert.AreEqual(SECOND, result.Second);
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMinutes_DateTime_Test() {
            var result = this.GetOrderQuery().Select(x => (DateTime)EntityFunctions.AddMinutes(this.TestDateTime, 1)).First();

            Assert.AreEqual(YEAR, result.Year);
            Assert.AreEqual(MONTH, result.Month);
            Assert.AreEqual(DAY, result.Day);
            Assert.AreEqual(HOUR, result.Hour);
            Assert.AreEqual(MINUTE + 1, result.Minute);
            Assert.AreEqual(SECOND, result.Second);
        }

        [TestMethod]
        public void EntityFunctionsTests_AddSeconds_DateTime_Test() {
            var result = this.GetOrderQuery().Select(x => (DateTime)EntityFunctions.AddSeconds(this.TestDateTime, 1)).First();

            Assert.AreEqual(YEAR, result.Year);
            Assert.AreEqual(MONTH, result.Month);
            Assert.AreEqual(DAY, result.Day);
            Assert.AreEqual(HOUR, result.Hour);
            Assert.AreEqual(MINUTE, result.Minute);
            Assert.AreEqual(SECOND + 1, result.Second);
        }

        #region NotSupported

        // I don't believe that some of these Not Supported functions can be implemented due to VFP limitations.  However, there are some that I think that could
        // be implemented but figured the effort was too much for a first attempt of creating the EF provider.

        [TestMethod]
        public void EntityFunctionsTests_DiffNanoseconds_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffNanoseconds(this.TestTimeSpan, this.TestTimeSpan)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffNanoseconds_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffNanoseconds(this.TestDateTimeOffset, this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffNanoseconds_DateTime_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffNanoseconds(this.TestDateTime, this.TestDateTime)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMicroseconds_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffMilliseconds(this.TestTimeSpan, this.TestTimeSpan)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMicroseconds_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffMicroseconds(this.TestDateTimeOffset, this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMicroseconds_DateTime_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffMicroseconds(this.TestDateTime, this.TestDateTime)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMilliseconds_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffMilliseconds(this.TestTimeSpan, this.TestTimeSpan)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMilliseconds_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffMilliseconds(this.TestDateTimeOffset, this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMilliseconds_DateTime_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffMilliseconds(this.TestDateTime, this.TestDateTime)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffYears_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffYears(this.TestDateTimeOffset, this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMonths_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffMonths(this.TestDateTimeOffset, this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffDays_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffDays(this.TestDateTimeOffset, this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffHours_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffHours(this.TestTimeSpan, this.TestTimeSpan)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffHours_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffHours(this.TestDateTimeOffset, this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMinutes_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffMinutes(this.TestTimeSpan, this.TestTimeSpan)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffMinutes_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffMinutes(this.TestDateTimeOffset, this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffSeconds_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffSeconds(this.TestTimeSpan, this.TestTimeSpan)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_DiffSeconds_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.DiffSeconds(this.TestDateTimeOffset, this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_Truncate_Double_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.Truncate((double)123.45, 2)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_Truncate_Decimal_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.Truncate((decimal)123.45, 2)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_TruncateTime_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMilliseconds(this.TestDateTimeOffset, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_StandardDeviation_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.StandardDeviation(x.OrderDetails.Select(d => d.UnitPrice))).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_VarP_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.VarP(x.OrderDetails.Select(d => d.UnitPrice))).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_Var_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.Var(x.OrderDetails.Select(d => d.UnitPrice))).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_Reverse_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.Reverse("test")).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_GetTotalOffsetMinutes_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.GetTotalOffsetMinutes(this.TestDateTimeOffset)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_CreateTime_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.CreateTime(HOUR, MINUTE, SECOND)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_CreateDateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.CreateDateTimeOffset(YEAR, MONTH, DAY, HOUR, MINUTE, SECOND, -5)).First();
            });
        }

        /// Not sure what is going on with these two functions.  I'm not able to hook into the expression tree to throw a NotSupportedException.
        //[TestMethod]
        //public void EntityFunctionsTests_AsUnicode_Test() {
        //    this.AssertException<NotSupportedException>(() => {
        //        this.GetOrderQuery().Select(x => EntityFunctions.AsUnicode("test")).First();
        //    });
        //}

        //[TestMethod]
        //public void EntityFunctionsTests_AsNonUnicode_Test() {
        //    this.AssertException<NotSupportedException>(() => {
        //        this.GetOrderQuery().Select(x => EntityFunctions.AsNonUnicode("test")).First();
        //    });
        //}

        [TestMethod]
        public void EntityFunctionsTests_AddNanoseconds_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddNanoseconds(this.TestTimeSpan, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddNanoseconds_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddNanoseconds(this.TestDateTimeOffset, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddNanoseconds_DateTime_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddNanoseconds(this.TestDateTime, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMilliseconds_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMilliseconds(this.TestTimeSpan, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMilliseconds_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMilliseconds(this.TestDateTimeOffset, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMilliseconds_DateTime_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMilliseconds(this.TestDateTime, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMicroseconds_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMicroseconds(this.TestTimeSpan, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMicroseconds_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMicroseconds(this.TestDateTimeOffset, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMicroseconds_DateTime_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMicroseconds(this.TestDateTime, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddYears_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddYears(this.TestDateTimeOffset, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMonths_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMonths(this.TestDateTimeOffset, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddDays_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddDays(this.TestDateTimeOffset, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddHours_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddHours(this.TestTimeSpan, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddHours_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddHours(this.TestDateTimeOffset, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMinutes_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMinutes(this.TestTimeSpan, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddMinutes_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddMinutes(this.TestDateTimeOffset, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddSeconds_TimeSpan_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddSeconds(this.TestTimeSpan, 1)).First();
            });
        }

        [TestMethod]
        public void EntityFunctionsTests_AddSeconds_DateTimeOffset_Test() {
            this.AssertException<NotSupportedException>(() => {
                this.GetOrderQuery().Select(x => EntityFunctions.AddSeconds(this.TestDateTimeOffset, 1)).First();
            });
        }

        #endregion
    }
}
