using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Space(10), Header("General Variables")]
    public int wave;

    public int spawnCount;
    public int aliveZombie;

    public static float multiple = 1;

    public static bool isWave;

    [Space(10), Header("General Settings")]
    public static int money;
    public static int kills;

    public static int sentryAmmo;

    public float spawnTimer;

    [Space(10), Header("Object Settings")]
    public Player player;

    [Space(10)]
    public GameObject normal_Zombie_A;
    public GameObject normal_Zombie_B;
    public GameObject normal_Zombie_C;

    [Space(10)]
    public GameObject running_Zombie_A;
    public GameObject running_Zombie_B;
    public GameObject running_Zombie_C;

    [Space(10)]
    public GameObject tyrant_Zombie;

    [Space(10), Header("Transform Settings")]
    public Transform[] spawnPoints;

    [Space(10), Header("UI Settings")]
    public Image waveShopPanel;
    public Image waveBatchPanel;
    public Image waveButtonPanel;

    public Image waveStartPanel;
    public Text waveStartText;

    public Text waveText;

    public Text moneyText;
    public Text killsText;

    public Text sentryAmmoText;
    public Text grenadeAmmoText;

    [Space(10), Header("Sound Settings")]
    public AudioSource effectAudioSource;
    public AudioSource musicAudioSource;

    [Space(10)]
    public AudioClip buyClip;
    public AudioClip approachingClip;
    
    [Space(10)]
    public AudioClip[] waveStartClip;
    public AudioClip[] waveClips;

    public int spawnCounter;

    void Start()
    {
        StartWave();
    }

    void Update()
    {
        if (isWave)
        {
            if (spawnTimer <= 0 && spawnCount > 0)
            {
                spawnTimer = 3;

                SpawnZombie();
            }
            else
                spawnTimer -= Time.deltaTime;

            OffShopPanel();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (money >= 20)
                {
                    player.MagazinePlus(1);
                    money -= 20;

                    PlaySound(effectAudioSource, buyClip);
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                if (money >= 60)
                {
                    SentryAmmoPM(30);
                    money -= 60;

                    PlaySound(effectAudioSource, buyClip);
                }
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                if (player.grenade < 3 && money >= 20)
                {
                    player.grenade++;
                    money -= 20;

                    PlaySound(effectAudioSource, buyClip);
                }
            }

            sentryAmmoText.text = "센트리건 탄약 : " + sentryAmmo.ToString("###,###,##0");
            grenadeAmmoText.text = "수류탄 탄약 : " + player.grenade + "/3";

            OnShopPanel();
        }

        if (spawnCount > 0)
            isWave = true;
        else if (spawnCount <= 0 && aliveZombie <= 0)
            isWave = false;

        aliveZombie = GameObject.FindGameObjectsWithTag("Zombie").Length;

        if (musicAudioSource.isPlaying && musicAudioSource.volume < 0.2f && !waveShopPanel.gameObject.activeSelf)
            musicAudioSource.volume += 0.1f * Time.deltaTime;
        else if(!musicAudioSource.isPlaying || waveShopPanel.gameObject.activeSelf)
            musicAudioSource.volume -= 0.1f * Time.deltaTime;

        moneyText.text = money.ToString("###,###,##0") + " 원";
        killsText.text = kills.ToString("###,###,##0") + " 킬";
    }

    public void StartWave()
    {
        wave++;

        waveText.text = wave.ToString() + " 웨이브";

        spawnCounter += 10;
        spawnCount += spawnCounter;

        MultiplePlus(0.01f);

        waveStartPanel.gameObject.SetActive(true);
        waveStartText.text = wave.ToString() + " 웨이브";

        OffShopPanel();

        PlaySound(effectAudioSource, waveStartClip[Random.Range(0, waveStartClip.Length)]);
        PlaySound(musicAudioSource, waveClips[Random.Range(0, waveClips.Length)]);
    }

    void OffShopPanel()
    {
        if (waveShopPanel.gameObject.activeSelf)
            waveShopPanel.GetComponent<Animation>().Play("OffWaveShopPanel");

        if (waveBatchPanel.gameObject.activeSelf)
            waveBatchPanel.GetComponent<Animation>().Play("OffWaveBatchPanel");

        if (waveButtonPanel.gameObject.activeSelf)
            waveButtonPanel.GetComponent<Animation>().Play("OffWaveButtonPanel");
    }

    void OnShopPanel()
    {
        waveShopPanel.gameObject.SetActive(true);
        waveBatchPanel.gameObject.SetActive(true);
        waveButtonPanel.gameObject.SetActive(true);
    }

    void SpawnZombie()
    {
        InstanceZombie(normal_Zombie_A, Random.Range(0,spawnPoints.Length));
        InstanceZombie(normal_Zombie_B, Random.Range(0,spawnPoints.Length));
        InstanceZombie(normal_Zombie_C, Random.Range(0,spawnPoints.Length));

        if(wave >= 3)
        {
            InstanceZombie(running_Zombie_B, Random.Range(0,spawnPoints.Length));
            InstanceZombie(running_Zombie_C, Random.Range(0,spawnPoints.Length));
            InstanceZombie(running_Zombie_C, Random.Range(0,spawnPoints.Length));
        }
        
        if(wave >= 5)
            InstanceZombie(tyrant_Zombie, Random.Range(0,spawnPoints.Length));
    }

    void InstanceZombie(GameObject obj, int spawnPointNumber)
    {
        Vector3 spawnPoint = Vector3.zero;

        if(spawnCount > 0)
        {
            if (spawnPointNumber == 0 || spawnPointNumber == 3)
            spawnPoint = new Vector3(spawnPoints[spawnPointNumber].position.x + Random.Range(-35, 35+1), 1, spawnPoints[spawnPointNumber].position.z + Random.Range(-5, 5+1));

            if (spawnPointNumber == 1 || spawnPointNumber == 2)
                spawnPoint = new Vector3(spawnPoints[spawnPointNumber].position.x + Random.Range(-5, 5+1), 1, spawnPoints[spawnPointNumber].position.z + Random.Range(-35, 35+1));

            Instantiate(obj, spawnPoint, Quaternion.identity);

            spawnCount--;
        }
    }

    void PlaySound(AudioSource audiosource, AudioClip audioclip)
    {
        audiosource.clip = audioclip;
        audiosource.Play();
    }

    public static void MoneyPlus(int value)
    {
        money += value;
    }

    public static void KillsCountPlus()
    {
        kills++;
    }

    public static void SentryAmmoPM(int value)
    {
        sentryAmmo += value;
    }

    public void MultiplePlus(float value)
    {
        multiple += value;
    }
}
