using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    //애니메이터 파라미터 컨트롤하는 변수
    //스트링보다 숫자열로 비교하는 게 좋기에 해쉬라는 숫자열로 변환하는 것
    private static readonly int IsMoving = Animator.StringToHash("isMove");

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Move(Vector2 obj)
    {
        //IsMoving의 벡터 값의 크기 비교해서 0.5보다 크면 true 반환
        animator.SetBool(IsMoving, obj.magnitude > .5f);
    }

}
