using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class Zed : DemoChampion
{
    public bool isMoved = true;
    public Dictionary<int, ZedShadow> shadows = new();
    public NavMeshAgent agent;

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

        Move();
    }

    private void Move()
    {
        if (isMoved)
        {
            float xMove = Input.GetAxis("Horizontal");
            float zMove = Input.GetAxis("Vertical");

            Vector3 getVel = new Vector3(xMove, 0, zMove) * data.moveSpeed;
            rigidBody.velocity = getVel;

            if (getVel != Vector3.zero)
                transform.forward = getVel;
        }
    }

    public void StopMove()
    {
        isMoved = false;
        rigidBody.velocity = Vector3.zero;
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
