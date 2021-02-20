using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Ship : Unit
{
    public Ship()
    {
        Name = "Ship";
    }

    // TODO: Figure out how to dynamically evaluate movement speed.
    // private int wholeEngineRooms = 1;
    // public float engineRoomsPerMovementPoint = 1 / 8f;
    // private int Convert.ToInt32((wholeEngineRooms / engineRoomsPerMovementPoint));

    public new int Movement = 8;
    public new int MovementRemaining = 2;

}
