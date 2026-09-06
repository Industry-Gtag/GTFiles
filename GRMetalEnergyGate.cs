using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007C1 RID: 1985
public class GRMetalEnergyGate : MonoBehaviour
{
	// Token: 0x060032AF RID: 12975 RVA: 0x00115D2F File Offset: 0x00113F2F
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x00115D60 File Offset: 0x00113F60
	private void OnDisable()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange -= this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x060032B1 RID: 12977 RVA: 0x00115DB8 File Offset: 0x00113FB8
	private void OnEnergyChange(GRTool tool, int energyChange, GameEntityId chargingEntityId)
	{
		GameEntity gameEntity = this.gameEntity.manager.GetGameEntity(chargingEntityId);
		GRPlayer grplayer = null;
		if (gameEntity != null)
		{
			grplayer = GRPlayer.Get(gameEntity.heldByActorNumber);
		}
		if (grplayer != null)
		{
			grplayer.IncrementCoresSpentPlayer(energyChange);
		}
		if (this.state == GRMetalEnergyGate.State.Closed && tool.energy >= tool.GetEnergyMax())
		{
			if (grplayer != null)
			{
				grplayer.IncrementGatesUnlocked(1);
			}
			this.SetState(GRMetalEnergyGate.State.Open);
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x00115E50 File Offset: 0x00114050
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!this.gameEntity.IsAuthority())
		{
			this.SetState((GRMetalEnergyGate.State)nextState);
		}
	}

	// Token: 0x060032B3 RID: 12979 RVA: 0x00115E68 File Offset: 0x00114068
	public void SetState(GRMetalEnergyGate.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.audioSource.PlayOneShot(this.doorOpenClip);
					for (int i = 0; i < this.enableObjectsOnOpen.Count; i++)
					{
						this.enableObjectsOnOpen[i].gameObject.SetActive(true);
					}
					for (int j = 0; j < this.disableObjectsOnOpen.Count; j++)
					{
						this.disableObjectsOnOpen[j].gameObject.SetActive(false);
					}
				}
			}
			else
			{
				this.audioSource.PlayOneShot(this.doorCloseClip);
				for (int k = 0; k < this.enableObjectsOnOpen.Count; k++)
				{
					this.enableObjectsOnOpen[k].gameObject.SetActive(false);
				}
				for (int l = 0; l < this.disableObjectsOnOpen.Count; l++)
				{
					this.disableObjectsOnOpen[l].gameObject.SetActive(true);
				}
			}
			if (this.doorAnimationCoroutine == null)
			{
				this.doorAnimationCoroutine = base.StartCoroutine(this.UpdateDoorAnimation());
			}
		}
	}

	// Token: 0x060032B4 RID: 12980 RVA: 0x00115F90 File Offset: 0x00114190
	public void OpenGate()
	{
		this.SetState(GRMetalEnergyGate.State.Open);
	}

	// Token: 0x060032B5 RID: 12981 RVA: 0x00115F99 File Offset: 0x00114199
	public void CloseGate()
	{
		this.SetState(GRMetalEnergyGate.State.Closed);
	}

	// Token: 0x060032B6 RID: 12982 RVA: 0x00115FA2 File Offset: 0x001141A2
	private IEnumerator UpdateDoorAnimation()
	{
		while ((this.state == GRMetalEnergyGate.State.Open && this.openProgress < 1f) || (this.state == GRMetalEnergyGate.State.Closed && this.openProgress > 0f))
		{
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.openProgress = Mathf.MoveTowards(this.openProgress, 1f, Time.deltaTime / this.doorOpenTime);
					float num = this.doorOpenCurve.Evaluate(this.openProgress);
					this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, num);
					this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, num);
				}
			}
			else
			{
				this.openProgress = Mathf.MoveTowards(this.openProgress, 0f, Time.deltaTime / this.doorOpenTime);
				float num2 = this.doorCloseCurve.Evaluate(this.openProgress);
				this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, num2);
				this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, num2);
			}
			yield return null;
		}
		this.doorAnimationCoroutine = null;
		yield break;
	}

	// Token: 0x040041B1 RID: 16817
	[SerializeField]
	public GRMetalEnergyGate.DoorParams upperDoor;

	// Token: 0x040041B2 RID: 16818
	[SerializeField]
	public GRMetalEnergyGate.DoorParams lowerDoor;

	// Token: 0x040041B3 RID: 16819
	[SerializeField]
	private float doorOpenTime = 1.5f;

	// Token: 0x040041B4 RID: 16820
	[SerializeField]
	private float doorCloseTime = 1.5f;

	// Token: 0x040041B5 RID: 16821
	[SerializeField]
	private AnimationCurve doorOpenCurve;

	// Token: 0x040041B6 RID: 16822
	[SerializeField]
	private AnimationCurve doorCloseCurve;

	// Token: 0x040041B7 RID: 16823
	[SerializeField]
	private AudioClip doorOpenClip;

	// Token: 0x040041B8 RID: 16824
	[SerializeField]
	private AudioClip doorCloseClip;

	// Token: 0x040041B9 RID: 16825
	[SerializeField]
	private List<Transform> enableObjectsOnOpen = new List<Transform>();

	// Token: 0x040041BA RID: 16826
	[SerializeField]
	private List<Transform> disableObjectsOnOpen = new List<Transform>();

	// Token: 0x040041BB RID: 16827
	[SerializeField]
	private GRTool tool;

	// Token: 0x040041BC RID: 16828
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x040041BD RID: 16829
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040041BE RID: 16830
	public GRMetalEnergyGate.State state;

	// Token: 0x040041BF RID: 16831
	private float openProgress;

	// Token: 0x040041C0 RID: 16832
	private Coroutine doorAnimationCoroutine;

	// Token: 0x020007C2 RID: 1986
	public enum State
	{
		// Token: 0x040041C2 RID: 16834
		Closed,
		// Token: 0x040041C3 RID: 16835
		Open
	}

	// Token: 0x020007C3 RID: 1987
	[Serializable]
	public struct DoorParams
	{
		// Token: 0x040041C4 RID: 16836
		public Transform doorTransform;

		// Token: 0x040041C5 RID: 16837
		public Transform doorClosedPosition;

		// Token: 0x040041C6 RID: 16838
		public Transform doorOpenPosition;
	}
}
