using System;
using System.Collections.Generic;

public static class ArrayUtils
{
    private static Dictionary<(Type, string), object> dict = new Dictionary<(Type, string), object>(128);

    #region 数组查找
    public static DATA Find<DATA, KEY>(DATA[] data, string property, KEY k)
        where DATA: new()
        where KEY:IEquatable<KEY>
    {
        if (data == null || data.Length == 0)
            return default(DATA);
        var tmpT = typeof(DATA);
        Func<DATA, KEY> call = GetPropertyGet<DATA, KEY>(property);
        if (call==null)
        {
            throw new Exception("No Property Find:");
        }

        var target = default(DATA);
        var nLen = data.Length;
        for (int i = 0; i < nLen; i++)
        {
            if (call.Invoke(data[i]).Equals(k))
            {
                target = data[i];
                break;
            }
        }
        return target;
    }
   
    #region 数组查找int 和或者string
    public static DATA Find<DATA>(DATA[] data, string property, int k)
        where DATA :class, new()
    {
        if (data == null || data.Length == 0)
            return default(DATA);
        var tmpT = typeof(DATA);
        Func<DATA, int> call = GetPropertyGet<DATA, int>(property);
        if (call == null)
        {
            throw new Exception("No Property Find:");
        }

        var target = default(DATA);
        var nLen = data.Length;
        for (int i = 0; i < nLen; i++)
        {
            if (call.Invoke(data[i])==k)
            {
                target = data[i];
                break;
            }
        }
        return target;
    }

    public static DATA Find<DATA>(DATA[] data, string property, string k)
    where DATA : class, new()
    {
        if (data == null || data.Length == 0)
            return default(DATA);
        var tmpT = typeof(DATA);
        var call = GetPropertyGet<DATA, string>(property);
        if (call == null)
        {
            throw new Exception("No Property Find:");
        }

        var target = default(DATA);
        var nLen = data.Length;
        for (int i = 0; i < nLen; i++)
        {
            if (call.Invoke(data[i]) == k)
            {
                target = data[i];
                break;
            }
        }
        return target;
    }
    #endregion

    #endregion

    #region List查找
    public static DATA Find<DATA,KEY>(List<DATA> data, string property, KEY k)
        where DATA :class, new()
        where KEY : IEquatable<KEY>
    {
        if (data == null || data.Count == 0)
            return default(DATA);
        var tmpT = typeof(DATA);
        Func<DATA, KEY> call = GetPropertyGet<DATA, KEY>(property);

        if (call == null)
        {
            throw new Exception("No Property Find:");
        }

        var target = default(DATA);
        var nLen = data.Count;
        for (int i = 0; i < nLen; i++)
        {
            if (call.Invoke(data[i]).Equals(k))
            {
                target = data[i];
                break;
            }
        }
        return target;
    }

    #region List查找Int或者string
    public static DATA Find<DATA>(List<DATA> data, string property, int k)
       where DATA :class, new()
    {
        if (data == null || data.Count == 0)
            return default(DATA);
        var tmpT = typeof(DATA);
        Func<DATA, int> call = GetPropertyGet<DATA, int>(property);

        if (call == null)
        {
            throw new Exception("No Property Find:");
        }

        var target = default(DATA);
        var nLen = data.Count;
        for (int i = 0; i < nLen; i++)
        {
            if (call.Invoke(data[i]) == (k))
            {
                target = data[i];
                break;
            }
        }
      
        return target;
    }

    public static DATA Find<DATA>(List<DATA> data, string property, string k)
      where DATA :class, new()
    {
        if (data == null || data.Count == 0)
            return default(DATA);
        var tmpT = typeof(DATA);
        Func<DATA, string> call = GetPropertyGet<DATA, string>(property);
        if (call == null)
        {
            throw new Exception("No Property Find:");
        }

        var target = default(DATA);

        var nLen = data.Count;
        for (int i = 0; i < nLen; i++)
        {
            if (call.Invoke(data[i]) == (k))
            {
                target = data[i];
                break; 
            }
        }
        return target;
    }
    #endregion
    #endregion

    #region  创建属性委托
    private static Func<DATA,KEY> GetPropertyGet<DATA,KEY>(string property)
    {
        Func<DATA, KEY> call;
        var tmpT = typeof(DATA);
        var tmpKey = (tmpT, property);
        if (dict.ContainsKey(tmpKey))
        {
            call = dict[tmpKey] as Func<DATA, KEY>;
        }
        else
        {
            //UnityEngine.Profiling.Profiler.BeginSample("GetPropertyGet_delegate");
            var propertyInfo = tmpT.GetProperty(property, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            call = Delegate.CreateDelegate(typeof(Func<DATA, KEY>), propertyInfo.GetGetMethod()) as Func<DATA, KEY>;
            dict[tmpKey] = call;
           // UnityEngine.Profiling.Profiler.EndSample();
        }
        return call;
    }
    #endregion
    
    public static void CreateOrCopy<T>(ref T[] source,T[] from)
    {
        if (from == null || from.Length == 0)
        {
            source = Array.Empty<T>();
        }
        else
        {
            int finalLength = from.Length;
            CreateOrResize<T>(ref source, finalLength);
            Array.Copy(from, source, finalLength);
        }
    }

    public static void CreateOrResize<T>(ref T[] source, int finalLength)
    {
        if (finalLength == 0)
        {
            source = Array.Empty<T>();
        }
        else
        {
            if (source == null)
            {
                source = new T[finalLength];
            }else if ( source.Length != finalLength)
            {
                Array.Resize<T>(ref source, finalLength);
            }
        }
    }

    public static void Fill<T>(T[] source, T value)
    {
        if(source == null || source.Length == 0)
            return;
        for (int i = 0; i < source.Length; i++)
        {
            source[i] = value;
        }
    }

    public static T SafeGet<T>(T[] source,int index,int defaultIndex = 0 )
    {
        if (source == null || source.Length == 0)
        {
            return default(T);
        }
        else
        {
           return index<source.Length?source[index]:source[defaultIndex];
        }
    }

    public static void Randomize(int[] arr, int n)
    {
        // Start from the last element and 
        // swap one by one. We don't need to run for the first element  
        // that's why i > 0 
        for (int i = n - 1; i > 0; i--)
        {
            // Pick a random index 
            // from 0 to i 
            int j = UnityEngine.Random.Range(0, i + 1);

            // Swap arr[i] with the  element at random index 
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
    }
}
