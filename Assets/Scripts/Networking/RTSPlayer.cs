using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RTS.Units.Core;
using RTS.Buildings;

namespace RTS.Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        [SerializeField] Building[] buildings = new Building[0];

        List<Unit> playerUnits = new();
        List<Building> playerBuildings = new();

        public List<Unit> GetPlayerUnits()
        {
            return playerUnits;
        }

        public List<Building> GetPlayerBuildings()
        {
            return playerBuildings;
        }

        #region Server

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
        }

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 placePosition)
        {
            Building buildingToPlace = null;

            foreach (Building building in buildings)
            {
                if (building.GetId() != buildingId)
                {
                    buildingToPlace = building;
                    break;
                }
            }
            if (buildingToPlace == null) return;

            GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, placePosition, buildingToPlace.transform.rotation);
            NetworkServer.Spawn(buildingInstance, connectionToClient);
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

        private void ServerHandleBuildingSpawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

            playerBuildings.Add(building);
        }

        private void ServerHandleBuildingDespawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

            playerBuildings.Remove(building);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;

            Unit.AuthorityOnUnitSpawned += AuthorityHadleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHadleUnitDespawned;
            Building.AuthorityOnBuildingSpawned += AuthorityHadleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned += AuthorityHadleBuildingDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;

            Unit.AuthorityOnUnitSpawned -= AuthorityHadleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHadleUnitDespawned;
            Building.AuthorityOnBuildingSpawned -= AuthorityHadleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned -= AuthorityHadleBuildingDespawned;
        }

        private void AuthorityHadleUnitSpawned(Unit unit)
        {
            playerUnits.Add(unit);
        }

        private void AuthorityHadleUnitDespawned(Unit unit)
        {
            playerUnits.Remove(unit);
        }

        private void AuthorityHadleBuildingSpawned(Building building)
        {
            playerBuildings.Add(building);
        }

        private void AuthorityHadleBuildingDespawned(Building building)
        {
            playerBuildings.Remove(building);
        }

        #endregion
    }
}
