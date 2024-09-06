﻿using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// 网上下载的一个多线程插件
/// </summary>
public class Loom : MonoBehaviour
{
    public static int maxThreads = 8;
    static int numThreads;

    private static Loom _current;
    //private int _count;
    public static Loom Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    static bool initialized;

    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
            UnityEngine.Object.DontDestroyOnLoad(g);
        }

    }
    public struct NoDelayedQueueItem
    {
        public Action<object> action;
        public object param;
    }

    private List<NoDelayedQueueItem> _actions = new List<NoDelayedQueueItem>();
    public struct DelayedQueueItem
    {
        public float time;
        public Action<object> action;
        public object param;
    }
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    public static void QueueOnMainThread(Action<object> taction, object tparam)
    {
        QueueOnMainThread(taction, tparam, 0f);
    }
    public static void QueueOnMainThread(Action<object> taction, object tparam, float time)
    {
        if(Current==null)
        {
            return;
        }
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = taction, param = tparam });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(new NoDelayedQueueItem { action = taction, param = tparam });
            }
        }
    }

    public static Thread RunAsync(Action a)
    {
        Initialize();
        //while (numThreads >= maxThreads)
        //{
        //    Thread.Sleep(100);
        //}
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }

    }


    void OnDisable()
    {
        if (_current == this)
        {

            _current = null;
        }
    }

    List<NoDelayedQueueItem> _currentActions = new List<NoDelayedQueueItem>();

    // Update is called once per frame
    void Update()
    {
        if (_actions.Count > 0)
        {
            lock (_actions)
            {
                _currentActions.Clear();
                for (int i = 0; i < _actions.Count; i++)
                {
                    _currentActions.Add(_actions[i]);
                }
                _actions.Clear();
            }
            for (int i = 0; i < _currentActions.Count; i++)
            {
                _currentActions[i].action(_currentActions[i].param);
            }
        }

        if (_delayed.Count > 0)
        {
            lock (_delayed)
            {
                _currentDelayed.Clear();
                var t = Time.time;                
                for (int i = 0; i < _delayed.Count; i++)
                {
                    var act = _delayed[i];
                    if( act.time<=t )
                    {
                        _currentDelayed.Add(_delayed[i]);
                        _delayed.RemoveAt(i);
                        i--;
                    }                    
                }
            }

            for (int i = 0; i < _currentDelayed.Count; i++)
            {
                _currentDelayed[i].action(_currentDelayed[i].param);
            }
        }
    }
}