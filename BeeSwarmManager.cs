using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class BeeSwarmManager : MonoBehaviour
{
	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000E63 RID: 3683 RVA: 0x0004F6A6 File Offset: 0x0004D8A6
	// (set) Token: 0x06000E64 RID: 3684 RVA: 0x0004F6AE File Offset: 0x0004D8AE
	public BeePerchPoint BeeHive { get; private set; }

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000E65 RID: 3685 RVA: 0x0004F6B7 File Offset: 0x0004D8B7
	// (set) Token: 0x06000E66 RID: 3686 RVA: 0x0004F6BF File Offset: 0x0004D8BF
	public float BeeSpeed { get; private set; }

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000E67 RID: 3687 RVA: 0x0004F6C8 File Offset: 0x0004D8C8
	// (set) Token: 0x06000E68 RID: 3688 RVA: 0x0004F6D0 File Offset: 0x0004D8D0
	public float BeeMaxTravelTime { get; private set; }

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000E69 RID: 3689 RVA: 0x0004F6D9 File Offset: 0x0004D8D9
	// (set) Token: 0x06000E6A RID: 3690 RVA: 0x0004F6E1 File Offset: 0x0004D8E1
	public float BeeAcceleration { get; private set; }

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000E6B RID: 3691 RVA: 0x0004F6EA File Offset: 0x0004D8EA
	// (set) Token: 0x06000E6C RID: 3692 RVA: 0x0004F6F2 File Offset: 0x0004D8F2
	public float BeeJitterStrength { get; private set; }

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000E6D RID: 3693 RVA: 0x0004F6FB File Offset: 0x0004D8FB
	// (set) Token: 0x06000E6E RID: 3694 RVA: 0x0004F703 File Offset: 0x0004D903
	public float BeeJitterDamping { get; private set; }

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000E6F RID: 3695 RVA: 0x0004F70C File Offset: 0x0004D90C
	// (set) Token: 0x06000E70 RID: 3696 RVA: 0x0004F714 File Offset: 0x0004D914
	public float BeeMaxJitterRadius { get; private set; }

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000E71 RID: 3697 RVA: 0x0004F71D File Offset: 0x0004D91D
	// (set) Token: 0x06000E72 RID: 3698 RVA: 0x0004F725 File Offset: 0x0004D925
	public float BeeNearDestinationRadius { get; private set; }

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000E73 RID: 3699 RVA: 0x0004F72E File Offset: 0x0004D92E
	// (set) Token: 0x06000E74 RID: 3700 RVA: 0x0004F736 File Offset: 0x0004D936
	public float AvoidPointRadius { get; private set; }

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000E75 RID: 3701 RVA: 0x0004F73F File Offset: 0x0004D93F
	// (set) Token: 0x06000E76 RID: 3702 RVA: 0x0004F747 File Offset: 0x0004D947
	public float BeeMinFlowerDuration { get; private set; }

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000E77 RID: 3703 RVA: 0x0004F750 File Offset: 0x0004D950
	// (set) Token: 0x06000E78 RID: 3704 RVA: 0x0004F758 File Offset: 0x0004D958
	public float BeeMaxFlowerDuration { get; private set; }

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000E79 RID: 3705 RVA: 0x0004F761 File Offset: 0x0004D961
	// (set) Token: 0x06000E7A RID: 3706 RVA: 0x0004F769 File Offset: 0x0004D969
	public float GeneralBuzzRange { get; private set; }

	// Token: 0x06000E7B RID: 3707 RVA: 0x0004F774 File Offset: 0x0004D974
	private void Awake()
	{
		this.bees = new List<AnimatedBee>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedBee animatedBee = default(AnimatedBee);
			animatedBee.InitVisual(this.beePrefab, this);
			this.bees.Add(animatedBee);
		}
		this.playerCamera = Camera.main.transform;
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0004F7D8 File Offset: 0x0004D9D8
	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.flowerSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				foreach (BeePerchPoint beePerchPoint in gameObject.GetComponentsInChildren<BeePerchPoint>())
				{
					this.allPerchPoints.Add(beePerchPoint);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0004F858 File Offset: 0x0004DA58
	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0004F870 File Offset: 0x0004DA70
	private void Update()
	{
		Vector3 position = this.playerCamera.transform.position;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		float num = 1f / (float)this.bees.Count;
		float num2 = float.PositiveInfinity;
		float num3 = this.GeneralBuzzRange * this.GeneralBuzzRange;
		int num4 = 0;
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee animatedBee = this.bees[i];
			animatedBee.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			Vector3 position2 = animatedBee.visual.transform.position;
			float sqrMagnitude = (position2 - position).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				vector = position2;
				num2 = sqrMagnitude;
			}
			if (sqrMagnitude < num3)
			{
				vector2 += position2;
				num4++;
			}
			this.bees[i] = animatedBee;
		}
		this.nearbyBeeBuzz.transform.position = vector;
		if (num4 > 0)
		{
			this.generalBeeBuzz.transform.position = vector2 / (float)num4;
			this.generalBeeBuzz.enabled = true;
			return;
		}
		this.generalBeeBuzz.enabled = false;
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x0004F9A0 File Offset: 0x0004DBA0
	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<BeePerchPoint> list = new List<BeePerchPoint>(this.allPerchPoints.Count);
		List<BeePerchPoint> list2 = new List<BeePerchPoint>(this.loopSizePerBee);
		List<float> list3 = new List<float>(this.loopSizePerBee);
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee animatedBee = this.bees[i];
			list2 = new List<BeePerchPoint>(this.loopSizePerBee);
			list3 = new List<float>(this.loopSizePerBee);
			this.PickPoints(this.loopSizePerBee, list, this.allPerchPoints, ref srand, list2);
			for (int j = 0; j < list2.Count; j++)
			{
				list3.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			animatedBee.InitRoute(list2, list3, this);
			animatedBee.InitRouteTimestamps();
			this.bees[i] = animatedBee;
		}
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0004FA94 File Offset: 0x0004DC94
	private void PickPoints(int n, List<BeePerchPoint> pickBuffer, List<BeePerchPoint> allPerchPoints, ref SRand rand, List<BeePerchPoint> resultBuffer)
	{
		resultBuffer.Add(this.BeeHive);
		n--;
		int num = 100;
		while (pickBuffer.Count < n && num-- > 0)
		{
			n -= pickBuffer.Count;
			resultBuffer.AddRange(pickBuffer);
			pickBuffer.Clear();
			pickBuffer.AddRange(allPerchPoints);
			rand.Shuffle<BeePerchPoint>(pickBuffer);
		}
		resultBuffer.AddRange(pickBuffer.GetRange(pickBuffer.Count - n, n));
		pickBuffer.RemoveRange(pickBuffer.Count - n, n);
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0004FB15 File Offset: 0x0004DD15
	public static void RegisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Add(obj);
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0004FB22 File Offset: 0x0004DD22
	public static void UnregisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Remove(obj);
	}

	// Token: 0x04001167 RID: 4455
	[SerializeField]
	private XSceneRef[] flowerSections;

	// Token: 0x04001168 RID: 4456
	[SerializeField]
	private int loopSizePerBee;

	// Token: 0x04001169 RID: 4457
	[SerializeField]
	private int numBees;

	// Token: 0x0400116A RID: 4458
	[SerializeField]
	private MeshRenderer beePrefab;

	// Token: 0x0400116B RID: 4459
	[SerializeField]
	private AudioSource nearbyBeeBuzz;

	// Token: 0x0400116C RID: 4460
	[SerializeField]
	private AudioSource generalBeeBuzz;

	// Token: 0x0400116D RID: 4461
	private GameObject[] flowerSectionsResolved;

	// Token: 0x0400117A RID: 4474
	private List<AnimatedBee> bees;

	// Token: 0x0400117B RID: 4475
	private Transform playerCamera;

	// Token: 0x0400117C RID: 4476
	private List<BeePerchPoint> allPerchPoints = new List<BeePerchPoint>();

	// Token: 0x0400117D RID: 4477
	public static readonly List<GameObject> avoidPoints = new List<GameObject>();
}
