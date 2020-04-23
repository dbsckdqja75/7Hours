using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [Space(10), Header("General Settings")]
    public int hp = 100; // 체력

    public int ammo = 0; // 탄약
    public int magazine = 12; // 탄창 (탄약 세트)
    public int grenade = 3; // 수류탄 (최대 3개)

    public float speed = 4.0f; // 이동 속도
    public float fireInterval = 0.1f; // 발포 간격

    [Space(10), Header("Object Settings")]
    public GameObject fireEffect; // 발포 효과
    public GameObject fireLight; // 발포 빛 효과

    public GameObject bulletPrefab; // 총알 프리팹
    public GameObject grenadePrefab; // 수류탄 프리팹

    [Space(10), Header("Transform Settings")]
    public Transform firePoint; // 발포 위치
    public Transform grenadePoint;

    private Quaternion targetRotation;

    [Space(10), Header("UI Settings")]
    public Image hpSprite; // 캐릭터 캔버스 HP 이미지
    public Image hpBar; // UI 캔버스 HP 이미지

    public Text ammoText;
    public Text magazineText;

    public Image gameOverTest; // DevTest

    [Space(10), Header("Sound Settings")]
    public AudioSource weaponAudioSource;
    public AudioSource reloadAudioSoruce;

    [Space(10)]
    public AudioClip[] itemClips;
    public AudioClip[] reloadClips;

    private float fireTimer;
    private float reloadTimer;
    private float grenadeTimer;

    private float h;
    private float v;

    private bool isDead;

    private Vector3 movePoint;
    private Rigidbody rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && GameManager.isWave && !isDead)
            Fire();

        if (Input.GetMouseButtonUp(0))
            StopFire();

        if (Input.GetKeyDown(KeyCode.R) && !isDead)
            Reload();

        if (Input.GetKeyDown(KeyCode.G) && GameManager.isWave && !isDead && !Input.GetMouseButton(0) && !animator.GetBool("isReload"))
            GrenadeThrowing();

        if (!GameManager.isWave)
            StopFire();



        if (animator.GetBool("isReload"))
        {
            if (reloadTimer > 0)
                reloadTimer -= Time.deltaTime;
            else
            {
                animator.SetBool("isReload", false);

                ammo = 30;
                ammoText.text = ammo.ToString() + "/30";

                magazine--;
                magazineText.text = magazine.ToString();
            }
        }

        if(animator.GetBool("isGrenade"))
        {
            if (grenadeTimer > 0)
                grenadeTimer -= Time.deltaTime;
            else
                animator.SetBool("isGrenade", false);
        }

        SetHpSprite();

        if(!isDead)
            LookDirection();

        if (isDead)
        {
            gameOverTest.gameObject.SetActive(true);

            if(Input.GetKeyDown(KeyCode.X))
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
            
        AnimatorSetting();
    }

    void FixedUpdate()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Move(h, v);
    }

    void Move(float h, float v)
    {
        movePoint.Set(h, 0, v);
        movePoint = movePoint.normalized * speed * Time.deltaTime;

        rb.MovePosition(transform.position + movePoint);
    }

    void LookDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        /* Layer
        int layer1 = 0;
        int layer2 = 10;

        int layerMask1 = 1 << layer1;
        int layerMask2 = 1 << layer2;

        int finalMask = layerMask1 | layerMask2;
        answers.unity.com/questions/8715/how-do-i-use-layermasks.html */

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 target = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            if(hit.collider.gameObject.CompareTag("Zombie"))
                targetRotation = Quaternion.LookRotation(hit.collider.gameObject.transform.position - transform.position);
            else if(hit.collider.gameObject.CompareTag("Floor"))
                targetRotation = Quaternion.LookRotation(target - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }

    void Fire()
    {
        if (ammo > 0 && !animator.GetBool("isReload") && !animator.GetBool("isGrenade"))
        {
            if (fireTimer < 0.05)
                fireLight.SetActive(false);
            else if (fireTimer < 0)
                fireLight.SetActive(true);

            if (fireTimer > 0)
                fireTimer -= Time.deltaTime;
            else
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                fireTimer = fireInterval;

                animator.SetBool("isAttack", true);

                fireEffect.SetActive(true);
                fireLight.SetActive(true);

                CameraShake.shake = true;

                ammo--;
                ammoText.text = ammo.ToString() + "/30";

                if (!weaponAudioSource.isPlaying)
                {
                    weaponAudioSource.loop = true;
                    weaponAudioSource.Play();
                }
            }
        }
        else
        {
            StopFire();

            if(ammo <= 0)
                Reload();
        }
    }

    public void StopFire()
    {
        fireEffect.SetActive(false);
        fireLight.SetActive(false);

        animator.SetBool("isAttack", false);

        CameraShake.shake = false;

        weaponAudioSource.loop = false;

        fireTimer = 0.05f;
    }

    void Reload()
    {
        if (!animator.GetBool("isReload") && magazine > 0 && ammo < 30)
        {
            reloadTimer = 2.4f;

            animator.SetBool("isReload", true);

            PlaySound(reloadAudioSoruce, reloadClips[Random.Range(0, reloadClips.Length)]);
        }
    }
    
    void GrenadeThrowing()
    {
        if(!animator.GetBool("isGrenade") && grenade > 0)
        {
            grenadeTimer = 0.7f;

            animator.SetBool("isGrenade", true);

            Instantiate(grenadePrefab, grenadePoint.position, transform.rotation);

            grenade--;
        }
    }

    void SetHpSprite()
    {
        hpSprite.fillAmount = Mathf.Lerp(hpSprite.fillAmount, (float)hp / 100, 10 * Time.deltaTime);
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, (float)hp / 100, 10 * Time.deltaTime);

        if (hp <= 30)
        {
            hpSprite.color = Color.red;
            hpBar.color = Color.red;
        }
        else
        {
            hpSprite.color = Color.green;
            hpBar.color = Color.green;
        }
    }

    void Dead()
    {
        animator.SetTrigger("Dead");

        isDead = true;

        rb.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<CapsuleCollider>().enabled = false;
    }

    void ApplyDamage(int damage)
    {
        hp -= damage;

        animator.SetTrigger("Damage");

        if (hp <= 0)
            Dead();
    }

    void PlaySound(AudioSource audiosource, AudioClip audioclip)
    {
        audiosource.clip = audioclip;
        audiosource.Play();
    }

    public void HpPlus(int value)
    {
        hp += value;

        if (hp > 100)
            hp = 100;
    }

    public void MagazinePlus(int value)
    {
        magazine += value;
        magazineText.text = magazine.ToString();
    }

    void AnimatorSetting()
    {
        animator.SetFloat("h", h);
        animator.SetFloat("v", v);
        animator.SetFloat("r", transform.rotation.y);

        if (transform.rotation.eulerAngles.y > 315 || transform.rotation.eulerAngles.y < 45)
        {
            if (transform.rotation.eulerAngles.y > 315)
            {
                if (transform.rotation.y > 0)
                    animator.SetFloat("r", transform.rotation.y * -1);
                else if (transform.rotation.y < 0)
                    animator.SetFloat("r", transform.rotation.y);
            }
            else
            {
                if (transform.rotation.y < 0)
                    animator.SetFloat("r", transform.rotation.y * -1);
                else if (transform.rotation.y > 0)
                    animator.SetFloat("r", transform.rotation.y);
            }
        }

        if (transform.rotation.eulerAngles.y > 45 && transform.rotation.eulerAngles.y < 135)
        {
            if (transform.rotation.y < 0)
                animator.SetFloat("r", transform.rotation.y * -1);
            else if (transform.rotation.y > 0)
                animator.SetFloat("r", transform.rotation.y);
        }

        if (transform.rotation.eulerAngles.y > 135 && transform.rotation.eulerAngles.y < 225)
        {
            if (transform.rotation.eulerAngles.y > 135 && transform.rotation.eulerAngles.y < 180)
            {
                if (transform.rotation.y < 0)
                    animator.SetFloat("r", transform.rotation.y * -1);
                else if (transform.rotation.y > 0)
                    animator.SetFloat("r", transform.rotation.y);
            }
            else
            {
                if (transform.rotation.y > 0)
                    animator.SetFloat("r", transform.rotation.y * -1);
                else if (transform.rotation.y < 0)
                    animator.SetFloat("r", transform.rotation.y);
            }
        }

        if (transform.rotation.eulerAngles.y > 225 && transform.rotation.eulerAngles.y < 315)
        {
            if (transform.rotation.y > 0)
                animator.SetFloat("r", transform.rotation.y * -1);
            else if (transform.rotation.y < 0)
                animator.SetFloat("r", transform.rotation.y);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("HpItem"))
        {
            PlaySound(reloadAudioSoruce, itemClips[0]);

            HpPlus(5); // HpPlus-Player

            Destroy(col.gameObject);
        }

        if(col.gameObject.CompareTag("AmmoItem"))
        {
            PlaySound(reloadAudioSoruce, itemClips[1]);

            MagazinePlus(1); // MagazinePlus-Player

            Destroy(col.gameObject);
        }
    }
}
