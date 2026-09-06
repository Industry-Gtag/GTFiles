using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F5E RID: 3934
	public class BuilderRecycler : MonoBehaviour
	{
		// Token: 0x060060D1 RID: 24785 RVA: 0x001EB614 File Offset: 0x001E9814
		private void Awake()
		{
			this.hasFans = this.effectBehaviors.Count > 0 && this.bladeSoundPlayer != null && this.recycleParticles != null;
			this.hasPipes = this.outputPipes.Count > 0;
		}

		// Token: 0x060060D2 RID: 24786 RVA: 0x001EB668 File Offset: 0x001E9868
		private void Start()
		{
			if (this.hasPipes)
			{
				this.numPipes = Mathf.Min(this.outputPipes.Count, 3);
				this.props = new MaterialPropertyBlock();
				this.ResetOutputPipes();
				this.totalRecycledCost = new int[3];
				this.currentChainCost = new int[3];
				for (int i = 0; i < this.totalRecycledCost.Length; i++)
				{
					this.totalRecycledCost[i] = 0;
					this.currentChainCost[i] = 0;
				}
			}
			this.zoneRenderers.Clear();
			if (this.hasPipes)
			{
				this.zoneRenderers.AddRange(this.outputPipes);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					Renderer component = monoBehaviour.GetComponent<Renderer>();
					if (component != null)
					{
						this.zoneRenderers.Add(component);
					}
				}
			}
			this.inBuilderZone = true;
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x060060D3 RID: 24787 RVA: 0x001EB79C File Offset: 0x001E999C
		private void OnDestroy()
		{
			if (ZoneManagement.instance != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x060060D4 RID: 24788 RVA: 0x001EB7D4 File Offset: 0x001E99D4
		private void OnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
			if (flag && !this.inBuilderZone)
			{
				using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Renderer renderer = enumerator.Current;
						renderer.enabled = true;
					}
					goto IL_008B;
				}
			}
			if (!flag && this.inBuilderZone)
			{
				foreach (Renderer renderer2 in this.zoneRenderers)
				{
					renderer2.enabled = false;
				}
			}
			IL_008B:
			this.inBuilderZone = flag;
		}

		// Token: 0x060060D5 RID: 24789 RVA: 0x001EB890 File Offset: 0x001E9A90
		private void OnTriggerEnter(Collider other)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(other);
			if (builderPieceFromCollider == null)
			{
				return;
			}
			if (!builderPieceFromCollider.isBuiltIntoTable && !builderPieceFromCollider.isArmShelf)
			{
				this.table.RequestRecyclePiece(builderPieceFromCollider, true, this.recyclerID);
			}
		}

		// Token: 0x060060D6 RID: 24790 RVA: 0x001EB8D4 File Offset: 0x001E9AD4
		public void OnRecycleRequestedAtRecycler(BuilderPiece piece)
		{
			if (this.hasPipes)
			{
				this.AddPieceCost(piece.cost);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					monoBehaviour.enabled = true;
				}
				this.recycleParticles.SetActive(true);
				this.bladeSoundPlayer.Play();
				this.timeToStopBlades = (double)(Time.time + this.recycleEffectDuration);
				this.playingBladeEffect = true;
			}
		}

		// Token: 0x060060D7 RID: 24791 RVA: 0x001EB974 File Offset: 0x001E9B74
		private void AddPieceCost(BuilderResources cost)
		{
			foreach (BuilderResourceQuantity builderResourceQuantity in cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					this.totalRecycledCost[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			if (!this.playingPipeEffect)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x060060D8 RID: 24792 RVA: 0x001EB9FC File Offset: 0x001E9BFC
		private Vector2 GetUVShiftOffset()
		{
			float y = Shader.GetGlobalVector(ShaderProps._Time).y;
			Vector4 vector = new Vector4(500f, 0f, 0f, 0f);
			Vector4 vector2 = vector / this.recycleEffectDuration;
			return new Vector2(-1f * (Mathf.Floor(y * vector2.x) * 1f / vector.x % 1f) * vector.x - vector.x + 165f, 0f);
		}

		// Token: 0x060060D9 RID: 24793 RVA: 0x001EBA88 File Offset: 0x001E9C88
		private void UpdatePipeLoop()
		{
			bool flag = false;
			for (int i = 0; i < this.numPipes; i++)
			{
				if (this.totalRecycledCost[i] > 0)
				{
					flag = true;
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					Vector4 vector = new Vector4(500f, 0f, 0f, 0f) / this.recycleEffectDuration;
					Vector2 uvshiftOffset = this.GetUVShiftOffset();
					this.props.SetColor(ShaderProps._BaseColor, this.builderResourceColors.colors[i].color);
					this.props.SetVector(ShaderProps._UvShiftRate, vector);
					this.props.SetVector(ShaderProps._UvShiftOffset, uvshiftOffset);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
					this.totalRecycledCost[i] = Mathf.Max(this.totalRecycledCost[i] - 1, 0);
				}
				else
				{
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					this.props.SetColor(ShaderProps._BaseColor, Color.black);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
				}
			}
			if (flag)
			{
				this.playingPipeEffect = true;
				this.timeToCheckPipes = (double)(Time.time + this.recycleEffectDuration);
				return;
			}
			this.playingPipeEffect = false;
		}

		// Token: 0x060060DA RID: 24794 RVA: 0x001EBBEC File Offset: 0x001E9DEC
		private void ResetOutputPipes()
		{
			foreach (MeshRenderer meshRenderer in this.outputPipes)
			{
				meshRenderer.GetPropertyBlock(this.props, 1);
				this.props.SetColor(ShaderProps._BaseColor, Color.black);
				meshRenderer.SetPropertyBlock(this.props, 1);
			}
		}

		// Token: 0x060060DB RID: 24795 RVA: 0x001EBC68 File Offset: 0x001E9E68
		public void UpdateRecycler()
		{
			if (this.playingBladeEffect && (double)Time.time > this.timeToStopBlades)
			{
				if (this.hasFans)
				{
					foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
					{
						monoBehaviour.enabled = false;
					}
					this.recycleParticles.SetActive(false);
				}
				this.playingBladeEffect = false;
			}
			if (this.playingPipeEffect && (double)Time.time > this.timeToCheckPipes)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x04006F69 RID: 28521
		public float recycleEffectDuration = 0.25f;

		// Token: 0x04006F6A RID: 28522
		private double timeToStopBlades = double.MinValue;

		// Token: 0x04006F6B RID: 28523
		private bool playingBladeEffect;

		// Token: 0x04006F6C RID: 28524
		private bool playingPipeEffect;

		// Token: 0x04006F6D RID: 28525
		private double timeToCheckPipes = double.MinValue;

		// Token: 0x04006F6E RID: 28526
		public List<MonoBehaviour> effectBehaviors;

		// Token: 0x04006F6F RID: 28527
		public GameObject recycleParticles;

		// Token: 0x04006F70 RID: 28528
		public SoundBankPlayer bladeSoundPlayer;

		// Token: 0x04006F71 RID: 28529
		public List<MeshRenderer> outputPipes;

		// Token: 0x04006F72 RID: 28530
		public BuilderResourceColors builderResourceColors;

		// Token: 0x04006F73 RID: 28531
		private bool hasFans;

		// Token: 0x04006F74 RID: 28532
		private bool hasPipes;

		// Token: 0x04006F75 RID: 28533
		private MaterialPropertyBlock props;

		// Token: 0x04006F76 RID: 28534
		private int[] totalRecycledCost;

		// Token: 0x04006F77 RID: 28535
		private int[] currentChainCost;

		// Token: 0x04006F78 RID: 28536
		private int numPipes;

		// Token: 0x04006F79 RID: 28537
		internal int recyclerID = -1;

		// Token: 0x04006F7A RID: 28538
		internal BuilderTable table;

		// Token: 0x04006F7B RID: 28539
		private List<Renderer> zoneRenderers = new List<Renderer>(10);

		// Token: 0x04006F7C RID: 28540
		private bool inBuilderZone;
	}
}
