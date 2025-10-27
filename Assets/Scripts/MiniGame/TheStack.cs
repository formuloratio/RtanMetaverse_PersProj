using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
    //����� ����� -> const
    private const float BoundSize = 3.5f; //��� ������
    private const float MovingBoundsSize = 3f; //�̵��ϴ� ��
    private const float StackMovingSpeed = 5.0f; //�̵� ���ǵ�
    private const float BlockMovingSpeed = 3.5f;
    private const float ErrorMargin = 0.1f; //�������� ����� ���� ����

    public GameObject originBlock = null; //������

    //������ ���� ó���ϱ� ���ؼ� �ʿ��� ������
    private Vector3 prevBlockPosition;
    private Vector3 desiredPosition;
    private Vector3 stackBounds = new Vector2(BoundSize, BoundSize); //���Ӱ� �����Ǵ� ���� ������

    //���ο� �� ������ ���� ����
    Transform lastBlock = null;
    float blockTransition = 0f;
    float secondaryPosition = 0f;

    int stackCount = -1; //������ �� +1�ؼ� ����� ��
    int comboCount = 0;
    int maxCombo = 0;

    //���ھ� ���
    public int Score { get { return stackCount; } }
    public int Combo { get { return comboCount; } }
    private int MaxCombo { get => maxCombo; }

    //���� �÷� ����
    public Color prevColor;
    public Color nextColor;

    bool isMovingX = true; //�̵� ���� �˻�

    int bestScore = 0;
    public int BestScore { get => bestScore; }

    int bestCombo = 0;
    public int BestCombo { get => bestCombo; }

    //�÷��̾� ������ ����� ���� �ʿ��� Ű��
    private const string BestScoreKey = "BestScore";
    private const string BestComboKey = "BestCombo";

    private bool isGameOver = true; //���� ���۽� �����ֵ��� true

    void Start()
    {
        if (originBlock == null)
        {
            Debug.Log("originBlock�� �����ϴ�.");
            return;
        }

        //������ �Ǿ��ִٸ� �ε�
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0); //Ű���� ������ 0�� �Ѱܹ���
        bestCombo = PlayerPrefs.GetInt(BestComboKey, 0);

        //�̸� ���� ����
        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        prevBlockPosition = Vector3.down;

        Spawn_Block(); //�� 1�� ����

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
                //���� ����
                Debug.Log("Game Over");
                UpdateScore();
                isGameOver = true;
                GameOverEffect();
            }
            
        }
        MoveBlock();

        // ���� �� TheStack �������ֱ� ����
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed * Time.deltaTime);
        // desiredPosition���� �ٷ� �̵����� �� ���� ������ ���ܺ���
        // Vector3.Lerp -> �ε巯�� �̵� ó�� -> Lerp(��������, ������, t(�ð���))
        // t���� 0~1�� ��. ������ ���� �������� �ΰ� �ۼ��������� ������. (t�� �ۼ�������)

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
            Debug.Log("���ο� ��� ������ �����߽��ϴ�.");
            return false;
        }

        ColorChange(newBlock); //���Ӱ� ������ ���Ŀ� ���� ����

        newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        newTrans.localRotation = Quaternion.identity; //Quaternion�� �ʱⰪ, �� ȸ���� ���� ����
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stackCount++;

        desiredPosition = Vector3.down * stackCount; //���� ī��Ʈ �����ϴ� ��ŭ TheStack�� ������
        blockTransition = 0f; //�̵� ó���� ���� ���ذ�(�ʱ�ȭ)

        lastBlock = newTrans;

        isMovingX = !isMovingX; //���� �ݴ��

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
        //stackCount % 11 -> 0���� 10���� ������ ��ȯ�� ���� ��
        // ����, ���� �÷� ���� �߰� ������ ������ ���缭 �ٲ�� ��

        Renderer rn = go.GetComponent<Renderer>(); //Renderer�� �׷����� ��
        //�޽��������� �θ�Ŭ������ ������. �θ� Ŭ������ �����ͼ� �÷� ���� ����

        if (rn == null)//����ó��
        {
            Debug.Log("�������� �������");
        }

        rn.material.color = applyColor;
        //�޽������� -> material�� ���� �ܰ��� ���� �����ϰ� ����.

        Camera.main.backgroundColor = applyColor - new Color(0.1f, 0.1f, 0.1f);

        if (applyColor.Equals(nextColor)) //�����÷��� �����÷��� �����ϴٸ� -> nextColor�� 10�� ������ �������� ����
        {
            prevColor = nextColor;
            nextColor = GetRandomColor(); //���Ӱ� �÷� �Ҵ�
        }
    }

    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed;

        //blockTransition�� �̵��ϴ� ��ġ�� �ۼ��������� ������ ����
        float movePosition = Mathf.PingPong(blockTransition, BoundSize) - BoundSize/2; //BoundSize�� �ݸ�ŭ �̵�
        //pingpong -> 0���� ���� ������ �������� ��ȯ��(�����). (sin�� �Ἥ �ϸ��� � ����. ��, ������ ������)

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

        if (isMovingX) //x��
        {
            float deltaX = prevBlockPosition.x - lastPosition.x; //�߷� �������ϴ� ũ��
            bool isNegativeNum = (deltaX < 0) ? true : false; //�߷����� ����� ������ ���� ����

            deltaX = Mathf.Abs(deltaX); //���밪(������ ���ȭ)

            if (deltaX > ErrorMargin) //�߶󳻾���
            {
                stackBounds.x -= deltaX; //stackBounds -> ���� ���� ������ ������
                if (stackBounds.x <= 0)
                {
                    return false; //���� ����
                }

                //���ο� ��ġ(��Ż �κ� ������ ������ �����)
                float middle = (prevBlockPosition.x + lastPosition.x) / 2f; //�� ���� �߽����� ã��
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.x = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                //Rubble ���� ����
                float rubbleHalfScale = deltaX / 2f;
                CreateRubble(
                    new Vector3(
                        isNegativeNum ? //x�� ����
                        lastPosition.x + stackBounds.x / 2 + rubbleHalfScale :
                        lastPosition.x - stackBounds.x / 2 - rubbleHalfScale,

                        lastPosition.y,
                        lastPosition.z),

                    new Vector3(deltaX, 1, stackBounds.y));

                //�޺� �ʱ�ȭ
                comboCount = 0;

            }
            else //��ġ ������
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up; //���� ��Ͽ��� ��ĭ ���� ���� ��ġ������ ������
                //�̰� ������ ���� �������� ��� ���� ������ �������� �߻��� ����.
            }

        }
        else //z��
        {
            float deltaZ = prevBlockPosition.z - lastPosition.z;
            bool isNegativeNum = (deltaZ < 0) ? true : false; //�߷����� ����� ������ ���� ����

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


                //Rubble ���� ����
                float rubbleHalfScale = deltaZ / 2f;
                CreateRubble(
                    new Vector3(
                        lastPosition.x,
                        lastPosition.y,

                        isNegativeNum ? //z�� ����
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

        //�̵��� ������ ���Ŀ� ���� x/z �� ���� ������.
        secondaryPosition = (isMovingX) ? lastBlock.localPosition.x : lastBlock.localPosition.z;
        //���� ��� ��ġ�� ����ؼ� �ٲ�� �ֱ� ������ ������ 0�� ��ġ�� ��� ��� ����
        // �׷��� �̵���Ų ���� ���� �����ص״ٰ� Moving���� ����ϰ� �ִ� ��

        return true;
    }

    //���� �����
    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(lastBlock.gameObject);
        go.transform.parent = this.transform;

        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localRotation = Quaternion.identity;

        go.AddComponent<Rigidbody>(); //������ �ٴ����� ���������ϱ⿡
        go.name = "Rubble"; //���ӿ�����Ʈ �̸� �����
    }

    void ComboCheck()
    {
        comboCount++;

        if (comboCount > maxCombo)
            maxCombo = comboCount;

        if ((comboCount % 5) == 0)
        {
            Debug.Log("5 �޺� ����");
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
            Debug.Log("���� ����");
            bestScore = stackCount;
            bestCombo = maxCombo;

            PlayerPrefs.SetInt(BestScoreKey, bestScore); //����
            PlayerPrefs.SetInt(BestComboKey, bestCombo);
        }
    }

    void GameOverEffect()
    {
        int  childCount = this.transform.childCount; //childCount -> Ʈ�������� ������ �ִ� ������Ʈ�� ���� (�ȿ� �ִ� ����)

        for (int i = 1; i <= 20; i++)
        {
            if (childCount < i) break; //�ε������� ����⿡ �극��ũ

            GameObject go = transform.GetChild(childCount - i).gameObject; //���� ������Ʈ�� �ε����� ã�ƿ���

            if (go.name.Equals("Rubble")) continue; //Rubble -> �ٴ����� �������� �ִ� ��

            Rigidbody rigid = go.AddComponent<Rigidbody>(); // ������ٵ� �޾��ֱ�
            rigid.AddForce( //�� �����ؼ� ����������
                (Vector3.up * Random.Range(0, 10f) + Vector3.right * (Random.Range(0, 10f) - 5f)) * 100f
                );
        }
    }

    public void Restart()
    { //���� ��� �� �ʱ�ȭ
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

        //ó�� ��ϰ� �̵� ��� 2�� ����
        Spawn_Block();
        Spawn_Block();
    }
}
