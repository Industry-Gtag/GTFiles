using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006BC RID: 1724
public class GameDock : MonoBehaviour
{
	// Token: 0x06002B0B RID: 11019 RVA: 0x000E70DF File Offset: 0x000E52DF
	private void Awake()
	{
		this.docked = new List<GameEntity>(1);
		if (this.dockMarker == null)
		{
			this.dockMarker = base.transform;
		}
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnable()
	{
	}

	// Token: 0x06002B0D RID: 11021 RVA: 0x000E7107 File Offset: 0x000E5307
	public bool CanDock(GameDockable dockable)
	{
		return !(dockable == null) && (this.dockType != GameDockType.GRToolDock || this.GetDockedCount() <= 0);
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x000E712A File Offset: 0x000E532A
	public int GetDockedCount()
	{
		return this.docked.Count;
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x000E7137 File Offset: 0x000E5337
	public void OnDock(GameEntity attachedGameEntity, GameEntity attachedToGameEntity)
	{
		this.dockSound.Play(null);
		this.docked.Add(attachedGameEntity);
		this.dockHaptic.PlayIfSnappedLocal(attachedToGameEntity);
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x000E715D File Offset: 0x000E535D
	public void OnUndock(GameEntity gameEntity, GameEntity attachedToGameEntity)
	{
		this.undockSound.Play(null);
		this.docked.Remove(gameEntity);
	}

	// Token: 0x040037DD RID: 14301
	public GameEntity gameEntity;

	// Token: 0x040037DE RID: 14302
	public GameDockType dockType;

	// Token: 0x040037DF RID: 14303
	public float dockRadius = 0.15f;

	// Token: 0x040037E0 RID: 14304
	public AbilitySound dockSound;

	// Token: 0x040037E1 RID: 14305
	public AbilitySound undockSound;

	// Token: 0x040037E2 RID: 14306
	public AbilityHaptic dockHaptic;

	// Token: 0x040037E3 RID: 14307
	public Transform dockMarker;

	// Token: 0x040037E4 RID: 14308
	private List<GameEntity> docked;
}
