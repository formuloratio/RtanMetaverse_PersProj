using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : BaseUI
{
    Button startButton;
    Button exitButton;

    protected override UIState GetUIState()
    {
        return UIState.Home;
    }

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);

        //버튼 찾아오기(public 이용 말고)
        //Find는 GetChild와 다르게 경로명을 받아옴
        startButton = transform.Find("StartButton").GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();

        //AddListener() -> () 안의 함수를 넣어줌
        //start버튼에 OnClick이 눌러진다면 OnClickStartButton이 실행됨
        startButton.onClick.AddListener(OnClickStartButton);
        exitButton.onClick.AddListener(OnClickExitButton);
    }

    void OnClickStartButton()
    {
        uiManager.OnClickStart();
    }

    void OnClickExitButton()
    {
        uiManager.OnClickExit();
    }

}
