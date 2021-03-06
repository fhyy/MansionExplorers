﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CommonOperations {
    public static Orientation getRandomOrientation()
    {
        int randomInt = (int)(Random.value * 4);
        switch (randomInt)
        {
            case 1:
                return Orientation.EAST;
            case 2:
                return Orientation.SOUTH;
            case 3:
                return Orientation.WEST;
            default:
                return Orientation.NORTH;
        }
    }

    public static T getRandomItemFromList<T>(T[] list)
    {
        int randomIndex = (int)(Random.value * (list.Length));
        if(randomIndex >= list.Length)
        {
            randomIndex = list.Length - 1;
        }
        return (T)list[randomIndex];
    }

    public static T getRandomItemFromList<T>(List<T> list)
    {
        int randomIndex = (int)(Random.value * (list.Count));
        if (randomIndex >= list.Count)
        {
            randomIndex = list.Count-1;
        }
        return (T)list[randomIndex];
    }

    public static Coordinate getTransformedCoordinate(Coordinate coordinateFromBase, Orientation orientation)
    {
        int x = coordinateFromBase.x;
        int y = coordinateFromBase.y;
        int z = coordinateFromBase.z;

        switch (orientation) {
            case Orientation.EAST:
                return new Coordinate(z, y, -x);
            case Orientation.SOUTH:
                return new Coordinate(-x, y, -z);
            case Orientation.WEST:
                return new Coordinate(-z, y, x);
            default:
                return coordinateFromBase;
        }
    }

    public static Orientation getOrientationBetweenCoordinates(Coordinate source, Coordinate target)
    {
        Coordinate coordDiff = target - source;
        if (System.Math.Abs(coordDiff.x) > System.Math.Abs(coordDiff.z))
        {
            if (coordDiff.x > 0)
                return Orientation.EAST;
            else
                return Orientation.WEST;
        }
        else
        {
            if (coordDiff.z < 0)
                return Orientation.SOUTH;
            else
                return Orientation.NORTH;
        }
    }

    private static ushort getOrientationNumber(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.EAST:
                return 1;
            case Orientation.SOUTH:
                return 2;
            case Orientation.WEST:
                return 3;
            default:
                return 4;
        }
    }

    private static Orientation getOrientationFromNumber(ushort orientationNum)
    {
        switch (orientationNum%4)
        {
            case 1:
                return Orientation.EAST;
            case 2:
                return Orientation.SOUTH;
            case 3:
                return Orientation.WEST;
            default:
                return Orientation.NORTH;
        }
    }

    public static Orientation getInvertedOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.EAST:
                return Orientation.WEST;
            case Orientation.SOUTH:
                return Orientation.NORTH;
            case Orientation.WEST:
                return Orientation.EAST;
            default:
                return Orientation.SOUTH;
        }
    }

    public static Orientation getTransformedOrientation(Orientation srcOrientation, Orientation rotatedOrientation)
    {
        ushort srcNum = getOrientationNumber(srcOrientation);
        ushort rotNum = getOrientationNumber(rotatedOrientation);
        return getOrientationFromNumber((ushort)(srcNum + rotNum));
    }
}
