using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class CarMover : MonoBehaviour
{
    [SerializeField]
    private float xSpeed;

    [SerializeField]
    private float slowDownXSpeed;

    private Rigidbody _rigidbody;

    private Vector3 moveForce;

    private Vector3 slowDownSpeed;

    private Vector3 beforeVelocity;

    private enum CarState
    {
        Moving,
        SlowDown,
        Stop,
    }

    private CarState state = CarState.Moving;

    private CollitionDetector collitionDetector;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        moveForce = new Vector3(xSpeed, 0, 0);

        slowDownSpeed = new Vector3(slowDownXSpeed, 0, 0);

        collitionDetector = GetComponent<CollitionDetector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResourceProvider.i.inputController.finishDraw.Subscribe(i =>
        {
            _rigidbody.velocity = beforeVelocity;

            state = CarState.Moving;
        });

        collitionDetector.collide.Subscribe(i =>
        {
            if (i == CollitionDetector.CollideObject.SpeedDown)
            {
                state = CarState.SlowDown;

                beforeVelocity = _rigidbody.velocity;

                _rigidbody.velocity = Vector3.zero;
            }
            else if (i == CollitionDetector.CollideObject.Wall
                     || i == CollitionDetector.CollideObject.Character)
            {
                state = CarState.Stop;
            }
        });
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
