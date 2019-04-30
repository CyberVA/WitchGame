using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MultiEnumerator<T> : IEnumerator<T>, IEnumerable<T>
{
    IEnumerable<T>[] enumerables;
    IEnumerator<T> e;
    int i = 0;

    public MultiEnumerator(params IEnumerable<T>[] enumerables)
    {
        this.enumerables = enumerables;
        e = enumerables[i].GetEnumerator();
    }

    public T Current => e.Current;

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        e.Dispose();
        enumerables = null;
    }

    public bool MoveNext()
    {
        if(e.MoveNext())
        {
            return true;
        }
        else
        {
            e.Dispose();
            i++;
            if (i < enumerables.Length)
            {
                e = enumerables[i].GetEnumerator();
                return MoveNext();
            }
            else
            {
                return false;
            }
        }
    }

    public void Reset()
    {
        e.Dispose();
        i = 0;
        e = enumerables[i].GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this;
    }
}
