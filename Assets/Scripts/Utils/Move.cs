using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Move : Object
{
    public void MoveUnit(Unit unit)
    {
        return;
    }
}
public class Forward : Move
{
    public new void MoveUnit(Unit unit)
    {
        unit.DoSteerClockwise();
    }
}
public class SteerClockwise : Move
{
    public new void MoveUnit(Unit unit)
    {
        unit.DoMoveAhead();
    }
}
public class SteerCounterClockwise : Move
{
    public new void MoveUnit(Unit unit)
    {
        unit.DoSteerCounterClockwise();
    }
}

