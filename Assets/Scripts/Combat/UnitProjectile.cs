using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class UnitProjectile : NetworkBehaviour 
    {
        [SerializeField] int damageToDeal = 20;
        [SerializeField] float launchForce;
        [SerializeField] float destroyAfterSeconds = 5f;

        Rigidbody projectileRigidbody;

        private void Awake()
        {
            projectileRigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            projectileRigidbody.velocity = transform.forward * launchForce;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient) return;
            }

            if (other.TryGetComponent(out Health health))
            {
                health.DealDamage(damageToDeal);
            }

            DestroySelf();
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

    }
}
