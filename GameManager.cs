using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ゲーム起動からゲームスタートまでを管理するスクリプト
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Canvas startCanvas;

    [SerializeField]
    private Canvas gameCanvas;

    public enum GameState
    {
        Pause,
        Start,
    }

    public GameState state { get; private set; } = GameState.Pause;

    void Awake()
    {
        Time.timeScale = 0;

        startCanvas.gameObject.SetActive(true);

        gameCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (state == GameState.Start) return;

        //初めて画面がクリックされた場合ゲームを開始させる
        //このスクリプトのstateがGamestate.Pauseの間はInputControllerでInputを受け付けさせない
        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(StartGame());
        }
    }

    //他のInputControllerのGetMouseButtonUpと同時に作動させないためにGetMouseButtonUp後、1フレームおく
    private IEnumerator StartGame()
    {
        yield return null;

        Time.timeScale = 1;

        state = GameState.Start;

        startCanvas.gameObject.SetActive(false);

        gameCanvas.gameObject.SetActive(true);

        SoundManager.i.PlayOneShot(5);

        SoundManager.i.PlayOneShot(4);
    }
}
