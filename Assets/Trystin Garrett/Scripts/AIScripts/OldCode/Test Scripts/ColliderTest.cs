using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest : MonoBehaviour {

    public Vector3 TileSize = new Vector3(1, 1, 1);
    public float X;
    public float Y;

    private void FixedUpdate()
    {
        ColliderOverlapCheck();
    }

    public bool ColliderOverlapCheck()
    {
        bool TooMuchOverLap = false;

        Collider[] NC = Physics.OverlapBox(transform.position, TileSize / 2);


        if (NC == null)
            return false;

        if (NC.Length > 1)
        {
            for (int index = 0; index < NC.Length; ++index)
            {
                TooMuchOverLap = CheckColliderDistance(NC[index]);
            }
        }
        else if (NC.Length == 1)
        {
            TooMuchOverLap = CheckColliderDistance(NC[0]);
        }

        if (TooMuchOverLap == true)
            Debug.Log("Nope!");

        return TooMuchOverLap;
    }

    bool CheckColliderDistance(Collider _Col)
    {
        bool NoGo = false;
        Vector3 ContactPoints = _Col.ClosestPoint(transform.position);

        float XBounds = ContactPoints.x - transform.position.x;
        float YBounds = ContactPoints.z - transform.position.z;

        XBounds = Mathf.Abs(XBounds);
        YBounds = Mathf.Abs(YBounds);
        X = XBounds;
        Y = YBounds;

        if (XBounds < 0.3f && XBounds > 0f && YBounds == 0f)
            NoGo = true;
        if (YBounds < 0.3f && YBounds > 0f && XBounds == 0f)
            NoGo = true;
        if (YBounds < 0.3f && XBounds < 0.3f)
            NoGo = true;
        return NoGo;
    }
}
