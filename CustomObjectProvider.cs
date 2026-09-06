using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using UnityEngine;

// Token: 0x0200042E RID: 1070
public class CustomObjectProvider : NetworkObjectProviderDefault
{
	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06001960 RID: 6496 RVA: 0x0008EAAC File Offset: 0x0008CCAC
	private static NetworkObjectBaker Baker
	{
		get
		{
			NetworkObjectBaker networkObjectBaker;
			if ((networkObjectBaker = CustomObjectProvider.baker) == null)
			{
				networkObjectBaker = (CustomObjectProvider.baker = new NetworkObjectBaker());
			}
			return networkObjectBaker;
		}
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x0008EAC2 File Offset: 0x0008CCC2
	public override NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
	{
		NetworkObjectAcquireResult networkObjectAcquireResult = base.AcquirePrefabInstance(runner, in context, out instance);
		if (networkObjectAcquireResult == NetworkObjectAcquireResult.Success)
		{
			this.IsGameMode(instance);
			return networkObjectAcquireResult;
		}
		instance = null;
		return networkObjectAcquireResult;
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x0008EADC File Offset: 0x0008CCDC
	private void IsGameMode(NetworkObject instance)
	{
		if (instance.gameObject.GetComponent<GameModeSerializer>() != null)
		{
			global::GorillaGameModes.GameMode.GetGameModeInstance(global::GorillaGameModes.GameMode.GetGameModeKeyFromRoomProp()).AddFusionDataBehaviour(instance);
			CustomObjectProvider.Baker.Bake(instance.gameObject);
		}
	}

	// Token: 0x06001963 RID: 6499 RVA: 0x0008EB12 File Offset: 0x0008CD12
	protected override void DestroySceneObject(NetworkRunner runner, NetworkSceneObjectId sceneObjectId, NetworkObject instance)
	{
		if (this.SceneObjects != null && this.SceneObjects.Contains(instance.gameObject))
		{
			return;
		}
		base.DestroySceneObject(runner, sceneObjectId, instance);
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x0008EB39 File Offset: 0x0008CD39
	protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
	{
		base.DestroyPrefabInstance(runner, prefabId, instance);
	}

	// Token: 0x04002472 RID: 9330
	public const int GameModeFlag = 1;

	// Token: 0x04002473 RID: 9331
	public const int PlayerFlag = 2;

	// Token: 0x04002474 RID: 9332
	private static NetworkObjectBaker baker;

	// Token: 0x04002475 RID: 9333
	internal List<GameObject> SceneObjects;
}
