using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 3000.0f;

    public GameObject hitEffect;

    private Rigidbody rb;
    private Vector3 hitPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        transform.Rotate(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-0.5f, 0.5f));

        rb.AddForce(transform.forward * speed);

        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, 1000);

        Debug.DrawRay(transform.position, transform.forward * 1000, Color.red);

        if(hit.transform)
        {
            if (hit.collider.CompareTag("Zombie"))
                hitPos = hit.point;
        }

        Destroy(gameObject, 4);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Zombie"))
        {
            Instantiate(hitEffect, hitPos, col.gameObject.transform.rotation);

            Destroy(gameObject);
        }
    }
}
