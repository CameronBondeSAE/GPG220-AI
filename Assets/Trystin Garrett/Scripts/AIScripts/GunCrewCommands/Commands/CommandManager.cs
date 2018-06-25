using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Trystin
{
    public class CommandManager : MonoBehaviour
    {
        public static CommandManager Instance;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        //TODO: When giving commands to a GCM that already has a movment command, Get it to clearup to current order then enstate new.... Also having a seperate Gameobject to house orders to avoid clutter
        public void IssueMovementCommand(GunCrewMember _GCM, Node _TargetNode)
        {
            MovementCommand NewMC = _GCM.gameObject.AddComponent<MovementCommand>();
            NewMC.PassInfo(_GCM, _TargetNode);
        }

        public void IssueLoadGunCommand(GunCrewMember _GCM, FieldGun _FG)
        {
            LoadGunCommand NewLGC = _GCM.gameObject.AddComponent<LoadGunCommand>();
            NewLGC.PassInfo(_GCM, _FG);
        }

        public void IssueSpotForGunCommand(GunCrewMember _GCM, FieldGun _FG)
        {
            SpotForGunCommand NewSFGC = _GCM.gameObject.AddComponent<SpotForGunCommand>();
            NewSFGC.PassInfo(_GCM, _FG);
        }

    }
}
