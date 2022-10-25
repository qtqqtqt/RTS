using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTS.Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] GameObject unitSpawnerPrefab;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            GameObject unitSpawnerInstance = Instantiate(
                unitSpawnerPrefab,
                conn.identity.transform.transform.position,
                conn.identity.transform.rotation);

            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
    }
}
