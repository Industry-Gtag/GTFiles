using System;
using GorillaExtensions;
using TagEffects;
using UnityEngine;

// Token: 0x020003DC RID: 988
[RequireComponent(typeof(Collider))]
public class HandEffectsTester : MonoBehaviour, IHandEffectsTrigger
{
	// Token: 0x17000241 RID: 577
	// (get) Token: 0x0600177C RID: 6012 RVA: 0x000874A5 File Offset: 0x000856A5
	public bool Static
	{
		get
		{
			return this.isStatic;
		}
	}

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x0600177D RID: 6013 RVA: 0x000874AD File Offset: 0x000856AD
	Transform IHandEffectsTrigger.Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x0600177E RID: 6014 RVA: 0x00036275 File Offset: 0x00034475
	VRRig IHandEffectsTrigger.Rig
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x0600177F RID: 6015 RVA: 0x000874B5 File Offset: 0x000856B5
	IHandEffectsTrigger.Mode IHandEffectsTrigger.EffectMode
	{
		get
		{
			return this.mode;
		}
	}

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x06001780 RID: 6016 RVA: 0x000874BD File Offset: 0x000856BD
	bool IHandEffectsTrigger.FingersDown
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.FistBump || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x06001781 RID: 6017 RVA: 0x000874D4 File Offset: 0x000856D4
	bool IHandEffectsTrigger.FingersUp
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.HighFive || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06001782 RID: 6018 RVA: 0x000874EA File Offset: 0x000856EA
	// (set) Token: 0x06001783 RID: 6019 RVA: 0x000874F2 File Offset: 0x000856F2
	public Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

	// Token: 0x17000248 RID: 584
	// (get) Token: 0x06001784 RID: 6020 RVA: 0x000874FB File Offset: 0x000856FB
	public bool RightHand { get; }

	// Token: 0x06001785 RID: 6021 RVA: 0x00087503 File Offset: 0x00085703
	private void Awake()
	{
		this.triggerZone = base.GetComponent<Collider>();
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x00087511 File Offset: 0x00085711
	private void OnEnable()
	{
		if (!HandEffectsTriggerRegistry.HasInstance)
		{
			HandEffectsTriggerRegistry.FindInstance();
		}
		HandEffectsTriggerRegistry.Instance.Register(this);
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x0008752A File Offset: 0x0008572A
	private void OnDisable()
	{
		HandEffectsTriggerRegistry.Instance.Unregister(this);
	}

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06001788 RID: 6024 RVA: 0x00087537 File Offset: 0x00085737
	Vector3 IHandEffectsTrigger.Velocity
	{
		get
		{
			if (this.mode == IHandEffectsTrigger.Mode.HighFive)
			{
				return Vector3.zero;
			}
			IHandEffectsTrigger.Mode mode = this.mode;
			return Vector3.zero;
		}
	}

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06001789 RID: 6025 RVA: 0x00087555 File Offset: 0x00085755
	TagEffectPack IHandEffectsTrigger.CosmeticEffectPack
	{
		get
		{
			return this.cosmeticEffectPack;
		}
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnTriggerEntered(IHandEffectsTrigger other)
	{
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x00087560 File Offset: 0x00085760
	public bool InTriggerZone(IHandEffectsTrigger t)
	{
		if (!(base.transform.position - t.Transform.position).IsShorterThan(this.triggerZone.bounds.size))
		{
			return false;
		}
		RaycastHit raycastHit;
		switch (this.mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			return t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), out raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.FistBump:
			return t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), out raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.HighFive_And_FistBump:
			return (t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), out raycastHit, this.triggerRadius)) || (t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), out raycastHit, this.triggerRadius));
		}
		return this.triggerZone.Raycast(new Ray(t.Transform.position, this.triggerZone.bounds.center - t.Transform.position), out raycastHit, this.triggerRadius);
	}

	// Token: 0x040022C4 RID: 8900
	[SerializeField]
	private TagEffectPack cosmeticEffectPack;

	// Token: 0x040022C5 RID: 8901
	private Collider triggerZone;

	// Token: 0x040022C6 RID: 8902
	public IHandEffectsTrigger.Mode mode;

	// Token: 0x040022C7 RID: 8903
	[SerializeField]
	private float triggerRadius = 0.07f;

	// Token: 0x040022C8 RID: 8904
	[SerializeField]
	private bool isStatic = true;
}
