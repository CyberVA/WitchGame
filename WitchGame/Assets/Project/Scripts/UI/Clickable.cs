using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public BoxCollider2D myBox;

    // Start is called before the first frame update
    void Awake()
    {
        myBox = GetComponent<BoxCollider2D>();
    }

    public virtual void OnClick() { }
    public virtual void OnClickStay() { }
    public virtual void OnHover() { }
    public virtual void OnEmpty() { }
}
