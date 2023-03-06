using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North, East, South, West
}

public enum DirectionChange
{
    None, TurnRight, TurnLeft, TurnAround
}

public static class DirectionChangeExtensions
{
    public static DirectionChange GetDirectionChangeTo(
        this Direction current, Direction next
    )
    {
        if (current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }
        return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction)
    {
        return (float)direction * 90f;
    }
}

public static class DirectionExtensions
{
    static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f),
    };

    public static Quaternion GetRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }

    static Vector3[] halfVectors =
    {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f,
    };

    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVectors[(int)direction];
    }
}

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow = default;

    GameTile north, east, south, west, nextOnPath;

    static Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);

    int distance;

    public bool HasPath => distance != int.MaxValue;

    public GameTile NextTileOnPath => nextOnPath;

    public Vector3 ExitPoint { get; private set; }

    public bool IsAlternative { get; set; }

    public Direction PathDirection { get; private set; }

    GameTileContent content;

    public GameTileContent Content
    {
        get => content;
        set
        {
            Debug.Assert(value != null, "Null assigned to content!");
            if (content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);
    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);

    public void ShowPath()
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation :
            westRotation;
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }

    GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        if (!HasPath || neighbor == null || neighbor.HasPath)
        {
            return null;
        }
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;

        neighbor.ExitPoint =
            neighbor.transform.localPosition + direction.GetHalfVector();

        neighbor.PathDirection = direction;
        return neighbor.Content.BlockPaths ? null : neighbor;
    }

    public static void MakeEastWestNeighbors (GameTile east, GameTile west)
    {
        Debug.Assert(
            west.east == null && east.west == null, "Redefined neighbors"
        );
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        Debug.Assert(
            south.north == null && north.south == null, "Redefined neighbors"
        );
        south.north = north;
        north.south = south;
    }
}
