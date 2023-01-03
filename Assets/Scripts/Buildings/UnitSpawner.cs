using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using RTS.Combat;

namespace RTS.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] GameObject unitPrefab = null;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] Health health;

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDeath;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDeath;
        }

        [Server]
        private void ServerHandleDeath()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            GameObject unitInstance = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(unitInstance, connectionToClient);
        }

        #endregion

        #region Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!hasAuthority) return;

            CmdSpawnUnit();
        }

        #endregion
    }
}
