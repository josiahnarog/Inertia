﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;

public abstract class Enumeration : IComparable
{
    public string Name { get; private set; }

    public int Id { get; private set; }

    // protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    protected Enumeration(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
}

public enum SteerDirection
{
    Clockwise = 1,
    Ahead = 0,
    Counterclockwise = -1
}

public class Direction : Enumeration
{
    public int Q { get; private set; }
    public int R { get; private set; }
    public int Rotation { get; private set; }
    

    
    protected Direction(int id, string name, int rotation, int q, int r) : base(id, name)
    {
        this.Rotation = rotation;
        this.Q = q;
        this.R = r;

    }

    public static Direction East = new Direction(1, nameof(East), 0, 1, 0);
    public static Direction SouthEast = new Direction(2, nameof(SouthEast), 60, 0, 1);
    public static Direction SouthWest = new Direction(3, nameof(SouthWest), 120, -1, 1);
    public static Direction West = new Direction(4, nameof(West), 180, -1, 0);
    public static Direction NorthWest = new Direction(5, nameof(NorthWest), 240, 0, -1);
    public static Direction NorthEast = new Direction(6, nameof(NorthEast), 320, 1, -1);
    
    private static readonly List<Direction> Compass = new List<Direction>()
    {
        NorthEast, East, SouthEast, SouthWest, West, NorthWest, NorthEast, East
    };

    public Direction DirectionAfterSteer(SteerDirection steer)
    {
        Debug.Log("Input Direction enum: " + this.Id);
        // Debug.Log("Input Steer enum: " + (int) steer);
        int compassOffset = this.Id + (int) steer;

        Direction newDirection = Direction.Compass[compassOffset];
        Debug.Log("Output Direction Enum: " + newDirection.Id);
        return newDirection;
    }
}

public class Movement
{
 
    public Direction FacingAfterSteer()
    {
        return Direction.West;
    }
}