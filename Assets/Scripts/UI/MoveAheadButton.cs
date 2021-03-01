using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAheadButton : MonoBehaviour
{
    public void MoveAhead()
    {
        SelectionController sc = GameObject.FindObjectOfType<SelectionController>();
        Unit u = sc.SelectedUnit;
        Forward forward = new Forward();
        //u.DoMoveAhead();
        Debug.Log("I pushed the Move Ahead button.");
        u.AddToMoveQueue(forward);
        Debug.Log("Move queue is length: " + u.GetMoveQueueLength());
    }
    
}
