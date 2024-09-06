using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.TestTools;
using Pool;

public class TestDataFactory
{
    [Test]
    public void TestError()
    {
        //Assert.Catch<MissingMethodException>(() =>
        //{
        //    var inst = System.Activator.CreateInstance<StringBuilder>();
        //});
        DataFactoryImpl.Init();
        LogAssert.Expect(LogType.Error, "Excludes Value Type");
        var vec3 = DataFactory.Get<Vector3>();
    }

    [Test]
    public void TestDataFactoryGet_NoValueType()
    {
        DataFactoryImpl.Init();
        var vec3 = DataFactory.Get<Vector3>();
        LogAssert.Expect(LogType.Error, "Excludes Value Type");
    }


    [Test]
    public void TestDataFactoryGet_NoMonoBehaviour()
    {
        DataFactoryImpl.Init();
        var vec3 = DataFactory.Get<Animator>();
        LogAssert.Expect(LogType.Error, "Excludes UnityEngine.Object");
    }

    [Test]
    public void TestDataFactoryGet_NoGameObject()
    {
        DataFactoryImpl.Init();
        var go = DataFactory.Get<GameObject>();
        LogAssert.Expect(LogType.Error, "Excludes UnityEngine.Object");
    }

    [Test]
    public void TestDataFactoryGet()
    {
        DataFactoryImpl.Init();
        var resetData = DataFactory.Get<StringBuilder>();
        Assert.NotNull(resetData);
    }

    [Test]
    public void TestDataFactoryDoubleRelease()
    {
        DataFactoryImpl.Init();
        var resetData = DataFactory.Get<StringBuilder>();
        DataFactory.Release(resetData);
        LogAssert.Expect(LogType.Error, $"Double release {resetData.GetType()}");

        DataFactory.Release(resetData);
    }

    [Test]
    public void TestDataFactoryDoubleRelease_release_get()
    {
        DataFactoryImpl.Init();
        DataFactory.Clear();
        var sb1 = DataFactory.Get<StringBuilder>();
        DataFactory.Release(sb1);
        var sb2 = DataFactory.Get<StringBuilder>();
        Assert.NotNull(sb2);
        Assert.AreEqual(sb1, sb2);
        DataFactory.Release(sb1);
    }

    [Test]
    public void TestDataFactoryDoubleRelease_release_clear()
    {
        DataFactoryImpl.Init();
        DataFactory.Clear();
        var sb1 = DataFactory.Get<StringBuilder>();
        DataFactory.Release(sb1);
        var sb2 = DataFactory.Get<StringBuilder>();
        Assert.NotNull(sb2);
        Assert.AreEqual(sb1, sb2);
        DataFactory.Clear();
        DataFactory.Release(sb1);
    }

    [Test]
    public void TestForceClear()
    {
        DataFactoryImpl.Init();
        DataFactory.Clear();
        var sb1 = DataFactory.Get<StringBuilder>();
        DataFactory.Release(sb1);
        var sb2 = DataFactory.Get<StringBuilder>();
        Assert.NotNull(sb2);
        Assert.AreEqual(sb1, sb2);
        DataFactory.ForceClear<StringBuilder>();
        sb2 = DataFactory.Get<StringBuilder>();
        Assert.AreNotEqual(sb1, sb2);
    }
}
