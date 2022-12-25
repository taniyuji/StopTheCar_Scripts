using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResourceProvider.i.inputController.finishDraw.Subscribe(i =>
        {
            StartCoroutine(SetDance());
        });
    }

    private IEnumerator SetDance()
    {
        yield return new WaitForSeconds(2);

        if (animator.enabled)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
            animator.SetBool("Dance", true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
