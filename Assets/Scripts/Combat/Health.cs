using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] int maxHealth = 100;

        public event Action ServerOnDie;
        public event Action<int, int> ClientOnHealthUpdated; 

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        int currentHealth;

        #region Server

        public override void OnStartServer()
        {
            currentHealth = maxHealth;
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (currentHealth == 0) return;

            currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

            if (currentHealth != 0) return;

            ServerOnDie?.Invoke();
            Debug.Log("Died");
        }

        #endregion

        #region Client

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        }

        #endregion
    }
}
