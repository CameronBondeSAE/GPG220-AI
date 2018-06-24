using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Trystin
{
    public class CommandManager : MonoBehaviour
    {
        public GunCrewMember TestMember;
        public Transform GridPositionToMoveTo;


        //TODO: When giving commands to a GCM that already has a movment command, Get it to clearup to current order then enstate new....
        void IssueMovementCommand(GunCrewMember _GCM, Node _TargetNode)
        {
            MovementCommand NewMC = _GCM.gameObject.AddComponent<MovementCommand>();
            NewMC.PassInfo(_GCM, _TargetNode);
        }



        //Just a testing script
        public void TestMovementCommand()
        {
            Node TargetNode = NodeManager.Instance.FindNodeFromWorldPosition(GridPositionToMoveTo.position);
            IssueMovementCommand(TestMember, TargetNode);
        }
    }
}
