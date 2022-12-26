using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//壁描写前に表示される「HELP」のUIのアニメーションを制御するスクリプト
public class UIMover : MonoBehaviour
{
    [SerializeField]
    private float shrinkAmount;

    [SerializeField]
    private float shrinkSpeed;

    private RectTransform rectTransform;

    private bool isPlus = false;

    private float shrinkCounter;

    private Vector3 shrinkVector;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        var speed = shrinkSpeed * Time.deltaTime;

        shrinkVector = new Vector3(speed, speed, speed);

        rectTransform.localScale = isPlus ?
            rectTransform.localScale + shrinkVector : rectTransform.localScale - shrinkVector;

        shrinkCounter += speed;

        if(shrinkCounter >= shrinkAmount)
        {
            isPlus = !isPlus;

            shrinkCounter = 0;
        }
    }
}
