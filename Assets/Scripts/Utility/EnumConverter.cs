using System;

public static class EnumConverter
{
    public static string GetString<T>(T e) where T : Enum
    {
        return e.ToString();
    }

    public static int GetInt<T>(T e) where T : Enum
    {
        return Convert.ToInt32(e);
    }
}
