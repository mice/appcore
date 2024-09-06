using UnityEngine;
using System.Collections.Generic;




public static class LocalCache
{
    private static Dictionary<string,string> _strDict = new Dictionary<string, string>();
    private static Dictionary<string,int> _intDict = new Dictionary<string, int>();
    private static Dictionary<string,float> _floatDict = new Dictionary<string, float>();
    private static Dictionary<string, bool> _keyDict = new Dictionary<string, bool>();
    private static bool _CheckPermission() {
        return true;
    }

    public static void FlushCache(){
        if(!_CheckPermission()) {
            return ;
        }
        foreach(var pairs in _strDict) {
            PlayerPrefs.SetString(pairs.Key, pairs.Value);
        }
        foreach(var pairs in _intDict) {
            PlayerPrefs.SetInt(pairs.Key, pairs.Value);
        }
        foreach(var pairs in _floatDict) {
            PlayerPrefs.SetFloat(pairs.Key, pairs.Value);
        }
    }

    public static string userKeyPrefix = string.Empty;
    public static string globalKeyPrefix = "mt";
    private static string _GetUserKey(string key)
    {
        return string.Format("{0}_{1}", userKeyPrefix, key);
    }

    public static bool HasLocalKey(string key)
    {
        if(!_CheckPermission()) {
            return _keyDict.ContainsKey(_GetUserKey(key));
        }
        return PlayerPrefs.HasKey(_GetUserKey(key));
    }

    public static void DeleteLocalKey(string key) {            
        PlayerPrefs.DeleteKey(_GetUserKey(key));
    }

    public static bool HasGlobalKey(string key)
    {
        if(!_CheckPermission()) {
            return _keyDict.ContainsKey(_GetGlobalKey(key));
        }
        return PlayerPrefs.HasKey(_GetGlobalKey(key));
    }

    public static void DeleteGlobalKey(string key) {            
        PlayerPrefs.DeleteKey(_GetGlobalKey(key));
    }

    /// <summary>
    /// 用户绑定,
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultVal"></param>
    /// <returns></returns>
    public static int GetLocalInt(string key, int defaultVal = 0)
    {
        key = _GetUserKey(key);
        if(!_CheckPermission()) {
            if(_intDict.ContainsKey(key)) {
                return _intDict[key];
            }
            return defaultVal;
        }
        return PlayerPrefs.GetInt(key, defaultVal);
    }

    public static float GetLocalFloat(string key)
    {
        key = _GetUserKey(key);
        if(!_CheckPermission()) {
            if(_floatDict.ContainsKey(key)) {
                return _intDict[key];
            }
            return 0f;
        }
        return PlayerPrefs.GetFloat(key, 0f);
    }

    public static string GetLocalString(string key, string defaultVal = "")
    {
        key = _GetUserKey(key);
        if(!_CheckPermission()) {
            if(_strDict.ContainsKey(key)) {
                return _strDict[key];
            }
            return defaultVal;
        }
        return PlayerPrefs.GetString(key, defaultVal);
    }

    public static void SetLocalString(string key, string val)
    {
        key = _GetUserKey(key);
        if(!_CheckPermission()) {
            _strDict[key] = val;
            _keyDict[key] = true;
            return ;
        }
        PlayerPrefs.SetString(key, val);
    }

    public static void SetLocalInt(string key, int val)
    {
        key = _GetUserKey(key);
        if(!_CheckPermission()) {
            _intDict[key] = val;
            _keyDict[key] = true;
            return ;
        }
        PlayerPrefs.SetInt(key, val);
    }

    public static void SetLocalFloat(string key, float val)
    {
        key = _GetUserKey(key);
        if(!_CheckPermission()) {
            _floatDict[key] = val;
            _keyDict[key] = true;
            return ;
        }
        PlayerPrefs.SetFloat(key, val);
    }

    private static string _GetGlobalKey(string key)
    {
        return string.Format("{0}_{1}", globalKeyPrefix, key);
    }

    public static void SetGlobalString(string key, string val)
    {
        key = _GetGlobalKey(key);
     
        if(!_CheckPermission()) {
            _strDict[key] = val;
            _keyDict[key] = true;
            return ;
        }
        PlayerPrefs.SetString(key, val);
    }

    /// <summary>
    /// 全局绑定,与用户ID无关.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultVal"></param>
    /// <returns></returns>
    public static string GetGlobalString(string key, string defaultVal = "")
    {
        key = _GetGlobalKey(key);
       
        if(!_CheckPermission()) {
            if(_strDict.ContainsKey(key)) {
                return _strDict[key];
            }
            return defaultVal;
        }
        return PlayerPrefs.GetString(key, defaultVal);
    }

    public static int GetGlobalInt(string key, int defaultVal = 0)
    {
        key = _GetGlobalKey(key);
        if(!_CheckPermission()) {
            if(_intDict.ContainsKey(key)) {
                return _intDict[key];
            }
            return defaultVal;
        }
        return PlayerPrefs.GetInt(key, defaultVal);
    }

    public static void SetGlobalInt(string key, int val)
    {
        key = _GetGlobalKey(key);
        if(!_CheckPermission()) {
            _intDict[key] = val;
            _keyDict[key] = true;
            return ;
        }
        PlayerPrefs.SetInt(key, val);
    }
}