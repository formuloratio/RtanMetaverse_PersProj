using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    //뒤집힘
    protected bool isFlipX = false;

    //이동하는 방향
    protected Vector2 movementDirection = Vector2.zero;
    public Vector2 MovementDirection { get { return movementDirection; } }

    //바라보는 방향
    protected Vector2 lookDirection = Vector2.zero;
    public Vector2 LookDirection { get { return lookDirection; } }

    //넉백 방향
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
        //Rotate(lookDirection); //회전
        SimpleRotate(isFlipX);
    }

    protected virtual void FixedUpdate()
    {
        Movement(movementDirection); // 이동
        if (knockbackDuration > 0.0f) //넉백 지속시간 줄여주기
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
        if (knockbackDuration > 0.0f) //넉백 지속이 남아있다면(넉백 적용 필요할 때)
        {
            direction *= 0.2f; // 이동 방향의 힘 줄이기
            direction += knockback; // 넉백의 힘 넣기
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
        //y와 x값을 받아서 그 사이의 세타값을 구함
        //라디안(파이) 값이 나옴 -> 디그리(180도) 값으로 변환 필요 (Mathf.Rad2Deg 곱하기)

        bool isLeft = Mathf.Abs(rotZ) > 90f; //절대값이 90보다 크면 왼쪽 바라보게
        characterRenderer.flipX = isLeft; // 이미지 뒤집힘

        if (weaponPivot != null)
        {
            //무기 회전 값
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, rotZ); // Deg값 가지고 있기에 오일러 값 사용
        }
    }

    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        knockback = -(other.position - transform.position).normalized * power;
        //normalized -> 이동 방향 정규화(벡터의 길이를 1로 만들어주는 작업)
    }


}
