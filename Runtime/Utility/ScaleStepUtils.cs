using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class ScaleStepUtils
    {
        public static float[] focLength = new float[] {
        80,76,72,70
    };

        /// <summary> 从小到大</summary>
        public static float[] scales = new float[] {
        0.8f,0.9f,1.0f,1.2f
    };

        public static Vector3 GetStepValue(float[] steps, float source, Vector3[] values)
        {
            //Debug.Assert(steps.Length == values.Length, steps.Length + "," + values.Length);

            (int index, float ratio) = GetStep(steps, source);
            if (ratio == 0)
            {
                return values[index];
            }
            else
            {
                return Vector3.Lerp(values[index], values[index + 1], ratio);
            }
        }

        public static Vector3 GetStepVec3(float[] steps, float source, Vector3[] values)
        {
            (int index, float ratio) = GetStep(steps, source);
            if (ratio == 0)
            {
                return values[index];
            }
            else
            {
                return Vector3.Lerp(values[index], values[index + 1], ratio);
            }
        }

        public static float GetStepValue(float[] steps, float source, float[] values)
        {
            //Debug.Assert(steps.Length == values.Length, steps.Length + "," + values.Length);

            (int index, float ratio) = GetStep(steps, source);
            if (ratio == 0)
            {
                return values[index];
            }
            else
            {
                return Mathf.Lerp(values[index], values[index + 1], ratio);
            }
        }

        public static (int, float) GetStep(float[] steps, float tmpStep)
        {
            if (steps.Length <= 2)
                throw new System.Exception("最少2个数据");
            if (steps[0] < steps[steps.Length - 1])
            {
                return GetUpStep(steps, tmpStep);
            }
            return GetDownStep(steps, tmpStep);
        }

        public static (int, float) GetUpStep(float[] steps, float tmpStep)
        {
            if (steps[0] >= tmpStep)
            {
                return (0, 0);
            }
            else if (steps[steps.Length - 1] <= tmpStep)
            {
                return (steps.Length - 1, 0);
            }

            for (int i = 0; i < steps.Length - 1; i++)
            {
                if (steps[i] <= tmpStep && tmpStep < steps[i + 1])
                {
                    var tmpRatio = (tmpStep - steps[i]) / (steps[i + 1] - steps[i]);
                    return (i, tmpRatio);
                }
            }
            return (0, 0);
        }

        public static (int, float) GetDownStep(float[] steps, float tmpStep)
        {
            if (steps[0] <= tmpStep)
            {
                return (0, 0);
            }
            else if (steps[steps.Length - 1] >= tmpStep)
            {
                return (steps.Length - 1, 0);
            }

            for (int i = 0; i < steps.Length - 1; i++)
            {
                if (tmpStep <= steps[i] && steps[i + 1] < tmpStep)
                {
                    var tmpRatio = (tmpStep - steps[i]) / (steps[i + 1] - steps[i]);
                    return (i, tmpRatio);
                }
            }
            return (0, 0);
        }

        private static float[] tmp_scalefact = new float[3];
        private static float[] tmp_scalereal = new float[3];
        public static float GetScale(Vector3[] scaleRange, Vector3[] scaleFact, int[] stepRange, int step, float ratio)
        {
            var tmpScale = 1.0f;
            if (scaleFact == null || scaleFact.Length == 0)
                return tmpScale;

            var tIdx = System.Array.IndexOf(stepRange, step);
            if (tIdx == -1)
            {
                return 1;
            }

            var scalefact = tIdx < scaleFact.Length ? scaleFact[tIdx] : scaleFact[scaleFact.Length - 1];
            var scalereal = tIdx < scaleRange.Length ? scaleRange[tIdx] : scaleRange[scaleRange.Length - 1];
            tmp_scalefact[0] = scalefact[0]; tmp_scalefact[1] = scalefact[1]; tmp_scalefact[2] = scalefact[2];
            tmp_scalereal[0] = scalereal[0]; tmp_scalereal[1] = scalereal[1]; tmp_scalereal[2] = scalereal[2];

            tmpScale = ScaleStepUtils.GetStepValue(tmp_scalefact, ratio, tmp_scalereal);

            return tmpScale;
        }
    }
}