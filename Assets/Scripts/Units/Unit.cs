﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QPath;
using System.Linq;

public class Unit : MapObject, IQPathUnit {

    
    public Unit()
    {
        // _unitNames = GameObject.FindObjectOfType<UnitNames>();
        // Name = _unitNames.GETNextName();
        Name = "ShipOne";
        MovementRating = 18;
        MovementPointsRemainingThisPulse = 6;
        SteerRating = 0;
        SteerPointsRemaining = 0;
    }

    public Direction Facing  = Direction.East;
    public int Movement = 2;
    public int MovementRemaining = 2;
    public bool CanBuildCities = false;
    public bool SkipThisUnit = false;

    // This is all my pulse movement stuff.
    public int MovementRating;
    public int MovementPointsRemainingThisPulse;
    public int SteerRating;
    public int SteerPointsRemaining;
    public delegate void ObjectSteeredDelegate ( Direction newFacing ); 
    public event ObjectSteeredDelegate OnObjectSteered;

    private Queue<Move> _moveQueue;
    private UnitNames _unitNames;

    // public void SetName()
    // {
    //
    // }
    
    public void ClearMoveQueue()
    {
        SkipThisUnit = false;
        this._moveQueue = new Queue<Move>();
    }
    
    public void AddToMoveQueue( Move move )
    {
        if((_moveQueue==null))
        {
            this.ClearMoveQueue();
        }
        
        this._moveQueue.Enqueue(move);
    }
    
    public Move TakeFromMoveQueue()
    {
        Move move = this._moveQueue.Dequeue();
        return move;
    }
    
    public void SetMoveQueue( IEnumerable<Move> moveArray )
    {
        SkipThisUnit = false;
        this._moveQueue = new Queue<Move>(moveArray);
    }
    
    public Move[] GetMoveQueue()
    {
        return _moveQueue?.ToArray();
    }
    
    public int GetMoveQueueLength()
    {
        return this._moveQueue.Count;
    }
    
    public bool DoQueuedMove()
    {
        Debug.Log("Movement Points Remaining = " + MovementPointsRemainingThisPulse);
        
        // Do queued move
        if (MovementPointsRemainingThisPulse <= 0)
        {
            Debug.Log("No Movement Points Remaining");
            return false;
        }

        if(_moveQueue == null || _moveQueue.Count == 0)
        {
            Debug.Log("No Moves Queued");
            return false;
        }

        // Grab the first move from our queue
        Move currentMove = _moveQueue.Dequeue();

        // Spend a move point.
        MovementPointsRemainingThisPulse -= 1;
        
        // Do the move
        currentMove.MoveUnit(this);

        return _moveQueue != null && MovementPointsRemainingThisPulse > 0;
    }
    
    /// <summary>
    /// List of hexes to walk through (from pathfinder).
    /// NOTE: First item is always the hex we are standing in.
    /// </summary>
    List<Hex> _hexPath;

    // TODO: This should probably be moved to some kind of central option/config file
    const bool MOVEMENT_RULES_LIKE_CIV6 = true;

    public void DUMMY_PATHING_FUNCTION()
    {

        /*QPath.CostEstimateDelegate ced = (IQPathTile a, IQPathTile b) => (
            return Hex.Distance(a, b);
        );*/

        Hex[] pathHexes = QPath.QPath.FindPath<Hex>( 
            Hex.HexMap, 
            this,
            Hex, 
            Hex.HexMap.GetHexAt( Hex.Q + 6, Hex.R ), 
            Hex.CostEstimate 
        );
            
        Debug.Log("Got pathfinding path of length: " + pathHexes.Length);

        SetHexPath(pathHexes);
    }

    public void ClearHexPath()
    {
        SkipThisUnit = false;
        this._hexPath = new List<Hex>();
    }

    public void SetHexPath( Hex[] hexArray )
    {
        SkipThisUnit = false;
        this._hexPath = new List<Hex>( hexArray );
    }

    public Hex[] GetHexPath()
    {
        return _hexPath?.ToArray();
    }

    public int GetHexPathLength()
    {
        return this._hexPath.Count;
    }

    public bool UnitWaitingForOrders()
    {
        if(SkipThisUnit)
        {
            return false;
        }

        // Returns true if we have movement left but nothing queued
        if( 
            MovementRemaining > 0 && 
            (_hexPath==null || _hexPath.Count==0) 
            // TODO: Maybe we've been told to Fortify/Alert/SkipTurn
        )
        {
            return true;
        }

        return false;
    }

    public void RefreshMovement()
    {
        SkipThisUnit = false;
        MovementRemaining = Movement;
    }
    
    public void ResetSteerPoints()
    {
        SteerPointsRemaining = SteerRating;
    }
    
    public void DoSteerCounterClockwise()
    {
        this.DoSteer(SteerDirection.Counterclockwise);
    }
    
    public void DoSteerClockwise()
    {
        this.DoSteer(SteerDirection.Clockwise);
    }
    
    public void DoSteer(SteerDirection steer)
    {
        if(SteerPointsRemaining == 0){
            Facing = Facing.DirectionAfterSteer(steer);
            ResetSteerPoints();
            OnObjectSteered(Facing);
        }
    }
    public void DoMoveAhead()
    {
        Hex hexUnitIsFacing = Hex.getHexInDirection(Facing);
        SetHex(hexUnitIsFacing);
    }

    /// <summary>
    /// Processes one tile worth of movement for the unit
    /// </summary>
    /// <returns>Returns true if this should be called immediately again.</returns>
    public bool DoMove()
    {
        Debug.Log("DoMove");
        // Do queued move

        if(MovementRemaining <= 0)
            return false;

        if(_hexPath == null || _hexPath.Count == 0)
        {
            return false;
        }

        // Grab the first hex from our queue
        Hex hexWeAreLeaving = _hexPath[0];
        Hex newHex = _hexPath[1];

        int costToEnter = MovementCostToEnterHex( newHex );

        if( costToEnter > MovementRemaining && MovementRemaining < Movement && MOVEMENT_RULES_LIKE_CIV6 )
        {
            // We can't enter the hex this turn
            return false;
        }

        _hexPath.RemoveAt(0);

        if( _hexPath.Count == 1 )
        {
            // The only hex left in the list, is the one we are moving to now,
            // therefore we have no more path to follow, so let's just clear
            // the queue completely to avoid confusion.
            _hexPath = null;
        }

        // Move to the new Hex
        SetHex( newHex );
        MovementRemaining = Mathf.Max(MovementRemaining-costToEnter, 0);

        return _hexPath != null && MovementRemaining > 0;
    }

    public int MovementCostToEnterHex( Hex hex )
    {
        // TODO:  Implement different movement traits

        return hex.BaseMovementCost( false, false, false );
    }

    public float AggregateTurnsToEnterHex( Hex hex, float turnsToDate )
    {
        // The issue at hand is that if you are trying to enter a tile
        // with a movement cost greater than your current remaining movement
        // points, this will either result in a cheaper-than expected
        // turn cost (Civ5) or a more-expensive-than expected turn cost (Civ6)

        float baseTurnsToEnterHex = MovementCostToEnterHex(hex) / Movement; // Example: Entering a forest is "1" turn

        if(baseTurnsToEnterHex < 0)
        {
            // Impassible terrain
            //Debug.Log("Impassible terrain at:" + hex.ToString());
            return -99999;
        }

        if(baseTurnsToEnterHex > 1)
        {
            // Even if something costs 3 to enter and we have a max move of 2, 
            // you can always enter it using a full turn of movement.
            baseTurnsToEnterHex = 1;
        }


        float turnsRemaining = MovementRemaining / Movement;    // Example, if we are at 1/2 move, then we have .5 turns left

        float turnsToDateWhole = Mathf.Floor(turnsToDate); // Example: 4.33 becomes 4
        float turnsToDateFraction = turnsToDate - turnsToDateWhole; // Example: 4.33 becomes 0.33

        if( (turnsToDateFraction > 0 && turnsToDateFraction < 0.01f) || turnsToDateFraction > 0.99f )
        {
            Debug.LogError("Looks like we've got floating-point drift: " + turnsToDate);

            if( turnsToDateFraction < 0.01f )
                turnsToDateFraction = 0;

            if( turnsToDateFraction > 0.99f )
            {
                turnsToDateWhole   += 1;
                turnsToDateFraction = 0;
            }
        }

        float turnsUsedAfterThismove = turnsToDateFraction + baseTurnsToEnterHex; // Example 0.33 + 1

        if(turnsUsedAfterThismove > 1)
        {
            // We have hit the situation where we don't actually have enough movement to complete this move.
            // What do we do?

            if( MOVEMENT_RULES_LIKE_CIV6 )
            {
                // We aren't ALLOWED to enter the tile this move. That means, we have to...

                if(turnsToDateFraction == 0)
                {
                    // We have full movement, but this isn't enough to enter the tile
                    // EXAMPLE: We have a max move of 2 but the tile costs 3 to enter.
                    // We are good to go.
                }
                else
                {
                    // We are NOT on a fresh turn -- therefore we need to 
                    // sit idle for the remainder of this turn.
                    turnsToDateWhole   += 1;
                    turnsToDateFraction = 0;
                }

                // So now we know for a fact that we are starting the move into difficult terrain
                // on a fresh turn.
                turnsUsedAfterThismove = baseTurnsToEnterHex;
            }
            else
            {
                // Civ5-style movement state that we can always enter a tile, even if we don't
                // have enough movement left.
                turnsUsedAfterThismove = 1;
            }
        }

        // turnsUsedAfterThismove is now some value from 0..1. (this includes
        // the factional part of moves from previous turns).


        // Do we return the number of turns THIS move is going to take?
        // I say no, this an an "aggregate" function, so return the total
        // turn cost of turnsToDate + turns for this move.

        return turnsToDateWhole + turnsUsedAfterThismove;

    }
    


    override public void SetHex( Hex newHex )
    {
        if(Hex != null)
        {
            Hex.RemoveUnit(this);
        }

        base.SetHex( newHex );

        Hex.AddUnit(this);
    }

    override public void Destroy(  )
    {
        base.Destroy(  );

        Hex.RemoveUnit(this);
    }

    /// <summary>
    /// Turn cost to enter a hex (i.e. 0.5 turns if a movement cost is 1 and we have 2 max movement)
    /// </summary>
    public float CostToEnterHex( IQPathTile sourceTile, IQPathTile destinationTile )
    {
        return 1;
    }
}
