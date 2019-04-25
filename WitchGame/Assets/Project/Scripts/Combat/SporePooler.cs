using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SporePooler : MonoBehaviour
{
    public static SporePooler instance;
    [SerializeField]
    protected GameObject prefab;
    public RoomController roomController;
    public uint startingSize;
    public uint maxSize;
    Stack<Spore> stack;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("bad");
        }
        stack = new Stack<Spore>();

        if (startingSize > 0)
        {
            Spore spore;
            for (int i = 0; i < startingSize; i++)
            {
                spore = Instantiate(prefab).GetComponent<Spore>();
                spore.Setup(roomController.staticColliders, GameController.Main.combatSettings);
                spore.gameObject.SetActive(false);
                stack.Push(spore);
            }
        }
    }

    public Spore GetSpore()
    {
        if (stack.Count == 0)
        {
            Spore spore = Instantiate(prefab).GetComponent<Spore>();
            spore.Setup(roomController.staticColliders, GameController.Main.combatSettings);
            return spore;
        }
        else
        {
            return stack.Pop();
        }
    }

    public void Retire(Spore obj)
    {
        if(stack.Count < maxSize)
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
        foreach (Spore g in stack)
        {
            Destroy(g.gameObject);
        }
        stack.Clear();
    }
}