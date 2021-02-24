﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using QPath;

/// <summary>
/// The Hex class defines the grid position, world space position, size, 
/// neighbours, etc... of a Hex Tile. However, it does NOT interact with
/// Unity directly in any way.
/// </summary>
public class Hex : IQPathTile {

    public Hex(HexMap hexMap, int q, int r)
    {
        this.HexMap = hexMap;

        this.Q = q;
        this.R = r;
        this.S = -(q + r);

        units = new HashSet<Unit>();
    }

    public Hex getHexInDirection(Direction direction)
    {
        var newQ =  (this.Q + direction.Q);
        var newR = (this.R + direction.R);
        Hex hexInDirection = HexMap.GetHexAt(newQ, newR);
        return hexInDirection;

    }

    // Q + R + S = 0
    // S = -(Q + R)

    public readonly int Q;  // Column
    public readonly int R;  // Row
    public readonly int S;

    // Data for map generation and maybe in-game effects
    public float Elevation;
    public float Moisture;

    public enum TERRAIN_TYPE { PLAINS, GRASSLANDS, MARSH, FLOODPLAINS, DESERT, LAKE, OCEAN }
    public enum ELEVATION_TYPE { FLAT, HILL, MOUNTAIN, WATER }

    public TERRAIN_TYPE TerrainType { get; set; }
    public ELEVATION_TYPE ElevationType { get; set; }

    public enum FEATURE_TYPE { NONE, FOREST, RAINFOREST, MARSH }
    public FEATURE_TYPE FeatureType { get; set; }


    // TODO: Need some kind of property to track hex time (plains, grasslands, etc...)
    // TODO: Need property to track hex detail (forest, mine, farm, etc...)

    public readonly HexMap HexMap;

    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;

    float radius = 1f;

    HashSet<Unit> units;
    public Unit[] Units { 
        get{
            return units.ToArray();
        }
    }
    public City City { get; protected set; }

    public override string ToString()
    {
        return Q + ", " + R;
    }

    /// <summary>
    /// Returns the world-space position of this hex
    /// </summary>
    public Vector3 Position()
    {
        return new Vector3(
            HexHorizontalSpacing() * (this.Q + this.R/2f),
            0,
            HexVerticalSpacing() * this.R
        );
    }

    public float HexHeight()
    {
        return radius * 2;
    }

    public float HexWidth()
    {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f;
    }

    public float HexHorizontalSpacing()
    {
        return HexWidth();
    }

    public Vector3 PositionFromCamera()
    {
        return HexMap.GetHexPosition(this);
    }

    public Vector3 PositionFromCamera( Vector3 cameraPosition, float numRows, float numColumns )
    {
        float mapHeight = numRows * HexVerticalSpacing();
        float mapWidth  = numColumns * HexHorizontalSpacing();

        Vector3 position = Position();

        if(HexMap.AllowWrapEastWest)
        {
            float howManyWidthsFromCamera = Mathf.Round((position.x - cameraPosition.x) / mapWidth);
            int howManyWidthToFix = (int)howManyWidthsFromCamera;

            position.x -= howManyWidthToFix * mapWidth;
        }

        if(HexMap.AllowWrapNorthSouth)
        {
            float howManyHeightsFromCamera = Mathf.Round((position.z - cameraPosition.z) / mapHeight);
            int howManyHeightsToFix = (int)howManyHeightsFromCamera;

            position.z -= howManyHeightsToFix * mapHeight;
        }


        return position;
    }

    public static float CostEstimate(IQPathTile aa, IQPathTile bb)
    {
        return Distance((Hex)aa, (Hex) bb);
    }

    public static float Distance(Hex a, Hex b)
    {
        // WARNING: Probably Wrong for wrapping
        int dQ = Mathf.Abs(a.Q - b.Q);
        if(a.HexMap.AllowWrapEastWest)
        {
            if(dQ > a.HexMap.NumColumns / 2)
                dQ = a.HexMap.NumColumns - dQ;
        }

        int dR = Mathf.Abs(a.R - b.R);
        if(a.HexMap.AllowWrapNorthSouth)
        {
            if(dR > a.HexMap.NumRows / 2)
                dR = a.HexMap.NumRows - dR;
        }

        return 
            Mathf.Max( 
                dQ,
                dR,
                Mathf.Abs(a.S - b.S)
            );
    }

    public void AddUnit( Unit unit )
    {
        if(units == null)
        {
            units = new HashSet<Unit>();
        }

        units.Add(unit);
    }

    public void RemoveUnit( Unit unit )
    {
        if(units != null)
        {
            units.Remove(unit);
        }
    }

    public void AddCity( City city )
    {
        if(this.City != null)
        {
            throw new UnityException("Trying to add a city to a hex that already has one!");
        }

        this.City = city;
    }

    public void RemoveCity( City city )
    {
        if(this.City == null)
        {
            Debug.LogError("Trying to remove a city where there isn't one!");
            return;
        }

        if(this.City != city)
        {
            Debug.LogError("Trying to remove a city that isn't ours!");
            return;
        }

        this.City = null;
    }

    /// <summary>
    /// Returns the most common movement cost for this tile, for a typical melee unit
    /// </summary>
    public int BaseMovementCost( bool isHillWalker, bool isForestWalker, bool isFlyer )
    {
        if( (ElevationType == ELEVATION_TYPE.MOUNTAIN || ElevationType == ELEVATION_TYPE.WATER) && isFlyer == false )
            return -99;

        int moveCost = 1;

        if( ElevationType == ELEVATION_TYPE.HILL && isHillWalker == false )
            moveCost += 1;

        if( (FeatureType == FEATURE_TYPE.FOREST || FeatureType == FEATURE_TYPE.RAINFOREST) && isForestWalker == false )
            moveCost += 1;

        return moveCost;
    }

    Hex[] neighbours;

    #region IQPathTile implementation
    public IQPathTile[] GetNeighbours()
    {
        if(this.neighbours != null)
            return this.neighbours;
        
        List<Hex> neighbours = new List<Hex>();

        neighbours.Add( HexMap.GetHexAt( Q +  1,  R +  0 ) );
        neighbours.Add( HexMap.GetHexAt( Q + -1,  R +  0 ) );
        neighbours.Add( HexMap.GetHexAt( Q +  0,  R + +1 ) );
        neighbours.Add( HexMap.GetHexAt( Q +  0,  R + -1 ) );
        neighbours.Add( HexMap.GetHexAt( Q + +1,  R + -1 ) );
        neighbours.Add( HexMap.GetHexAt( Q + -1,  R + +1 ) );

        List<Hex> neighbours2 = new List<Hex>();

        foreach(Hex h in neighbours)
        {
            if(h != null)
            {
                neighbours2.Add(h);
            }
        }

        this.neighbours = neighbours2.ToArray();

        return this.neighbours;
    }

    public float AggregateCostToEnter(float costSoFar, IQPathTile sourceTile, IQPathUnit theUnit)
    {
        // TODO: We are ignoring source tile right now, this will have to change when
        // we have rivers.
        return ((Unit)theUnit).AggregateTurnsToEnterHex(this, costSoFar);
    }
    #endregion
}
