using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    public Transform playerTransform;
    public float moveLeft = -5f;
    public float moveRight = 5f;
    public float moveDown = -5f;
    public float moveUp = 5f;


    // Start is called before the first frame update
    void Start()
    {
        moveLeft = -5f;
        moveRight = 5f;
        moveDown = -5f;
        moveUp = 5f;

}

    // Update is called once per frame
    void Update()
    {
        float tranLeft = playerTransform.position.x;
        float tranRight = playerTransform.position.x;
        float tranDown = playerTransform.position.y;
        float tranUp = playerTransform.position.y;

        float transEverX = this.transform.position.x;
        float transEverY = this.transform.position.y;

        if (UnityEngine.Input.GetKey(KeyCode.A))
        {
            tranLeft += moveLeft * Time.deltaTime;
            playerTransform.position = new Vector3(tranLeft, transEverY, 0);
        }
        else if (UnityEngine.Input.GetKey(KeyCode.D))
        {
            tranRight += moveRight * Time.deltaTime;
            playerTransform.position = new Vector3(tranRight, transEverY, 0);
        }
        else if (UnityEngine.Input.GetKey(KeyCode.W))
        {
            tranUp += moveUp * Time.deltaTime;
            playerTransform.position = new Vector3(transEverX, tranUp, 0);
        }
        else if (UnityEngine.Input.GetKey(KeyCode.S))
        {
            tranDown += moveDown * Time.deltaTime;
            playerTransform.position = new Vector3(transEverX, tranDown, 0);
        }
    }
}
