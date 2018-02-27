
using System;
using System.Collections.Generic;
using MiniJSON;

/*
    * JsonUtil class is a group of static methods associated with JSON things.
    */
public class JsonUtil {
    public static string JsonSerialize( object data ) {
        return Json.Serialize(data);
    }

    public static Dictionary<string, object> JsonDeserialize( string jsonString ) {
        return Json.Deserialize(jsonString) as Dictionary<string, object>;
    }

    public static byte[] JsonObjectToByteArray( object data ) {
        string jsonString = JsonSerialize(data);

        return StringToByteArray(jsonString);
    }

    public static Dictionary<string, object> ByteArrayToJsonObject( byte[] data ) {
        string jsonString = ByteArrayToString(data);
        return JsonDeserialize(jsonString);
    }

    public static byte[] StringToByteArray( string str ) {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }

    public static string ByteArrayToString( byte[] bytes ) {
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    public static byte[] ObjectListToByteArray( List<object> data ) {
        int count = data.Count;
        byte[] bytes = new byte[count];

        for (int i = 0; i < count; i++) {
            bytes[i] = (byte)(long)data[i];
        }

        return bytes;
    }

    public static short GetShortValue( Dictionary<string, object> dict, string key ) {
        return GetShortValue(dict, key, 0);
    }

    public static short GetShortValue( Dictionary<string, object> dict, string key, short defaultValue ) {
        object obj = GetSafeObjectValue(dict, key);
        return (obj == null) ? defaultValue : (short)(long)obj;
    }

    public static int GetIntValue( Dictionary<string, object> dict, string key ) {
        return GetIntValue(dict, key, 0);
    }

    public static int GetIntValue( Dictionary<string, object> dict, string key, int defaultValue ) {
        object obj = GetSafeObjectValue(dict, key);
        return (obj == null) ? defaultValue : (int)(long)obj;
    }

    public static uint GetUIntValue(Dictionary<string, object> dict, string key)
    {
        return GetUIntValue(dict, key, 0);
    }

    public static uint GetUIntValue(Dictionary<string, object> dict, string key, uint defaultValue)
    {
        object obj = GetSafeObjectValue(dict, key);
        return (obj == null) ? defaultValue : (uint)(long)obj;
    }

    public static long GetLongValue( Dictionary<string, object> dict, string key ) {
        return GetLongValue(dict, key, 0);
    }

    public static long GetLongValue( Dictionary<string, object> dict, string key, long defaultValue ) {
        object obj = GetSafeObjectValue(dict, key);
        return (obj == null) ? defaultValue : (long)obj;
    }

    public static float GetFloatValue( Dictionary<string, object> dict, string key ) {
        return (float)GetDoubleValue(dict, key);
    }

    public static float GetFloatValue( Dictionary<string, object> dict, string key, float defaultValue ) {
        return (float)GetDoubleValue(dict, key, defaultValue);
    }

    public static double GetDoubleValue( Dictionary<string, object> dict, string key ) {
        return GetDoubleValue(dict, key, 0.0);
    }

    public static double GetDoubleValue( Dictionary<string, object> dict, string key, double defaultValue ) {
        object obj = GetSafeObjectValue(dict, key);
        double value = 0.0f;

        if (obj == null) {
            return defaultValue;
        }

        try {
            value = (double)obj;
        } catch (InvalidCastException e) {
            e.ToString(); // just to hide warning messages.
            value = (double)(long)obj;
        }

        return value;
    }

    public static float GetFloatValue( object obj ) {
        return (float)GetDoubleValue(obj);
    }

    public static double GetDoubleValue( object obj ) {
        double value = 0.0f;

        if (obj == null) {
            return value;
        }

        try {
            value = (double)obj;
        } catch (InvalidCastException e) {
            e.ToString(); // just to hide warning messages.
            value = (double)(long)obj;
        }

        return value;
    }

    public static bool GetBoolValue( Dictionary<string, object> dict, string key ) {
        return GetBoolValue(dict, key, false);
    }

    public static bool GetBoolValue( Dictionary<string, object> dict, string key, bool defaultValue ) {
        object obj = GetSafeObjectValue(dict, key);
        return (obj == null) ? defaultValue : (bool)obj;
    }

    public static string GetStringValue( Dictionary<string, object> dict, string key ) {
        return GetStringValue(dict, key, string.Empty);
    }

    public static string GetStringValue( Dictionary<string, object> dict, string key, string defaultValue ) {
        object obj = GetSafeObjectValue(dict, key);
        return (obj == null) ? defaultValue : (string)obj;
    }

    public static List<object> GetListValue( Dictionary<string, object> dict, string key ) {
        object obj = GetSafeObjectValue(dict, key);
        return (obj == null) ? new List<object>() : (List<object>)obj;
    }

    private static object GetSafeObjectValue( Dictionary<string, object> dict, string key ) {
        object value = null;

        if (dict.TryGetValue(key, out value) == false) {

        }

        return value;
    }
}
