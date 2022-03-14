using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
	[SerializeField] private GameObject landingPagePane = null;
	[SerializeField] private TMP_InputField addressInput = null;
	[SerializeField] private Button joinButotn = null;

	private void OnEnable() 
	{
		RTSNetworkManager.ClientOnConnected += HandleClientConnected;	
		RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
	}

	private void OnDisable()
	{
		RTSNetworkManager.ClientOnConnected -= HandleClientConnected;	
		RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
	}

	public void Join()
	{
		string address = addressInput.text;

		NetworkManager.singleton.networkAddress = address;
		NetworkManager.singleton.StartClient();

		joinButotn.interactable = false;
	}

	private void HandleClientConnected()
	{
		joinButotn.interactable = true;

		gameObject.SetActive(false);
		landingPagePane.SetActive(false);
	}

	private void HandleClientDisconnected()
	{
		joinButotn.interactable = true;
	}	

}