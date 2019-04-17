using UnityEngine;
using System.Collections;

public class UIAnchorManager : MonoBehaviour
{
    public Camera uiCamera;
    public UIAnchor[] uiAnchors;
    float lastAspect;

    void SetAnchors()
    {
        Vector2 dim = new Vector2(uiCamera.orthographicSize * uiCamera.aspect, uiCamera.orthographicSize);
        foreach (UIAnchor a in uiAnchors)
        {
            a.Postition = a.anchorScaler * dim;
        }
    }

    void Start()
    {
        if (!uiCamera) uiCamera = GetComponent<Camera>();
        SetAnchors();
    }
    
    // updates positions whenever aspect ratio changes. wasteful if not using resizable windows. 
    void LateUpdate()
    {
        if (uiCamera.aspect != lastAspect)
        {
            lastAspect = uiCamera.aspect;
            SetAnchors();
        }
    }
    

}
