using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002DF RID: 735
public class HandTapEffect : MonoBehaviour
{
	// Token: 0x060012C5 RID: 4805 RVA: 0x00064008 File Offset: 0x00062208
	private void Awake()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		this.leftHandEffect.handContext = componentInParent.LeftHandEffect;
		this.rightHandEffect.handContext = componentInParent.RightHandEffect;
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x0006403E File Offset: 0x0006223E
	private void OnEnable()
	{
		this.leftHandEffect.OnEnable();
		this.rightHandEffect.OnEnable();
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x00064056 File Offset: 0x00062256
	private void OnDisable()
	{
		this.leftHandEffect.OnDisable();
		this.rightHandEffect.OnDisable();
	}

	// Token: 0x040016DF RID: 5855
	public HandTapEffect.HandTapEffectLeftRight leftHandEffect;

	// Token: 0x040016E0 RID: 5856
	public HandTapEffect.HandTapEffectLeftRight rightHandEffect;

	// Token: 0x020002E0 RID: 736
	[Serializable]
	public class HandTapEffectDownUp
	{
		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060012C9 RID: 4809 RVA: 0x0006406E File Offset: 0x0006226E
		public bool HasOverrides
		{
			get
			{
				return this.overrides.overrideSurfacePrefab || this.overrides.overrideGamemodePrefab || this.overrides.overrideSound;
			}
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x00064098 File Offset: 0x00062298
		internal void OnTap(HandEffectContext handContext)
		{
			UnityEvent unityEvent = this.onTapUnityEvents;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			for (int i = 0; i < this.onTapBehaviours.Length; i++)
			{
				this.onTapBehaviours[i].OnTap(handContext);
			}
		}

		// Token: 0x040016E1 RID: 5857
		public HandTapBehaviour[] onTapBehaviours;

		// Token: 0x040016E2 RID: 5858
		public UnityEvent onTapUnityEvents;

		// Token: 0x040016E3 RID: 5859
		[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
		public HashWrapper onTapPrefabToSpawn;

		// Token: 0x040016E4 RID: 5860
		public HandTapOverrides overrides;
	}

	// Token: 0x020002E1 RID: 737
	[Serializable]
	public class HandTapEffectLeftRight
	{
		// Token: 0x060012CC RID: 4812 RVA: 0x000640D8 File Offset: 0x000622D8
		public void OnEnable()
		{
			if (this.separateUpTapCooldown)
			{
				this.handContext.SeparateUpTapCooldown = true;
			}
			if ((in this.downTapEffect.onTapPrefabToSpawn) != -1)
			{
				this.handContext.AddFXPrefab(in this.downTapEffect.onTapPrefabToSpawn);
			}
			if (this.downTapEffect.HasOverrides)
			{
				this.handContext.DownTapOverrides = this.downTapEffect.overrides;
			}
			if (this.upTapEffect.HasOverrides)
			{
				this.handContext.UpTapOverrides = this.upTapEffect.overrides;
			}
			this.handContext.handTapDown += this.downTapEffect.OnTap;
			this.handContext.handTapUp += this.upTapEffect.OnTap;
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x000641A8 File Offset: 0x000623A8
		public void OnDisable()
		{
			if (this.separateUpTapCooldown)
			{
				this.handContext.SeparateUpTapCooldown = false;
			}
			if ((in this.downTapEffect.onTapPrefabToSpawn) != -1)
			{
				this.handContext.RemoveFXPrefab(in this.downTapEffect.onTapPrefabToSpawn);
			}
			if (this.downTapEffect.HasOverrides && this.handContext.DownTapOverrides == this.downTapEffect.overrides)
			{
				this.handContext.DownTapOverrides = null;
			}
			if (this.upTapEffect.HasOverrides && this.handContext.UpTapOverrides == this.upTapEffect.overrides)
			{
				this.handContext.UpTapOverrides = null;
			}
			this.handContext.handTapDown -= this.downTapEffect.OnTap;
			this.handContext.handTapUp -= this.upTapEffect.OnTap;
		}

		// Token: 0x040016E5 RID: 5861
		public bool separateUpTapCooldown;

		// Token: 0x040016E6 RID: 5862
		public HandTapEffect.HandTapEffectDownUp downTapEffect;

		// Token: 0x040016E7 RID: 5863
		public HandTapEffect.HandTapEffectDownUp upTapEffect;

		// Token: 0x040016E8 RID: 5864
		internal HandEffectContext handContext;
	}
}
