using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICallbackReciever
{
    void Callback(uint callBackCode);
}

public static class Callbacks
{
    private static uint nextCode = 0;
    public static uint GetCallbackCode() => nextCode++;
}

