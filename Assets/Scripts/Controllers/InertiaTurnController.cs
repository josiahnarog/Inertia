using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class InertiaTurnController : MonoBehaviour
{
    
    HexMap hexMap;
    SelectionController selectionController;

    public GameObject doMovePulseButton;
    public GameObject nextUnitButton;

    public bool doingMovementPulse;
    
    // Start is called before the first frame update
    
    void Start () {

        hexMap = GameObject.FindObjectOfType<HexMap>();
        selectionController = GameObject.FindObjectOfType<SelectionController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (doingMovementPulse)
        {
            Debug.Log("Doing Movement Pulse");
            Unit[] units = hexMap.CurrentPlayer.Units;
        
            // First check to see if there are any units that have enqueued moves.
            foreach(Unit u in units)
            {

                // Do those moves
                if( u.DoQueuedMove() )
                {
                    GameObject unitGO = hexMap.GetUnitGO(u);
                    UnitView unitView = unitGO.GetComponentInChildren<UnitView>();

                    if(unitView.unitAnimationPlaying == false) {
                        Debug.LogError("WE GOT HERE");
                        return; // Wait one frame
                    }
                    else
                    {
                        u.DoQueuedMove();
                    }
                }
            }
        
            // Now are any units waiting for orders? If so, halt DoMovementPulse()
            // foreach(Unit u in units)
            // {
            //     if(u.UnitWaitingForOrders())
            //     {
            //         // Select the unit
            //         selectionController.SelectedUnit = u;
            //
            //         // Stop processing the end turn
            //         doMovementPulse = false;
            //     }
            // }
            //
            // // Reset unit movement
            // foreach(Unit u in units)
            // {
            //     u.RefreshMovement();
            // }
        
        
            // Go to next player
            selectionController.SelectedUnit = null;
            hexMap.AdvanceToNextPlayer();
            doingMovementPulse = false;
        }
    }

    public void DoMovementPulse()
    {
        doingMovementPulse = true;
    }
}
