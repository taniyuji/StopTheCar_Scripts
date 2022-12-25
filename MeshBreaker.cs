using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

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

    private CollitionDetector collitionDetector;


    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        collitionDetector = GetComponent<CollitionDetector>();

        particle.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        vertPos = meshFilter.mesh.vertices;

        collitionDetector.collide.Subscribe(i =>
        {
            if (vertexList.Count != 0) return;

            if(i == CollitionDetector.CollideObject.Wall)
            {
                for (int j = 0; j < vertPos.Length; j++)
                {
                    vertexList.Add(vertPos[j]);

                    vertexList[j] += new Vector3(UnityEngine.Random.Range(-breakValue, breakValue),
                                                 UnityEngine.Random.Range(-breakValue, breakValue), 0);
                }

                meshFilter.mesh.SetVertices(vertexList);

                wheelRigidBody.isKinematic = false;

                wheelRigidBody.AddForce(new Vector3(5, 10, -5), ForceMode.Impulse);

                particle.gameObject.SetActive(true);

                //SoundManager.i.PlayOneShot(1);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
