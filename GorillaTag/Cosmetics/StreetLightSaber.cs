using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001363 RID: 4963
	public class StreetLightSaber : MonoBehaviour
	{
		// Token: 0x17000C29 RID: 3113
		// (get) Token: 0x06007C61 RID: 31841 RVA: 0x0028A3C7 File Offset: 0x002885C7
		private StreetLightSaber.State CurrentState
		{
			get
			{
				return StreetLightSaber.values[this.currentIndex];
			}
		}

		// Token: 0x06007C62 RID: 31842 RVA: 0x0028A3D8 File Offset: 0x002885D8
		private void Awake()
		{
			foreach (StreetLightSaber.StaffStates staffStates in this.allStates)
			{
				this.allStatesDict[staffStates.state] = staffStates;
			}
			this.currentIndex = 0;
			this.autoSwitchEnabledTime = 0f;
			this.hashId = Shader.PropertyToID(this.shaderColorProperty);
			List<Material> list;
			using (CollectionPool<List<Material>, Material>.Get(out list))
			{
				this.meshRenderer.GetSharedMaterials(list);
				this.instancedMaterial = new Material(list[this.materialIndex]);
				list[this.materialIndex] = this.instancedMaterial;
				this.meshRenderer.SetSharedMaterials(list);
			}
		}

		// Token: 0x06007C63 RID: 31843 RVA: 0x0028A4A0 File Offset: 0x002886A0
		private void Update()
		{
			if (this.autoSwitch && Time.time - this.autoSwitchEnabledTime > this.autoSwitchTimer)
			{
				this.UpdateStateAuto();
			}
		}

		// Token: 0x06007C64 RID: 31844 RVA: 0x0028A4C4 File Offset: 0x002886C4
		private void OnDestroy()
		{
			this.allStatesDict.Clear();
		}

		// Token: 0x06007C65 RID: 31845 RVA: 0x0028A4D1 File Offset: 0x002886D1
		private void OnEnable()
		{
			this.ForceSwitchTo(StreetLightSaber.State.Off);
		}

		// Token: 0x06007C66 RID: 31846 RVA: 0x0028A4DC File Offset: 0x002886DC
		public void UpdateStateManual()
		{
			int num = (this.currentIndex + 1) % StreetLightSaber.values.Length;
			this.SwitchState(num);
		}

		// Token: 0x06007C67 RID: 31847 RVA: 0x0028A504 File Offset: 0x00288704
		private void UpdateStateAuto()
		{
			StreetLightSaber.State state = ((this.CurrentState == StreetLightSaber.State.Green) ? StreetLightSaber.State.Red : StreetLightSaber.State.Green);
			int num = Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, state);
			this.SwitchState(num);
			this.autoSwitchEnabledTime = Time.time;
		}

		// Token: 0x06007C68 RID: 31848 RVA: 0x0028A53D File Offset: 0x0028873D
		public void EnableAutoSwitch(bool enable)
		{
			this.autoSwitch = enable;
		}

		// Token: 0x06007C69 RID: 31849 RVA: 0x0028A4D1 File Offset: 0x002886D1
		public void ResetStaff()
		{
			this.ForceSwitchTo(StreetLightSaber.State.Off);
		}

		// Token: 0x06007C6A RID: 31850 RVA: 0x0028A548 File Offset: 0x00288748
		public void HitReceived(Vector3 contact)
		{
			if (this.velocityTracker != null && this.velocityTracker.GetLatestVelocity(true).magnitude >= this.minHitVelocityThreshold)
			{
				StreetLightSaber.StaffStates staffStates = this.allStatesDict[this.CurrentState];
				if (staffStates == null)
				{
					return;
				}
				staffStates.OnSuccessfulHit.Invoke(contact);
			}
		}

		// Token: 0x06007C6B RID: 31851 RVA: 0x0028A5A0 File Offset: 0x002887A0
		private void SwitchState(int newIndex)
		{
			if (newIndex == this.currentIndex)
			{
				return;
			}
			StreetLightSaber.State currentState = this.CurrentState;
			StreetLightSaber.State state = StreetLightSaber.values[newIndex];
			StreetLightSaber.StaffStates staffStates;
			if (this.allStatesDict.TryGetValue(currentState, out staffStates))
			{
				UnityEvent onExitState = staffStates.onExitState;
				if (onExitState != null)
				{
					onExitState.Invoke();
				}
			}
			this.currentIndex = newIndex;
			StreetLightSaber.StaffStates staffStates2;
			if (this.allStatesDict.TryGetValue(state, out staffStates2))
			{
				UnityEvent onEnterState = staffStates2.onEnterState;
				if (onEnterState != null)
				{
					onEnterState.Invoke();
				}
				if (this.trailRenderer != null)
				{
					this.trailRenderer.startColor = staffStates2.color;
				}
				if (this.meshRenderer != null)
				{
					this.instancedMaterial.SetColor(this.hashId, staffStates2.color);
				}
			}
		}

		// Token: 0x06007C6C RID: 31852 RVA: 0x0028A654 File Offset: 0x00288854
		private void ForceSwitchTo(StreetLightSaber.State targetState)
		{
			int num = Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, targetState);
			if (num >= 0)
			{
				this.SwitchState(num);
			}
		}

		// Token: 0x04008EC6 RID: 36550
		[SerializeField]
		private float autoSwitchTimer = 5f;

		// Token: 0x04008EC7 RID: 36551
		[SerializeField]
		private TrailRenderer trailRenderer;

		// Token: 0x04008EC8 RID: 36552
		[SerializeField]
		private Renderer meshRenderer;

		// Token: 0x04008EC9 RID: 36553
		[SerializeField]
		private string shaderColorProperty;

		// Token: 0x04008ECA RID: 36554
		[SerializeField]
		private int materialIndex;

		// Token: 0x04008ECB RID: 36555
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04008ECC RID: 36556
		[SerializeField]
		private float minHitVelocityThreshold;

		// Token: 0x04008ECD RID: 36557
		private static readonly StreetLightSaber.State[] values = (StreetLightSaber.State[])Enum.GetValues(typeof(StreetLightSaber.State));

		// Token: 0x04008ECE RID: 36558
		[Space]
		[Header("Staff State Settings")]
		public StreetLightSaber.StaffStates[] allStates = new StreetLightSaber.StaffStates[0];

		// Token: 0x04008ECF RID: 36559
		private int currentIndex;

		// Token: 0x04008ED0 RID: 36560
		private Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates> allStatesDict = new Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates>();

		// Token: 0x04008ED1 RID: 36561
		private bool autoSwitch;

		// Token: 0x04008ED2 RID: 36562
		private float autoSwitchEnabledTime;

		// Token: 0x04008ED3 RID: 36563
		private int hashId;

		// Token: 0x04008ED4 RID: 36564
		private Material instancedMaterial;

		// Token: 0x02001364 RID: 4964
		[Serializable]
		public class StaffStates
		{
			// Token: 0x04008ED5 RID: 36565
			public StreetLightSaber.State state;

			// Token: 0x04008ED6 RID: 36566
			public Color color;

			// Token: 0x04008ED7 RID: 36567
			public UnityEvent onEnterState;

			// Token: 0x04008ED8 RID: 36568
			public UnityEvent onExitState;

			// Token: 0x04008ED9 RID: 36569
			public UnityEvent<Vector3> OnSuccessfulHit;
		}

		// Token: 0x02001365 RID: 4965
		public enum State
		{
			// Token: 0x04008EDB RID: 36571
			Off,
			// Token: 0x04008EDC RID: 36572
			Green,
			// Token: 0x04008EDD RID: 36573
			Red
		}
	}
}
