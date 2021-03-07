using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

public class SteerClockwiseButton : MonoBehaviour
{
    public void SteerClockwise()
    {
        SelectionController sc = GameObject.FindObjectOfType<SelectionController>();
        Unit u = sc.SelectedUnit;
        SteerClockwise steerClockwise = new SteerClockwise();
        u.AddToMoveQueue(steerClockwise);
    }
}
