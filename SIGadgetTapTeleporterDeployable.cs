using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class SIGadgetTapTeleporterDeployable : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x060006D2 RID: 1746 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x000263A7 File Offset: 0x000245A7
	private void OnEnable()
	{
		this.activateTime = Time.time + this.activateDelay;
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x000263BC File Offset: 0x000245BC
	private void LateUpdate()
	{
		if (Time.time > this.timeToDie && this.gameEntity.IsAuthority())
		{
			if (this.linkedPoint != null)
			{
				this.linkedPoint.ClearLink();
			}
			this.gameEntity.manager.RequestDestroyItem(this.gameEntity.id);
		}
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00026418 File Offset: 0x00024618
	public void OnEntityInit()
	{
		int num;
		BitPackUtils.UnpackIntsFromLong(this.gameEntity.createData, out this.selectionId, out num);
		if ((float)num < 0f)
		{
			this.timeToDie = float.PositiveInfinity;
		}
		else
		{
			this.timeToDie = Time.time + (float)num;
		}
		this.UpdateSelectionDisplay();
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00026467 File Offset: 0x00024667
	private void UpdateSelectionDisplay()
	{
		if (this.selectionId == 0)
		{
			this.selectionColorDisplay.material = this.selectionColor1;
			return;
		}
		if (this.selectionId == 1)
		{
			this.selectionColorDisplay.material = this.selectionColor2;
		}
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x000264A0 File Offset: 0x000246A0
	public void OnEntityStateChange(long prevState, long newState)
	{
		if (this.gameEntity.IsAuthority())
		{
			return;
		}
		int num;
		int num2;
		BitPackUtils.UnpackIntsFromLong(newState, out num, out num2);
		GameEntity gameEntityFromNetId = this.gameEntity.manager.GetGameEntityFromNetId(num);
		if (gameEntityFromNetId != null)
		{
			SIGadgetTapTeleporter component = gameEntityFromNetId.GetComponent<SIGadgetTapTeleporter>();
			this._pad = component;
			this.identifierColor = this._pad.identifierColor;
		}
		GameEntity gameEntityFromNetId2 = this.gameEntity.manager.GetGameEntityFromNetId(num2);
		if (gameEntityFromNetId2 != null)
		{
			this.linkedPoint = gameEntityFromNetId2.GetComponent<SIGadgetTapTeleporterDeployable>();
			if (this.linkedPoint.linkedPoint == null)
			{
				this.linkedPoint.linkedPoint = this;
				this.linkedPoint._pad = this._pad;
				this.linkedPoint.identifierColor = this.identifierColor;
				this.linkedPoint.UpdateLinkDisplay();
			}
		}
		else
		{
			this.linkedPoint = null;
		}
		this.UpdateLinkDisplay();
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00026584 File Offset: 0x00024784
	public void SetLink(SIGadgetTapTeleporter newPad, SIGadgetTapTeleporterDeployable newLink)
	{
		this._pad = newPad;
		this.linkedPoint = newLink;
		this.identifierColor = this._pad.identifierColor;
		int num = -1;
		if (this.linkedPoint != null)
		{
			num = this.linkedPoint.gameEntity.GetNetId();
		}
		this.gameEntity.RequestState(this.gameEntity.id, BitPackUtils.PackIntsIntoLong(this._pad.gameEntity.GetNetId(), num));
		this.UpdateLinkDisplay();
		this.stealth.enabled = this._pad.useStealthTeleporters;
		this.maintainVelocity = this._pad.isVelocityPreserved;
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x0002662A File Offset: 0x0002482A
	private void ClearLink()
	{
		this.linkedPoint = null;
		this.gameEntity.RequestState(this.gameEntity.id, BitPackUtils.PackIntsIntoLong(this._pad.gameEntity.GetNetId(), -1));
		this.UpdateLinkDisplay();
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x00026668 File Offset: 0x00024868
	private void UpdateLinkDisplay()
	{
		Renderer[] array = this.identifierColorDisplay;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.color = this.identifierColor;
		}
		if (this.linkedPoint != null)
		{
			Vector3 vector = this.linkedPoint.transform.position - base.transform.position;
			this.linkDirectionIndicator.gameObject.SetActive(true);
			this.linkDirectionIndicator.transform.rotation = Quaternion.LookRotation(base.transform.forward, vector.normalized);
			return;
		}
		this.linkDirectionIndicator.gameObject.SetActive(false);
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00026716 File Offset: 0x00024916
	public void TryTeleport()
	{
		if (this.activateTime < Time.time && SIGadgetTapTeleporterDeployable.reteleportTime < Time.time && (!this.requiresSurfaceTapSinceTeleport || GorillaTagger.Instance.hasTappedSurface))
		{
			this.TeleportToLinked();
		}
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x0002674B File Offset: 0x0002494B
	private void ResetRetriggerBlock()
	{
		SIGadgetTapTeleporterDeployable.reteleportTime = Time.time + SIGadgetTapTeleporterDeployable.reteleportDelay;
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00026760 File Offset: 0x00024960
	private void TeleportToLinked()
	{
		if (this.linkedPoint == null || !this.linkedPoint.gameObject.activeSelf)
		{
			return;
		}
		Vector3 position = this.destination.position;
		if (Vector3.Distance(GTPlayer.Instance.transform.position, position) > this.teleportCheckDistance)
		{
			return;
		}
		this.ResetRetriggerBlock();
		if (this.requiresSurfaceTapSinceTeleport)
		{
			GorillaTagger.Instance.ResetTappedSurfaceCheck();
		}
		Vector3 position2 = this.linkedPoint.destination.position;
		Quaternion rotation = GTPlayer.Instance.transform.rotation;
		GTPlayer.Instance.TeleportTo(position2, rotation, this.maintainVelocity, true);
		this.linkedPoint.teleportSoundbank.Play();
	}

	// Token: 0x0400085B RID: 2139
	public GameEntity gameEntity;

	// Token: 0x0400085C RID: 2140
	[SerializeField]
	private Transform destination;

	// Token: 0x0400085D RID: 2141
	[SerializeField]
	private Renderer[] identifierColorDisplay;

	// Token: 0x0400085E RID: 2142
	[SerializeField]
	private Transform linkDirectionIndicator;

	// Token: 0x0400085F RID: 2143
	[SerializeField]
	private Renderer selectionColorDisplay;

	// Token: 0x04000860 RID: 2144
	[SerializeField]
	private Material selectionColor1;

	// Token: 0x04000861 RID: 2145
	[SerializeField]
	private Material selectionColor2;

	// Token: 0x04000862 RID: 2146
	[SerializeField]
	private SoundBankPlayer teleportSoundbank;

	// Token: 0x04000863 RID: 2147
	[SerializeField]
	private SIGameEntityStealthVisibility stealth;

	// Token: 0x04000864 RID: 2148
	[SerializeField]
	private bool requiresSurfaceTapSinceTeleport;

	// Token: 0x04000865 RID: 2149
	private bool maintainVelocity;

	// Token: 0x04000866 RID: 2150
	private int selectionId;

	// Token: 0x04000867 RID: 2151
	private SIGadgetTapTeleporter _pad;

	// Token: 0x04000868 RID: 2152
	private SIGadgetTapTeleporterDeployable linkedPoint;

	// Token: 0x04000869 RID: 2153
	private float activateDelay = 0.3f;

	// Token: 0x0400086A RID: 2154
	private float activateTime;

	// Token: 0x0400086B RID: 2155
	private static float reteleportDelay = 0.3f;

	// Token: 0x0400086C RID: 2156
	private static float reteleportTime;

	// Token: 0x0400086D RID: 2157
	private Color identifierColor;

	// Token: 0x0400086E RID: 2158
	private float timeToDie = -1f;

	// Token: 0x0400086F RID: 2159
	private float teleportCheckDistance = 2f;
}
