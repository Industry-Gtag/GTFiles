using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020004D9 RID: 1241
public class WorldShareableItemManager : MonoBehaviour
{
	// Token: 0x06001E40 RID: 7744 RVA: 0x000A2532 File Offset: 0x000A0732
	protected void Awake()
	{
		if (WorldShareableItemManager.hasInstance && WorldShareableItemManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		WorldShareableItemManager.SetInstance(this);
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x000A2555 File Offset: 0x000A0755
	protected void OnDestroy()
	{
		if (WorldShareableItemManager.instance == this)
		{
			WorldShareableItemManager.hasInstance = false;
			WorldShareableItemManager.instance = null;
		}
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x000A2570 File Offset: 0x000A0770
	protected void Update()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < WorldShareableItemManager.worldShareableItems.Count; i++)
		{
			if (WorldShareableItemManager.worldShareableItems[i] != null)
			{
				WorldShareableItemManager.worldShareableItems[i].TriggeredUpdate();
			}
		}
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x000A25BD File Offset: 0x000A07BD
	public static void CreateManager()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		WorldShareableItemManager.SetInstance(new GameObject("WorldShareableItemManager").AddComponent<WorldShareableItemManager>());
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x000A25DB File Offset: 0x000A07DB
	private static void SetInstance(WorldShareableItemManager manager)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		WorldShareableItemManager.instance = manager;
		WorldShareableItemManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001E45 RID: 7749 RVA: 0x000A25FE File Offset: 0x000A07FE
	public static void Register(WorldShareableItem worldShareableItem)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!WorldShareableItemManager.hasInstance)
		{
			WorldShareableItemManager.CreateManager();
		}
		if (!WorldShareableItemManager.worldShareableItems.Contains(worldShareableItem))
		{
			WorldShareableItemManager.worldShareableItems.Add(worldShareableItem);
		}
	}

	// Token: 0x06001E46 RID: 7750 RVA: 0x000A262C File Offset: 0x000A082C
	public static void Unregister(WorldShareableItem worldShareableItem)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!WorldShareableItemManager.hasInstance)
		{
			WorldShareableItemManager.CreateManager();
		}
		if (WorldShareableItemManager.worldShareableItems.Contains(worldShareableItem))
		{
			WorldShareableItemManager.worldShareableItems.Remove(worldShareableItem);
		}
	}

	// Token: 0x04002881 RID: 10369
	public static WorldShareableItemManager instance;

	// Token: 0x04002882 RID: 10370
	private static bool hasInstance = false;

	// Token: 0x04002883 RID: 10371
	public static readonly List<WorldShareableItem> worldShareableItems = new List<WorldShareableItem>(1024);
}
