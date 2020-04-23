using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieTypeA : MonoBehaviour
{

    [Space(10), Header("General Variables")]
    public float hp = 10; // 체력
    public float damage = 2; // 공격력 (회당)

    public float speed = 0.6f; // 이동 속도
    public float attackInterval = 2.0f; // 공격 간격

    [Space(10), Header("General Settings")]
    public float minHp = 10;
    public float maxHp = 15;

    public float minDamage = 2;
    public float maxDamage = 6;

    public bool onRandomAppearance;

    [Space(10), Header("Object Settings")]
    public GameObject healthUp;
    public GameObject magazineUp;
    public GameObject textDamage;

    [Space(10)]
    public GameObject object_head;
    public GameObject[] object_L;
    public GameObject[] object_R;

    [Space(10), Header("Sound Settings")]
    public AudioClip[] attackClips;
    public AudioClip[] damagedClips;

    public GameObject target;
    public GameObject subTarget;

    private float attackTimer;
    private bool isAttack;

    public float targetDinstance;
    public float subTargetDinstance;

    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        target = GameObject.FindWithTag("Player");

        if(onRandomAppearance)
            RandomAppearance();

        hp = Random.Range(minHp, maxHp+1);
        damage = Random.Range(minDamage, maxDamage+1);

        hp = hp * GameManager.multiple;
        damage = damage * GameManager.multiple;
        speed = speed * GameManager.multiple;

        animator.SetFloat("multiplie", GameManager.multiple);
    }

    void Update()
    {
        if(hp > 0)
        {
            LookDirection();

            if (target && !isAttack)
                animator.SetBool("isWalk", true);
            else
                animator.SetBool("isWalk", false);
        }

        if (target && !subTarget)
            targetDinstance = Vector3.Distance(transform.position, target.transform.position);
        else
        {
            targetDinstance = Vector3.Distance(transform.position, subTarget.transform.position);
            subTargetDinstance = Vector3.Distance(transform.position, target.transform.position);
        }

        if(targetDinstance < 3 && subTarget || subTargetDinstance > 10)
        {
            target = GameObject.FindWithTag("Player");
            subTarget = null;

            isAttack = false;
            LockPosition(false);

            subTargetDinstance = 0;
        }

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if(target && animator.GetBool("isWalk"))
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    void LookDirection()
    {
        Vector3 targetpos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        Quaternion targetRotation = Quaternion.LookRotation(targetpos - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
    }

    void Attack()
    {
        LockPosition(true);

        if (!animator.GetBool("isAttack"))
        {
            isAttack = true;

            if (attackTimer > 0)
            {
                // attackTimer -= Time.deltaTime;
                animator.SetTrigger("Idle");
            }
            else
            {
                target.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

                attackTimer = attackInterval;
                animator.SetTrigger("Attack");

                if (!audioSource.isPlaying)
                {
                    PlaySound(audioSource, attackClips[Random.Range(0, attackClips.Length)]);
                }
            }
        }
    }

    void LockPosition(bool TF)
    {
        if(TF)
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
    }

    void RandomAppearance()
    {
        int number = Random.Range(0, 7);

        if (number <= 1)
            object_head.SetActive(false);

        if (number == 0 || number == 2)
        {
            number = Random.Range(1, object_L.Length+1);

            for (int i = 0; i < number; i++)
                object_L[i].SetActive(false);
        }
        else if(number == 1 || number == 3)
        {
            number = Random.Range(1, object_R.Length + 1);

            for (int i = 0; i < number; i++)
                object_R[i].SetActive(false);
        }
    }

    void ApplyDamage(int damage)
    {
        hp -= damage;

        GameObject textObj = Instantiate(textDamage, transform.position, Quaternion.identity, transform);
        textObj.gameObject.GetComponentInChildren<Text>().text = "-" + damage.ToString();

        if (hp <= 0)
        {
            animator.SetTrigger("Dead");

            GameManager.MoneyPlus(Random.Range(10, 41));
            GameManager.KillsCountPlus();

            LockPosition(true);
            GetComponent<CapsuleCollider>().enabled = false;

            if (!audioSource.isPlaying)
            {
                PlaySound(audioSource, damagedClips[Random.Range(0, damagedClips.Length)]);
            }

            int spawnRandom = Random.Range(0, 11);

            if (spawnRandom < 1)
                Instantiate(healthUp, transform.position, Quaternion.identity);
            else if (spawnRandom < 2)
                Instantiate(magazineUp, transform.position, Quaternion.identity);

            gameObject.tag = "Untagged";

            Destroy(gameObject, 10);
        }
    }

    void PlaySound(AudioSource audiosource, AudioClip audioclip)
    {
        audiosource.clip = audioclip;
        audiosource.Play();
    }

    void OnCollisionStay(Collision col) 
    {
        if (col.gameObject.CompareTag("BatchType") && targetDinstance > 3)
        {
            subTarget = GameObject.FindWithTag("Player");
            target = col.gameObject;
        }

        if (col.gameObject.CompareTag("Player") || col.gameObject == target && !animator.GetBool("isAttack"))
            Attack();
    }

    void OnCollisionExit(Collision col) 
    {
        if (col.gameObject.CompareTag("Player") && !subTarget)
        {
            isAttack = false;
            LockPosition(false);
        }

        if (col.gameObject.CompareTag("BatchType"))
        {
            target = GameObject.FindWithTag("Player");
            subTarget = null;

            isAttack = false;
            LockPosition(false);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Bullet")) // 나중에 따로 처리
        {
            ApplyDamage(2);
        }
    }
}
