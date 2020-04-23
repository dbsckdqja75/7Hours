using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public Transform camTransform;

    public static bool shake;

    public float shakeAmount = 0.3f;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
            camTransform = GetComponent(typeof(Transform)) as Transform;
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        if (shake)
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
        else
            camTransform.localPosition = originalPos;
    }
}
