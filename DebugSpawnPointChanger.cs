using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020005B7 RID: 1463
public class DebugSpawnPointChanger : MonoBehaviour
{
	// Token: 0x06002519 RID: 9497 RVA: 0x000C6E84 File Offset: 0x000C5084
	private void AttachSpawnPoint(VRRig rig, Transform[] spawnPts, int locationIndex)
	{
		if (spawnPts == null)
		{
			return;
		}
		GTPlayer gtplayer = Object.FindAnyObjectByType<GTPlayer>();
		if (gtplayer == null)
		{
			return;
		}
		this.lastLocationIndex = locationIndex;
		int i = 0;
		while (i < spawnPts.Length)
		{
			Transform transform = spawnPts[i];
			if (transform.name == this.levelTriggers[locationIndex].levelName)
			{
				rig.transform.position = transform.position;
				rig.transform.rotation = transform.rotation;
				gtplayer.transform.position = transform.position;
				gtplayer.transform.rotation = transform.rotation;
				gtplayer.InitializeValues();
				SpawnPoint component = transform.GetComponent<SpawnPoint>();
				if (component != null)
				{
					gtplayer.SetScaleMultiplier(component.startSize);
					ZoneManagement.SetActiveZone(component.startZone);
					return;
				}
				Debug.LogWarning("Attempt to spawn at transform that does not have SpawnPoint component will be ignored: " + transform.name);
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000C6F78 File Offset: 0x000C5178
	private void ChangePoint(int index)
	{
		SpawnManager spawnManager = Object.FindAnyObjectByType<SpawnManager>();
		if (spawnManager != null)
		{
			Transform[] array = spawnManager.ChildrenXfs();
			foreach (VRRig vrrig in Object.FindObjectsByType<VRRig>(FindObjectsSortMode.None))
			{
				this.AttachSpawnPoint(vrrig, array, index);
			}
		}
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x000C6FBF File Offset: 0x000C51BF
	public List<string> GetPlausibleJumpLocation()
	{
		return this.levelTriggers[this.lastLocationIndex].canJumpToIndex.Select((int index) => this.levelTriggers[index].levelName).ToList<string>();
	}

	// Token: 0x0600251C RID: 9500 RVA: 0x000C6FF0 File Offset: 0x000C51F0
	public void JumpTo(int canJumpIndex)
	{
		DebugSpawnPointChanger.GeoTriggersGroup geoTriggersGroup = this.levelTriggers[this.lastLocationIndex];
		this.ChangePoint(geoTriggersGroup.canJumpToIndex[canJumpIndex]);
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000C7020 File Offset: 0x000C5220
	public void SetLastLocation(string levelName)
	{
		for (int i = 0; i < this.levelTriggers.Length; i++)
		{
			if (!(this.levelTriggers[i].levelName != levelName))
			{
				this.lastLocationIndex = i;
				return;
			}
		}
	}

	// Token: 0x0400308E RID: 12430
	[SerializeField]
	private DebugSpawnPointChanger.GeoTriggersGroup[] levelTriggers;

	// Token: 0x0400308F RID: 12431
	private int lastLocationIndex;

	// Token: 0x020005B8 RID: 1464
	[Serializable]
	private struct GeoTriggersGroup
	{
		// Token: 0x04003090 RID: 12432
		public string levelName;

		// Token: 0x04003091 RID: 12433
		public GorillaGeoHideShowTrigger enterTrigger;

		// Token: 0x04003092 RID: 12434
		public GorillaGeoHideShowTrigger[] leaveTrigger;

		// Token: 0x04003093 RID: 12435
		public int[] canJumpToIndex;
	}
}
