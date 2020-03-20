using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utility
{
    //Two functions that do the same thing, just differently (waiting, then doing! [or just waiting, don't pass in anything])
    public static IEnumerator WaitFor(float seconds, Action action = null)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }

    public static IEnumerator WaitUntil(Func<bool> predicate, Action action = null)
    {
        yield return new WaitUntil(predicate);
        action?.Invoke();
    }

    #region Extension Methods
    public static Vector3 SetX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 SetY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 SetZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 FlipZ(this Vector3 v)
    {
        return new Vector3(v.x, v.y, -v.z); 
    }

    public static Vector3 FlipX(this Vector3 v)
    {
        return new Vector3(-v.x, v.y, v.z);
    }

    //formats point as string (only used by score manager, we can put this function in that class)
    public static string FormatPointAsString(this int pointValue)
    {
        string formatted = pointValue.ToString();
        //Replace with appropriate score name (strings are immutable (do not change values), thus the use of formatted = )
        //replaced in an order that doesn't make wierd replacements (e.g. 15 -> 1adv.)
        formatted = formatted.Replace("0", "love");
        formatted = formatted.Replace("5", "adv.");
        formatted = formatted.Replace("1", "15");
        formatted = formatted.Replace("4", "deuce");
        formatted = formatted.Replace("3", "40");
        formatted = formatted.Replace("2", "30");
        if (pointValue > 6)
            Debug.LogError("Point Value is Out of Bounds at timestamp: " + Time.time);
        return formatted;
    }
    #endregion
}
