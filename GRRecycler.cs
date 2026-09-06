using System;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

// Token: 0x020007DE RID: 2014
public class GRRecycler : MonoBehaviourTick
{
	// Token: 0x0600335E RID: 13150 RVA: 0x00118F80 File Offset: 0x00117180
	public override void Tick()
	{
		if (this.closed && !this.anim.isPlaying)
		{
			if (!this.playedAudio)
			{
				this.audioSource.volume = this.recyclerRunningAudioVolume;
				this.audioSource.PlayOneShot(this.recyclerRunningAudio);
				this.playedAudio = true;
			}
			this.timeRemaining -= Time.deltaTime;
			if (this.timeRemaining <= 0f)
			{
				this.anim.PlayQueued("Recycler_Open", QueueMode.CompleteOthers);
				this.closed = false;
				if (this.closeEffects != null && this.openEffects != null)
				{
					this.closeEffects.Stop();
					this.openEffects.Play();
				}
			}
		}
	}

	// Token: 0x0600335F RID: 13151 RVA: 0x00119043 File Offset: 0x00117243
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06003360 RID: 13152 RVA: 0x0011904C File Offset: 0x0011724C
	public int GetRecycleValue(GRTool.GRToolType type)
	{
		return this.reactor.toolProgression.GetRecycleShiftCredit(type);
	}

	// Token: 0x06003361 RID: 13153 RVA: 0x0011905F File Offset: 0x0011725F
	public void ScanItem(GameEntityId id)
	{
		this.scanner.ScanItem(id);
	}

	// Token: 0x06003362 RID: 13154 RVA: 0x00119070 File Offset: 0x00117270
	public void RecycleItem()
	{
		if (this.anim != null)
		{
			this.anim.Play("Recycler_Close");
		}
		if (this.closeEffects != null && this.openEffects != null)
		{
			this.openEffects.Stop();
			this.closeEffects.Play();
		}
		this.closed = true;
		this.playedAudio = false;
		this.timeRemaining = this.closeDuration;
	}

	// Token: 0x06003363 RID: 13155 RVA: 0x001190E8 File Offset: 0x001172E8
	private void OnTriggerEnter(Collider other)
	{
		if (this.reactor == null)
		{
			Debug.LogFormat("GRRecycler reactor is null?", Array.Empty<object>());
			return;
		}
		if (!this.reactor.grManager.IsAuthority())
		{
			Debug.LogFormat("GRRecycler is not authority.", Array.Empty<object>());
			return;
		}
		GRTool componentInParent = other.gameObject.GetComponentInParent<GRTool>();
		if (componentInParent == null)
		{
			Debug.LogFormat("GRRecycler Colliding Object is not a GRTool.", Array.Empty<object>());
			return;
		}
		GRTool.GRToolType toolType = other.gameObject.GetToolType();
		int recycleValue = this.GetRecycleValue(toolType);
		if (this.reactor != null)
		{
			int count = this.reactor.vrRigs.Count;
			for (int i = 0; i < count; i++)
			{
				GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
				if (grplayer != null)
				{
					grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, (float)recycleValue);
				}
			}
		}
		Debug.LogFormat("GRRecycler Recycle Value is {0}", new object[] { recycleValue });
		if (GRPlayer.Get(componentInParent.gameEntity.lastHeldByActorNumber) == null)
		{
			Debug.LogFormat("GRRecycler Tool Not last held by a player (?), can't recycle.", Array.Empty<object>());
			return;
		}
		Debug.LogFormat("GRRecycler Refunding player {0} {1} Currency and Destroying Tool.", new object[]
		{
			componentInParent.gameEntity.lastHeldByActorNumber,
			recycleValue
		});
		if (toolType != GRTool.GRToolType.None)
		{
			this.reactor.grManager.RequestRecycleItem(componentInParent.gameEntity.lastHeldByActorNumber, componentInParent.gameEntity.id, toolType);
		}
	}

	// Token: 0x040042A4 RID: 17060
	private GameEntity gameEntity;

	// Token: 0x040042A5 RID: 17061
	public ParticleSystem closeEffects;

	// Token: 0x040042A6 RID: 17062
	public ParticleSystem openEffects;

	// Token: 0x040042A7 RID: 17063
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x040042A8 RID: 17064
	public GRRecyclerScanner scanner;

	// Token: 0x040042A9 RID: 17065
	public Animation anim;

	// Token: 0x040042AA RID: 17066
	public float closeDuration = 1f;

	// Token: 0x040042AB RID: 17067
	private float timeRemaining;

	// Token: 0x040042AC RID: 17068
	private bool closed;

	// Token: 0x040042AD RID: 17069
	private bool playedAudio;

	// Token: 0x040042AE RID: 17070
	public AudioSource audioSource;

	// Token: 0x040042AF RID: 17071
	public AudioClip recyclerRunningAudio;

	// Token: 0x040042B0 RID: 17072
	public float recyclerRunningAudioVolume = 0.5f;
}
