using System.Collections;
using System.Collections.Generic;
using RTS.Buildings;
using RTS.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Units.Core
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] LayerMask layerMask = new();

        Camera mainCamera;
        UnitSelectionHandler unitSelectionHandler;

        private void Awake()
        {
            unitSelectionHandler = GetComponent<UnitSelectionHandler>();
        }

        private void Start()
        {
            mainCamera = Camera.main;
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            if(hit.collider.TryGetComponent(out Targetable target))
            {
                if (target.hasAuthority)
                {
                    TryMove(hit.point);
                    return;
                }
                TryTarget(target);
                return;
            }
            TryMove(hit.point);
        }

        private void TryMove(Vector3 point)
        {
            foreach (Unit unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetUnitMovement().CmdMove(point);
            }
        }

        private void TryTarget(Targetable target)
        {
            foreach (Unit unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetTargeter().CmdSetTarget(target.gameObject);
            }
        }

        private void ClientHandleGameOver(string winner)
        {
            enabled = false;
        }
    }

}