using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;

namespace Sheepshead.Tests
{
    [TestClass]
    public class LongIdTests
    {
        [TestMethod]
        public void CallerMaySetButNotChangeRecordId()
        {
            var dataObj = new LongId();
            Assert.AreEqual(0, dataObj.Id, "Object starts with an ID of 0.");
            dataObj.Id = 5;
            Assert.AreEqual(5, dataObj.Id, "Object ID was changed.");
            try
            {
                dataObj.Id = 6;
                Assert.Fail("An exception should be thrown when an object's unique ID is changed.");
            }
            catch (IdAlreadySetException ex)
            {
                Assert.IsTrue(true, "Threw an exception when object's unique ID was changed.");
            }
            catch (ApplicationException ex)
            {
                Assert.Fail("Wrong exception occurred while changing the object's unique ID.");
            }
        }
    }
}
