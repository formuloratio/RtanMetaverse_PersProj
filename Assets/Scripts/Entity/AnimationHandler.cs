using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    //�ִϸ����� �Ķ���� ��Ʈ���ϴ� ����
    //��Ʈ������ ���ڿ��� ���ϴ� �� ���⿡ �ؽ���� ���ڿ��� ��ȯ�ϴ� ��
    private static readonly int IsMoving = Animator.StringToHash("isMove");

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Move(Vector2 obj)
    {
        //IsMoving�� ���� ���� ũ�� ���ؼ� 0.5���� ũ�� true ��ȯ
        animator.SetBool(IsMoving, obj.magnitude > .5f);
    }

}
