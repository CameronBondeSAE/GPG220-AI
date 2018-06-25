using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Trystin
{
    public abstract class Command : MonoBehaviour
    {

        public abstract void Start();

        public abstract void OnEnterCommand();

        public abstract void Update();

        public abstract void FixedUpdate();

        public abstract void OnExitCommand();

        public abstract void DecommissionCommand();
    }
}
