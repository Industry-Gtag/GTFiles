using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F40 RID: 3904
	public class Mole : Tappable
	{
		// Token: 0x140000A8 RID: 168
		// (add) Token: 0x06005FD0 RID: 24528 RVA: 0x001E66A8 File Offset: 0x001E48A8
		// (remove) Token: 0x06005FD1 RID: 24529 RVA: 0x001E66E0 File Offset: 0x001E48E0
		public event Mole.MoleTapEvent OnTapped;

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x06005FD2 RID: 24530 RVA: 0x001E6715 File Offset: 0x001E4915
		// (set) Token: 0x06005FD3 RID: 24531 RVA: 0x001E671D File Offset: 0x001E491D
		public bool IsLeftSideMole { get; set; }

		// Token: 0x06005FD4 RID: 24532 RVA: 0x001E6728 File Offset: 0x001E4928
		private void Awake()
		{
			this.currentState = Mole.MoleState.Hidden;
			Vector3 position = base.transform.position;
			this.origin = (this.target = position);
			this.visiblePosition = new Vector3(position.x, position.y + this.positionOffset, position.z);
			this.hiddenPosition = new Vector3(position.x, position.y - this.positionOffset, position.z);
			this.travelTime = this.normalTravelTime;
			this.animCurve = (this.normalAnimCurve = AnimationCurves.EaseInOutQuad);
			this.hitAnimCurve = AnimationCurves.EaseOutBack;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				if (this.moleTypes[i].isHazard)
				{
					this.hazardMoles.Add(i);
				}
				else
				{
					this.safeMoles.Add(i);
				}
			}
			this.randomMolePickedIndex = -1;
		}

		// Token: 0x06005FD5 RID: 24533 RVA: 0x001E6810 File Offset: 0x001E4A10
		public void InvokeUpdate()
		{
			if (this.currentState == Mole.MoleState.Ready)
			{
				return;
			}
			switch (this.currentState)
			{
			case Mole.MoleState.Reset:
			case Mole.MoleState.Hidden:
				this.currentState = Mole.MoleState.Ready;
				break;
			case Mole.MoleState.TransitionToVisible:
			case Mole.MoleState.TransitionToHidden:
			{
				float num = this.animCurve.Evaluate(Mathf.Clamp01((Time.time - this.animStartTime) / this.travelTime));
				base.transform.position = Vector3.Lerp(this.origin, this.target, num);
				if (num >= 1f)
				{
					this.currentState++;
				}
				break;
			}
			}
			if (Time.time - this.currentTime >= this.showMoleDuration && this.currentState > Mole.MoleState.Ready && this.currentState < Mole.MoleState.TransitionToHidden)
			{
				this.HideMole(false);
			}
		}

		// Token: 0x06005FD6 RID: 24534 RVA: 0x001E68DB File Offset: 0x001E4ADB
		public bool CanPickMole()
		{
			return this.currentState == Mole.MoleState.Ready;
		}

		// Token: 0x06005FD7 RID: 24535 RVA: 0x001E68E8 File Offset: 0x001E4AE8
		public void ShowMole(float _showMoleDuration, int randomMoleTypeIndex)
		{
			if (randomMoleTypeIndex >= this.moleTypes.Length || randomMoleTypeIndex < 0)
			{
				return;
			}
			this.randomMolePickedIndex = randomMoleTypeIndex;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				this.moleTypes[i].gameObject.SetActive(i == randomMoleTypeIndex);
				if (this.moleTypes[i].monkeMoleDefaultMaterial != null)
				{
					this.moleTypes[i].MeshRenderer.material = this.moleTypes[i].monkeMoleDefaultMaterial;
				}
			}
			this.showMoleDuration = _showMoleDuration;
			this.origin = base.transform.position;
			this.target = this.visiblePosition;
			this.animCurve = this.normalAnimCurve;
			this.currentState = Mole.MoleState.TransitionToVisible;
			this.animStartTime = (this.currentTime = Time.time);
			this.travelTime = this.normalTravelTime;
		}

		// Token: 0x06005FD8 RID: 24536 RVA: 0x001E69C0 File Offset: 0x001E4BC0
		public void HideMole(bool isHit = false)
		{
			if (this.currentState < Mole.MoleState.TransitionToVisible || this.currentState > Mole.MoleState.Visible)
			{
				return;
			}
			this.origin = base.transform.position;
			this.target = this.hiddenPosition;
			this.animCurve = (isHit ? this.hitAnimCurve : this.normalAnimCurve);
			this.animStartTime = Time.time;
			this.travelTime = (isHit ? this.hitTravelTime : this.normalTravelTime);
			this.currentState = Mole.MoleState.TransitionToHidden;
		}

		// Token: 0x06005FD9 RID: 24537 RVA: 0x001E6A40 File Offset: 0x001E4C40
		public bool CanTap()
		{
			Mole.MoleState moleState = this.currentState;
			return moleState == Mole.MoleState.TransitionToVisible || moleState == Mole.MoleState.Visible;
		}

		// Token: 0x06005FDA RID: 24538 RVA: 0x001E6A65 File Offset: 0x001E4C65
		public override bool CanTap(bool isLeftHand)
		{
			return this.CanTap();
		}

		// Token: 0x06005FDB RID: 24539 RVA: 0x001E6A70 File Offset: 0x001E4C70
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!this.CanTap())
			{
				return;
			}
			bool flag = info.Sender.ActorNumber == NetworkSystem.Instance.LocalPlayerID;
			bool flag2 = flag && GorillaTagger.Instance.lastLeftTap >= GorillaTagger.Instance.lastRightTap;
			MoleTypes moleTypes = null;
			if (this.randomMolePickedIndex >= 0 && this.randomMolePickedIndex < this.moleTypes.Length)
			{
				moleTypes = this.moleTypes[this.randomMolePickedIndex];
			}
			if (moleTypes != null)
			{
				Mole.MoleTapEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(moleTypes, base.transform.position, flag, flag2);
			}
		}

		// Token: 0x06005FDC RID: 24540 RVA: 0x001E6B0E File Offset: 0x001E4D0E
		public void ResetPosition()
		{
			base.transform.position = this.hiddenPosition;
			this.currentState = Mole.MoleState.Reset;
		}

		// Token: 0x06005FDD RID: 24541 RVA: 0x001E6B28 File Offset: 0x001E4D28
		public int GetMoleTypeIndex(bool useHazardMole)
		{
			if (!useHazardMole)
			{
				return this.safeMoles[Random.Range(0, this.safeMoles.Count)];
			}
			return this.hazardMoles[Random.Range(0, this.hazardMoles.Count)];
		}

		// Token: 0x04006E42 RID: 28226
		public float positionOffset = 0.2f;

		// Token: 0x04006E43 RID: 28227
		public MoleTypes[] moleTypes;

		// Token: 0x04006E44 RID: 28228
		private float showMoleDuration;

		// Token: 0x04006E45 RID: 28229
		private Vector3 visiblePosition;

		// Token: 0x04006E46 RID: 28230
		private Vector3 hiddenPosition;

		// Token: 0x04006E47 RID: 28231
		private float currentTime;

		// Token: 0x04006E48 RID: 28232
		private float animStartTime;

		// Token: 0x04006E49 RID: 28233
		private float travelTime;

		// Token: 0x04006E4A RID: 28234
		private float normalTravelTime = 0.3f;

		// Token: 0x04006E4B RID: 28235
		private float hitTravelTime = 0.2f;

		// Token: 0x04006E4C RID: 28236
		private AnimationCurve animCurve;

		// Token: 0x04006E4D RID: 28237
		private AnimationCurve normalAnimCurve;

		// Token: 0x04006E4E RID: 28238
		private AnimationCurve hitAnimCurve;

		// Token: 0x04006E4F RID: 28239
		private Mole.MoleState currentState;

		// Token: 0x04006E50 RID: 28240
		private Vector3 origin;

		// Token: 0x04006E51 RID: 28241
		private Vector3 target;

		// Token: 0x04006E52 RID: 28242
		private int randomMolePickedIndex;

		// Token: 0x04006E54 RID: 28244
		public CallLimiter rpcCooldown;

		// Token: 0x04006E55 RID: 28245
		private int moleScore;

		// Token: 0x04006E56 RID: 28246
		private List<int> safeMoles = new List<int>();

		// Token: 0x04006E57 RID: 28247
		private List<int> hazardMoles = new List<int>();

		// Token: 0x02000F41 RID: 3905
		// (Invoke) Token: 0x06005FE0 RID: 24544
		public delegate void MoleTapEvent(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeft);

		// Token: 0x02000F42 RID: 3906
		public enum MoleState
		{
			// Token: 0x04006E5A RID: 28250
			Reset,
			// Token: 0x04006E5B RID: 28251
			Ready,
			// Token: 0x04006E5C RID: 28252
			TransitionToVisible,
			// Token: 0x04006E5D RID: 28253
			Visible,
			// Token: 0x04006E5E RID: 28254
			TransitionToHidden,
			// Token: 0x04006E5F RID: 28255
			Hidden
		}
	}
}
