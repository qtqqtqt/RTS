using System.Collections;
using System.Collections.Generic;
using Mirror;
using RTS.Buildings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RTS.Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] GameObject unitSpawnerPrefab;
        [SerializeField] GameOverHandler gameOverHandlerPrefab;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            GameObject unitSpawnerInstance = Instantiate(
                unitSpawnerPrefab,
                conn.identity.transform.transform.position,
                conn.identity.transform.rotation);

            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Map"))
            {
                GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
                NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            }
        }
    }
}
