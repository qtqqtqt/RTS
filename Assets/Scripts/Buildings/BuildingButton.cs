using System.Collections;
using System.Collections.Generic;
using Mirror;
using RTS.Buildings;
using RTS.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Building building = null;
    [SerializeField] Image iconImage = null;
    [SerializeField] TMP_Text priceText = null;
    [SerializeField] LayerMask floorMask = new();

    Camera mainCamera;
    RTSPlayer player;
    GameObject buildingPreviewInstance;
    Renderer buildingRendererInstance;

    private void Start()
    {
        mainCamera = Camera.main;
        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetBuildingPrice().ToString();
    }

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (buildingPreviewInstance == null) return;

        UpdatePreviewBuilding();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        buildingPreviewInstance = Instantiate(building.GetBuildingPreview(), Mouse.current.position.ReadValue(), Quaternion.identity);
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false); 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingPreviewInstance == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }

        Destroy(buildingPreviewInstance);
    }

    private void UpdatePreviewBuilding()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) return;

        buildingPreviewInstance.transform.position = hit.point;
        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }
    }


}
