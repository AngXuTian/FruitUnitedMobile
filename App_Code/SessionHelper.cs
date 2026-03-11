using System;
using System.Collections;
using System.Web;
using System.Collections.Generic;


public static class SessionHelper
{
    /// <summary>
    /// Checks if a session key exists and has a non-null value.
    /// </summary>
    public static bool IsSessionValueSet(string key)
    {
        return HttpContext.Current?.Session[key] != null;
    }

    /// <summary>
    /// Gets the session value as a specific type, or default if not set.
    /// </summary>
    public static T GetSessionValue<T>(string key)
    {
        var value = HttpContext.Current?.Session[key];
        return value == null ? default(T) : (T)value;
    }

    /// <summary>
    /// Sets a value in the session.
    /// </summary>
    public static void SetSessionValue(string key, object value)
    {
        if (HttpContext.Current != null)
        {
            HttpContext.Current.Session[key] = value;
        }
    }

    /// <summary>
    /// Checks if the session value is exactly of type T.
    /// </summary>
    public static bool IsSessionValueOfType<T>(string key)
    {
        var value = HttpContext.Current?.Session[key];
        return value != null && value is T;
    }

    /// <summary>
    /// Checks if the session value is an array (e.g., string[], int[], custom[]).
    /// </summary>
    public static bool IsSessionValueArray(string key)
    {
        var value = HttpContext.Current?.Session[key];
        return value != null && value.GetType().IsArray;
    }

    /// <summary>
    /// Checks if the session value is a collection (List<T>, HashSet<T>, etc.), but not string or array.
    /// </summary>
    public static bool IsSessionValueCollection(string key)
    {
        var value = HttpContext.Current?.Session[key];
        if (value == null) return false;
        if (value is string || value.GetType().IsArray) return false;
        return value is IEnumerable;
    }

    /// <summary>
    /// Checks if the session value is a custom object (not primitive, string, array, or collection).
    /// </summary>
    public static bool IsSessionValueObject(string key)
    {
        var value = HttpContext.Current?.Session[key];
        if (value == null) return false;

        Type type = value.GetType();
        return !type.IsPrimitive &&
                !type.IsArray &&
                type != typeof(string) &&
                type != typeof(decimal) &&
                type != typeof(DateTime) &&
                !(value is IEnumerable);
    }

    /// <summary>
    /// Returns the full type name of the stored session value (useful for debugging).
    /// </summary>
    public static string GetSessionValueTypeName(string key)
    {
        var value = HttpContext.Current?.Session[key];
        return value?.GetType().FullName ?? "null";
    }

    /// <summary>
    /// Safely tries to get the session value as type T.
    /// </summary>
    public static bool TryGetSessionValue<T>(string key, out T result)
    {
        result = default(T);
        var value = HttpContext.Current?.Session[key];
        if (value == null) return false;

        if (value is T casted)
        {
            result = casted;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns a string with all current session keys and values (for debugging).
    /// </summary>
    public static string GetAllSessionData()
    {
        if (HttpContext.Current?.Session == null) return "No session available.";

        var data = new System.Text.StringBuilder();
        foreach (string key in HttpContext.Current.Session.Keys)
        {
            var value = HttpContext.Current.Session[key];
            data.AppendLine($"{key}: {(value ?? "null")} [{(value?.GetType().Name ?? "null")}]");
        }
        return data.ToString();
    }
}
