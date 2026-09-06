using System;
using UnityEngine;

// Token: 0x0200091E RID: 2334
[CreateAssetMenu(fileName = "MapModeQueueSet", menuName = "Game Settings/Map Mode Queue Set")]
public class MapModeQueueSet : ScriptableObject
{
	// Token: 0x04004DCC RID: 19916
	public string[] maps;

	// Token: 0x04004DCD RID: 19917
	public string[] modes;

	// Token: 0x04004DCE RID: 19918
	public string[] queues;
}
