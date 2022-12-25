using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchRagdoll : MonoBehaviour
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private List<ParticleSystem> particles;

    [SerializeField]
    private Vector3 collideForce = new Vector3(-5, 50, -10);

    private List<Rigidbody> ragdollRigidBodies = new List<Rigidbody>();

    private Animator animator;

    private Rigidbody parentRigidbody;
    // Start is called before the first frame update

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

    void OnCollisionEnter(Collision collisionInfo)
    {
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
