using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    const int MAX_POOL = 20;
    public bool isRunning;
    private static int _id = 0;
    private static Queue<CoroutineRunner> _runnerPool = new Queue<CoroutineRunner>();
    private IEnumerator AutoStop(IEnumerator routine)
    {
        yield return routine;
        Stop(this);
    }

    /// <summary>
    /// autoStop=false，就需要自己调用Stop方法.
    /// autoStop=true，不要保留返回的对象操作停止，不然会出问题
    /// </summary>
    /// <param name="routine"></param>
    /// <param name="autoStop"></param>
    /// <returns></returns>
    public static CoroutineRunner Start(IEnumerator routine, bool autoStop = true)
    {
        CoroutineRunner runner = _SpawnRunner();
        if (autoStop)
        {
            runner.StartCoroutine(runner.AutoStop(routine));
        }
        else
        {
            runner.StartCoroutine(routine);
        }
        runner.isRunning = true;
        return runner;
    }

    private static CoroutineRunner _SpawnRunner()
    {
        CoroutineRunner runner = null;
        while (_runnerPool.Count > 0)
        {
            runner = _runnerPool.Dequeue();
            if (runner != null)
            {
                break;
            }
        }
        if (runner == null)
        {
            GameObject go = new GameObject($"CortoutineRunner_{_id}");
            _id++;
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideInHierarchy;
            runner = go.AddComponent<CoroutineRunner>();
        }

        return runner;
    }

    public static void Stop(CoroutineRunner runner)
    {
        if (runner != null && runner.isRunning)
        {
            runner.StopAllCoroutines();
            runner.isRunning = false;
            if (_runnerPool.Count > MAX_POOL)
            {
                Object.Destroy(runner.gameObject);
            }
            else
            {
                _runnerPool.Enqueue(runner);
            }
        }
    }

    public static void Clear()
    {
        while (_runnerPool.Count > MAX_POOL)
        {
            var runner = _runnerPool.Dequeue();
            if (runner != null)
            {
                Object.Destroy(runner.gameObject);
            }
        }
    }
}