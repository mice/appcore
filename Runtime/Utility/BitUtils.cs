
using UnityEngine;

public static class BitUtils
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static ulong Combine(uint high,uint low){
        return (ulong)high << 32 | low;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void DeCombine(ulong mixed,out uint high,out uint low){
        low = (uint)(mixed & uint.MaxValue);
        high = (uint)(mixed >> 32);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool IsPOT(uint v) => v!=0 && ((v&v-1) ==0);

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static uint NextPOT(uint v){
        v--;
        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;
        v++;
        return v;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static Color ColorFrom(uint value)
    {
        return new Color32((byte)(value >> 16 & 0xff), (byte)(value >> 8 & 0xff), (byte)(value & 0xff), (byte)(value >> 24 & 0xff));
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static uint ColorTo(Color32 value)
    {
        return (uint)(value.a << 24) | (uint)(value.r << 16) | (uint)(value.g << 8) | (uint)(value.b);
    }
}