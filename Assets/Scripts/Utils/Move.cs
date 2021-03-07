using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Move : Object
{
    public virtual void MoveUnit(Unit unit)
    {
        Debug.LogError("Why am I doing an empty move?");
    }

}

public class Forward : Move
{
    public override void MoveUnit(Unit unit)
    {
        unit.DoMoveAhead();
    }
}

public class SteerClockwise : Move
{
    public override void MoveUnit(Unit unit)
    {
        unit.DoSteerClockwise();
    }
}
public class SteerCounterClockwise : Move
{
    public override void MoveUnit(Unit unit)
    {
        unit.DoSteerCounterClockwise();
    }
}

