using System;
using UnityEngine;

// Token: 0x02000E13 RID: 3603
public class GTSubScene : ScriptableObject
{
	// Token: 0x0600582F RID: 22575 RVA: 0x001CB140 File Offset: 0x001C9340
	public void SwitchToScene(int index)
	{
		this.scenes[index].LoadAsync();
	}

	// Token: 0x06005830 RID: 22576 RVA: 0x001CB150 File Offset: 0x001C9350
	public void SwitchToScene(GTScene scene)
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			GTScene gtscene = this.scenes[i];
			if (!(scene == gtscene))
			{
				gtscene.UnloadAsync();
			}
		}
		scene.LoadAsync();
	}

	// Token: 0x06005831 RID: 22577 RVA: 0x001CB190 File Offset: 0x001C9390
	public void LoadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].LoadAsync();
		}
	}

	// Token: 0x06005832 RID: 22578 RVA: 0x001CB1C0 File Offset: 0x001C93C0
	public void UnloadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].UnloadAsync();
		}
	}

	// Token: 0x0400687D RID: 26749
	[DragDropScenes]
	public GTScene[] scenes = new GTScene[0];
}
