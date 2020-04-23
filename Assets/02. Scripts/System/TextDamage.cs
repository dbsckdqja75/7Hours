using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDamage : MonoBehaviour
{
    void Start()
    {
        transform.position += Vector3.up * 2;

        Destroy(gameObject, 0.5f);
    }

    void Update()
    {
        transform.position = transform.position + Vector3.up * 4 * Time.deltaTime;
    }
}
