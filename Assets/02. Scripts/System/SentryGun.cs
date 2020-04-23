using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryGun : MonoBehaviour
{

    [Space(10), Header("General Settings")]
    public int hp = 300; // 체력

    public float fireInterval = 0.1f; // 발포 간격
    public float radius;

    [Space(10), Header("Object Settings")]
    public GameObject fireEffect; // 발포 효과
    public GameObject fireLight; // 발포 빛 효과

    public GameObject bulletPrefab; // 총알 프리팹

    [Space(10), Header("Transform Settings")]
    public Transform firePoint; // 발포 위치
    public Transform fireHead; // 본체 머리 
    public Transform target; // 타겟

    [Space(10), Header("Sound Settings")]
    public AudioSource fireAudioSource;
    public AudioSource effectAudioSource;

    public AudioClip spotClip;

    [Space(10)]
    public Collider[] cols;

    private float fireTimer;

    private Vector3 targetPos;
    private Collider col;
    private Rigidbody rb;

    Quaternion targetRotation;

    void Awake()
    {
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        GameManager.SentryAmmoPM(60);
    }

    void Update()
    {
        int layerId = 10;
        int layerMask = 1 << layerId;

        cols = Physics.OverlapSphere(fireHead.position, radius, layerMask);

        if (!target && cols.Length > 0 && GameManager.sentryAmmo > 0)
        {
            target = cols[0].transform;
            effectAudioSource.PlayOneShot(spotClip);
        }

        if (target && GameManager.sentryAmmo > 0)
            Targeting();
        else
            StopFire();

        if (GameManager.isWave)
            gameObject.layer = 13;
        else
            gameObject.layer = 14;
    }

    void Targeting()
    {
        if (target)
        {
            targetPos = new Vector3(target.position.x, fireHead.position.y, target.position.z);

            targetRotation = Quaternion.LookRotation(targetPos - fireHead.position);

            fireHead.rotation = Quaternion.Slerp(fireHead.rotation, targetRotation, 10 * Time.deltaTime);

            if (!target.GetComponent<CapsuleCollider>().enabled)
                target = null;

            Fire();
        }
    }

    void Fire()
    {
        if (GameManager.sentryAmmo > 0)
        {
            if (fireTimer < 0.2)
            {
                fireEffect.SetActive(false);
                fireLight.SetActive(false);
            }
            else if (fireTimer < 0)
            {
                fireEffect.SetActive(true);
                fireLight.SetActive(true);
            }

            if (fireTimer > 0)
                fireTimer -= Time.deltaTime;
            else
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                fireTimer = fireInterval;

                fireEffect.SetActive(true);
                fireLight.SetActive(true);

                GameManager.SentryAmmoPM(-1);
                
                if (!fireAudioSource.isPlaying)
                {
                    fireAudioSource.loop = true;
                    fireAudioSource.Play();
                }
            }
        }
        else
        {
            StopFire();
        }
    }

    void StopFire()
    {
        fireEffect.SetActive(false);
        fireLight.SetActive(false);

        fireAudioSource.loop = false;

        fireTimer = 0.05f;
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, (radius * 2));
    }
}
