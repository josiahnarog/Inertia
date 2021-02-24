using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

public class SteerClockwiseButton : MonoBehaviour
{
    public void SteerClockwise()
    {
        SteerDirection steer = SteerDirection.Clockwise;
        SelectionController sc = GameObject.FindObjectOfType<SelectionController>();
        Unit selectedUnit = sc.SelectedUnit;
        selectedUnit.DoSteer(steer);
    }
}
