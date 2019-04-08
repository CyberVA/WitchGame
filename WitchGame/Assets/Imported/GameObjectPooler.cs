using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPooler : MonoBehaviour
{
    [SerializeField]
    protected GameObject prefab;
    Stack<GameObject> stack;

    private void Awake()
    {
        stack = new Stack<GameObject>();
    }

    public GameObject GetObject()
    {
        if (stack.Count == 0)
        {
            if(prefab == null)
            {
                return new GameObject("Pooled Object");
            }
            else
            {
                return Instantiate(prefab);
            }
        }
        else
        {
            GameObject obj = stack.Pop();
            obj.SetActive(true);
            return obj;
        }
    }

    public void Retire(GameObject obj)
    {
        obj.SetActive(false);
        stack.Push(obj);
    }

    public void Clear()
    {
        foreach (GameObject g in stack)
        {
            Object.Destroy(g);
        }
        stack.Clear();
    }

    public void DeepClear()
    {
        Clear();
        stack.TrimExcess();
    }
}