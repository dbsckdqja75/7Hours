using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{

    public GameObject player;

    public float smoothSpeed = 3.0f;

    public Vector3 offset = new Vector3(0,9,-12);

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void FixedUpdate()
    {
        Vector3 viewPosition = player.transform.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, viewPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothPosition;
    }
}
