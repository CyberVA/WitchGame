using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Effects
{
    public readonly static uint fadeCallback;

    static Effects()
    {
        fadeCallback = Callbacks.GetCallbackCode();
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
    }
}
