namespace Utils
{
    public static class HashUtils
    {
        public static uint GetHash(string str)
        {
            uint num = 0u;
            int i = 0;
            int length = str.Length;
            while (i < length)
            {
                num = (num << 1 | (num >> 31 & 1u));
                num += (uint)str[i];
                i++;
            }
            return num;
        }
    }
}