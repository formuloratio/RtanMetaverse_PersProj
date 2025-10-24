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

    //블럭의 컬러 지정
    public Color prevColor;
    public Color nextColor;

    bool isMovingX = true; //이동 방향 검사


    void Start()
    {
        if (originBlock == null)
        {
            Debug.Log("originBlock이 없습니다.");
            return;
        }

        //미리 색상 지정
        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        prevBlockPosition = Vector3.down;

        Spawn_Block(); //블럭 1개 생성

        Spawn_Block();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Spawn_Block();
        }
        MoveBlock();

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

        ColorChange(newBlock); //새롭게 생성된 이후에 색상 변경

        newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        newTrans.localRotation = Quaternion.identity; //Quaternion의 초기값, 즉 회전이 없는 상태
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stackCount++;

        desiredPosition = Vector3.down * stackCount; //스택 카운트 증가하는 만큼 TheStack을 내리기
        blockTransition = 0f; //이동 처리를 위한 기준값(초기화)

        lastBlock = newTrans;

        isMovingX = !isMovingX; //방향 반대로

        return true;
    }

    Color GetRandomColor()
    {
        float r = Random.Range(100f, 250f) /255f;
        float g = Random.Range(100f, 250f) /255f;
        float b = Random.Range(100f, 250f) / 255f;

        return new Color(r, g, b);
    }

    void ColorChange(GameObject go)
    {
        Color applyColor = Color.Lerp(prevColor, nextColor, (stackCount % 11) /10f);
        //stackCount % 11 -> 0부터 10까지 값들이 순환을 돌게 됨
        // 이전, 다음 컬러 값의 중간 값들이 순서에 맞춰서 바뀌게 됨

        Renderer rn = go.GetComponent<Renderer>(); //Renderer는 그려내는 것
        //메쉬렌더러의 부모클래스가 렌더러. 부모 클래스를 가져와서 컬러 값을 변경

        if (rn == null)//예외처리
        {
            Debug.Log("렌더러가 비어있음");
        }

        rn.material.color = applyColor;
        //메쉬렌더러 -> material로 형태 외곽의 재질 결정하고 있음.

        Camera.main.backgroundColor = applyColor - new Color(0.1f, 0.1f, 0.1f);

        if (applyColor.Equals(nextColor)) //적용컬러와 다음컬러가 동일하다면 -> nextColor가 10의 단위에 도달함을 뜻함
        {
            prevColor = nextColor;
            nextColor = GetRandomColor(); //새롭게 컬러 할당
        }
    }

    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed;

        //blockTransition이 이동하는 수치의 퍼센테이지를 가져갈 것임
        float movePosition = Mathf.PingPong(blockTransition, BoundSize) - BoundSize/2; //BoundSize의 반만큼 이동
        //pingpong -> 0부터 내가 지정한 값까지를 순환함(양수만). (sin을 써서 완만한 곡선 가능. 단, 음수도 범위임)

        if (isMovingX)
        {
            lastBlock.localPosition = new Vector3(movePosition * MovingBoundsSize, stackCount, secondaryPosition);
        }
        else
        {
            lastBlock.localPosition = new Vector3(secondaryPosition, stackCount, -movePosition * MovingBoundsSize);
        }
    }
}
