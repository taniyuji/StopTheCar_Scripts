using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

//車が壁とぶつかった際にメッシュをぐしゃぐしゃにするスクリプト
public class MeshBreaker : MonoBehaviour
{
    [SerializeField]
    private float breakValue = 0.1f;

    [SerializeField]
    private Rigidbody wheelRigidBody;

    [SerializeField]
    private ParticleSystem particle;

    private List<Vector3> vertexList = new List<Vector3>();

    MeshFilter meshFilter;

    Vector3[] vertPos;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        particle.gameObject.SetActive(false);
    }

    void Start()
    {
        vertPos = meshFilter.mesh.vertices;
    }

    //壁とぶつかったら
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (!collisionInfo.gameObject.CompareTag("Wall")) return;

        if (vertexList.Count != 0) return;

        //メッシュの各頂点をランダムに動かし、メッシュをぐしゃぐしゃにする
        for (int j = 0; j < vertPos.Length; j++)
        {
            vertexList.Add(vertPos[j]);

            vertexList[j] += new Vector3(UnityEngine.Random.Range(-breakValue, breakValue),
                                            UnityEngine.Random.Range(-breakValue, breakValue), 0);
        }

        meshFilter.mesh.SetVertices(vertexList);

        //タイヤを飛ばす
        wheelRigidBody.isKinematic = false;

        wheelRigidBody.AddForce(new Vector3(5, 10, -5), ForceMode.Impulse);

        particle.gameObject.SetActive(true);

        SoundManager.i.PlayOneShot(1);
    }
}
