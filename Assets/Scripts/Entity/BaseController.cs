using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    //������
    protected bool isFlipX = false;

    //�̵��ϴ� ����
    protected Vector2 movementDirection = Vector2.zero;
    public Vector2 MovementDirection { get { return movementDirection; } }

    //�ٶ󺸴� ����
    protected Vector2 lookDirection = Vector2.zero;
    public Vector2 LookDirection { get { return lookDirection; } }

    //�˹� ����
    private Vector2 knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    protected AnimationHandler animationHandler;

    protected virtual void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleAction();
        //Rotate(lookDirection); //ȸ��
        SimpleRotate(isFlipX);
    }

    protected virtual void FixedUpdate()
    {
        Movement(movementDirection); // �̵�
        if (knockbackDuration > 0.0f) //�˹� ���ӽð� �ٿ��ֱ�
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    protected virtual void HandleAction()
    {

    }

    private void Movement(Vector2 direction)
    {
        direction = direction * 5;
        if (knockbackDuration > 0.0f) //�˹� ������ �����ִٸ�(�˹� ���� �ʿ��� ��)
        {
            direction *= 0.2f; // �̵� ������ �� ���̱�
            direction += knockback; // �˹��� �� �ֱ�
        }

        _rigidbody.velocity = direction;
        animationHandler.Move(direction);
    }

    private void SimpleRotate(bool isFlip)
    {
        characterRenderer.flipX = isFlip;
    }

    private void Rotate(Vector2 direction)
    {
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //y�� x���� �޾Ƽ� �� ������ ��Ÿ���� ����
        //����(����) ���� ���� -> ��׸�(180��) ������ ��ȯ �ʿ� (Mathf.Rad2Deg ���ϱ�)

        bool isLeft = Mathf.Abs(rotZ) > 90f; //���밪�� 90���� ũ�� ���� �ٶ󺸰�
        characterRenderer.flipX = isLeft; // �̹��� ������

        if (weaponPivot != null)
        {
            //���� ȸ�� ��
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, rotZ); // Deg�� ������ �ֱ⿡ ���Ϸ� �� ���
        }
    }

    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        knockback = -(other.position - transform.position).normalized * power;
        //normalized -> �̵� ���� ����ȭ(������ ���̸� 1�� ������ִ� �۾�)
    }


}
