using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainZombie : MonoBehaviour
{

    public float speed; // 이동 속도

    public GameObject object_head;
    public GameObject[] object_L;
    public GameObject[] object_R;

    private Rigidbody rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        RandomAppearance();

        animator.SetBool("isWalk", true);
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    void RandomAppearance()
    {
        int number = Random.Range(0, 7);

        if (number <= 1)
            object_head.SetActive(false);

        if (number == 0 || number == 2)
        {
            number = Random.Range(1, object_L.Length + 1);

            for (int i = 0; i < number; i++)
                object_L[i].SetActive(false);
        }
        else if (number == 1 || number == 3)
        {
            number = Random.Range(1, object_R.Length + 1);

            for (int i = 0; i < number; i++)
                object_R[i].SetActive(false);
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Bullet"))
            transform.RotateAround(transform.position, Vector3.up, Random.Range(0, 361));
    }
}
