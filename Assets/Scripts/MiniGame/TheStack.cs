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

    //���� �÷� ����
    public Color prevColor;
    public Color nextColor;

    bool isMovingX = true; //�̵� ���� �˻�


    void Start()
    {
        if (originBlock == null)
        {
            Debug.Log("originBlock�� �����ϴ�.");
            return;
        }

        //�̸� ���� ����
        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        prevBlockPosition = Vector3.down;

        Spawn_Block(); //�� 1�� ����

        Spawn_Block();
    }

    void Update()
    {
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

            }
            else //��ġ ������
            {
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
            }
            else
            {
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
}
