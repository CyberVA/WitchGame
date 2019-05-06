using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Effects
{
    public readonly static uint fadeCallback;
    public readonly static uint squishCallback;

    static Effects()
    {
        fadeCallback = Callbacks.GetCallbackCode();
        squishCallback = Callbacks.GetCallbackCode();
    }

    public static IEnumerator FadeAway(SpriteRenderer spriteRenderer, float length, Vector2 vector, ICallbackReciever callbackReciever)
    {
        float t = length;
        Color c = spriteRenderer.color;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            spriteRenderer.transform.position += (Vector3)(vector * Time.deltaTime);
            spriteRenderer.color -= new Color(0f, 0f, 0f, Time.deltaTime / length);
            yield return null;
        }

        if(callbackReciever != null) callbackReciever.Callback(fadeCallback);
    }
    /// <summary>
    /// Bad
    /// </summary>
    public static IEnumerator Squish(Transform transform, float length, Vector2 vector, ICallbackReciever callbackReciever)
    {
        float t = length;
        Vector3 newScale = new Vector3(1f, 1f, 1f);
        while (t > length * 0.5f)
        {
            t -= Time.deltaTime;
            newScale -= (Vector3)(vector * (Time.deltaTime / length));
            transform.localScale = newScale;
            yield return null;
        }
        newScale = new Vector3(1f, 1f, 1f) - (Vector3)vector;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            newScale += (Vector3)(vector * (Time.deltaTime / length));
            transform.localScale = newScale;
            yield return null;
        }

        transform.localScale = new Vector3(1f, 1f, 1f);
        if (callbackReciever != null) callbackReciever.Callback(squishCallback);
    }
    /*
    public static IEnumerator FadeAway(SpriteRenderer spriteRenderer, float length, Vector2 vector, ICallbackReciever callbackReciever, uint callBackCode)
    {
        float t = length;
        Color c = spriteRenderer.color;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            spriteRenderer.transform.position += (Vector3)(vector * Time.deltaTime);
            spriteRenderer.color -= new Color(0f, 0f, 0f, Time.deltaTime / length);
            yield return null;
        }

        callbackReciever.Callback(callBackCode);
    }*/
}
