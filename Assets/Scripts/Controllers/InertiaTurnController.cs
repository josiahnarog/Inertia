using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;


public class InertiaTurnController : MonoBehaviour
{
    private HexMap _hexMap;
    private SelectionController _selectionController;
    private Player[] _players;
    private Player[] _initiativeOrder;
    public int numPlayers = 2;

    public GameObject advanceTurnPulseButton;
    public GameObject nextUnitButton;

    public bool areMovesRemaining;
    public int currentPlayerIndex;

    private Random r = new Random();
    public enum TacticalTurnPulse
    {
        Setup = -1,
        StartTurn =0,
        FirstMove = 1,
        SecondMove = 2,
        ThirdMove = 3,
        Firing = 4,
        EndTurn = 5
    }
    
    public List<TacticalTurnPulse> movePulses;
    
    public TacticalTurnPulse turnPulse;

    private void Start ()
    {
        _hexMap = GameObject.FindObjectOfType<HexMap>();
        _selectionController = GameObject.FindObjectOfType<SelectionController>();
        turnPulse = TacticalTurnPulse.Setup;
        
        movePulses = new List<TacticalTurnPulse>()
        {
            TacticalTurnPulse.FirstMove,
            TacticalTurnPulse.SecondMove,
            TacticalTurnPulse.ThirdMove
        };
        
        GeneratePlayers(numPlayers);
        AdvanceTurnPulse();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Current Pulse: " + turnPulse);
    }
    
    private void GeneratePlayers( int numPlayers )
    {
        _players = new Player[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            _players[i] = new Player( "Player " + (i+1).ToString() );
            _players[i].Type = Player.PlayerType.AI;
        }
        //CurrentPlayer = Players[0];
        _players[0].Type = Player.PlayerType.LOCAL;
        currentPlayerIndex = 0;
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
            _initiativeOrder = DetermineInitiativeOrder();
            // TODO RefreshTurnMoveRatings(); 
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
            _selectionController.SelectedUnit = null;
            // hexMap.AdvanceToNextPlayer();
        }
    }

    private int d100()
    {
        return r.Next(0, 100);
    }
    private Player[] DetermineInitiativeOrder()
    {
        
        int[] playerInitiative = new int[_players.Length];
        for (int player = 0; player < _players.Length; player++)
        {
            playerInitiative[player] = d100() + _players[player].InitiativeBonus;
        }

        var sorted = playerInitiative
            .Select((x, i) => new KeyValuePair<int, int>(x, i))
            .OrderByDescending(x => x.Key)
            .ToList();
        
        List<int> _initIndex = sorted.Select(x => x.Value).ToList();

        foreach(var item in playerInitiative)
        {
            Debug.Log("Player's rolled: " + item.ToString());
        }

        Player[] intiativeOrder = new Player[_players.Length];
        
        for (int order = 0; order < _players.Length; order++)
        {
            intiativeOrder[order] = _players[_initIndex[order]];
        }
        
        foreach(var item in intiativeOrder)
        {
            Debug.Log("Player Order: " + item.PlayerName.ToString());
        }

        return intiativeOrder;
    }
    
    IEnumerator DoAllQueuedUnitMoves()
    {
        Debug.Log("Doing DoAllQueuedUnitMoves.");
        advanceTurnPulseButton.SetActive(false); //Don't want to advance while moving.
        areMovesRemaining = true;
        while (areMovesRemaining)
        {
            foreach (Unit u in _hexMap.CurrentPlayer.Units)
            {
                areMovesRemaining = false; //Assume we won't loop again unless needed.
                
                var unitGO = _hexMap.GetUnitGO(u);
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
        foreach (Unit u in _hexMap.CurrentPlayer.Units)
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
