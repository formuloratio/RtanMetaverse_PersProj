using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class TriggerManager : MonoBehaviour
{
    public GameObject miniGameWindow;
    public GameObject loadingEffect;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Trigger")
        {
            StartCoroutine(MiniGameTriggerWait());
        }
    }

    public void OnMiniGameYes()
    {
        GameManger.Instance.OnSavePosition();
        Time.timeScale = 1;
        OnLoadingEffect();
        StartCoroutine(OnLoadingScene());
    }

    public void OnMiniGameNo()
    {
        miniGameWindow.SetActive(false);
        Time.timeScale = 1;
    }

    IEnumerator MiniGameTriggerWait()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
        miniGameWindow.SetActive(true);
    }

    IEnumerator OnLoadingScene()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("MiniGameScene");
    }

    private void OnLoadingEffect()
    {
        loadingEffect.SetActive(true);
    }
}
