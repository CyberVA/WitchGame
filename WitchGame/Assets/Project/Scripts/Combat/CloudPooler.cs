using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudPooler : MonoBehaviour
{
    public static CloudPooler instance;
    [SerializeField]
    protected GameObject prefab;
    public RoomController roomController;
    public uint startingSize;
    public uint maxSize;
    Stack<Cloud> stack;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("bad");
        }
        stack = new Stack<Cloud>();

        if (startingSize > 0)
        {
            Cloud cloud;
            for (int i = 0; i < startingSize; i++)
            {
                cloud = Instantiate(prefab).GetComponent<Cloud>();
                cloud.Setup(GameController.Main.combatSettings);
                cloud.gameObject.SetActive(false);
                stack.Push(cloud);
            }
        }
    }

    public Cloud GetCloud()
    {
        if (stack.Count == 0)
        {
            Cloud cloud = Instantiate(prefab).GetComponent<Cloud>();
            cloud.Setup(GameController.Main.combatSettings);
            return cloud;
        }
        else
        {
            return stack.Pop();
        }
    }

    public void Retire(Cloud obj)
    {
        if (stack.Count < maxSize)
        {
            stack.Push(obj);
        }
        else
        {
            Destroy(obj.gameObject);
        }
    }

    public void Clear()
    {
        foreach (Cloud g in stack)
        {
            Destroy(g.gameObject);
        }
        stack.Clear();
    }
}