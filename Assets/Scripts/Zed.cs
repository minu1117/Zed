using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Zed : DemoChampion
{
    public bool isMoved = true;
    public Dictionary<int, ZedShadow> shadows = new();
    public CharacterController controller;
    private Vector3 dir;

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
            if (controller.isGrounded)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                dir = new Vector3(h * data.moveSpeed, 0f, v * data.moveSpeed);

                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.Euler(0, Mathf.Atan2(h, v) * Mathf.Rad2Deg, 0);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    dir.y = data.jumpSpeed;
                }
            }
            else
            {
                dir.y -= data.gravity * Time.deltaTime;
            }

            controller.Move(dir * Time.deltaTime);
        }
    }

    public void StopMove()
    {
        isMoved = false;
        dir = Vector3.zero;
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
