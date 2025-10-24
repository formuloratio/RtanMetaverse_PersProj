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


    void Start()
    {
        if (originBlock == null)
        {
            Debug.Log("originBlock�� �����ϴ�.");
            return;
        }
        prevBlockPosition = Vector3.down;

        Spawn_Block(); //�� 1�� ����
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Spawn_Block();
        }

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

        newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        newTrans.localRotation = Quaternion.identity; //Quaternion�� �ʱⰪ, �� ȸ���� ���� ����
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stackCount++;

        desiredPosition = Vector3.down * stackCount; //���� ī��Ʈ �����ϴ� ��ŭ TheStack�� ������
        blockTransition = 0f; //�̵� ó���� ���� ���ذ�(�ʱ�ȭ)

        lastBlock = newTrans;

        return true;
    }
}
