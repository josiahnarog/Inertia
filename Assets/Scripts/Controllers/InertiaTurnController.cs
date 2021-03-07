using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class InertiaTurnController : MonoBehaviour
{
    
    HexMap hexMap;
    SelectionController selectionController;

    public GameObject advanceTurnPulseButton;
    public GameObject nextUnitButton;

    public bool areMovesRemaining;

    // Start is called before the first frame update

    public enum TacticalTurnPulse
    {
        StartTurn =0,
        FirstMove = 1,
        SecondMove = 2,
        ThirdMove = 3,
        Firing = 4,
        EndTurn = 5
    }

    public List<TacticalTurnPulse> movePulses = new List<TacticalTurnPulse>();
    public TacticalTurnPulse turnPulse;
    
    void Start () {
        hexMap = GameObject.FindObjectOfType<HexMap>();
        selectionController = GameObject.FindObjectOfType<SelectionController>();
        turnPulse = TacticalTurnPulse.StartTurn;
        
        movePulses = new List<TacticalTurnPulse>()
        {
            TacticalTurnPulse.FirstMove,
            TacticalTurnPulse.SecondMove,
            TacticalTurnPulse.ThirdMove
        };
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Current Pulse: " + turnPulse);
    }
    
    public void AdvanceTurnPulse()
    {

        if (turnPulse == TacticalTurnPulse.EndTurn)
        {
            turnPulse = TacticalTurnPulse.StartTurn;
        }
        else
        {
            //Increment the turn pulse.
            // TODO Check for units waiting orders.
            turnPulse = turnPulse += 1;
        }
        
        Debug.Log(string.Format("Advancing Turn Pulse To {0}", turnPulse));
        
        if (turnPulse == TacticalTurnPulse.StartTurn)
        {
            // TODO  RefreshTurnMoveRatings(); 
            // TODO RefreshUnitShields();
            // TODO SetMoveControlsActive();
        }
        
        //We do moves when we advance TO a move pulse.
        if (movePulses.Contains(turnPulse))
        {
            Debug.Log("Refreshing Pulse Move Points.");
            RefreshPulseMovePoints();
            
            Debug.Log("Completing Queued Moves.");
            StartCoroutine(DoAllQueuedUnitMoves());
        }
        
        if (turnPulse == TacticalTurnPulse.Firing)
        {
            // Need to do combat here!
            // TODO SetFireControlsActive();
        }
        
        if (turnPulse == TacticalTurnPulse.EndTurn)
        {
            // Go to next player
            selectionController.SelectedUnit = null;
            // hexMap.AdvanceToNextPlayer();
        }
    }
    
    IEnumerator DoAllQueuedUnitMoves()
    {
        Debug.Log("Doing DoAllQueuedUnitMoves.");
        advanceTurnPulseButton.SetActive(false); //Don't want to advance while moving.
        areMovesRemaining = true;
        while (areMovesRemaining)
        {
            foreach (Unit u in hexMap.CurrentPlayer.Units)
            {
                areMovesRemaining = false; //Assume we won't loop again unless needed.
                
                var unitGO = hexMap.GetUnitGO(u);
                var unitView = unitGO.GetComponentInChildren<UnitView>();

                if(unitView.unitAnimationPlaying == true) {
                    //If a unit is animating, need to loop again.
                    areMovesRemaining = true; 
                    yield return null; // Wait one frame
                }
                else
                {
                    //Trigger a queued move call.
                    //If the DoQueuedMove returns true, this unit has more to do;
                    //If a different unit returned true, then they have more to do;
                    //In either case, areMovesRemaining is true, and the while loop 
                    //will be called again. Otherwise, we exit the loop and end the turn.
                    
                    // Debug.Log("areMovesRemaining returned:" + areMovesRemaining);
                    // Debug.Log("DoQueuedMove returned:" + u.DoQueuedMove());

                    areMovesRemaining = (areMovesRemaining || u.DoQueuedMove());
                    
                    
                    Debug.Log("Final areMovesRemaining returned:" + areMovesRemaining);
                    yield return null;
                }
            }

        }
        advanceTurnPulseButton.SetActive(true); //Let the player advance when ready.
        Debug.Log("Ending DoAllQueuedUnitMoves");
        yield break;
    }



    // public void DoMovePulse()
    // {
    //     StartCoroutine(DoAllQueuedUnitMoves());
    // }
    
    public void RefreshPulseMovePoints()
    {
        foreach (Unit u in hexMap.CurrentPlayer.Units)
        {
            int movePoints = u.MovementRating / 3; //Will round down.

            if ((movePoints % 3) >= (int) turnPulse)
            {
                movePoints += 1;
            }
            
            u.MovementPointsRemainingThisPulse = movePoints;
        }
    }
}
