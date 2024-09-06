using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Utils;

namespace Tests
{
    public class TestActivityContainer
    {
        [Test]
        public void TestCreateActivityContainerFail()
        {
            Assert.Throws<Exception>(() =>
            {
                var activity = new ActivityGroup(0);
            });
        }

        [Test]
        public void TestCreateActivityContainer()
        {
            var activityContainer = new ActivityGroup(1);
            Assert.NotNull(activityContainer);
        }


        [Test]
        public void TestTimeUtils()
        {
            TimeUtils.SetServerTimeStamp(1, 0);
            Assert.AreEqual(TimeUtils.serverTimestamp, 1);
        }


        [Test]
        public void TestCreateActivityAddState0()
        {
            var activityContainer = new ActivityGroup(1);
            var activity1 = new ActivityInfo(1, 1).SetTimeSpan(10, 100);
            var activity2 = new ActivityInfo(2, 1).SetTimeSpan(10, 100);
            TimeUtils.SetServerTimeStamp(1, 0);
            activityContainer.AddActivity(activity1);
            activityContainer.AddActivity(activity2);
            Assert.IsFalse(activityContainer.IsEmpty());
            Assert.IsTrue(activityContainer.HasNearestOpen());
            Assert.IsFalse(activityContainer.HasNearestClose());
        }

        [Test]
        public void TestCreateActivityAddState1()
        {
            var activityContainer = new ActivityGroup(1);
            var activity1 = new ActivityInfo(1, 1).SetTimeSpan(10, 100);
            var activity2 = new ActivityInfo(2, 1).SetTimeSpan(10, 100);
            TimeUtils.SetServerTimeStamp(11, 0);
            activityContainer.AddActivity(activity1);
            activityContainer.AddActivity(activity2);
            Assert.IsFalse(activityContainer.IsEmpty());
            Assert.IsFalse(activityContainer.HasNearestOpen());
            Assert.IsTrue(activityContainer.HasNearestClose());
        }

        [Test]
        public void TestCreateActivityAddState2()
        {
            var activityContainer = new ActivityGroup(1);
            var activity1 = new ActivityInfo(1, 1).SetTimeSpan(10, 100);
            var activity2 = new ActivityInfo(2, 1).SetTimeSpan(10, 100);
            TimeUtils.SetServerTimeStamp(111, 0);
            activityContainer.AddActivity(activity1);
            activityContainer.AddActivity(activity2);
            Assert.IsFalse(activityContainer.IsEmpty());
            Assert.IsFalse(activityContainer.HasNearestOpen());
            Assert.IsFalse(activityContainer.HasNearestClose());
        }
    }
}
