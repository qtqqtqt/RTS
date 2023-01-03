using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTS.Buildings
{
    public class Building : NetworkBehaviour
    {
        [SerializeField] GameObject buildingPreview = null;
        [SerializeField] Sprite icon = null;
        [SerializeField] int id = -1;
        [SerializeField] int buildingPrice = 100;

        public static event Action<Building> ServerOnBuildingSpawned;
        public static event Action<Building> ServerOnBuildingDespawned;

        public static event Action<Building> AuthorityOnBuildingSpawned;
        public static event Action<Building> AuthorityOnBuildingDespawned;

        public GameObject GetBuildingPreview()
        {
            return buildingPreview;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public int GetId()
        {
            return id;
        }

        public int GetBuildingPrice()
        {
            return buildingPrice;
        }

        #region Server

        public override void OnStartServer()
        {
            ServerOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnBuildingDespawned?.Invoke(this);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            AuthorityOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!hasAuthority) return;

            AuthorityOnBuildingDespawned?.Invoke(this);
        }

        #endregion
    }
}
