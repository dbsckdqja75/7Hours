using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveLayer : MonoBehaviour
{

    public string backTag;
    public string nextTag;

    void Update()
    {
        if (GameManager.isWave)
            gameObject.tag = nextTag;
        else
            gameObject.tag = backTag;
    }
}
