using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatchFrame : MonoBehaviour
{

    public float height;

    public GameObject batchPrefab;

    public MeshRenderer meshRenderer;

    public Color t, f;

    private Transform target;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (target.position.x < 15 && target.position.x > -15 && target.position.z < 15 && target.position.z > -15)
            transform.position = new Vector3(Mathf.Round(target.position.x), height, Mathf.Round(target.position.z));
    }

    void BuildUp()
    {
        Instantiate(batchPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void Buildable(bool TF)
    {
        if(TF)
        {
            BatchManager.isBuildable = true;
            meshRenderer.material.color = t;
        }
        else
        {
            BatchManager.isBuildable = false;
            meshRenderer.material.color = f;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("BatchType") && BatchManager.isBuildable)
            Buildable(false);
        else
            Buildable(true);
    }

    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.CompareTag("BatchType") && BatchManager.isBuildable)
            Buildable(false);
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("BatchType") && !BatchManager.isBuildable)
            Buildable(true);
    }
}
