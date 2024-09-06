using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TestPeriodData
{

    [Test]
    public void TestPeriodDataCtr()
    {
        Assert.Catch<MissingMethodException>(() =>
        {
            var inst = System.Activator.CreateInstance<CircleResetInfo>();
        });
    }

    [Test]
    public void TestPeriodDataDayCtr()
    {
        var resetData = CircleResetInfo.CreateDay(1, 0, -1);
        Assert.AreEqual(resetData.ConfigID, 1);
        Assert.AreEqual(resetData.ResetType, (uint)CircleResetType.Day);
    }

    /// <summary>
    /// 这个是每日:0点刷新,持续到下次
    /// 也就是这个活动是每日都进行的,长期进行的
    /// </summary>
    [Test]
    public void TestPeriodDataDayState()
    {
        var resetData = CircleResetInfo.CreateDay(1, 0, -1);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp), 1);
    }

    /// <summary>
    /// 这个是每日:0点刷新,持续一个小时的
    /// </summary>
    [Test]
    public void TestPeriodDataDayState1()
    {
        var resetData = CircleResetInfo.CreateDay(1, 0, (int)TimeSpan.FromHours(1).TotalSeconds);
        var zoneDiff = TimeSpan.FromHours(8);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;

        TimeUtils.SetServerTimeStamp(totalSeconds, (int)zoneDiff.TotalSeconds);
        UnityEngine.Debug.Log(TimeUtils.serverTimestamp + ":" + TimeUtils.serverDateTime);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp), 1);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + 59), 1);
        UnityEngine.Debug.Log(TimeUtils.serverTimestamp + ":" + TimeUtils.TimestampToDateTime(TimeUtils.serverTimestamp + (int)TimeSpan.FromHours(1).TotalSeconds + 1));
        Assert.AreEqual(2, resetData.GetState(TimeUtils.serverTimestamp + (int)TimeSpan.FromHours(1).TotalSeconds + 2));
    }

    /// <summary>
    /// 这个是每日:8点刷新,持续一个小时的
    /// 测试时间:2022-11-10 00::10::00
    /// 测试时间:2022-11-10 00::10::00
    /// 测试时间:2022-11-10 00::10::00
    /// </summary>
    [Test]
    public void TestPeriodDataDayState2()
    {
        var resetData = CircleResetInfo.CreateDay(1, (uint)TimeSpan.FromHours(8).TotalSeconds, (int)TimeSpan.FromHours(1).TotalSeconds);
        var zoneDiff = TimeSpan.FromHours(8);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;

        TimeUtils.SetServerTimeStamp(totalSeconds, (int)zoneDiff.TotalSeconds);
        var dateTime_1 = new DateTime(2022, 11, 10, 00, 00, 00);//0
        var dateTime_2 = new DateTime(2022, 11, 10, 08, 59, 00);//1
        var dateTime_3 = new DateTime(2022, 11, 10, 09, 00, 00);//1
        var dateTime_4 = new DateTime(2022, 11, 10, 09, 10, 00);//2
        var dateTime_5 = new DateTime(2022, 11, 10, 09, 59, 00);
        var dateTime_6 = new DateTime(2022, 11, 10, 10, 01, 00);
        var dateTime_7 = new DateTime(2022, 11, 10, 11, 00, 00);


        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_1 - currentDateTime).TotalSeconds), 0);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_2 - currentDateTime).TotalSeconds), 1);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_3 - currentDateTime).TotalSeconds), 1);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_4 - currentDateTime).TotalSeconds), 2);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_5 - currentDateTime).TotalSeconds), 2);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_6 - currentDateTime).TotalSeconds), 2);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_7 - currentDateTime).TotalSeconds), 2);
    }

    /// <summary>
    /// 每天刷新,8点开始持续一个小时
    /// </summary>
    [Test]
    public void TesDayInterval2_8To9_interval0()
    {
        var resetData = CircleResetInfo.CreateDay(1, (uint)TimeSpan.FromHours(8).TotalSeconds, (int)TimeSpan.FromHours(1).TotalSeconds);
        var zoneDiff = TimeSpan.FromHours(0);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;
        TimeUtils.SetServerTimeStamp(totalSeconds, (int)zoneDiff.TotalSeconds);


        var dateTime_0 = new DateTime(2022, 11, 10, 00, 00, 01);//0
        var dateTime_1 = new DateTime(2022, 11, 10, 07, 59, 59);//0
        var dateTime_2 = new DateTime(2022, 11, 10, 08, 00, 00);//1
        var dateTime_3 = new DateTime(2022, 11, 10, 09, 00, 00);//2
        var dateTime_4 = new DateTime(2022, 11, 10, 09, 00, 01);//2
        var dateTime_5 = new DateTime(2022, 11, 10, 09, 59, 00);
        var dateTime_6 = new DateTime(2022, 11, 10, 10, 01, 00);
        var dateTime_7 = new DateTime(2022, 11, 10, 11, 00, 00);

        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_0 - currentDateTime).TotalSeconds, 0), 0);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_1 - currentDateTime).TotalSeconds, 0), 0);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_2 - currentDateTime).TotalSeconds, 0), 1);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_3 - currentDateTime).TotalSeconds, 0), 2);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_4 - currentDateTime).TotalSeconds, 0), 2);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_5 - currentDateTime).TotalSeconds, 0), 2);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_6 - currentDateTime).TotalSeconds, 0), 2);
        Assert.AreEqual(resetData.GetState(TimeUtils.serverTimestamp + (long)(dateTime_7 - currentDateTime).TotalSeconds, 0), 2);
    }


    /// <summary>
    /// 每2天刷新一次,8点开始持续一个小时
    /// </summary>
    [Test]
    public void TesDayInterval2_8To9_interval1()
    {
        var resetData = CircleResetInfo.CreateDay(1, (uint)TimeSpan.FromHours(8).TotalSeconds, (int)TimeSpan.FromHours(1).TotalSeconds, 2);
        var zoneDiff = TimeSpan.FromHours(0);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;
        TimeUtils.SetServerTimeStamp(totalSeconds, (int)zoneDiff.TotalSeconds);

        var timeDataResultDict = new System.Collections.Generic.List<(DateTime, uint)>()
        {
            (new DateTime(2022, 11, 11, 00, 00, 00),0),
            (new DateTime(2022, 11, 11, 08, 01, 00),1),
            (new DateTime(2022, 11, 11, 08, 59, 00),1),
            (new DateTime(2022, 11, 11, 09, 00, 00),1),
            (new DateTime(2022, 11, 11, 09, 10, 00),2),
            (new DateTime(2022, 11, 11, 09, 59, 00),2),
            (new DateTime(2022, 11, 11, 10, 01, 00),2),
            (new DateTime(2022, 11, 11, 11, 00, 00),2)
        };

        var currentServerTS = (uint)TimeUtils.serverTimestamp - (uint)TimeSpan.FromDays(1).TotalSeconds;

        for (int i = 0; i < timeDataResultDict.Count; i++)
        {
            var item = timeDataResultDict[i];

            Assert.AreEqual(item.Item2, resetData.GetState(TimeUtils.serverTimestamp + (long)(item.Item1 - currentDateTime).TotalSeconds, currentServerTS), $"$check index:{i}:{item.Item1}:Result:{item.Item2}");
        }
    }

    //1周一次
    [Test]
    public void TesWeekInterval1_h8_h9()
    {
        //1周一次,星期8点刷新,持续1小时.
        var resetData = CircleResetInfo.CreateWeek(1, (uint)TimeSpan.FromHours(8).TotalSeconds, (int)TimeSpan.FromHours(1).TotalSeconds, 2, 1);

        var zoneDiff = TimeSpan.FromHours(8);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        UnityEngine.Debug.Log($"CurrentDay::{currentDateTime}::days:{currentDateTime.DayOfWeek}");//Thursday
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;
        TimeUtils.SetServerTimeStamp(totalSeconds, (int)zoneDiff.TotalSeconds);

        var timeDataResultDict = new System.Collections.Generic.List<(DateTime, uint)>()
        {
            (new DateTime(2022, 11, 7, 00, 00, 00),0),//星期一
            (new DateTime(2022, 11, 8, 00, 00, 00),0),//星期二
            (new DateTime(2022, 11, 8, 08, 00, 01),1),//星期二
            (new DateTime(2022, 11, 8, 08, 59, 00),1),//星期二
            (new DateTime(2022, 11, 8, 09, 01, 00),2),//星期二
            (new DateTime(2022, 11, 11, 08, 01, 00),2),//Friday
            (new DateTime(2022, 11, 12, 10, 01, 00),2),//星期6
            (new DateTime(2022, 11, 13, 10, 01, 00),2),//星期天
            (new DateTime(2022, 11, 14, 11, 00, 00),0)//下个星期1
        };

        var currentServerTS = (uint)TimeUtils.serverTimestamp - (uint)TimeSpan.FromDays(1).TotalSeconds;

        for (int i = 0; i < timeDataResultDict.Count; i++)
        {
            var item = timeDataResultDict[i];
            UnityEngine.Debug.Log($"=========ItemDay:::{item.Item1}::days:{item.Item1.DayOfWeek}");
            Assert.AreEqual(item.Item2, resetData.GetState(TimeUtils.serverTimestamp + (long)(item.Item1 - currentDateTime).TotalSeconds, currentServerTS), $"$check:{item.Item1}:Result:{item.Item2}::At:{i}");
        }

    }

    //1周一次
    [Test]
    public void TesWeekInterval1_h8_end()
    {
        //1周一次,星期8点刷新,持续1小时.
        var resetData = CircleResetInfo.CreateWeek(1, (uint)TimeSpan.FromHours(8).TotalSeconds, 0, 2, 1);

        var zoneDiff = TimeSpan.FromHours(8);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        UnityEngine.Debug.Log($"CurrentDay::{currentDateTime}::days:{currentDateTime.DayOfWeek}");//Thursday
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;
        TimeUtils.SetServerTimeStamp(totalSeconds, (int)zoneDiff.TotalSeconds);
        //没有2状态.
        var timeDataResultDict = new System.Collections.Generic.List<(DateTime, uint)>()
        {
            (new DateTime(2022, 11, 7, 00, 00, 00),0),//星期一
            (new DateTime(2022, 11, 8, 00, 00, 00),0),//星期二
            (new DateTime(2022, 11, 8, 08, 00, 01),1),//星期二
            (new DateTime(2022, 11, 8, 08, 59, 00),1),//星期二
            (new DateTime(2022, 11, 8, 09, 01, 00),1),//星期二
            (new DateTime(2022, 11, 11, 08, 01, 00),1),//Friday
            (new DateTime(2022, 11, 12, 10, 01, 00),1),//星期6
            (new DateTime(2022, 11, 13, 10, 01, 00),1),//星期天
            (new DateTime(2022, 11, 14, 11, 00, 00),0)//下个星期1
        };

        var currentServerTS = (uint)TimeUtils.serverTimestamp - (uint)TimeSpan.FromDays(1).TotalSeconds;

        for (int i = 0; i < timeDataResultDict.Count; i++)
        {
            var item = timeDataResultDict[i];
            UnityEngine.Debug.Log($"ItemDay:::{item.Item1}::days:{item.Item1.DayOfWeek}");
            Assert.AreEqual(item.Item2, resetData.GetState(TimeUtils.serverTimestamp + (long)(item.Item1 - currentDateTime).TotalSeconds, currentServerTS), $"$check:{item.Item1}:Result:{item.Item2}::At:{i}");
        }

    }

    // //两周一次,星期2,8点到9点
    [Test]
    public void TesWeekInterval2_8h_9h()
    {
        //两周一次,星期2,8点到9点
        var resetData = CircleResetInfo.CreateWeek(1, (uint)TimeSpan.FromHours(8).TotalSeconds, (int)TimeSpan.FromHours(1).TotalSeconds, 2, 2);
        var zoneDiff = TimeSpan.FromHours(8);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        UnityEngine.Debug.Log($"CurrentDay::{currentDateTime}::days:{currentDateTime.DayOfWeek}");//Thursday
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;
        TimeUtils.SetServerTimeStamp(totalSeconds, (int)zoneDiff.TotalSeconds);

        //没有2状态.
        var timeDataResultDict = new System.Collections.Generic.List<(DateTime, uint)>()
        {
            (new DateTime(2022, 11, 7, 00, 00, 00),0),//星期一
            (new DateTime(2022, 11, 8, 00, 00, 00),0),//星期二
            (new DateTime(2022, 11, 8, 08, 00, 01),1),//星期二
            (new DateTime(2022, 11, 8, 08, 59, 00),1),//星期二
            (new DateTime(2022, 11, 8, 09, 01, 00),2),//星期二
            (new DateTime(2022, 11, 11, 08, 01, 00),2),//Friday
            (new DateTime(2022, 11, 12, 10, 01, 00),2),//星期6
            (new DateTime(2022, 11, 13, 10, 01, 00),2),//星期天
            (new DateTime(2022, 11, 14, 11, 00, 00),2),//下个星期1
            (new DateTime(2022, 11, 15, 11, 00, 00),2),//下个星期2
            (new DateTime(2022, 11, 16, 11, 00, 00),2),//下个星期3
            (new DateTime(2022, 11, 17, 11, 00, 00),2),//下个星期4
            (new DateTime(2022, 11, 18, 11, 00, 00),2),//下个星期5
            (new DateTime(2022, 11, 19, 11, 00, 00),2),//下个星期6
            (new DateTime(2022, 11, 20, 11, 00, 00),2),//下个星期7
            (new DateTime(2022, 11, 21, 11, 00, 00),2)//下下个星期1,应该是下个周期的未开始状态,这个解释有点模糊.
        };

        var currentServerTS = (uint)TimeUtils.serverTimestamp;

        for (int i = 0; i < timeDataResultDict.Count; i++)
        {
            var item = timeDataResultDict[i];
            UnityEngine.Debug.Log($"ItemDay:::{item.Item1}::days:{item.Item1.DayOfWeek}");
            Assert.AreEqual(item.Item2, resetData.GetState(TimeUtils.serverTimestamp + (long)(item.Item1 - currentDateTime).TotalSeconds, currentServerTS), $"$check:{item.Item1}:Result:{item.Item2}::At:{i}");
        }
    }

    // //两周一次,星期2,8点到结束
    [Test]
    public void TesWeekInterval2_8h_0h()
    {
        //两周一次,星期2,8点到9点
        var resetData = CircleResetInfo.CreateWeek(1, (uint)TimeSpan.FromHours(8).TotalSeconds, 0, 2, 2);
        var zoneDiff = TimeSpan.FromHours(8);
        var currentDateTime = new DateTime(2022, 11, 10, 0, 10, 0);
        UnityEngine.Debug.Log($"CurrentDay::{currentDateTime}::days:{currentDateTime.DayOfWeek}");//Thursday
        var totalSeconds = (long)(currentDateTime - TimeUtils.Get1970()).TotalSeconds;
        TimeUtils.SetServerTimeStamp(totalSeconds, (int)zoneDiff.TotalSeconds);

        //没有2状态.
        var timeDataResultDict = new System.Collections.Generic.List<(DateTime, uint)>()
        {
            (new DateTime(2022, 11, 7, 00, 00, 00),0),//星期一
            (new DateTime(2022, 11, 8, 00, 00, 00),0),//星期二
            (new DateTime(2022, 11, 8, 08, 00, 01),1),//星期二
            (new DateTime(2022, 11, 8, 08, 59, 00),1),//星期二
            (new DateTime(2022, 11, 8, 09, 01, 00),1),//星期二
            (new DateTime(2022, 11, 11, 08, 01, 00),1),//Friday
            (new DateTime(2022, 11, 12, 10, 01, 00),1),//星期6
            (new DateTime(2022, 11, 13, 10, 01, 00),1),//星期天
            (new DateTime(2022, 11, 14, 11, 00, 00),1),//下个星期1
            (new DateTime(2022, 11, 15, 11, 00, 00),1),//下个星期2
            (new DateTime(2022, 11, 16, 11, 00, 00),1),//下个星期3
            (new DateTime(2022, 11, 17, 11, 00, 00),1),//下个星期4
            (new DateTime(2022, 11, 18, 11, 00, 00),1),//下个星期5
            (new DateTime(2022, 11, 19, 11, 00, 00),1),//下个星期6
            (new DateTime(2022, 11, 20, 11, 00, 00),1),//下个星期7
            (new DateTime(2022, 11, 21, 11, 00, 00),2)//下下个星期1,应该是下个周期的未开始状态,这个解释有点模糊.
        };

        var currentServerTS = (uint)TimeUtils.serverTimestamp;

        for (int i = 0; i < timeDataResultDict.Count; i++)
        {
            var item = timeDataResultDict[i];
            UnityEngine.Debug.Log($"ItemDay:::{item.Item1}::days:{item.Item1.DayOfWeek}");
            Assert.AreEqual(item.Item2, resetData.GetState(TimeUtils.serverTimestamp + (long)(item.Item1 - currentDateTime).TotalSeconds, currentServerTS), $"$check:{item.Item1}:Result:{item.Item2}::At:{i}");
        }
    }
}