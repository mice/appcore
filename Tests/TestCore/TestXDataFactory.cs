using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.TestTools;

public class TestXDataFactory
{
    [Test]
    public void TestDataFactoryGet()
    {
        var resetData = DataFactory.Get<StringBuilder>();
        Assert.NotNull(resetData);
    }

    [Test]
    public void TestDataFactorySetImpl()
    {
        DataFactory.SetDataFactory(null);
        var resetData = DataFactory.Get<StringBuilder>();
        Assert.NotNull(resetData);
    }

    [Test]
    public void TestDataFactoryRelease()
    {
        var resetData = DataFactory.Get<StringBuilder>();
        Assert.NotNull(resetData);
        DataFactory.Release(resetData);
        Assert.NotNull(resetData);
    }
}
