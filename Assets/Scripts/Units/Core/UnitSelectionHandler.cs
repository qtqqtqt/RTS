using System;
using System.Collections;
using Mirror;
using System.Collections.Generic;
using RTS.Networking;
using UnityEngine;
using UnityEngine.InputSystem;
using RTS.Buildings;

namespace RTS.Units.Core
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        public List<Unit> SelectedUnits { get; } = new();

        [SerializeField] RectTransform unitSelectionArea = null;
        [SerializeField] LayerMask layerMask = new();

        Camera mainCamera;
        RTSPlayer player;
        Vector2 startPosition;

        private void Start()
        {
            mainCamera = Camera.main;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private void Update()
        {
            if(player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.IsPressed())
            {
                UpdateSelectionArea();
            }
        }

        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                DeselectUnits();
            }
            unitSelectionArea.gameObject.SetActive(true);
            startPosition = GetMousePosition();
            UpdateSelectionArea();

        }

        private void ClearSelectionArea()
        {
            unitSelectionArea.gameObject.SetActive(false);
            if (unitSelectionArea.sizeDelta.magnitude == 0)
            {
                SelectSingleUnit();
                return;
            }

            foreach (Unit unit in player.GetPlayerUnits())
            {
                if (unit.IsHighlighted())
                {
                    SelectedUnits.Add(unit);
                }
            }
        }

        private void UpdateSelectionArea()
        {
            Vector2 mousePosition = GetMousePosition();

            float selectionAreaWidth = mousePosition.x - startPosition.x;
            float selectionAreaHight = mousePosition.y - startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(selectionAreaWidth), Mathf.Abs(selectionAreaHight));
            unitSelectionArea.anchoredPosition = startPosition + new Vector2(selectionAreaWidth / 2, selectionAreaHight / 2);

            SelectMultipleUnits();
        }

        private void SelectMultipleUnits()
        {
            Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
            Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

            foreach (Unit unit in player.GetPlayerUnits())
            {
                if (SelectedUnits.Contains(unit)) continue;

                Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

                if (screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
                {
                    unit.SelectUnit();
                }
                else
                {
                    unit.DeselectUnit();
                }
            }
        }

        private void SelectSingleUnit()
        {
            Ray ray = mainCamera.ScreenPointToRay(GetMousePosition());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;
            if (!hit.collider.TryGetComponent(out Unit unit)) return;
            if (!unit.hasAuthority) return;

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.SelectUnit();
            }
        }

        private void DeselectUnits()
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.DeselectUnit();
            }

            SelectedUnits.Clear();
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            SelectedUnits.Remove(unit); 
        }

        private void ClientHandleGameOver(string winner)
        {
            enabled = false;
        }

        private Vector2 GetMousePosition()
        {
            return Mouse.current.position.ReadValue();
        }


    }
}
