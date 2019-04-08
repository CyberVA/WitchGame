using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tileset", menuName = "TileSet") ]
public class TileSet : ScriptableObject, IEnumerable<Sprite>
{
    [SerializeField]
    protected Sprite[] tiles;
    public Sprite this[int i]
    {
        get
        {
            return tiles[i];
        }
    }

    public int TileCount
    {
        get
        {
            return tiles.Length;
        }
    }

    IEnumerator<Sprite> IEnumerable<Sprite>.GetEnumerator()
    {
        return ((IEnumerable<Sprite>)tiles).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Sprite>)tiles).GetEnumerator();
    }
}
