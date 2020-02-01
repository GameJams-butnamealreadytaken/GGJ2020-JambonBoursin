using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Crew
{
    public class CrewController : MonoBehaviour
    {
        [Tooltip("[Debug] If true the crew member can be directly moved by mouse click")]
        public bool m_autoAttachToWorldClicker;

        private NavMeshAgent    m_navMeshAgent;
        private Animator        m_animator;

        private int m_speedHash;

        void Start()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_animator = GetComponent<Animator>();

            m_speedHash = Animator.StringToHash("Speed");

            m_navMeshAgent.updateRotation = false;

            if (m_autoAttachToWorldClicker)
            {
                GameObject gameManager = GameObject.Find("GameManager");
                gameManager.GetComponent<WorldClickDestinationSetter>().AddOnClickedCallback(SetDestination);
            }
        }

        void Update()
        {
            m_animator.SetFloat(m_speedHash, m_navMeshAgent.velocity.normalized.magnitude);
        }

        void LateUpdate()
        {
            if (m_navMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon)
            {
                Vector3 lookPosition = m_navMeshAgent.velocity;
                lookPosition.y = 0.0f;

                transform.rotation = Quaternion.LookRotation(lookPosition.normalized);
            }
        }

        public void SetDestination(Vector3 destination)
        {
            m_navMeshAgent.destination = destination;
        }
    }
}
