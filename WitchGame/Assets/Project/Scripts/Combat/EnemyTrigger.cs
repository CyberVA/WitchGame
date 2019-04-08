using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoStepCollision;

public enum TriggerType {None, PlayerShroom, ShroomPunch }

public class Trigger
{
    public Box bounds;

    public TriggerType type;
    public int otherData;
}
