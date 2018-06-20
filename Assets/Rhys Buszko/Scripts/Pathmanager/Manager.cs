using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhys
{
    public class Manager : Singleton<Manager>
    {
        protected Manager() { } // guarantee this will be always a singleton only - can't use the constructor!

        public string myGlobalVar = "whatever";
        private Collider bounding;

        public GameObject[,] map;

        public GameObject node = new GameObject("node");

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



              int length = (int) bounding.bounds.size.x ;
              int height = (int) bounding.bounds.size.z;
            

            map = new GameObject[length, height];

            for(int i = 0; i < length; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    map[i, j] = Instantiate(node, new Vector3(i, 1, j), new Quaternion(0, 0, 0, 0));
                    print(map[i, j]);
                }
            }
        }
    }
}
