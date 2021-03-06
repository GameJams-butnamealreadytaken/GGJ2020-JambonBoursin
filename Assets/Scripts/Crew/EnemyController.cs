﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Objects.InteractableObject ObjectTarget;
    private GameObject CrewTarget;

    private NavMeshAgent m_navMeshAgent;
    private Animator m_animator;

    private int m_speedHash;
    private int m_attackHash;

    public AudioClip m_deathSound;

    private int m_hp;

    private UI.UnitUI m_ui;

    private void Start()
    {
        m_ui = transform.Find("EnemyUI").GetComponent<UI.UnitUI>();

        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();

        m_speedHash = Animator.StringToHash("Speed");
        m_attackHash = Animator.StringToHash("Attack");

        m_hp = 100;
    }

    void Update()
    {
        if (!ObjectTarget && !CrewTarget)
        {
            // Select target
            InteractableObjectManager manager = GameManager.m_instance.GetComponent<InteractableObjectManager>();
            if (manager)
            {
                Vector3 vLocation;
                ObjectTarget = manager.GetNearestAvailableOBject(gameObject.transform.position);
                if (ObjectTarget)
                {
                    // Attack object
                    if (ObjectTarget.GetPlacementPoint(gameObject, out vLocation))
                    {
                        //GameObject prim2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        //prim2.transform.position = vLocation;
                        m_navMeshAgent.destination = vLocation;
                    }
                }
                else
                {
                    // Attack nearest NPC
                    // TODO
                }
            }

            //Debug
            //ObjectTarget = null;

            // No more object to attack
            if (!ObjectTarget)
            {
                GameObject[] Crew;
                Crew = GameObject.FindGameObjectsWithTag("Crew");

                float fDist = float.MaxValue;
                foreach (GameObject crew in Crew)
                {
                    float fVal = (crew.transform.position - gameObject.transform.position).sqrMagnitude;
                    if (fVal < fDist)
                    {
                        fDist = fVal;
                        CrewTarget = crew;
                    }
                }
            }
        }
        else if (ObjectTarget)
        {
            if (m_navMeshAgent.enabled)
            {
                if (m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(ObjectTarget.transform.position - transform.position);
                    float fVal = Mathf.Min(2.0f * Time.deltaTime, 1);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, fVal);
                    m_animator.SetBool(m_attackHash, true);
                }
            }
        }
        else if (CrewTarget)
        {
            if (m_navMeshAgent.enabled)
            {
                m_navMeshAgent.destination = CrewTarget.transform.position;
                if (m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
                {
                    m_animator.SetBool(m_attackHash, true);
                }
                else
                {
                    m_animator.SetBool(m_attackHash, false);
                }
            }
        }

        if (!ObjectTarget && !CrewTarget)
        {
            m_animator.SetBool(m_attackHash, false);
        }

        m_animator.SetFloat(m_speedHash, m_navMeshAgent.velocity.normalized.magnitude);
    }

    public void OnTargetHpChanged(int newHP)
    {
        if (newHP <= 0)
        {
            ObjectTarget = null;
            CrewTarget = null;
            m_animator.SetBool(m_attackHash, false);
        }
    }

    public void OnDeath()
    {
        if (ObjectTarget != null)
        {
            ObjectTarget.FreePlacement(gameObject);
        }
    }

    public void PlayDeathSound()
    {
		AudioSource.PlayClipAtPoint(m_deathSound, Camera.main.transform.position, 0.4f);
	}

    public void StopSounds()
    {
        GetComponent<AudioSource>().Stop();
    }

    public void Damage()
    {

    }

    public int TakeDamage(int damages)
    {
        m_hp -= damages;

        m_ui.SetLifeBarFill(m_hp / 100.0f);

        if (m_hp <= 0)
        {
            PlayDeathSound();
            Destroy(gameObject);
        }

        return m_hp;
    }

    public void Attack()
    {
        if (CrewTarget)
        {
            Unit crew = CrewTarget.GetComponent<Unit>();
            if (crew)
            {
                OnTargetHpChanged(crew.TakeDamages(10));
                return;
            }
        }

        ObjectTarget?.Interact(gameObject);
    }
}
