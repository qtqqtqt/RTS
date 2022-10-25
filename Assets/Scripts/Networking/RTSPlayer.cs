using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RTS.Units.Core;

namespace RTS.Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        [SerializeField] List<Unit> playerUnits = new();

        public List<Unit> GetPlayerUnits()
        {
            return playerUnits;
        }

        #region Server

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            playerUnits.Add(unit);
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            playerUnits.Remove(unit);
        }

        #endregion

        #region Client

        public override void OnStartClient()
        {
            if (!isClientOnly) return;

            Unit.AuthorityOnUnitSpawned += AuthorityHadleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHadleUnitDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly) return;

            Unit.AuthorityOnUnitSpawned -= AuthorityHadleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHadleUnitDespawned;
        }

        private void AuthorityHadleUnitSpawned(Unit unit)
        {
            if (!hasAuthority) return;

            playerUnits.Add(unit);
        }

        private void AuthorityHadleUnitDespawned(Unit unit)
        {
            if (!hasAuthority) return;

            playerUnits.Remove(unit);
        }

        #endregion
    }
}
