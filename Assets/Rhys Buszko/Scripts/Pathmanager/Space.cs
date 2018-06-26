using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space {

    public bool Type;
    public Vector3 Location;

    public Space(bool _type, Vector3 WorldPos )
    {
        Type = _type;
        Location = WorldPos;
    }
}
