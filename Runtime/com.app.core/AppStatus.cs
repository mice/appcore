public static class AppStatus
{
    /// <summary>
    /// 应用是否已退出
    /// <para>在游戏入口实始化时设置为false, OnApplicationQuit时设置为true </para>
    /// <para>用于多线程无法访问 Application.isPlaying</para>
    /// </summary>
    public static volatile bool isApplicationQuit = true;
    /// <summary>
    /// <para>对应 Time.realtimeSinceStartup </para>
    /// <para>在UpdateManager.Update中更新</para>
    /// <para>用于多线程无法访问Time.realtimeSinceStartup</para>
    /// </summary>
    public static volatile float realtimeSinceStartup;
    public static volatile int frameCount;
    /// <summary>
    /// 在DLL里，如果用宏判断，在生成DLL时就已定性，在UNITY定义的宏无法改变。所以用变量来区分
    /// <para>ENABLE_LOG</para>
    /// </summary>
    public static volatile bool isEnableLog = false;
    public static volatile bool supportInstancing = true;
    public static string persistentDataPath;
}