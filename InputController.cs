using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

//マウスやタッチの入力を管理する
public class InputController : MonoBehaviour
{
    [SerializeField]
    private float fixDegree;

    [SerializeField]
    private float maxScrollSpeed = 0.05f;

    [SerializeField]
    private RectTransform handTransform;

    [SerializeField]
    private GameObject helpUI;

    private LineDrawer_v2 lineDrawer;

    private enum LineState
    {
        Wait,
        CanDraw,
        StartCount,
    }

    private LineState state = LineState.CanDraw;

    private Subject<bool> _finishDraw = new Subject<bool>();
    //UniRxでプレイヤーが壁描写を終了したことを通知
    public IObservable<bool> finishDraw
    {
        get { return _finishDraw; }
    }

    private float intervalTime;

    private RectTransform cameraRectTransform;

    void Awake()
    {
        lineDrawer = GetComponent<LineDrawer_v2>();

        cameraRectTransform = Camera.main.gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (state == LineState.Wait) return;

        StartCountInterval();

        GetMouseInput();
    }
    //メッシュの生成間隔を制限するメソッド
    private void StartCountInterval()
    {
        if (state != LineState.StartCount) return;

        if (intervalTime >= maxScrollSpeed)
        {
            state = LineState.CanDraw;

            intervalTime = 0;

            return;
        }

        intervalTime += Time.deltaTime;
    }
    //マウスの入力を取得するメソッド
    private void GetMouseInput()
    {
        //入力を始めた場合、以前のメッシュ情報を削除し、入力初めのポジションを取得
        if (Input.GetMouseButtonDown(0))
        {
            lineDrawer.PenDown(CalculateTouchPoint());

            helpUI.SetActive(false);

            state = LineState.StartCount;
        }
        else if (Input.GetMouseButton(0) && state == LineState.CanDraw)
        {
            //入力している間、入力前後の入力位置からメッシュの大きさを計算し更新
            lineDrawer.PenMove(CalculateTouchPoint());

            state = LineState.StartCount;

            if (handTransform != null)
            {
                handTransform.gameObject.SetActive(true);
                handTransform.position = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //ドラッグが終了したらUniRxで通知する
            _finishDraw.OnNext(true);

            lineDrawer.PenUp();

            if(handTransform != null) handTransform.gameObject.SetActive(false);
        }
    }

    private Vector3 CalculateTouchPoint()
    {
        var mousePosition = Input.mousePosition;

        var cameraTransform = cameraRectTransform == null
             ? Camera.main.gameObject.transform : cameraRectTransform;

        mousePosition = new Vector3(mousePosition.x,
                                    mousePosition.y,
                                    -cameraTransform.position.z);

        var screenPoint = Camera.main.ScreenToWorldPoint(mousePosition);

        var fixTouch = new Vector3(screenPoint.x, screenPoint.y, transform.position.z) * Mathf.Sin(fixDegree * Mathf.Deg2Rad);

        screenPoint = new Vector3(screenPoint.x - fixTouch.x,
                                  screenPoint.y - fixTouch.y,
                                  screenPoint.z + transform.position.z);

        return screenPoint;
    }
}
