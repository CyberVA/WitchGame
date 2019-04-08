using Byteable;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Layer { Tex, Rotation, Collision, Other }

public class Room : IByteable
{
    const byte fileversion = 0;
    const int layerCount = 4;
    const string defExit = "start";
    public int width;
    public int height;
    private int arrayLength;

    public string exitNorth;
    public string exitSouth;
    public string exitEast;
    public string exitWest;

    //tile data is stored in [layer][grid position]
    private byte[][] tileData;
    
    LinkedList<Decoration> decorations;

    public Room(int width, int height)
    {
        this.width = width;
        this.height = height;

        exitNorth = defExit;
        exitSouth = defExit;
        exitEast = defExit;
        exitWest = defExit;

        arrayLength = this.width * this.height;
        decorations = new LinkedList<Decoration>();

        tileData = new byte[layerCount][];
        for (int i = 0; i < layerCount; i++)
        {
            tileData[i] = new byte[arrayLength];
        }
    }
    public Room()
    {
        decorations = new LinkedList<Decoration>();
        tileData = new byte[layerCount][];

        exitNorth = defExit;
        exitSouth = defExit;
        exitEast = defExit;
        exitWest = defExit;
    }

    public byte GetValue(GridPos p, Layer layer)
    {
        if (Inbounds(p))
        {
            int i = p.y * width + p.x;
            return tileData[(int)layer][i];
        }
        else
        {
            return 0;
        }
    }
    public void SetValue(GridPos p, Layer layer, byte value)
    {
        try
        {
            int i = p.y * width + p.x;
            tileData[(int)layer][i] = value;
        }
        catch(ArgumentOutOfRangeException)
        {
            throw new Exception("Value set outside of grid");
        }
    }
    /// <summary>
    /// Initializes decoration in list for efficiency
    /// </summary>
    public void AddDecoration(float x, float y, byte value)
    {
        decorations.AddFirst(new Decoration(x, y, value));
        decorations.First.Value.node = decorations.First;
    }
    public void DeleteDecoration(Decoration d)
    {
        decorations.Remove(d.node);
    }
    public IEnumerator<Decoration> GetDecorations()
    {
        return ((IEnumerable<Decoration>)decorations).GetEnumerator();
    }

    public bool Inbounds(GridPos p)
    {
        return !(p.x < 0 || p.x >= width || p.y < 0 || p.y >= height);
    }

    void IByteable.Write(ByteScribe writer)
    {
        writer.Write(fileversion);

        writer.Write((byte)width);
        writer.Write((byte)height);

        writer.Write(exitNorth);
        writer.Write(exitSouth);
        writer.Write(exitEast);
        writer.Write(exitWest);

        for (int l = 0; l < layerCount; l++)
        {
            for(int i = 0; i < arrayLength; i++)
            {
                writer.Write(tileData[l][i]);
            }
        }

        writer.Write((byte)decorations.Count);
        foreach(Decoration d in decorations)
        {
            writer.Write(d);
        }

    }

    void IByteable.Read(ByteScribe reader)
    {
        reader.ReadByte();//fileVersion
        width = reader.ReadByte();
        height = reader.ReadByte();

        exitNorth = reader.ReadString();
        exitSouth = reader.ReadString();
        exitEast = reader.ReadString();
        exitWest = reader.ReadString();

        if (arrayLength != width * height)
        {
            arrayLength = width * height;
            for (int i = 0; i < layerCount; i++)
            {
                tileData[i] = new byte[arrayLength];
            }
        }

        for (int l = 0; l < layerCount; l++)
        {
            for (int i = 0; i < arrayLength; i++)
            {
                tileData[l][i] = reader.ReadByte();
            }
        }

        decorations.Clear();
        int decCount = reader.ReadByte();
        for (int i = 0; i < decCount; i++)
        {
            AddDecoration(reader.ReadFloat(), reader.ReadFloat(), reader.ReadByte());
        }
    }

    int IByteable.GetSize()
    {
        return 4 + arrayLength * layerCount + decorations.Count * (sizeof(float) * 2 + 1) +
            sizeof(int) * 4 + exitNorth.Length + exitSouth.Length + exitEast.Length + exitWest.Length;
    }
    
}

public class Decoration : IByteable
{
    public float x, y;
    public byte value;
    public LinkedListNode<Decoration> node;

    public Decoration(float x, float y, byte value)
    {
        this.x = x;
        this.y = y;
        this.value = value;
    }

    int IByteable.GetSize()
    {
        return sizeof(float) * 2 + 1;
    }

    void IByteable.Read(ByteScribe reader)
    {
        x = reader.ReadFloat();
        y = reader.ReadFloat();
        value = reader.ReadByte();
    }

    void IByteable.Write(ByteScribe writer)
    {
        writer.Write(x);
        writer.Write(y);
        writer.Write(value);
    }
}