using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000734 RID: 1844
public class GRAbilityBase
{
	// Token: 0x06002EC7 RID: 11975 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnStart()
	{
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnStop()
	{
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnThink(float dt)
	{
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnUpdateShared(float dt)
	{
	}

	// Token: 0x06002ECB RID: 11979 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnUpdateRemote(float dt)
	{
	}

	// Token: 0x06002ECC RID: 11980 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x00023F0C File Offset: 0x0002210C
	public virtual bool IsCoolDownOver()
	{
		return true;
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x000FFD70 File Offset: 0x000FDF70
	public virtual void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.root = root;
		this.anim = anim;
		if (anim == null)
		{
			this.animator = null;
		}
		this.agent = agent;
		this.head = head;
		this.audioSource = audioSource;
		this.lineOfSight = lineOfSight;
		this.rb = agent.GetComponent<Rigidbody>();
		this.entity = agent.GetComponent<GameEntity>();
		this.attributes = agent.GetComponent<GRAttributes>();
		this.walkableArea = NavMesh.GetAreaFromName("walkable");
	}

	// Token: 0x06002ECF RID: 11983 RVA: 0x000FFDEE File Offset: 0x000FDFEE
	public void Start()
	{
		this.startTime = Time.timeAsDouble;
		this.OnStart();
	}

	// Token: 0x06002ED0 RID: 11984 RVA: 0x000FFE01 File Offset: 0x000FE001
	public void Stop()
	{
		this.stopTime = Time.timeAsDouble;
		this.OnStop();
	}

	// Token: 0x06002ED1 RID: 11985 RVA: 0x000FFE14 File Offset: 0x000FE014
	public float GetAbilityTime(double currTime)
	{
		return (float)(currTime - this.startTime);
	}

	// Token: 0x06002ED2 RID: 11986 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool IsDone()
	{
		return false;
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x000FFE1F File Offset: 0x000FE01F
	public void Think(float dt)
	{
		this.OnThink(dt);
	}

	// Token: 0x06002ED4 RID: 11988 RVA: 0x000FFE28 File Offset: 0x000FE028
	public void UpdateAuthority(float dt)
	{
		this.OnUpdateShared(dt);
		this.OnUpdateAuthority(dt);
	}

	// Token: 0x06002ED5 RID: 11989 RVA: 0x000FFE38 File Offset: 0x000FE038
	public void UpdateRemote(float dt)
	{
		this.OnUpdateShared(dt);
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002ED6 RID: 11990 RVA: 0x000FFE48 File Offset: 0x000FE048
	protected virtual void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			if (this.anim.GetClip(animName) == null)
			{
				Debug.LogErrorFormat("Anim Clip {0} does not exist in (1)", new object[] { animName, this.anim });
				return;
			}
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06002ED7 RID: 11991 RVA: 0x000FFEBC File Offset: 0x000FE0BC
	public bool IsCoolDownOver(float coolDown)
	{
		return (float)(Time.timeAsDouble - this.stopTime) > coolDown;
	}

	// Token: 0x06002ED8 RID: 11992 RVA: 0x000E252C File Offset: 0x000E072C
	public virtual float GetRange()
	{
		return 0f;
	}

	// Token: 0x04003BF4 RID: 15348
	protected GameAgent agent;

	// Token: 0x04003BF5 RID: 15349
	protected GameEntity entity;

	// Token: 0x04003BF6 RID: 15350
	protected Animation anim;

	// Token: 0x04003BF7 RID: 15351
	protected Animator animator;

	// Token: 0x04003BF8 RID: 15352
	protected Transform root;

	// Token: 0x04003BF9 RID: 15353
	protected Transform head;

	// Token: 0x04003BFA RID: 15354
	protected AudioSource audioSource;

	// Token: 0x04003BFB RID: 15355
	protected GRSenseLineOfSight lineOfSight;

	// Token: 0x04003BFC RID: 15356
	protected Rigidbody rb;

	// Token: 0x04003BFD RID: 15357
	protected GRAttributes attributes;

	// Token: 0x04003BFE RID: 15358
	[ReadOnly]
	public double startTime;

	// Token: 0x04003BFF RID: 15359
	[ReadOnly]
	public double stopTime;

	// Token: 0x04003C00 RID: 15360
	protected int walkableArea = -1;
}
