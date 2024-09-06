using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestActivity
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestCreateActivity()
        {
            var activity = new ActivityInfo(1,1);
            Assert.IsNotNull(activity);
        }

        [Test]
        public void TestCreateActivityState()
        {
            var activity = new ActivityInfo(1,1).SetTimeSpan(10,100);
           
            Assert.AreEqual(1u, activity.GetState(20));
            Assert.AreEqual(0u, activity.GetState(1));
            Assert.AreEqual(2u, activity.GetState(101));
        }
    }
}
