using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class AnimationUtils
    {
        ////默认为1;
        public static float GetDuration(AnimationCurve curve)
        {
            if(curve == null || curve.length==0)
                return 1;
            var duration = curve[0].time;
            for (int i = 1; i < curve.length; i++)
            {
                duration = Mathf.Max(duration,curve[i].time);
            }
            return duration;
        }

        public static (float,float) GetMinMaxValue(AnimationCurve curve,float defaultMin = .1f,float defaultMax = 1.0f)
        {
            if(curve == null || curve.length==0)
                return (defaultMin,defaultMax);
            var val = curve[0].value;
            var min = val;
            var max= val;
            for (int i = 1; i < curve.length; i++)
            {
                var tmpValue = curve[i].value;
                min = Mathf.Min(min,tmpValue);
                max = Mathf.Max(max,tmpValue);
            }
            return (min,max);
        }

        public static bool IsNullOrEmpty(AnimationCurve curve)
        {
            return  curve == null || curve.length==0;
        }
    }

}