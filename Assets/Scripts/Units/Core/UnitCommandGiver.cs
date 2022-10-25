using System.Collections;
using System.Collections.Generic;
using RTS.Units.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Units.Core
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] LayerMask layerMask = new();

        Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            TryMove(hit.point);
        }

        private void TryMove(Vector3 point)
        {
            var unitSelectionHandler = GetComponent<UnitSelectionHandler>();

            foreach (Unit unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetComponent<UnitMovement>().CmdMove(point);
            }
        }
    }

}