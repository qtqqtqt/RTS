using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

namespace RTS.Units.Movement
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] NavMeshAgent agent = null;

        #region Server

        [ServerCallback]
        private void Update()
        {
            if (!agent.hasPath) return;
            if (agent.remainingDistance > agent.stoppingDistance) return;
            
            agent.ResetPath();
            
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 0.1f, NavMesh.AllAreas)) return;

            agent.SetDestination(hit.position);
        }

        #endregion

        #region Client

        #endregion
    }
}
