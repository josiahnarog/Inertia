using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Move : Object
{
    public virtual void MoveUnit(Unit unit)
    {
        Debug.Log("Why am I doing an empty move?");
    }

}

public class Forward : Move
{
    public override void MoveUnit(Unit unit)
    {
        Debug.Log("Doing DoMoveAhead");
        unit.DoMoveAhead();
    }
}

public class SteerClockwise : Move
{
    public override void MoveUnit(Unit unit)
    {
        Debug.Log("Doing DoSteerClockwise");
        unit.DoSteerClockwise();
    }
}
public class SteerCounterClockwise : Move
{
    public override void MoveUnit(Unit unit)
    {
        Debug.Log("Doing DoSteerCounterClockwise");
        unit.DoSteerCounterClockwise();
    }
}

