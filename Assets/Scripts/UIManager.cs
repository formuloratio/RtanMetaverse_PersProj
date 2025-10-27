using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public enum UIState
{
    Home,
    Game,
    Score,
}

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
    }

    UIState currentState = UIState.Home;
    HomeUI homeUI = null;
    GameUI gameUI = null;
    ScoreUI scoreUI = null;

    TheStack theStack = null;

    public GameObject loadingEffect;

    private void Awake()
    {
        instance = this;

        theStack = FindObjectOfType<TheStack>();

        homeUI = GetComponentInChildren<HomeUI>(true); //캔버스 하위에 달아줄 것이기에 겟칠드런으로 찾을 수 있음
        //true -> 꺼져 있는 오브젝트도 찾는데에 포함
        homeUI?.Init(this); // ? -> 홈ui가 null이 아니면 동작해라

        gameUI = GetComponentInChildren<GameUI>(true);
        gameUI?.Init(this);

        scoreUI = GetComponentInChildren<ScoreUI>(true);
        scoreUI?.Init(this);

        ChangeState(UIState.Home);
    }

    public void ChangeState(UIState state)
    {   //각각의 창들이 체인지스테이트할 때마다 온오프 되게
        currentState = state;
        homeUI?.SetActive(currentState);
        gameUI?.SetActive(currentState);
        scoreUI?.SetActive(currentState);
    }

    public void OnClickStart()
    {
        theStack.Restart();
        ChangeState(UIState.Game);
    }

    public void OnClickExit()
    {
        OnLoadingEffect();
        StartCoroutine(OnLoadingScene());

//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//        Application.Quit();
//#endif
    }

    public void UpdateScore()
    {
        gameUI.SetUI(theStack.Score, theStack.Combo, theStack.MaxCombo);
    }

    public void SetScoreUI()
    {
        scoreUI.SetUI(theStack.Score, theStack.MaxCombo, theStack.BestScore, theStack.BestCombo);
        ChangeState(UIState.Score);
    }

    public void OnLoadingEffect()
    {
        loadingEffect.SetActive(true);
    }

    IEnumerator OnLoadingScene()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("WorldScene");
    }
}
