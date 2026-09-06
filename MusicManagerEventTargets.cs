using System;
using UnityEngine;

// Token: 0x02000428 RID: 1064
public class MusicManagerEventTargets : MonoBehaviour
{
	// Token: 0x06001951 RID: 6481 RVA: 0x0008E96C File Offset: 0x0008CB6C
	public void StopAllMusic()
	{
		this.StopAllMusic(null);
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x0008E975 File Offset: 0x0008CB75
	public void StopAllMusic(AudioClip clip)
	{
		MusicManager.StopAllMusic(clip);
	}
}
