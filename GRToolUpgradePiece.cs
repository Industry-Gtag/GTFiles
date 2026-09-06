using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000826 RID: 2086
public class GRToolUpgradePiece : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06003577 RID: 13687 RVA: 0x00125E5C File Offset: 0x0012405C
	private void Start()
	{
		MeshFilter componentInChildren = base.GetComponentInChildren<MeshFilter>();
		if (componentInChildren != null)
		{
			this.meshCollider.sharedMesh = componentInChildren.sharedMesh;
		}
	}

	// Token: 0x06003578 RID: 13688 RVA: 0x00125E8C File Offset: 0x0012408C
	private void EnableProcAnimLoop()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.Tick));
		if (!this.humAudioSource.isPlaying)
		{
			this.humAudioSource.volume = 0f;
			this.humAudioSource.GTPlay();
		}
	}

	// Token: 0x06003579 RID: 13689 RVA: 0x00125EE8 File Offset: 0x001240E8
	private void DisableProcAnimLoop()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(this.Tick));
		this.SwitchMagnetizedTarget(null);
		this.childVisualTransform.localPosition = Vector3.zero;
		this.childVisualTransform.localRotation = Quaternion.identity;
		this.childVisualTransform.localScale = Vector3.one;
		this.humAudioSource.Stop();
		if (this.attractParticleSystem != null)
		{
			this.attractParticleSystem.Stop();
		}
	}

	// Token: 0x0600357A RID: 13690 RVA: 0x00125F77 File Offset: 0x00124177
	private void SwitchMagnetizedTarget(GameEntity entity)
	{
		this.currentMagnetizingTool = entity;
	}

	// Token: 0x0600357B RID: 13691 RVA: 0x00125F80 File Offset: 0x00124180
	private void Tick()
	{
		Vector3 position = base.transform.position;
		List<GameEntity> gameEntities = this.gameEntity.manager.GetGameEntities();
		int num = this.gameEntityListCheckIndex;
		int num2 = ((this.toolSearchesPerFrame < gameEntities.Count) ? this.toolSearchesPerFrame : gameEntities.Count);
		GRTool grtool = ((this.currentMagnetizingTool != null) ? this.currentMagnetizingTool.GetComponent<GRTool>() : null);
		GRTool.Upgrade upgrade = ((grtool != null) ? grtool.FindMatchingUpgrade(this.matchingUpgrade) : null);
		float num3 = ((grtool != null) ? grtool.GetPointDistanceToUpgrade(position, upgrade) : 1E+10f);
		if (num3 > this.minDistToStartMagnetize)
		{
			this.SwitchMagnetizedTarget(null);
			grtool = null;
			upgrade = null;
			num3 = 1E+10f;
		}
		for (int i = 0; i < num2; i++)
		{
			num = (num + 1) % gameEntities.Count;
			GameEntity gameEntity = gameEntities[num];
			if (!(gameEntity == null))
			{
				GRTool component = gameEntity.GetComponent<GRTool>();
				if (component != null && gameEntity.heldByActorNumber != -1)
				{
					GRTool.Upgrade upgrade2 = component.FindMatchingUpgrade(this.matchingUpgrade);
					if (upgrade2 != null)
					{
						float pointDistanceToUpgrade = component.GetPointDistanceToUpgrade(position, upgrade2);
						if (pointDistanceToUpgrade > 0f && pointDistanceToUpgrade < num3 && pointDistanceToUpgrade < this.minDistToStartMagnetize)
						{
							this.SwitchMagnetizedTarget(gameEntity);
							grtool = component;
							upgrade = upgrade2;
							num3 = pointDistanceToUpgrade;
						}
					}
				}
			}
		}
		this.gameEntityListCheckIndex = num;
		if (grtool != null)
		{
			Transform upgradeAttachTransform = grtool.GetUpgradeAttachTransform(upgrade);
			if (num3 >= this.minDistToSnap)
			{
				float num4 = Mathf.Clamp01(num3 / this.minDistToStartMagnetize);
				this.humAudioSource.volume = Mathf.Lerp(this.magnetizingLoopMaxVolume, this.magnetizingLoopMinVolume, num4);
				float num5 = this.shakeMaxAmount * (1f - num4);
				float num6 = Mathf.Clamp01((this.visualDistanceCurve != null) ? this.visualDistanceCurve.Evaluate(num4) : num4);
				this.shakePhase += Time.deltaTime * this.shakeFrequency;
				if (this.shakePhase > 6.2831855f)
				{
					this.shakePhase -= 6.2831855f;
				}
				Transform transform = base.transform;
				if (this.childVisualTransform != null)
				{
					Vector3 vector = Vector3.Lerp(upgradeAttachTransform.position, transform.position, num6);
					Quaternion quaternion = Quaternion.Slerp(upgradeAttachTransform.rotation, transform.rotation, num6);
					Vector3 vector2 = Vector3.Lerp(upgradeAttachTransform.localScale, transform.localScale, num6);
					vector2.x /= transform.localScale.x;
					vector2.y /= transform.localScale.y;
					vector2.z /= transform.localScale.y;
					quaternion *= Quaternion.Euler(new Vector3(num5 * Mathf.Sin(this.shakePhase), num5 * Mathf.Cos(this.shakePhase), 0f));
					this.childVisualTransform.position = vector;
					this.childVisualTransform.rotation = quaternion;
					this.childVisualTransform.localScale = vector2;
				}
				if (this.attractParticleSystem != null)
				{
					if (!this.attractParticleSystem.isPlaying)
					{
						this.attractParticleSystem.Play();
					}
					this.attractParticleSystem.emission.enabled = true;
				}
				this.forceField.transform.position = upgradeAttachTransform.position;
				return;
			}
			this.humAudioSource.volume = 0f;
			if (this.attractParticleSystem != null)
			{
				this.attractParticleSystem.Stop();
			}
			this.childVisualTransform.position = upgradeAttachTransform.position;
			this.childVisualTransform.rotation = upgradeAttachTransform.rotation;
			this.childVisualTransform.localScale = new Vector3(upgradeAttachTransform.localScale.x / base.transform.localScale.x, upgradeAttachTransform.localScale.y / base.transform.localScale.y, upgradeAttachTransform.localScale.z / base.transform.localScale.z);
			if (this.currentMagnetizingTool != null)
			{
				GhostReactor instance = GhostReactor.instance;
				if (instance != null)
				{
					instance.grManager.ToolSnapRequestUpgrade(this.gameEntity.GetNetId(), this.matchingUpgrade, this.currentMagnetizingTool.GetComponent<GameEntity>().GetNetId());
					return;
				}
			}
		}
		else
		{
			if (this.attractParticleSystem != null)
			{
				this.attractParticleSystem.emission.enabled = false;
			}
			this.humAudioSource.volume = 0f;
		}
	}

	// Token: 0x0600357C RID: 13692 RVA: 0x00126430 File Offset: 0x00124630
	private void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.ReleasedByPlayer));
	}

	// Token: 0x0600357D RID: 13693 RVA: 0x0012648C File Offset: 0x0012468C
	private void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.ReleasedByPlayer));
	}

	// Token: 0x0600357E RID: 13694 RVA: 0x001264E8 File Offset: 0x001246E8
	public void GrabbedByPlayer()
	{
		if (this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer)
			{
				grplayer.GrabbedItem(this.gameEntity.id, base.gameObject.name);
			}
		}
		this.EnableProcAnimLoop();
	}

	// Token: 0x0600357F RID: 13695 RVA: 0x00126549 File Offset: 0x00124749
	public void ReleasedByPlayer()
	{
		this.DisableProcAnimLoop();
	}

	// Token: 0x06003580 RID: 13696 RVA: 0x00126554 File Offset: 0x00124754
	public void OnEntityInit()
	{
		GhostReactor.ToolEntityCreateData toolEntityCreateData = GhostReactor.ToolEntityCreateData.Unpack(this.gameEntity.createData);
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null)
		{
			GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = ghostReactorManager.GetToolUpgradeStationFullForIndex(toolEntityCreateData.stationIndex);
			if (toolUpgradeStationFullForIndex != null)
			{
				toolUpgradeStationFullForIndex.InitLinkedEntity(this.gameEntity);
			}
		}
	}

	// Token: 0x06003581 RID: 13697 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003582 RID: 13698 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x040045D0 RID: 17872
	public GameEntity gameEntity;

	// Token: 0x040045D1 RID: 17873
	public GRToolProgressionManager.ToolParts matchingUpgrade;

	// Token: 0x040045D2 RID: 17874
	private int gameEntityListCheckIndex;

	// Token: 0x040045D3 RID: 17875
	private GameEntity currentMagnetizingTool;

	// Token: 0x040045D4 RID: 17876
	public AnimationCurve visualDistanceCurve;

	// Token: 0x040045D5 RID: 17877
	public float shakeMaxAmount = 10f;

	// Token: 0x040045D6 RID: 17878
	public float shakeFrequency = 100f;

	// Token: 0x040045D7 RID: 17879
	public Transform childVisualTransform;

	// Token: 0x040045D8 RID: 17880
	public AudioSource humAudioSource;

	// Token: 0x040045D9 RID: 17881
	public AudioSource audioSource;

	// Token: 0x040045DA RID: 17882
	public AudioClip snapAudioClip;

	// Token: 0x040045DB RID: 17883
	public MeshCollider meshCollider;

	// Token: 0x040045DC RID: 17884
	public ParticleSystem attractParticleSystem;

	// Token: 0x040045DD RID: 17885
	public ParticleSystemForceField forceField;

	// Token: 0x040045DE RID: 17886
	public float minDistToStartMagnetize = 0.5f;

	// Token: 0x040045DF RID: 17887
	public float minDistToSnap;

	// Token: 0x040045E0 RID: 17888
	public float magnetizingLoopMinVolume = 0.2f;

	// Token: 0x040045E1 RID: 17889
	public float magnetizingLoopMaxVolume = 1f;

	// Token: 0x040045E2 RID: 17890
	public float snapAudioVolume = 1f;

	// Token: 0x040045E3 RID: 17891
	private int toolSearchesPerFrame = 5;

	// Token: 0x040045E4 RID: 17892
	private float shakePhase;
}
