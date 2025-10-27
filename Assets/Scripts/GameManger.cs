using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public GameObject playerObj;

    float playerPosX = 0;
    float playerPosY = 0;

    private const string PlayerPosXKey = "PlayerPositionX";
    private const string PlayerPosYKey = "PlayerPositionY";

    private void Start()
    {
        playerPosX = PlayerPrefs.GetFloat(PlayerPosXKey, 0);
        playerPosY = PlayerPrefs.GetFloat(PlayerPosYKey, 0);

        Debug.Log($"��ġ�� {playerPosX}, {playerPosY} �ҷ��ɴϴ�");

        Vector3 playerPos = new Vector3(playerPosX, playerPosY, 0);
        playerObj.transform.position = playerPos;
    }

    public void OnSavePosition()
    {
        playerPosX = playerObj.transform.position.x;
        playerPosY = playerObj.transform.position.y;

        Debug.Log($"��ġ�� {playerPosX}, {playerPosY} ����Ǿ����ϴ�");

        PlayerPrefs.SetFloat(PlayerPosXKey, playerPosX);
        PlayerPrefs.SetFloat(PlayerPosYKey, playerPosY - 2f);
    }
}
