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
    int maxCombo = 0;

    //스코어 기능
    public int Score { get { return stackCount; } }
    public int Combo { get { return comboCount; } }
    private int MaxCombo { get => maxCombo; }

    //블럭의 컬러 지정
    public Color prevColor;
    public Color nextColor;

    bool isMovingX = true; //이동 방향 검사

    int bestScore = 0;
    public int BestScore { get => bestScore; }

    int bestCombo = 0;
    public int BestCombo { get => bestCombo; }

    //플레이어 프리펩 사용을 위해 필요한 키값
    private const string BestScoreKey = "BestScore";
    private const string BestComboKey = "BestCombo";

    private bool isGameOver = true; //게임 시작시 멈춰있도록 true

    void Start()
    {
        if (originBlock == null)
        {
            Debug.Log("originBlock이 없습니다.");
            return;
        }

        //저장이 되어있다면 로드
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0); //키값이 없으면 0을 넘겨받음
        bestCombo = PlayerPrefs.GetInt(BestComboKey, 0);

        //미리 색상 지정
        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        prevBlockPosition = Vector3.down;

        Spawn_Block(); //블럭 1개 생성

        Spawn_Block();
    }

    void Update()
    {
        if (isGameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceBlock())
            {
                Spawn_Block();
            }
            else
            {
                //게임 오버
                Debug.Log("Game Over");
                UpdateScore();
                isGameOver = true;
                GameOverEffect();
            }
            
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

    bool PlaceBlock()
    {
        Vector3 lastPosition = lastBlock.localPosition;

        if (isMovingX) //x축
        {
            float deltaX = prevBlockPosition.x - lastPosition.x; //잘려 나가야하는 크기
            bool isNegativeNum = (deltaX < 0) ? true : false; //잘려나간 블록이 생성될 방향 지정

            deltaX = Mathf.Abs(deltaX); //절대값(무조건 양수화)

            if (deltaX > ErrorMargin) //잘라내야함
            {
                stackBounds.x -= deltaX; //stackBounds -> 다음 블럭을 생성할 사이즈
                if (stackBounds.x <= 0)
                {
                    return false; //게임 오버
                }

                //새로운 패치(이탈 부분 제외한 블럭으로 재생성)
                float middle = (prevBlockPosition.x + lastPosition.x) / 2f; //두 블럭의 중심지점 찾기
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.x = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                //Rubble 생성 구간
                float rubbleHalfScale = deltaX / 2f;
                CreateRubble(
                    new Vector3(
                        isNegativeNum ? //x축 변경
                        lastPosition.x + stackBounds.x / 2 + rubbleHalfScale :
                        lastPosition.x - stackBounds.x / 2 - rubbleHalfScale,

                        lastPosition.y,
                        lastPosition.z),

                    new Vector3(deltaX, 1, stackBounds.y));

                //콤보 초기화
                comboCount = 0;

            }
            else //위치 보정만
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up; //이전 블록에서 한칸 위의 것을 위치값으로 재조정
                //이거 없으면 에러 마진보다 계속 작은 값으로 차이점이 발생할 것임.
            }

        }
        else //z축
        {
            float deltaZ = prevBlockPosition.z - lastPosition.z;
            bool isNegativeNum = (deltaZ < 0) ? true : false; //잘려나간 블록이 생성될 방향 지정

            deltaZ = Mathf.Abs(deltaZ);

            if (deltaZ > ErrorMargin)
            {
                stackBounds.y -= deltaZ;
                if (stackBounds.y <= 0)
                {
                    return false;
                }

                float middle = (prevBlockPosition.z + lastPosition.z) / 2f;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.z = middle;
                lastBlock.localPosition = lastPosition = tempPosition;


                //Rubble 생성 구간
                float rubbleHalfScale = deltaZ / 2f;
                CreateRubble(
                    new Vector3(
                        lastPosition.x,
                        lastPosition.y,

                        isNegativeNum ? //z축 변경
                        lastPosition.z + stackBounds.y / 2 + rubbleHalfScale :
                        lastPosition.z - stackBounds.y / 2 - rubbleHalfScale),

                    new Vector3(stackBounds.x, 1, deltaZ));

                comboCount = 0;
            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }

        //이동한 방향이 어디냐에 따라서 x/z 축 값을 저장함.
        secondaryPosition = (isMovingX) ? lastBlock.localPosition.x : lastBlock.localPosition.z;
        //이전 블록 위치가 계속해서 바뀌고 있기 때문에 중점인 0의 위치를 계속 사용 못함
        // 그래서 이동시킨 축의 값을 저장해뒀다가 Moving에서 사용하고 있는 것

        return true;
    }

    //파편 만들기
    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(lastBlock.gameObject);
        go.transform.parent = this.transform;

        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localRotation = Quaternion.identity;

        go.AddComponent<Rigidbody>(); //조각은 바닥으로 떨어져야하기에
        go.name = "Rubble"; //게임오브젝트 이름 변경됨
    }

    void ComboCheck()
    {
        comboCount++;

        if (comboCount > maxCombo)
            maxCombo = comboCount;

        if ((comboCount % 5) == 0)
        {
            Debug.Log("5 콤보 성공");
            stackBounds += new Vector3(0.5f, 0.5f);
            stackBounds.x =
                (stackBounds.x > BoundSize) ? BoundSize : stackBounds.x;
            stackBounds.y =
                (stackBounds.y > BoundSize) ? BoundSize : stackBounds.y;
        }
    }

    void UpdateScore()
    {
        if(bestScore < stackCount)
        {
            Debug.Log("점수 갱신");
            bestScore = stackCount;
            bestCombo = maxCombo;

            PlayerPrefs.SetInt(BestScoreKey, bestScore); //저장
            PlayerPrefs.SetInt(BestComboKey, bestCombo);
        }
    }

    void GameOverEffect()
    {
        int  childCount = this.transform.childCount; //childCount -> 트랜스폼의 하위에 있는 오브젝트의 개수 (안에 있는 블럭들)

        for (int i = 1; i <= 20; i++)
        {
            if (childCount < i) break; //인덱스에서 벗어나기에 브레이크

            GameObject go = transform.GetChild(childCount - i).gameObject; //하위 오브젝트를 인덱스로 찾아오기

            if (go.name.Equals("Rubble")) continue; //Rubble -> 바닥으로 떨어지고 있는 애

            Rigidbody rigid = go.AddComponent<Rigidbody>(); // 리지드바디 달아주기
            rigid.AddForce( //힘 적용해서 날려버리기
                (Vector3.up * Random.Range(0, 10f) + Vector3.right * (Random.Range(0, 10f) - 5f)) * 100f
                );
        }
    }

    public void Restart()
    { //거의 모든 값 초기화
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        isGameOver = false;

        lastBlock = null;
        desiredPosition = Vector3.zero;
        stackBounds = new Vector3(BoundSize, BoundSize);

        stackCount = -1;
        isMovingX = true;
        blockTransition = 0f;
        secondaryPosition = 0f;

        comboCount = 0;
        maxCombo = 0;

        prevBlockPosition = Vector3.down;

        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        //처음 블록과 이동 블록 2개 생성
        Spawn_Block();
        Spawn_Block();
    }
}
