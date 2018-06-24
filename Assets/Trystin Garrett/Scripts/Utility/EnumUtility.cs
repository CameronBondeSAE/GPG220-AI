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


    public enum GunCrewRole
    {
        UnAssigned,
        Spotter,
        GunOperator
    }

    public enum SpotterSubRole
    {
        Idle,
        Runner,
        Radio
    }

    public enum GunnerSubRole
    {
        Idle,
        Loader,
        Lookout,
    }

    public enum GSLoaderState
    {
        Idle,
        RequestingPathToBreach,
        MovingToBreach,
        AtBreach,
        LoadingShell,
        LoadingComplete
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

    public enum SearchDirection
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
}
