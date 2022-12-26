using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

//キャラクターのアニメーションを制御するスクリプト
public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;

    public bool isGoal { get; private set; } = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //壁描写終了通知を受け取ったらゴール判定用のコルーチンメソッドを作動
        ResourceProvider.i.inputController.finishDraw.Subscribe(i =>
        {
            if(isGoal) return;
            
            StartCoroutine(SetDance());
        });
    }

    //壁を描き終わってから２秒間壁とも車とも衝突しなかった場合はゴールとして判定し踊らせる
    private IEnumerator SetDance()
    {
        yield return new WaitForSeconds(2);

        //壁か車と衝突した場合、ragdoll(体をふにゃふにゃにするアセット）起動によってanimatorがfalseになるため、
        //animatorがtrueのままだった場合は衝突していないことになる。
        if (animator.enabled)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
            animator.SetBool("Dance", true);
            SoundManager.i.PlayOneShot(3);
            isGoal = true;
        }
    }
}
