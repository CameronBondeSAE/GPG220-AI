using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ResetScene : MonoBehaviour
{
    public float resetTime = 180;

    // Use this for initialization
    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(resetTime);
            Reset();
        }
    }

    void Reset()
    {
        SceneManager.LoadScene(0);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
        }
    }
}
