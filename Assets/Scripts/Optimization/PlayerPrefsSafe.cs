using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class PlayerPrefsSafe
{
    private const int SALT = 830572948;

    public static void SetInt(string key, int value)
    {
        var salted = value ^ SALT;
        PlayerPrefs.SetInt(StringHash(key), salted);
        PlayerPrefs.SetInt(StringHash("_" + key), IntHash(value));
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        var hashedKey = StringHash(key);
        if (!PlayerPrefs.HasKey(hashedKey)) return defaultValue;

        var salted = PlayerPrefs.GetInt(hashedKey);
        var value = salted ^ SALT;

        var loadedHash = PlayerPrefs.GetInt(StringHash("_" + key));
        return loadedHash != IntHash(value) ? defaultValue : value;
    }

    public static void SetFloat(string key, float value)
    {
        var intValue = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        var salted = intValue ^ SALT;
        
        PlayerPrefs.SetInt(StringHash(key), salted);
        PlayerPrefs.SetInt(StringHash("_" + key), IntHash(intValue));
    }

    public static float GetFloat(string key, float defaultValue = 0)
    {
        var hashedKey = StringHash(key);
        if (!PlayerPrefs.HasKey(hashedKey)) return defaultValue;

        var salted = PlayerPrefs.GetInt(hashedKey);
        var value = salted ^ SALT;

        var loadedHash = PlayerPrefs.GetInt(StringHash("_" + key));
        return loadedHash != IntHash(value) ? defaultValue : BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
    }

    private static int IntHash(int x)
    {
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        x = (x >> 16) ^ x;
        return x;
    }

    public static string StringHash(string x)
    {
        HashAlgorithm algorithm = SHA256.Create();
        var sb = new StringBuilder();

        var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(x));
        foreach (var b in bytes) sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(StringHash(key));
        PlayerPrefs.DeleteKey(StringHash("_" + key));
    }

    public static bool HasKey(string key)
    {
        var hashedKey = StringHash(key);
        if (!PlayerPrefs.HasKey(hashedKey)) return false;

        var salted = PlayerPrefs.GetInt(hashedKey);
        var value = salted ^ SALT;

        var loadedHash = PlayerPrefs.GetInt(StringHash("_" + key));

        return loadedHash == IntHash(value);
    }
}