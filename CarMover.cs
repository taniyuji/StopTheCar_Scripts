using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

//車を動かすスクリプト
public class CarMover : MonoBehaviour
{
    [SerializeField]
    private float xSpeed;

    [SerializeField]
    private float slowDownXSpeed;//壁が描写されるまでスピードを落とす際の速さ

    private Rigidbody _rigidbody;

    private Vector3 moveForce;//車をAddForceで横に動かす

    private Vector3 slowDownSpeed;//減速する際の速さ

    private Vector3 beforeVelocity = Vector3.zero;//再び動かし始める時の速さを格納する変数

    private enum CarState
    {
        Moving,
        SlowDown,
        Stop,
    }

    private CarState state = CarState.Moving;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        moveForce = new Vector3(xSpeed, 0, 0);

        slowDownSpeed = new Vector3(slowDownXSpeed, 0, 0);
    }

    void Start()
    {
        //壁描写終了の通知受け取ったら再び動き出させる。既に壁かプレイヤーにぶつかっている場合は無効
        ResourceProvider.i.inputController.finishDraw.Subscribe(i =>
        {
            if(state == CarState.Stop) return;
            
            _rigidbody.velocity = beforeVelocity;

            state = CarState.Moving;
        });
    }

    void OnTriggerEnter(Collider other)
    {
        //減速判定用のコライダーと衝突したら減速する
        if (other.gameObject.CompareTag("SpeedDown"))
        {
            if(beforeVelocity != Vector3.zero) return;

            other.gameObject.SetActive(false);

            state = CarState.SlowDown;

            //再び動き出す際のスピードを格納
            beforeVelocity = _rigidbody.velocity;

            _rigidbody.velocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        //壁かプレイヤーと衝突したら止める
        if (collisionInfo.gameObject.CompareTag("Wall")
                 || collisionInfo.gameObject.CompareTag("Character"))
        {
            state = CarState.Stop;
        }
    }

    void FixedUpdate()
    {
        if (state == CarState.SlowDown)
        {
            _rigidbody.velocity = slowDownSpeed;
        }
        else if (state == CarState.Moving)
        {
            _rigidbody.AddForce(moveForce);
        }
    }


}
