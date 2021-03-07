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
        u.AddToMoveQueue(forward);
    }
}
