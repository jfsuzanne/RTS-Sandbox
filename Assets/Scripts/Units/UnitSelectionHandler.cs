using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
	[SerializeField] private RectTransform unitSelectionArea = null;

	private Vector2 startPosition;

    private Camera mainCamera;
	private RTSPlayer player;

    public List<Unit> SelectedUnits { get; }= new List<Unit>();

    private void Start()
    {
        mainCamera = Camera.main;

		player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

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
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
		else if (Mouse.current.leftButton.isPressed)
		{
			UptadeSelectionArea();
		}
    }

	private void UptadeSelectionArea()
	{
		Vector2 mousePosition = Mouse.current.position.ReadValue();

		float areaWidth = mousePosition.x - startPosition.x;
		float areaHeight = mousePosition.y - startPosition.y;

		unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
		unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
	}

	private void StartSelectionArea()
	{
		if(!Keyboard.current.leftShiftKey.isPressed)
		{
			foreach (Unit selectedUnit in SelectedUnits)
        	{
            	selectedUnit.Deselect();
        	}

        	SelectedUnits.Clear();
		}

		unitSelectionArea.gameObject.SetActive(true);

		startPosition = Mouse.current.position.ReadValue();

		UptadeSelectionArea();
	}

    private void ClearSelectionArea()
    {
		unitSelectionArea.gameObject.SetActive(false);


		if(unitSelectionArea.sizeDelta.magnitude == 0)
		{
			Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        	if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        	if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

        	if (!unit.hasAuthority) { return; }

        	SelectedUnits.Add(unit);

        	foreach(Unit selectedUnit in SelectedUnits)
        	{
            	selectedUnit.Select();
	        }

			return;
		}

		Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
		Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

		foreach(Unit unit in player.GetMyUnits())
		{
			if(SelectedUnits.Contains(unit)) {continue;}

			Vector3 screenPostion = mainCamera.WorldToScreenPoint(unit.transform.position);

			if(screenPostion.x > min.x && screenPostion.x < max.x &&
				screenPostion.y > min.y && screenPostion.y < max.y)
			{
				SelectedUnits.Add(unit);
				unit.Select();
			}
		}

    }

	private void AuthorityHandleUnitDespawned(Unit unit)
	{
		SelectedUnits.Remove(unit);
	}

	private void ClientHandleGameOver(string winnerName)
	{
		enabled = false;
	}
}
