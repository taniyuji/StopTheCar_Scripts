using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//プレイヤーが車か壁とぶつかった際にプレイヤーの体をぐにゃぐにゃにして血を出すスクリプト
public class SwitchRagdoll : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private List<ParticleSystem> particles;

    //吹き飛ぶ方向と距離
    [SerializeField]
    private Vector3 collideForce = new Vector3(-5, 50, -10);

    private List<Rigidbody> ragdollRigidBodies = new List<Rigidbody>();

    private Animator animator;

    private Rigidbody parentRigidbody;

    void Awake()
    {
        var _ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();

        ragdollRigidBodies.AddRange(_ragdollRigidBodies);

        animator = GetComponent<Animator>();

        parentRigidbody = GetComponent<Rigidbody>();

        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].gameObject.SetActive(false);
        }
    }

    void Start()
    {
        for (int i = 0; i < ragdollRigidBodies.Count; i++)
        {
            ragdollRigidBodies[i].isKinematic = true;
        }
    }
    
    //壁か車とぶつかったらragdollをオンにしてプレイヤーをaddForceで吹き飛ばし、血のエフェクトを全てオンにする
    void OnCollisionEnter(Collision collisionInfo)
    {
        if(ResourceProvider.i.isGoal) return;

        if (collisionInfo.gameObject.CompareTag("Car") || collisionInfo.gameObject.CompareTag("Wall"))
        {
            if (!animator.enabled) return;

            for (int i = 0; i < ragdollRigidBodies.Count; i++)
            {
                ragdollRigidBodies[i].isKinematic = false;

                animator.enabled = false;

                ragdollRigidBodies[i].AddForce(collideForce, ForceMode.Impulse);

            }

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].gameObject.SetActive(true);
            }

            SoundManager.i.PlayOneShot(2);

            image.gameObject.SetActive(false);
        }
    }
}
