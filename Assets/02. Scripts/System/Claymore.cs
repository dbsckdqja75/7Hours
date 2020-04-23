using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claymore : MonoBehaviour
{

    public GameObject explosionEffect;
    public float radius;

    public Collider[] cols;

    private bool onExplosion;

    void Update()
    {
        int layerId = 10;
        int layerMask = 1 << layerId;

        cols = Physics.OverlapSphere(transform.position, radius, layerMask);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Zombie"))
            Explosion();
    }

    void Explosion()
    {
        if (!onExplosion)
        {
            onExplosion = true;

            foreach (Collider col in cols)
                col.gameObject.SendMessage("ApplyDamage", 100, SendMessageOptions.DontRequireReceiver);

            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, radius * 2);
    }
}
