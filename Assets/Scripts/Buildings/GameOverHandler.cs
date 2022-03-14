using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
	public static event Action ServerOnGameOver;

	public static event Action<string> ClientOnGameOver;

	[SerializeField]
	private List<UnitBase> bases = new List<UnitBase>();

	#region Server

	public override void OnStartServer()
	{
		UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawn;
		UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawn;
	}

	public override void OnStopServer()
	{
		UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawn;
		UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawn;
	}

	[Server]
	private void ServerHandleBaseSpawn(UnitBase unitBase)
	{
		bases.Add(unitBase);
	}

	[Server]
	private void ServerHandleBaseDespawn(UnitBase unitBase)
	{
		bases.Remove(unitBase);

		if (bases.Count != 1) { return; }

		int playerId = bases[0].connectionToClient.connectionId;

		RpcGameOver($"Player {playerId}");

		ServerOnGameOver?.Invoke();
	}

	#endregion

	#region Client

	[ClientRpc]
	private void RpcGameOver(string winner)
	{
		ClientOnGameOver?.Invoke(winner);
	}

	#endregion
}
