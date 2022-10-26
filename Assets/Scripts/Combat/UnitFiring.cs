using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] Transform projectileSpawnPoint;
        [SerializeField] float attackRange = 7f;
        [SerializeField] float fireRate = 1f;
        [SerializeField] float rotationSpeed = 20f;

        Targeter targeter;
        float lastfireTime;

        private void Awake()
        {
            targeter = GetComponent<Targeter>();
        }

        private Vector3 GetTargetPosition()
        {
            return targeter.GetTarget().transform.position;
        }

        [ServerCallback]
        private void Update()
        {
            if (targeter.GetTarget() == null) return;
            if (!CanFireAtTarget()) return;

            Quaternion targetRotaion = Quaternion.LookRotation(GetTargetPosition() - transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotaion, rotationSpeed * Time.deltaTime);

            if(Time.time > (1 / fireRate) + lastfireTime)
            {
                FireProjectile();
            }
        }

        [Server]
        private void FireProjectile()
        {
            Quaternion projectileRotation = Quaternion.LookRotation(targeter.GetTargetAimAtPoint().position - projectileSpawnPoint.position);
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastfireTime = Time.time;
        }

        [Server]
        private bool CanFireAtTarget()
        {
            return (GetTargetPosition() - transform.position).sqrMagnitude <= attackRange * attackRange;
        }
    }
}
