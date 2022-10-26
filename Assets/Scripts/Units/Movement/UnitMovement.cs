using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using RTS.Combat;

namespace RTS.Units.Movement
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] NavMeshAgent agent = null;
        [SerializeField] Targeter targeter = null;
        [SerializeField] float chaseRange = 5f;

        #region Server

        [ServerCallback]
        private void Update()
        {

            Targetable target = targeter.GetTarget();

            if (target != null)
            {
                if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    agent.SetDestination(target.transform.position);
                }
                else if(agent.hasPath)
                {
                    agent.ResetPath();
                }

                return;
            }

            if (!agent.hasPath) return;
            if (agent.remainingDistance > agent.stoppingDistance) return;
            
            agent.ResetPath();
            
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            targeter.ClearTarget();

            if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 0.1f, NavMesh.AllAreas)) return;

            agent.SetDestination(hit.position);
        }

        #endregion

        #region Client

        #endregion
    }
}
