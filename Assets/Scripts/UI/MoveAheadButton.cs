using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAheadButton : MonoBehaviour
{
    public void MoveAhead()
    {
        SelectionController sc = GameObject.FindObjectOfType<SelectionController>();
        Unit u = sc.SelectedUnit;
        
        Debug.LogError("I pushed the Move Ahead button.");
        Debug.LogError("I start out this direction:" + u.Facing);
    }
    
}
