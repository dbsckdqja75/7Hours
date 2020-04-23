using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public Renderer objRenderer;

    private float aliveTime = 30;
    private bool destroyTime;

    void Start()
    {
        Destroy(gameObject, 40);
    }

    void Update()
    {
        if (aliveTime <= 0 && !destroyTime)
            StartCoroutine(FadeInOut());
        else
            aliveTime -= Time.deltaTime;
    }

    IEnumerator FadeInOut()
    {
        destroyTime = true;

        while (true)
        {
            objRenderer.enabled = false;

            yield return new WaitForSeconds(0.5f);

            objRenderer.enabled = true;

            yield return new WaitForSeconds(0.5f);
        }
    }
}
