using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

namespace RTS.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] GameObject unitPrefab = null;
        [SerializeField] Transform spawnPoint = null;

        #region Server

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
