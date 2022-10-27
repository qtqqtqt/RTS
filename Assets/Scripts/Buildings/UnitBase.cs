using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using RTS.Combat;
using UnityEngine;

namespace RTS.Buildings
{
    public class UnitBase : NetworkBehaviour
    {
        [SerializeField] Health health;

        public static event Action<int> ServerOnPlayerDie;
        public static event Action<UnitBase> ServerOnBaseSpawned;
        public static event Action<UnitBase> ServerOnBaseDespawned;

        #region Server

        public override void OnStartServer()
        {
            ServerOnBaseSpawned?.Invoke(this);
            health.ServerOnDie += ServerHandleDeath;
        }

        public override void OnStopServer()
        {
            ServerOnBaseDespawned?.Invoke(this);
            health.ServerOnDie -= ServerHandleDeath;
        }

        [Server]
        private void ServerHandleDeath()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        #endregion
    }
}
