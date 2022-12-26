using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

//ゲーム中のマウスやタッチの入力を管理する
public class InputController : MonoBehaviour
{
    [SerializeField]
    private float maxScrollSpeed = 0.05f;//マイフレーム処理させないことで処理を軽くさせる

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

    private Subject<Unit> _finishDraw = new Subject<Unit>();
    //UniRxでプレイヤーが壁描写を終了したことを通知
    public IObservable<Unit> finishDraw
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
        //ゲームマネージャーがゲーム開始を判断するまで入力を受け付けない
        if (ResourceProvider.i.gameManager.state == GameManager.GameState.Pause) return;

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
            _finishDraw.OnNext(Unit.Default);

            lineDrawer.PenUp();

            if (handTransform != null) handTransform.gameObject.SetActive(false);
        }
    }

    //mousePositionをワールドポジションに変換し、描きたい奥行きを加算した上で返すメソッド
    private Vector3 CalculateTouchPoint()
    {
        var mousePosition = Input.mousePosition;

        var cameraTransform = cameraRectTransform == null
             ? Camera.main.gameObject.transform : cameraRectTransform;

        mousePosition = new Vector3(mousePosition.x,
                                    mousePosition.y,
                                    -cameraTransform.position.z);

        var screenPoint = Camera.main.ScreenToWorldPoint(mousePosition);

        //このスクリプトがアタッチされたオブジェクトのzPositionで壁の奥行きを調整
        screenPoint = new Vector3(screenPoint.x,
                                  screenPoint.y,
                                  screenPoint.z + transform.position.z);

        return screenPoint;
    }
}
