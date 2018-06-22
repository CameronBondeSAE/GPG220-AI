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
        MovingToBreach,
        AtBreach,
        LoadingShell,
        LoadingComplete
    }
}
