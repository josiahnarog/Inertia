using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertiaTurnController : MonoBehaviour
{
    
    HexMap hexMap;
    SelectionController selectionController;

    public GameObject DoMovePulseButton;
    public GameObject NextUnitButton;
    
    // Start is called before the first frame update
    
    void Start () {
        hexMap = GameObject.FindObjectOfType<HexMap>();
        selectionController = GameObject.FindObjectOfType<SelectionController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DoMovementPulse()
    {
        Debug.Log("Do Movement Pulse");
        // Unit[] units = hexMap.CurrentPlayer.Units;
        //
        // // First check to see if there are any units that have enqueued moves.
        // foreach(Unit u in units)
        // {
        //     // Do those moves
        //     while( u.DoMove() )
        //     {
        //         // TODO: WAIT FOR ANIMATION TO COMPLETE!
        //     }
        // }
        //
        // // Now are any units waiting for orders? If so, halt DoMovementPulse()
        // foreach(Unit u in units)
        // {
        //     if(u.UnitWaitingForOrders())
        //     {
        //         // Select the unit
        //         selectionController.SelectedUnit = u;
        //
        //         // Stop processing the end turn
        //         return;
        //     }
        // }
        //
        // // Reset unit movement
        // foreach(Unit u in units)
        // {
        //     u.RefreshMovement();
        // }
        //
        //
        // // Go to next player
        // selectionController.SelectedUnit = null;
        // hexMap.AdvanceToNextPlayer();

    }
}
