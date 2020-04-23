using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSelf : MonoBehaviour
{
    public void SetObjActvie()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
