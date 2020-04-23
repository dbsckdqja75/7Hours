using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public GameObject explosionEffect;
    public float radius;

    public Collider[] cols;

    private bool onExplosion;

    private GameObject togetherBomb;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.velocity = Vector3.zero;

        rb.AddForce(transform.forward * 3000);
        rb.AddForce(transform.up * 2000);
    }

    void Update()
    {
        int layerId = 10;
        int layerMask = 1 << layerId;

        cols = Physics.OverlapSphere(transform.position, radius, layerMask);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Floor"))
            Invoke("Explosion", 0.5f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Claymore"))
            togetherBomb = col.gameObject;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Claymore"))
            togetherBomb = null;
    }

    void Explosion()
    {
        if(!onExplosion)
        {
            onExplosion = true;

            foreach(Collider col in cols)
                col.gameObject.SendMessage("ApplyDamage", 60, SendMessageOptions.DontRequireReceiver);

            if(togetherBomb)
                togetherBomb.SendMessage("Explosion", SendMessageOptions.DontRequireReceiver);

            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }
}
