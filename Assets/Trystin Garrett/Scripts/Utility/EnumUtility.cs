using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public enum FieldGunState
    {
        Idle,
        Loading,
        Rotating,
        MovingBarrel,
        Firing
    }


    public enum AIStatus
    {
        InActive,
        Active
    }


    public enum ArtillerySpawnStatus
    {
        UnSpawned,
        SpawnRequested,
        GunDropPointFound,
        GunDroppingIn,
        GunLanded,
        DroppingCrew,
        CrewLanded,
        Ready
    }


    public enum ArtilleryCrewRole
    {
        UnAssigned,
        RadioOperator,
        RunnerSpotter,
        Spotter,
        GunOperator
    }

    public enum GunLoaderState
    {
        Idle,
        RequestingPathToBreach,
        MovingToBreach,
        AtBreach,
        LoadingShell,
        LoadingComplete
    }

    public enum GunSpotterState
    {
        Idle,
        RequestingPathToSpotterPos,
        MovingToSpotterPos,
        AtSpotterPos,
        ScanningForTargets,
        TargetFound,
        CallingInStrike
    }

    public enum RunnerSpotterState
    {
        Idle,
        SearchingForTarget,
        TargetFound,
        CallingInStrike,
        Retreating
    }


    public enum PathRequestStatus
    {
        NoneRequested,
        RequestedAndWaiting,
        RecivedAndValid
    }

    public enum MovementStatus
    {
        Null,
        Idle,
        MovingToNextWapoint,
        ArrivedAtWaypoint,
        ArrivedAtTargetNode
    }

    public enum MovementSpeed
    {
        Idle,
        Walking,
        Jogging,
        Running,
        Sprinting
    }

    public enum WaypointStatus
    {
        AtWaypoint,
        AtTargetNode,
        BetweenWaypoints
    }

    public enum Direction
    {
        SouthWest,
        West,
        NorthWest,
        South,
        Centre,
        North,
        SouthEast,
        East,
        NorthEast,
    }

    public enum ArtilleryOrderStatus
    {
        InActive,
        InitialOrders,
        Monitoring,
        Exploration,
        FindingTarget,
        ReOrganisingCrew
    }
}
