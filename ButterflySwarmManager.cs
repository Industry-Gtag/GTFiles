using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class ButterflySwarmManager : MonoBehaviour
{
	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000E85 RID: 3717 RVA: 0x0004FB4F File Offset: 0x0004DD4F
	// (set) Token: 0x06000E86 RID: 3718 RVA: 0x0004FB57 File Offset: 0x0004DD57
	public float PerchedFlapSpeed { get; private set; }

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000E87 RID: 3719 RVA: 0x0004FB60 File Offset: 0x0004DD60
	// (set) Token: 0x06000E88 RID: 3720 RVA: 0x0004FB68 File Offset: 0x0004DD68
	public float PerchedFlapPhase { get; private set; }

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000E89 RID: 3721 RVA: 0x0004FB71 File Offset: 0x0004DD71
	// (set) Token: 0x06000E8A RID: 3722 RVA: 0x0004FB79 File Offset: 0x0004DD79
	public float BeeSpeed { get; private set; }

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000E8B RID: 3723 RVA: 0x0004FB82 File Offset: 0x0004DD82
	// (set) Token: 0x06000E8C RID: 3724 RVA: 0x0004FB8A File Offset: 0x0004DD8A
	public float BeeMaxTravelTime { get; private set; }

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000E8D RID: 3725 RVA: 0x0004FB93 File Offset: 0x0004DD93
	// (set) Token: 0x06000E8E RID: 3726 RVA: 0x0004FB9B File Offset: 0x0004DD9B
	public float BeeAcceleration { get; private set; }

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000E8F RID: 3727 RVA: 0x0004FBA4 File Offset: 0x0004DDA4
	// (set) Token: 0x06000E90 RID: 3728 RVA: 0x0004FBAC File Offset: 0x0004DDAC
	public float BeeJitterStrength { get; private set; }

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000E91 RID: 3729 RVA: 0x0004FBB5 File Offset: 0x0004DDB5
	// (set) Token: 0x06000E92 RID: 3730 RVA: 0x0004FBBD File Offset: 0x0004DDBD
	public float BeeJitterDamping { get; private set; }

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000E93 RID: 3731 RVA: 0x0004FBC6 File Offset: 0x0004DDC6
	// (set) Token: 0x06000E94 RID: 3732 RVA: 0x0004FBCE File Offset: 0x0004DDCE
	public float BeeMaxJitterRadius { get; private set; }

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000E95 RID: 3733 RVA: 0x0004FBD7 File Offset: 0x0004DDD7
	// (set) Token: 0x06000E96 RID: 3734 RVA: 0x0004FBDF File Offset: 0x0004DDDF
	public float BeeNearDestinationRadius { get; private set; }

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000E97 RID: 3735 RVA: 0x0004FBE8 File Offset: 0x0004DDE8
	// (set) Token: 0x06000E98 RID: 3736 RVA: 0x0004FBF0 File Offset: 0x0004DDF0
	public float DestRotationAlignmentSpeed { get; private set; }

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000E99 RID: 3737 RVA: 0x0004FBF9 File Offset: 0x0004DDF9
	// (set) Token: 0x06000E9A RID: 3738 RVA: 0x0004FC01 File Offset: 0x0004DE01
	public Vector3 TravellingLocalRotationEuler { get; private set; }

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000E9B RID: 3739 RVA: 0x0004FC0A File Offset: 0x0004DE0A
	// (set) Token: 0x06000E9C RID: 3740 RVA: 0x0004FC12 File Offset: 0x0004DE12
	public Quaternion TravellingLocalRotation { get; private set; }

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000E9D RID: 3741 RVA: 0x0004FC1B File Offset: 0x0004DE1B
	// (set) Token: 0x06000E9E RID: 3742 RVA: 0x0004FC23 File Offset: 0x0004DE23
	public float AvoidPointRadius { get; private set; }

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000E9F RID: 3743 RVA: 0x0004FC2C File Offset: 0x0004DE2C
	// (set) Token: 0x06000EA0 RID: 3744 RVA: 0x0004FC34 File Offset: 0x0004DE34
	public float BeeMinFlowerDuration { get; private set; }

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000EA1 RID: 3745 RVA: 0x0004FC3D File Offset: 0x0004DE3D
	// (set) Token: 0x06000EA2 RID: 3746 RVA: 0x0004FC45 File Offset: 0x0004DE45
	public float BeeMaxFlowerDuration { get; private set; }

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000EA3 RID: 3747 RVA: 0x0004FC4E File Offset: 0x0004DE4E
	// (set) Token: 0x06000EA4 RID: 3748 RVA: 0x0004FC56 File Offset: 0x0004DE56
	public Color[] BeeColors { get; private set; }

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0004FC60 File Offset: 0x0004DE60
	private void Awake()
	{
		this.TravellingLocalRotation = Quaternion.Euler(this.TravellingLocalRotationEuler);
		this.butterflies = new List<AnimatedButterfly>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedButterfly animatedButterfly = default(AnimatedButterfly);
			animatedButterfly.InitVisual(this.beePrefab, this);
			if (this.BeeColors.Length != 0)
			{
				animatedButterfly.SetColor(this.BeeColors[i % this.BeeColors.Length]);
			}
			this.butterflies.Add(animatedButterfly);
		}
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x0004FCE8 File Offset: 0x0004DEE8
	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.perchSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				List<GameObject> list = new List<GameObject>();
				this.allPerchZones.Add(list);
				foreach (object obj in gameObject.transform)
				{
					Transform transform = (Transform)obj;
					list.Add(transform.gameObject);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x0004FDA8 File Offset: 0x0004DFA8
	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0004FDC0 File Offset: 0x0004DFC0
	private void Update()
	{
		for (int i = 0; i < this.butterflies.Count; i++)
		{
			AnimatedButterfly animatedButterfly = this.butterflies[i];
			animatedButterfly.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			this.butterflies[i] = animatedButterfly;
		}
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x0004FE10 File Offset: 0x0004E010
	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<List<GameObject>> list = new List<List<GameObject>>(this.allPerchZones.Count);
		for (int i = 0; i < this.allPerchZones.Count; i++)
		{
			List<GameObject> list2 = new List<GameObject>();
			list2.AddRange(this.allPerchZones[i]);
			list.Add(list2);
		}
		List<GameObject> list3 = new List<GameObject>(this.loopSizePerBee);
		List<float> list4 = new List<float>(this.loopSizePerBee);
		for (int j = 0; j < this.butterflies.Count; j++)
		{
			AnimatedButterfly animatedButterfly = this.butterflies[j];
			animatedButterfly.SetFlapSpeed(srand.NextFloat(this.minFlapSpeed, this.maxFlapSpeed));
			list3.Clear();
			list4.Clear();
			this.PickPoints(this.loopSizePerBee, list, ref srand, list3);
			for (int k = 0; k < list3.Count; k++)
			{
				list4.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			if (list3.Count == 0)
			{
				this.butterflies.Clear();
				return;
			}
			animatedButterfly.InitRoute(list3, list4, this);
			this.butterflies[j] = animatedButterfly;
		}
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x0004FF54 File Offset: 0x0004E154
	private void PickPoints(int n, List<List<GameObject>> pickBuffer, ref SRand rand, List<GameObject> resultBuffer)
	{
		int num = rand.NextInt(0, pickBuffer.Count);
		int num2 = -1;
		int num3 = n - 2;
		while (resultBuffer.Count < n)
		{
			int num4;
			if (resultBuffer.Count < num3)
			{
				num4 = rand.NextIntWithExclusion(0, pickBuffer.Count, num2);
			}
			else
			{
				num4 = rand.NextIntWithExclusion2(0, pickBuffer.Count, num2, num);
			}
			int num5 = 10;
			while (num4 == num2 || pickBuffer[num4].Count == 0)
			{
				num4 = (num4 + 1) % pickBuffer.Count;
				num5--;
				if (num5 <= 0)
				{
					return;
				}
			}
			num2 = num4;
			List<GameObject> list = pickBuffer[num2];
			while (list.Count == 0)
			{
				num2 = (num2 + 1) % pickBuffer.Count;
				list = pickBuffer[num2];
			}
			resultBuffer.Add(list[list.Count - 1]);
			list.RemoveAt(list.Count - 1);
		}
	}

	// Token: 0x0400117E RID: 4478
	[SerializeField]
	private XSceneRef[] perchSections;

	// Token: 0x0400117F RID: 4479
	[SerializeField]
	private int loopSizePerBee;

	// Token: 0x04001180 RID: 4480
	[SerializeField]
	private int numBees;

	// Token: 0x04001181 RID: 4481
	[SerializeField]
	private MeshRenderer beePrefab;

	// Token: 0x04001182 RID: 4482
	[SerializeField]
	private float maxFlapSpeed;

	// Token: 0x04001183 RID: 4483
	[SerializeField]
	private float minFlapSpeed;

	// Token: 0x04001194 RID: 4500
	private List<AnimatedButterfly> butterflies;

	// Token: 0x04001195 RID: 4501
	private List<List<GameObject>> allPerchZones = new List<List<GameObject>>();
}
