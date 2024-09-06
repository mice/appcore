using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TestNextReset
{
    [Test]
    public void TestDayNextReset()
    {
        //每天8点开始9点结束.
        var resetData = CircleResetInfo.CreateDay(1, (uint)TimeSpan.FromHours(8).TotalSeconds, (int)TimeSpan.FromHours(1).TotalSeconds);
        var zoneDiff = TimeSpan.FromHours(8);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        UnityEngine.Debug.Log($"CurrentDay::{currentDateTime}::days:{currentDateTime.DayOfWeek}");//Thursday
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;

        TimeUtils.SetServerTimeStamp(totalSeconds,0);

        var nestTs = resetData.GetNextResetTS(TimeUtils.serverTimestamp,0);
        Assert.AreEqual(new DateTime(2022, 11, 10, 8, 0, 0),TimeUtils.TimestampToDateTime(nestTs));
        nestTs = resetData.GetNextResetTS(TimeUtils.serverTimestamp + (long)TimeSpan.FromHours(8).TotalSeconds, 0);
        Assert.AreEqual(new DateTime(2022, 11, 11, 8, 0, 0),TimeUtils.TimestampToDateTime(nestTs));

        nestTs = resetData.GetNextResetTS(TimeUtils.serverTimestamp + (long)TimeSpan.FromHours(10).TotalSeconds, 0);
        Assert.AreEqual(new DateTime(2022, 11, 11, 8, 0, 0), TimeUtils.TimestampToDateTime(nestTs));
    }

    [Test]
    public void TestWeekNextReset()
    {
        //1周一次,星期8点刷新,持续1小时.
        var resetData = CircleResetInfo.CreateWeek(1, (uint)TimeSpan.FromHours(8).TotalSeconds, (int)TimeSpan.FromHours(1).TotalSeconds, 2, 1);
        var zoneDiff = TimeSpan.FromHours(8);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        UnityEngine.Debug.Log($"CurrentDay::{currentDateTime}::days:{currentDateTime.DayOfWeek}");//Thursday
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;

        TimeUtils.SetServerTimeStamp(totalSeconds, 0);

        var nestTs = resetData.GetNextResetTS(TimeUtils.serverTimestamp, 0);
        Assert.AreEqual(2, resetData.GetState(TimeUtils.serverTimestamp, 0));
        //Assert.AreEqual(new DateTime(2022, 11, 10 + 5, 8, 0, 0), TimeUtils.TimestampToDateTime(nestTs));
        //nestTs = resetData.GetNextResetTS(TimeUtils.serverTimestamp + (long)TimeSpan.FromHours(8).TotalSeconds, 0);
        //Assert.AreEqual(new DateTime(2022, 11, 11, 8, 0, 0), TimeUtils.TimestampToDateTime(nestTs));

        //nestTs = resetData.GetNextResetTS(TimeUtils.serverTimestamp + (long)TimeSpan.FromHours(10).TotalSeconds, 0);
        //Assert.AreEqual(new DateTime(2022, 11, 11, 8, 0, 0), TimeUtils.TimestampToDateTime(nestTs));
    }

    [Test]
    public void TestHourNextReset()
    {
        //1周一次,星期8点刷新,持续1小时.
        var resetData = CircleResetInfo.CreateHour(1, (uint)TimeSpan.FromMinutes(30).TotalSeconds, (int)TimeSpan.FromMinutes(30).TotalSeconds, 1);
        var zoneDiff = TimeSpan.FromHours(8);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        UnityEngine.Debug.Log($"CurrentDay::{currentDateTime}::days:{currentDateTime.DayOfWeek}");//Thursday
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;

        TimeUtils.SetServerTimeStamp(totalSeconds, 0);

        var nestTs = resetData.GetNextResetTS(TimeUtils.serverTimestamp, 0);
        Assert.AreEqual(0, resetData.GetState(TimeUtils.serverTimestamp, 0));
        //Assert.AreEqual(new DateTime(2022, 11, 10,0 , 30, 0), TimeUtils.TimestampToDateTime(nestTs));
        //nestTs = resetData.GetNextResetTS(TimeUtils.serverTimestamp + (long)TimeSpan.FromHours(8).TotalSeconds, 0);
        //Assert.AreEqual(new DateTime(2022, 11, 11, 8, 0, 0), TimeUtils.TimestampToDateTime(nestTs));

        //nestTs = resetData.GetNextResetTS(TimeUtils.serverTimestamp + (long)TimeSpan.FromHours(10).TotalSeconds, 0);
        //Assert.AreEqual(new DateTime(2022, 11, 11, 8, 0, 0), TimeUtils.TimestampToDateTime(nestTs));
    }
}
