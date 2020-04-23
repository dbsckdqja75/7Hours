using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour
{

    public int hp = 150; // 체력

    private Collider col;
    private Rigidbody rb;

    void Awake()
    {
        col = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GameManager.isWave)
            gameObject.layer = 13;
        else
        {
            gameObject.layer = 14;
            hp = 150;
        }
    }

    void ApplyDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            col.isTrigger = true;
            rb.isKinematic = false;

            gameObject.tag = "Untagged";

            Destroy(gameObject, 3);
        }
    }
}
