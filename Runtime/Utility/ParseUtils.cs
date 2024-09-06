using UnityEngine;

namespace Utils
{
    public static class ParseUtils
    {
        /// <summary>
        /// #ffffff这样的方式;
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Color ParseColor(string content)
        {
            if (!content.StartsWith("#"))
                content = "#" + content;
            return ColorUtility.TryParseHtmlString(content, out var attr) ? attr : Color.white;
        }
    }
}
