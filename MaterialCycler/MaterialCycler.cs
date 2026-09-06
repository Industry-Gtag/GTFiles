using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MaterialCycler
{
	// Token: 0x0200116A RID: 4458
	public class MaterialCycler : MonoBehaviour
	{
		// Token: 0x17000AA6 RID: 2726
		// (get) Token: 0x06006FBD RID: 28605 RVA: 0x002408F3 File Offset: 0x0023EAF3
		// (set) Token: 0x06006FBE RID: 28606 RVA: 0x002408FB File Offset: 0x0023EAFB
		public int index { get; private set; }

		// Token: 0x17000AA7 RID: 2727
		// (get) Token: 0x06006FBF RID: 28607 RVA: 0x00240904 File Offset: 0x0023EB04
		public GrabbingColorPicker ColorPicker
		{
			get
			{
				return this._colorPicker;
			}
		}

		// Token: 0x17000AA8 RID: 2728
		// (get) Token: 0x06006FC0 RID: 28608 RVA: 0x0024090C File Offset: 0x0023EB0C
		public int NumMaterials
		{
			get
			{
				return this.materials.Length;
			}
		}

		// Token: 0x17000AA9 RID: 2729
		// (get) Token: 0x06006FC1 RID: 28609 RVA: 0x00240918 File Offset: 0x0023EB18
		public int KeyHash
		{
			get
			{
				int num = this._keyHash.GetValueOrDefault();
				if (this._keyHash == null)
				{
					num = this._cyclerKey.GetStaticHash();
					this._keyHash = new int?(num);
					return num;
				}
				return num;
			}
		}

		// Token: 0x06006FC2 RID: 28610 RVA: 0x00240959 File Offset: 0x0023EB59
		private void Awake()
		{
			this.SetMaterials();
			if (string.IsNullOrEmpty(this._cyclerKey))
			{
				throw new Exception("Must have a defined Cycler Key on all MaterialCyclers.");
			}
		}

		// Token: 0x06006FC3 RID: 28611 RVA: 0x00240979 File Offset: 0x0023EB79
		private void OnEnable()
		{
			if (string.IsNullOrEmpty(this._cyclerKey))
			{
				throw new Exception("Must have a defined Cycler Key on all MaterialCyclers.");
			}
			MaterialCyclerManager.Instance.RegisterCycler(this.KeyHash, this);
		}

		// Token: 0x06006FC4 RID: 28612 RVA: 0x002409A4 File Offset: 0x0023EBA4
		private void OnDisable()
		{
			MaterialCyclerManager.Instance.UnregisterCycler(this);
		}

		// Token: 0x06006FC5 RID: 28613 RVA: 0x002409B4 File Offset: 0x0023EBB4
		internal void MaterialCyclerNetworked_OnSynchronize(int idx, Color rgb)
		{
			if (idx < 0 || idx >= this.materials.Length)
			{
				return;
			}
			this.index = idx;
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.renderers[i].material = this.materials[this.index].Materials[i];
				this.renderers[i].material.SetColor(this.setColorTarget, rgb);
			}
			this.reset.Invoke(new Vector3(this.renderers[0].material.color.r, this.renderers[0].material.color.g, this.renderers[0].material.color.b));
		}

		// Token: 0x06006FC6 RID: 28614 RVA: 0x00240A7C File Offset: 0x0023EC7C
		public void SynchronizeLocal(float r, float g, float b)
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.renderers[i].material = this.materials[this.index].Materials[i];
				this.renderers[i].material.SetColor(this.setColorTarget, new Color(r, g, b));
			}
			this.reset.Invoke(new Vector3(this.renderers[0].material.color.r, this.renderers[0].material.color.g, this.renderers[0].material.color.b));
		}

		// Token: 0x06006FC7 RID: 28615 RVA: 0x00240B34 File Offset: 0x0023ED34
		private void SetMaterials()
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				if (this.materials[this.index].Materials.Length > i)
				{
					this.renderers[i].material = this.materials[this.index].Materials[i];
				}
				else
				{
					this.renderers[i].material = null;
				}
			}
			this.reset.Invoke(new Vector3(this.renderers[0].material.color.r, this.renderers[0].material.color.g, this.renderers[0].material.color.b));
		}

		// Token: 0x06006FC8 RID: 28616 RVA: 0x00240BF3 File Offset: 0x0023EDF3
		public void NextMaterial()
		{
			this.CycleMaterial((this.index + 1) % this.materials.Length);
		}

		// Token: 0x06006FC9 RID: 28617 RVA: 0x00240C0C File Offset: 0x0023EE0C
		internal void CycleMaterial(int newIndex)
		{
			this.index = newIndex;
			this.SetMaterials();
			this.SetDirty();
		}

		// Token: 0x06006FCA RID: 28618 RVA: 0x00240C21 File Offset: 0x0023EE21
		private void SetDirty()
		{
			this.synchTime = Time.time + MaterialCyclerManager.Instance.SyncTimeOut;
			if (this.crDirty == null)
			{
				this.crDirty = base.StartCoroutine(this.timeOutDirty());
			}
		}

		// Token: 0x06006FCB RID: 28619 RVA: 0x00240C53 File Offset: 0x0023EE53
		private IEnumerator timeOutDirty()
		{
			while (this.synchTime > Time.time)
			{
				yield return null;
			}
			this.synchronize();
			this.crDirty = null;
			yield break;
		}

		// Token: 0x06006FCC RID: 28620 RVA: 0x00240C62 File Offset: 0x0023EE62
		private void synchronize()
		{
			MaterialCyclerManager.Instance.Synchronize(this.KeyHash, this.index, this.renderers[0].material.color);
		}

		// Token: 0x06006FCD RID: 28621 RVA: 0x00240C8C File Offset: 0x0023EE8C
		public void SetColor(Vector3 rgb)
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.renderers[i].material.SetColor(this.setColorTarget, new Color(rgb.x, rgb.y, rgb.z));
			}
			this.SetDirty();
		}

		// Token: 0x04007FB6 RID: 32694
		[SerializeField]
		private string _cyclerKey;

		// Token: 0x04007FB7 RID: 32695
		[SerializeField]
		private MaterialCycler.MaterialPack[] materials;

		// Token: 0x04007FB8 RID: 32696
		[SerializeField]
		private Renderer[] renderers;

		// Token: 0x04007FBA RID: 32698
		[SerializeField]
		private string setColorTarget = "_BaseColor";

		// Token: 0x04007FBB RID: 32699
		[SerializeField]
		private UnityEvent<Vector3> reset;

		// Token: 0x04007FBC RID: 32700
		[SerializeField]
		private GrabbingColorPicker _colorPicker;

		// Token: 0x04007FBD RID: 32701
		private Coroutine crDirty;

		// Token: 0x04007FBE RID: 32702
		private float synchTime;

		// Token: 0x04007FBF RID: 32703
		private int? _keyHash;

		// Token: 0x0200116B RID: 4459
		[Serializable]
		private class MaterialPack
		{
			// Token: 0x17000AAA RID: 2730
			// (get) Token: 0x06006FCF RID: 28623 RVA: 0x00240CF4 File Offset: 0x0023EEF4
			public Material[] Materials
			{
				get
				{
					return this.materials;
				}
			}

			// Token: 0x04007FC0 RID: 32704
			[SerializeField]
			private Material[] materials;
		}
	}
}
