using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F7B RID: 3963
	[NetworkBehaviourWeaved(1)]
	public class ChristmasTree : NetworkComponent
	{
		// Token: 0x06006241 RID: 25153 RVA: 0x001FA018 File Offset: 0x001F8218
		protected override void Awake()
		{
			base.Awake();
			foreach (AttachPoint attachPoint in this.hangers.GetComponentsInChildren<AttachPoint>())
			{
				this.attachPointsList.Add(attachPoint);
				AttachPoint attachPoint2 = attachPoint;
				attachPoint2.onHookedChanged = (UnityAction)Delegate.Combine(attachPoint2.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.lightRenderers = this.lights.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = this.lightRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material = this.lightsOffMaterial;
			}
			this.wasActive = false;
			this.isActive = false;
		}

		// Token: 0x06006242 RID: 25154 RVA: 0x001FA0B9 File Offset: 0x001F82B9
		private void Update()
		{
			if (this.spinTheTop && this.topOrnament)
			{
				this.topOrnament.transform.Rotate(0f, this.spinSpeed * Time.deltaTime, 0f, Space.World);
			}
		}

		// Token: 0x06006243 RID: 25155 RVA: 0x001FA0F8 File Offset: 0x001F82F8
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (AttachPoint attachPoint in this.attachPointsList)
			{
				attachPoint.onHookedChanged = (UnityAction)Delegate.Remove(attachPoint.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.attachPointsList.Clear();
		}

		// Token: 0x06006244 RID: 25156 RVA: 0x001FA178 File Offset: 0x001F8378
		private void UpdateHangers()
		{
			if (this.attachPointsList.Count == 0)
			{
				return;
			}
			using (List<AttachPoint>.Enumerator enumerator = this.attachPointsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsHooked())
					{
						if (base.IsMine)
						{
							this.updateLight(true);
						}
						return;
					}
				}
			}
			if (base.IsMine)
			{
				this.updateLight(false);
			}
		}

		// Token: 0x06006245 RID: 25157 RVA: 0x001FA1F8 File Offset: 0x001F83F8
		private void updateLight(bool enable)
		{
			this.isActive = enable;
			for (int i = 0; i < this.lightRenderers.Length; i++)
			{
				this.lightRenderers[i].material = (enable ? this.lightsOnMaterials[i % this.lightsOnMaterials.Length] : this.lightsOffMaterial);
			}
			this.spinTheTop = enable;
		}

		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x06006246 RID: 25158 RVA: 0x001FA24F File Offset: 0x001F844F
		// (set) Token: 0x06006247 RID: 25159 RVA: 0x001FA279 File Offset: 0x001F8479
		[Networked]
		[NetworkedWeaved(0, 1)]
		private unsafe NetworkBool Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(NetworkBool*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(NetworkBool*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06006248 RID: 25160 RVA: 0x001FA2A4 File Offset: 0x001F84A4
		public override void WriteDataFusion()
		{
			this.Data = this.isActive;
		}

		// Token: 0x06006249 RID: 25161 RVA: 0x001FA2B7 File Offset: 0x001F84B7
		public override void ReadDataFusion()
		{
			this.wasActive = this.isActive;
			this.isActive = this.Data;
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x0600624A RID: 25162 RVA: 0x001FA2F0 File Offset: 0x001F84F0
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			stream.SendNext(this.isActive);
		}

		// Token: 0x0600624B RID: 25163 RVA: 0x001FA314 File Offset: 0x001F8514
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			this.wasActive = this.isActive;
			this.isActive = (bool)stream.ReceiveNext();
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x0600624D RID: 25165 RVA: 0x001FA384 File Offset: 0x001F8584
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600624E RID: 25166 RVA: 0x001FA39C File Offset: 0x001F859C
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0400710B RID: 28939
		public GameObject hangers;

		// Token: 0x0400710C RID: 28940
		public GameObject lights;

		// Token: 0x0400710D RID: 28941
		public GameObject topOrnament;

		// Token: 0x0400710E RID: 28942
		public float spinSpeed = 60f;

		// Token: 0x0400710F RID: 28943
		private readonly List<AttachPoint> attachPointsList = new List<AttachPoint>();

		// Token: 0x04007110 RID: 28944
		private MeshRenderer[] lightRenderers;

		// Token: 0x04007111 RID: 28945
		private bool wasActive;

		// Token: 0x04007112 RID: 28946
		private bool isActive;

		// Token: 0x04007113 RID: 28947
		private bool spinTheTop;

		// Token: 0x04007114 RID: 28948
		[SerializeField]
		private Material lightsOffMaterial;

		// Token: 0x04007115 RID: 28949
		[SerializeField]
		private Material[] lightsOnMaterials;

		// Token: 0x04007116 RID: 28950
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private NetworkBool _Data;
	}
}
