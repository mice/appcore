using System.Collections.Generic;
using System.Text;

public static class ListExtensions
{
    public static void Swap<T>(this List<T> array,int indexA,int indexB)
    {
        var tmpT = array[indexA];
        array[indexA] = array[indexB];
        array[indexB] = tmpT;
    }

    public static void Ensure<T>(this List<T> array, int index, T defaultValue)
    {
        while (array.Count <= index)
        {
            array.Add(defaultValue);
        }
    }

    public static void MakeSure<T>(this List<T> list,int count)
    {
        if (list.Count > count)
        {
            return;
        }

        for (int i = 0; i < count - list.Count; i++)
        {
            list.Add(default(T));
        }
    }
    
    public static bool ContainsAll<T>(this List<T> array, List<T> val)
    {
        if (array == null || val == null)
        {
            return false;
        }
       
        var hashSet = new HashSet<T>(array);
        for (int i = 0; i < val.Count; i++)
        {
            if (!hashSet.Contains(val[i]))
            {
                return false;
            }
        }
        return true;
    }


    public static bool Contains(this char[] array, char val)
    {
        if (array == null)
        {
            return false;
        }
        var len = array.Length;
        for (int i = 0; i < len; i++)
        {
            if (array[i]==val) return true;
        }
        return false;
    }

    

    public static bool UniqAdd<T>(this List<T> array, T val)
    {
        if (!array.Contains(val))
        {
            array.Add(val);
            return true;
        }
        return false;
    }

    public static void UniqAddRange<T>(this List<T> array, List<T> val)
    {
        for (int i = 0; i < val.Count; i++)
        {
            T item = val[i];
            if (!array.Contains(item))
            {
                array.Add(item);
            }
        }
    }

    public static void UniqAddRange<T>(this List<T> array, T[] val)
    {
        for (int i = 0; i < val.Length; i++)
        {
            T item = val[i];
            if (!array.Contains(item))
            {
                array.Add(item);
            }
        }
    }
    public static int IndexOf<T>(this List<T> array, T val, System.Func<T, T, bool> lf)
    {
        for (int i = 0; i < array.Count; i++)
        {
            T item = array[i];
            if (lf(item,val))
            {
                return i;
            }
        }
        return -1;
    }

    public static int IndexOf<T>(this List<T> array, System.Func<T, bool> lf)
    {
        for (int i = 0; i < array.Count; i++)
        {
            T item = array[i];
            if (lf(item))
            {
                return i;
            }
        }
        return -1;
    }

    public static bool HasIntersect<T>(List<T> g1,List<T> g2)
    {
        if (g1.Count == 0 || g2.Count == 2)
        {
            return false;
        }
        bool hasDup = false;
        var hashSet = new HashSet<T>(g1);
        for (int i = 0; i < g2.Count; i++)
        {
            if(hashSet.Contains(g2[i]))
            {
                hasDup = true;
                break;
            }
        }

        return hasDup;
    }

    public static U Reduce<T, U>(this List<T> array, U initValue, System.Func<U, T, U> callBack)
    {
        var sumValue = initValue;
        for (int i = 0; i < array.Count; i++)
        {
            sumValue = callBack(sumValue, array[i]);
        }
        return sumValue;
    }

    public static void SortList<T>(List<T> arr, List<float> sortValues)
    {
        //无法使用index,交换会到导致混乱.
        var tmpArray = DataFactory.GetList<KeyValuePair<float, T>>();
        for (int i = 0; i < sortValues.Count; i++)
        {
            tmpArray.Add(new KeyValuePair<float, T>(sortValues[i], arr[i]));
        }

        tmpArray.Sort((item1, item2) => item2.Key.CompareTo(item1.Key));
        for (int i = 0; i < tmpArray.Count; i++)
        {
            arr[i] = tmpArray[i].Value;
        }
        DataFactory.Release(tmpArray);
    }

    public static void SortList<T>(List<T> arr, List<long> sortValues)
    {
        //无法使用index,交换会到导致混乱.
        var tmpArray = DataFactory.GetList<KeyValuePair<long, T>>();
        for (int i = 0; i < sortValues.Count; i++)
        {
            tmpArray.Add(new KeyValuePair<long, T>(sortValues[i], arr[i]));
        }

        tmpArray.Sort((item1, item2) => item2.Key.CompareTo(item1.Key));
        for (int i = 0; i < tmpArray.Count; i++)
        {
            arr[i] = tmpArray[i].Value;
        }
        DataFactory.Release(tmpArray);
    }

    public static void SortList<T>(List<T> arr, List<int> sortValues)
    {
        //无法使用index,交换会到导致混乱.
        var tmpArray = DataFactory.GetList<KeyValuePair<int, T>>();
        for (int i = 0; i < sortValues.Count; i++)
        {
            tmpArray.Add(new KeyValuePair<int, T>(sortValues[i], arr[i]));
        }

        tmpArray.Sort((item1, item2) => item2.Key.CompareTo(item1.Key));
        for (int i = 0; i < tmpArray.Count; i++)
        {
            arr[i] = tmpArray[i].Value;
        }
        DataFactory.Release(tmpArray);
    }

    public static void SortList2<T>(List<T> arr, List<(int, int)> sortValues)
    {
        //无法使用index,交换会到导致混乱.
        var tmpArray = DataFactory.GetList<KeyValuePair<(int, int), T>>();
        for (int i = 0; i < sortValues.Count; i++)
        {
            tmpArray.Add(new KeyValuePair<(int, int), T>(sortValues[i], arr[i]));
        }

        tmpArray.Sort((item1, item2) => item2.Key.CompareTo(item1.Key));
        for (int i = 0; i < tmpArray.Count; i++)
        {
            arr[i] = tmpArray[i].Value;
        }
        DataFactory.Release(tmpArray);
    }


    public static void SortList3<T>(List<T> arr, List<(int,int,int)> sortValues)
    {
        //无法使用index,交换会到导致混乱.
        var tmpArray = DataFactory.GetList<KeyValuePair<(int, int, int), T>>();
        for (int i = 0; i < sortValues.Count; i++)
        {
            tmpArray.Add(new KeyValuePair<(int, int, int), T>(sortValues[i], arr[i]));
        }

        tmpArray.Sort((item1, item2) => item2.Key.CompareTo(item1.Key));
        for (int i = 0; i < tmpArray.Count; i++)
        {
            arr[i] = tmpArray[i].Value;
        }
        DataFactory.Release(tmpArray);
    }

    private static System.Threading.ThreadLocal<StringBuilder> _threadStringBuilder = new System.Threading.ThreadLocal<StringBuilder>(()=>new StringBuilder());
    public static string ToJoinString<T>(this IList<T> list, string split = ",", string format = null)
    {
        var sb = _threadStringBuilder;
        sb.Value.Clear();
        int i = 0;
        int count = list.Count;
        while (i < count)
        {
            _ToJoinString<T>(sb.Value, list[i], split, format, i == count - 1);
            i++;
        }
        var str = sb.Value.ToString();
        return str;
    }

    
    private static void _ToJoinString<T>(StringBuilder sb, T element, string split, string format, bool last)
    {
        if (format != null && element is int)
        {
            sb.Append(((int)((object)element)).ToString(format));
        }
        else if (format != null && element is float)
        {
            sb.Append(((float)((object)element)).ToString(format));
        }
        else
        {
            sb.Append(element.ToString());
        }
        if (!last)
        {
            sb.Append(split);
        }
    }
    public static bool IsNullOrEmpty<T>(this IList<T> dataList)
    {
        return dataList == null || dataList.Count == 0;
    }

    public static bool IsNullOrEmpty<T>(this T[] dataList)
    {
        return dataList == null || dataList.Length == 0;
    }


    public static bool IsNullOrEmpty(this string dataList)
    {
        return dataList == null || dataList.Length == 0;
    }

    public static void RemoveNull<T>(this List<T> dataList) where T : class
    {
        if (dataList.IsNullOrEmpty())
            return;
        var len = dataList.Count;
        int j = 0;
        for (int i = 0; i < len; i++)
        {
            if (dataList[i] != null)
            {
                dataList[j++] = dataList[i];
            }
        }
        dataList.RemoveRange(j, len - j);
    }

    //<summary>左闭右开,upper不可以取值</summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static int BinarySearchEX<T,KEY>(this List<T> list,KEY key,System.Func<KEY,T,int> compare){
        if(list.Count==0){
            return -1;
        }
        int lower = 0;
        int upper = list.Count;
        while(lower<upper){
            var targetIndex = lower + (upper - lower)/2;
            var tmpInt = compare.Invoke(key,list[targetIndex]);
            if(tmpInt==0){
                return targetIndex;
            }else if(tmpInt<0){
                upper = targetIndex;
            }else if(tmpInt>0){
                lower = targetIndex+1;
            }
        }
        return -1;
    }

    public interface IConvertCompare<KEY,T>{
        int Invoke(KEY key,T target);
    }
}