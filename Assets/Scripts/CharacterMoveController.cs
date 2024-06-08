using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMoveController : MonoBehaviour
{
    public bool isMoved = true;
    public CinemachineVirtualCamera virtualCamera;

    private float moveSpeed;
    private Vector3 dir;
    private Vector3 normalizedCameraForward;
    private Vector3 normalizedCameraRight;
    private Rigidbody rb;
    private NavMeshAgent agent;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        moveSpeed = GetComponent<DemoChampion>().data.moveSpeed;

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
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 moveDirection = (v * normalizedCameraForward + h * normalizedCameraRight).normalized;
            dir = moveDirection * moveSpeed;
            if (moveDirection != Vector3.zero)
            {
                rb.transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            rb.velocity = dir;
        }
    }

    public void StopMove()
    {
        isMoved = false;
        dir = Vector3.zero;
        rb.velocity = dir;
    }

    public void FixedUpdate()
    {
        Move();
    }
}
