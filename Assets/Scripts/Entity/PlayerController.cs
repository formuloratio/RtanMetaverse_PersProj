using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : BaseController
{
    private Camera camera;

    protected override void Start()
    {
        base.Start();
        camera = Camera.main;
    }

    protected override void HandleAction()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertial = Input.GetAxisRaw("Vertical");
        movementDirection = new Vector2(horizontal, vertial).normalized;

        Debug.Log(movementDirection);

        if (movementDirection.x > 0)
        {
            isFlipX = false;
        }
        else if (movementDirection.x < 0)
        {
            isFlipX = true;
        }

        //�Ʒ��� ���콺�� �¿� �����ϴ� Rotate(lookDirection); �� �ʿ��� ����
        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);
        lookDirection = (worldPos - (Vector2)transform.position);

        if (lookDirection.magnitude < 0.9f) //lookDirection�� ������ ũ�Ⱑ 0.9���� ������ ����ó��
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            lookDirection = lookDirection.normalized;
        }
    }
}
