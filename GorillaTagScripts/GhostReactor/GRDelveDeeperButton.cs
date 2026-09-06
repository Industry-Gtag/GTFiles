using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02001016 RID: 4118
	[RequireComponent(typeof(GorillaPressableButton))]
	public sealed class GRDelveDeeperButton : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x0600668F RID: 26255 RVA: 0x0020F14F File Offset: 0x0020D34F
		private void Awake()
		{
			this.CountMonkes();
		}

		// Token: 0x06006690 RID: 26256 RVA: 0x0020F158 File Offset: 0x0020D358
		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawCube(this._drillCollider.bounds.center, this._drillCollider.bounds.size);
		}

		// Token: 0x06006691 RID: 26257 RVA: 0x0020F190 File Offset: 0x0020D390
		private void CountMonkes()
		{
			int num = Physics.OverlapBoxNonAlloc(this._drillCollider.bounds.center, this._drillCollider.bounds.extents, this._overlapBoxResults, this._drillCollider.transform.rotation, 2048);
			this._numGorillasInDrill = 0;
			for (int i = 0; i < num; i++)
			{
				if (this._overlapBoxResults[i].GetComponent<VRRig>() != null && this._drillCollider.bounds.Contains(this._overlapBoxResults[i].transform.position))
				{
					this._numGorillasInDrill++;
				}
			}
		}

		// Token: 0x06006692 RID: 26258 RVA: 0x0020F241 File Offset: 0x0020D441
		private void OnEnable()
		{
			if (this._shiftManager == null)
			{
				throw new Exception("_shiftManager unset for GREndShiftButton.");
			}
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this._button = base.GetComponent<GorillaPressableButton>();
			this.UpdateButton();
		}

		// Token: 0x06006693 RID: 26259 RVA: 0x00012134 File Offset: 0x00010334
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06006694 RID: 26260 RVA: 0x0020F275 File Offset: 0x0020D475
		public void SliceUpdate()
		{
			this.CountMonkes();
			this.UpdateButton();
		}

		// Token: 0x06006695 RID: 26261 RVA: 0x0020F284 File Offset: 0x0020D484
		private void UpdateButton()
		{
			if (this._shiftManager.authorizedToDelveDeeper && this._numGorillasInDrill == this._shiftManager.reactor.NumActivePlayers)
			{
				this._button.enabled = true;
				this._text.text = "DELVE\nNOW";
				return;
			}
			this._button.enabled = false;
			this._text.text = "DISABLED";
		}

		// Token: 0x06006696 RID: 26262 RVA: 0x0020F2F4 File Offset: 0x0020D4F4
		public void DelveDeeper()
		{
			this._shiftManager.EndShift();
		}

		// Token: 0x04007564 RID: 30052
		[SerializeField]
		private BoxCollider _drillCollider;

		// Token: 0x04007565 RID: 30053
		[SerializeField]
		private GhostReactorShiftManager _shiftManager;

		// Token: 0x04007566 RID: 30054
		[SerializeField]
		private TextMeshPro _text;

		// Token: 0x04007567 RID: 30055
		private GorillaPressableButton _button;

		// Token: 0x04007568 RID: 30056
		private int _numGorillasInDrill;

		// Token: 0x04007569 RID: 30057
		private readonly Collider[] _overlapBoxResults = new Collider[200];
	}
}
