using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhys
{
    public class Manager : Singleton<Manager>
    {
        protected Manager() { } // guarantee this will be always a singleton only - can't use the constructor!

        public string myGlobalVar = "whatever";
        public Collider bounding;

        public Space[,] map;

        private void Start()
        {
            GameObject[] setup = FindObjectsOfType<GameObject>();

            foreach(GameObject n in setup)
            {
                if(n.tag == "Bounds")
                {

                    bounding = n.GetComponent<Collider>() ;
                }
            }



              int length = (int) bounding.bounds.size.x;
              int height = (int) bounding.bounds.size.z;

            Vector3 local = new Vector3(0, 0, 0);

            map = new Space[length, height];

            for(int i = 0; i < length; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    local.x = length - i;
                    local.z = height - j;
                    map[i, j] = new Space(true, local);
                }
            }
        }
    }
}
