using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMoveController : MonoBehaviour
{
    public bool isMoved = true;
    public CinemachineVirtualCamera virtualCamera;
    private CharacterAnimationController animationController;

    private float moveSpeed;
    private Vector3 dir;
    private Vector3 normalizedCameraForward;
    private Vector3 normalizedCameraRight;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Vector2 movement = Vector2.zero;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        moveSpeed = GetComponent<DemoChampion>().data.moveSpeed;
        animationController = GetComponent<CharacterAnimationController>();

        normalizedCameraForward = virtualCamera.transform.forward;
        normalizedCameraRight = virtualCamera.transform.right;

        normalizedCameraForward.y = 0;
        normalizedCameraRight.y = 0;
        normalizedCameraForward.Normalize();
        normalizedCameraRight.Normalize();
    }

    public NavMeshAgent GetAgent() { return agent; }

    private void Move()
    {
        if (isMoved)
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");

            Vector3 moveDirection = (movement.y * normalizedCameraForward + movement.x * normalizedCameraRight).normalized;
            dir = moveDirection * moveSpeed;
            if (moveDirection != Vector3.zero)
            {
                rb.transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            transform.position += dir * Time.deltaTime;
        }
    }

    public void FixedUpdate()
    {
        Move();
    }

    public void Update()
    {
        animationController.UpdateMoveAnimation(movement);
    }

    public void StopMove()
    {
        isMoved = false;
        movement = Vector2.zero;
        dir = Vector3.zero;
        rb.velocity = dir;
    }
}
