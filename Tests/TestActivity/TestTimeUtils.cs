using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;


public class TestTimeUtils 
{
    private static TimeZoneInfo GetZoneByOff(TimeSpan timeDiff)
    {
        foreach (var item in TimeZoneInfo.GetSystemTimeZones())
        {
            if (item.BaseUtcOffset == timeDiff)
            {
                return item;
            }
        }
        var tmpZone = "Custom" + timeDiff.TotalHours;
        var zone = TimeZoneInfo.CreateCustomTimeZone(tmpZone, timeDiff, tmpZone, tmpZone);
        return zone;
    }

    [Test]
    public void TestDateTime()
    {
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;
        var diffSpan = TimeSpan.FromHours(8);
        var zone = GetZoneByOff(diffSpan);
        UnityEngine.Debug.Log("Zone::" + zone);
        UnityEngine.Debug.Log(TimeZone.CurrentTimeZone.GetUtcOffset(currentDateTime).TotalSeconds);
        UnityEngine.Debug.Log(TimeZoneInfo.Utc.GetUtcOffset(currentDateTime).TotalSeconds);
        UnityEngine.Debug.Log(TimeZoneInfo.Local.GetUtcOffset(currentDateTime).TotalSeconds);
        
        UnityEngine.Debug.Log(TimeZoneInfo.Local.GetUtcOffset(currentDateTime).TotalSeconds);
        TimeUtils.SetServerTimeStamp(totalSeconds, (int)diffSpan.TotalSeconds);
        UnityEngine.Debug.Log(TimeUtils.serverTimestamp + ":" + TimeUtils.serverDateTime);
    }

    [Test]
    public void TestDateTime2()
    {
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0, DateTimeKind.Local);
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;
        var diffSpan = TimeSpan.FromHours(8);
        var zone = GetZoneByOff(diffSpan);

        TimeUtils.SetServerTimeStamp(totalSeconds, (int)diffSpan.TotalSeconds);
        
        var dateTime_1 = new DateTime(2022, 11, 10, 00, 00, 00,DateTimeKind.Local);//local时间,TimeUtils.TimestampToDateTime为服务器时间.需要补回来.时差
        Assert.AreEqual(TimeUtils.TimestampToDateTime(TimeUtils.serverTimestamp + (long)(dateTime_1 - currentDateTime).TotalSeconds - (long)diffSpan.TotalSeconds), dateTime_1);
    }


    [Test]
    public void TestStartDate()
    {
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;
        var diffSpan = TimeSpan.FromHours(8);
        TimeUtils.SetServerTimeStamp(totalSeconds, (int)diffSpan.TotalSeconds);
        var currentTS = TimeUtils.serverTimestamp;
        var tmpData = TimeUtils.GetHourOfDayTS(currentTS,0);
        Assert.AreEqual(TimeUtils.TimestampToDateTime(tmpData),new DateTime(2022,11,10,0,0,0));

        tmpData = TimeUtils.GetHourOfDayTS(currentTS, 1);
        Assert.AreEqual(TimeUtils.TimestampToDateTime(tmpData), new DateTime(2022, 11, 10, 1, 0, 0));
    }
}
