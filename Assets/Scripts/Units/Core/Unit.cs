using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;
using RTS.Units.Movement;
using RTS.Combat;

namespace RTS.Units.Core
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] UnitMovement unitMovement;
        [SerializeField] Health health;
        [SerializeField] Targeter targeter;
        [SerializeField] UnityEvent onSelected;
        [SerializeField] UnityEvent onDeselected;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;

        bool isHighlighted = false;

        public UnitMovement GetUnitMovement()
        {
            return unitMovement;
        }

        public Targeter GetTargeter()
        {
            return targeter;
        }

        public bool IsHighlighted()
        {
            return isHighlighted;
        }

        #region Server

        public override void OnStartServer()
        {
            ServerOnUnitSpawned?.Invoke(this);
            health.ServerOnDie += ServerHandleDeath;
        }

        public override void OnStopServer()
        {
            ServerOnUnitDespawned?.Invoke(this);
            health.ServerOnDie -= ServerHandleDeath;
        }

        [Server]
        private void ServerHandleDeath()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!hasAuthority) return;

            AuthorityOnUnitDespawned?.Invoke(this);
        }

        [Client]
        public void SelectUnit()
        {
            if (!hasAuthority) return;

            onSelected?.Invoke();
            isHighlighted = true;
        }

        [Client]
        public void DeselectUnit()
        {
            if (!hasAuthority) return;

            onDeselected?.Invoke();
            isHighlighted = false;
        }

        #endregion
    }
}
