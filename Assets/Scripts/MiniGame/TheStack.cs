using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
    //상수값 만들기 -> const
    private const float BoundSize = 3.5f; //블록 사이즈
    private const float MovingBoundsSize = 3f; //이동하는 양
    private const float StackMovingSpeed = 5.0f; //이동 스피드
    private const float BlockMovingSpeed = 3.5f;
    private const float ErrorMargin = 0.1f; //성공으로 취급할 에러 마진

    public GameObject originBlock = null; //프리펩

    //각각의 값을 처리하기 위해서 필요한 변수들
    private Vector3 prevBlockPosition;
    private Vector3 desiredPosition;
    private Vector3 stackBounds = new Vector2(BoundSize, BoundSize); //새롭게 생성되는 블럭의 사이즈

    //새로운 블럭 생성을 위한 변수
    Transform lastBlock = null;
    float blockTransition = 0f;
    float secondaryPosition = 0f;

    int stackCount = -1; //시작할 때 +1해서 사용할 것
    int comboCount = 0;


    void Start()
    {
        if (originBlock == null)
        {
            Debug.Log("originBlock이 없습니다.");
            return;
        }
        prevBlockPosition = Vector3.down;

        Spawn_Block(); //블럭 1개 생성
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Spawn_Block();
        }

        // 생성 후 TheStack 움직여주기 위해
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed * Time.deltaTime);
        // desiredPosition으로 바로 이동시켜 줄 수도 있지만 끊겨보임
        // Vector3.Lerp -> 부드러운 이동 처리 -> Lerp(시작지점, 끝지점, t(시간값))
        // t값은 0~1이 들어감. 일정한 값을 선형으로 두고 퍼센테이지로 가져감. (t가 퍼센테이지)
    }

    bool Spawn_Block()
    {
        if(lastBlock != null)
            prevBlockPosition = lastBlock.localPosition;

        GameObject newBlock = null;
        Transform newTrans = null;

        newBlock = Instantiate(originBlock);

        if (newBlock == null )
        {
            Debug.Log("새로운 블록 생성에 실패했습니다.");
            return false;
        }

        newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        newTrans.localRotation = Quaternion.identity; //Quaternion의 초기값, 즉 회전이 없는 상태
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stackCount++;

        desiredPosition = Vector3.down * stackCount; //스택 카운트 증가하는 만큼 TheStack을 내리기
        blockTransition = 0f; //이동 처리를 위한 기준값(초기화)

        lastBlock = newTrans;

        return true;
    }
}
