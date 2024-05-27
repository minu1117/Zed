using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.XR;

public class Zed : DemoChampion
{
    public bool isMoved = true;
    public Dictionary<int, ZedShadow> shadows = new();
    private Vector3 dir;
    private Vector3 normalizedCameraForward;
    private Vector3 normalizedCameraRight;
    public CinemachineVirtualCamera virtualCamera;
    private Rigidbody rb;
    private NavMeshAgent agent;

    public override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        base.Awake();
        normalizedCameraForward = virtualCamera.transform.forward;
        normalizedCameraRight = virtualCamera.transform.right;

        normalizedCameraForward.y = 0;
        normalizedCameraRight.y = 0;
        normalizedCameraForward.Normalize();
        normalizedCameraRight.Normalize();

    }

    public NavMeshAgent GetAgent() { return agent; }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Skill useSkill = UseSkill("Q");
            CopySkill(useSkill, slot.GetSlotDict()["Q"].GetPool());
        }

        if (Input.GetKeyDown(KeyCode.F)) 
        {
            var hit = Raycast.GetHit(Input.mousePosition, "Shadow");
            if (hit.collider == null)
            {
                UseSkill("F");
            }
            else
            {
                if (hit.collider.gameObject.TryGetComponent(out ZedShadow shadow))
                {
                    TeleportShadow(shadow);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Skill useSkill = UseSkill("E");
            CopySkill(useSkill, slot.GetSlotDict()["E"].GetPool());
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Skill useSkill = UseSkill("R");
            CopySkill(useSkill, slot.GetSlotDict()["R"].GetPool());
        }
    }

    public void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (isMoved)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 moveDirection = (v * normalizedCameraForward + h * normalizedCameraRight).normalized;
            dir = moveDirection * data.moveSpeed;
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

    public void AddShadow(ZedShadow shadow)
    {
        shadows.Add(shadow.GetID(), shadow);
    }
    public void RemoveShadow(int id)
    {
        shadows.Remove(id);
    }
    public Dictionary<int, ZedShadow> GetShadowDict()
    {
        return shadows;
    }    
    private void CopySkill(Skill useSkill, IObjectPool<Skill> skillPool)
    {
        if (useSkill == null)
            return;

        if (shadows.Count > 0)
        {
            foreach (var shadow in shadows)
            {
                if (useSkill.data.type != SkillType.Dash)
                    shadow.Value.UseCopySkill(useSkill, skillPool);
                else
                {
                    shadow.Value.UseCopyDash();
                }
            }
        }
    }

    public void TeleportShadow(ZedShadow shadow)
    {
        var hit = Raycast.GetHit(Input.mousePosition, "Shadow");
        if (hit.collider == null || !shadow.isReady)
            return;

        Vector3 position = gameObject.transform.position;
        Vector3 shadowPosition = shadow.transform.position;
        Quaternion rotation = transform.rotation;
        Quaternion shadowRotation = shadow.transform.rotation;

        gameObject.transform.position = shadowPosition;
        shadow.transform.position = position;
        gameObject.transform.rotation = shadowRotation;
        shadow.transform.rotation = rotation;
    }
}
