using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Fusion;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000912 RID: 2322
[NetworkBehaviourWeaved(4)]
public class MagicCauldron : NetworkComponent
{
	// Token: 0x06003CD4 RID: 15572 RVA: 0x0014B5DC File Offset: 0x001497DC
	private new void Awake()
	{
		this.currentIngredients.Clear();
		this.witchesComponent.Clear();
		this.currentStateElapsedTime = 0f;
		this.currentRecipeIndex = -1;
		this.ingredientIndex = -1;
		if (this.flyingWitchesContainer != null)
		{
			for (int i = 0; i < this.flyingWitchesContainer.transform.childCount; i++)
			{
				NoncontrollableBroomstick componentInChildren = this.flyingWitchesContainer.transform.GetChild(i).gameObject.GetComponentInChildren<NoncontrollableBroomstick>();
				this.witchesComponent.Add(componentInChildren);
				if (componentInChildren)
				{
					componentInChildren.gameObject.SetActive(false);
				}
			}
		}
		if (this.reusableFXContext == null)
		{
			this.reusableFXContext = new MagicCauldron.IngrediantFXContext();
		}
		if (this.reusableIngrediantArgs == null)
		{
			this.reusableIngrediantArgs = new MagicCauldron.IngredientArgs();
		}
		this.reusableFXContext.fxCallBack = new MagicCauldron.IngrediantFXContext.Callback(this.OnIngredientAdd);
	}

	// Token: 0x06003CD5 RID: 15573 RVA: 0x0014B6BA File Offset: 0x001498BA
	private new void Start()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06003CD6 RID: 15574 RVA: 0x0014B6C3 File Offset: 0x001498C3
	private void LateUpdate()
	{
		this.UpdateState();
	}

	// Token: 0x06003CD7 RID: 15575 RVA: 0x0014B6CB File Offset: 0x001498CB
	private IEnumerator LevitationSpellCoroutine()
	{
		GTPlayer.Instance.SetHalloweenLevitation(this.levitationStrength, this.levitationDuration, this.levitationBlendOutDuration, this.levitationBonusStrength, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield return new WaitForSeconds(this.levitationSpellDuration);
		GTPlayer.Instance.SetHalloweenLevitation(0f, this.levitationDuration, this.levitationBlendOutDuration, 0f, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield break;
	}

	// Token: 0x06003CD8 RID: 15576 RVA: 0x0014B6DC File Offset: 0x001498DC
	private void ChangeState(MagicCauldron.CauldronState state)
	{
		this.currentState = state;
		if (base.IsMine)
		{
			this.currentStateElapsedTime = 0f;
		}
		bool flag = state == MagicCauldron.CauldronState.summoned;
		foreach (NoncontrollableBroomstick noncontrollableBroomstick in this.witchesComponent)
		{
			if (noncontrollableBroomstick.gameObject.activeSelf != flag)
			{
				noncontrollableBroomstick.gameObject.SetActive(flag);
			}
		}
		if (this.currentState == MagicCauldron.CauldronState.summoned && Vector3.Distance(GTPlayer.Instance.transform.position, base.transform.position) < this.levitationRadius)
		{
			base.StartCoroutine(this.LevitationSpellCoroutine());
		}
		switch (this.currentState)
		{
		case MagicCauldron.CauldronState.notReady:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronNotReadyColor);
			return;
		case MagicCauldron.CauldronState.ready:
			this.UpdateCauldronColor(this.CauldronActiveColor);
			return;
		case MagicCauldron.CauldronState.recipeCollecting:
			if (this.ingredientIndex >= 0 && this.ingredientIndex < this.allIngredients.Length)
			{
				this.UpdateCauldronColor(this.allIngredients[this.ingredientIndex].color);
				return;
			}
			break;
		case MagicCauldron.CauldronState.recipeActivated:
			if (this.audioSource && this.recipes[this.currentRecipeIndex].successAudio)
			{
				this.audioSource.GTPlayOneShot(this.recipes[this.currentRecipeIndex].successAudio, 1f);
			}
			if (this.successParticle)
			{
				this.successParticle.Play();
				return;
			}
			break;
		case MagicCauldron.CauldronState.summoned:
			break;
		case MagicCauldron.CauldronState.failed:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			this.audioSource.GTPlayOneShot(this.recipeFailedAudio, 1f);
			return;
		case MagicCauldron.CauldronState.cooldown:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			break;
		default:
			return;
		}
	}

	// Token: 0x06003CD9 RID: 15577 RVA: 0x0014B8D4 File Offset: 0x00149AD4
	private void UpdateState()
	{
		if (base.IsMine)
		{
			this.currentStateElapsedTime += Time.deltaTime;
			switch (this.currentState)
			{
			case MagicCauldron.CauldronState.notReady:
			case MagicCauldron.CauldronState.ready:
				break;
			case MagicCauldron.CauldronState.recipeCollecting:
				if (this.currentStateElapsedTime >= this.maxTimeToAddAllIngredients && !this.CheckIngredients())
				{
					this.ChangeState(MagicCauldron.CauldronState.failed);
					return;
				}
				break;
			case MagicCauldron.CauldronState.recipeActivated:
				if (this.currentStateElapsedTime >= this.waitTimeToSummonWitches)
				{
					this.ChangeState(MagicCauldron.CauldronState.summoned);
					return;
				}
				break;
			case MagicCauldron.CauldronState.summoned:
				if (this.currentStateElapsedTime >= this.summonWitchesDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.cooldown);
					return;
				}
				break;
			case MagicCauldron.CauldronState.failed:
				if (this.currentStateElapsedTime >= this.recipeFailedDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
					return;
				}
				break;
			case MagicCauldron.CauldronState.cooldown:
				if (this.currentStateElapsedTime >= this.cooldownDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
				}
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06003CDA RID: 15578 RVA: 0x0014B99D File Offset: 0x00149B9D
	public void OnEventStart()
	{
		this.ChangeState(MagicCauldron.CauldronState.ready);
	}

	// Token: 0x06003CDB RID: 15579 RVA: 0x0014B6BA File Offset: 0x001498BA
	public void OnEventEnd()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06003CDC RID: 15580 RVA: 0x0014B9A6 File Offset: 0x00149BA6
	[PunRPC]
	public void OnIngredientAdd(int _ingredientIndex, PhotonMessageInfo info)
	{
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x06003CDD RID: 15581 RVA: 0x0014B9B8 File Offset: 0x00149BB8
	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RPC_OnIngredientAdd(int _ingredientIndex, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 1) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void MagicCauldron::RPC_OnIngredientAdd(System.Int32,Fusion.RpcInfo)", base.Object, 1);
				}
				else
				{
					int num = 8;
					num += 4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void MagicCauldron::RPC_OnIngredientAdd(System.Int32,Fusion.RpcInfo)", num);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
							int num2 = 8;
							*(int*)(ptr2 + num2) = _ingredientIndex;
							num2 += 4;
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0012;
						}
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x06003CDE RID: 15582 RVA: 0x0014BB1C File Offset: 0x00149D1C
	private void OnIngredientAddShared(int _ingredientIndex, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "OnIngredientAdd");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.reusableFXContext.playerSettings = rigContainer.Rig.fxSettings;
		this.reusableIngrediantArgs.key = _ingredientIndex;
		FXSystem.PlayFX<MagicCauldron.IngredientArgs>(FXType.HWIngredients, this.reusableFXContext, this.reusableIngrediantArgs, info);
	}

	// Token: 0x06003CDF RID: 15583 RVA: 0x0014BB80 File Offset: 0x00149D80
	private void OnIngredientAdd(int _ingredientIndex)
	{
		if (this.audioSource)
		{
			this.audioSource.GTPlayOneShot(this.ingredientAddedAudio, 1f);
		}
		if (!RoomSystem.AmITheHost)
		{
			return;
		}
		if (_ingredientIndex < 0 || _ingredientIndex >= this.allIngredients.Length || (this.currentState != MagicCauldron.CauldronState.ready && this.currentState != MagicCauldron.CauldronState.recipeCollecting))
		{
			return;
		}
		MagicIngredientType magicIngredientType = this.allIngredients[_ingredientIndex];
		Debug.Log(string.Format("Received ingredient RPC {0} = {1}", _ingredientIndex, magicIngredientType));
		MagicIngredientType magicIngredientType2 = null;
		if (this.recipes[0].recipeIngredients.Count > this.currentIngredients.Count)
		{
			magicIngredientType2 = this.recipes[0].recipeIngredients[this.currentIngredients.Count];
		}
		if (!(magicIngredientType == magicIngredientType2))
		{
			Debug.Log(string.Format("Failure: Expected ingredient {0}, got {1} from recipe[{2}]", magicIngredientType2, magicIngredientType, this.currentIngredients.Count));
			this.ChangeState(MagicCauldron.CauldronState.failed);
			return;
		}
		this.ingredientIndex = _ingredientIndex;
		this.currentIngredients.Add(magicIngredientType);
		if (this.CheckIngredients())
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeActivated);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.ready)
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeCollecting);
			return;
		}
		this.UpdateCauldronColor(magicIngredientType.color);
	}

	// Token: 0x06003CE0 RID: 15584 RVA: 0x0014BCB4 File Offset: 0x00149EB4
	private bool CheckIngredients()
	{
		foreach (MagicCauldron.Recipe recipe in this.recipes)
		{
			if (this.currentIngredients.SequenceEqual(recipe.recipeIngredients))
			{
				this.currentRecipeIndex = this.recipes.IndexOf(recipe);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003CE1 RID: 15585 RVA: 0x0014BD2C File Offset: 0x00149F2C
	private void UpdateCauldronColor(Color color)
	{
		if (this.bubblesParticle)
		{
			if (this.bubblesParticle.isPlaying)
			{
				if (this.currentState == MagicCauldron.CauldronState.failed || this.currentState == MagicCauldron.CauldronState.notReady)
				{
					this.bubblesParticle.Stop();
				}
			}
			else
			{
				this.bubblesParticle.Play();
			}
		}
		this.currentColor = this.cauldronColor;
		if (this.currentColor == color)
		{
			return;
		}
		if (this.rendr)
		{
			this._liquid.AnimateColorFromTo(this.cauldronColor, color, 1f);
			this.cauldronColor = color;
		}
		if (this.bubblesParticle)
		{
			this.bubblesParticle.main.startColor = color;
		}
	}

	// Token: 0x06003CE2 RID: 15586 RVA: 0x0014BDE8 File Offset: 0x00149FE8
	private void OnTriggerEnter(Collider other)
	{
		ThrowableSetDressing componentInParent = other.GetComponentInParent<ThrowableSetDressing>();
		if (componentInParent == null || componentInParent.IngredientTypeSO == null || componentInParent.InHand())
		{
			return;
		}
		if (componentInParent.IsLocalOwnedWorldShareable)
		{
			if (componentInParent.IngredientTypeSO != null && (this.currentState == MagicCauldron.CauldronState.ready || this.currentState == MagicCauldron.CauldronState.recipeCollecting))
			{
				int num = this.allIngredients.IndexOfRef(componentInParent.IngredientTypeSO);
				Debug.Log(string.Format("Sending ingredient RPC {0} = {1}", componentInParent.IngredientTypeSO, num));
				base.SendRPC("OnIngredientAdd", RpcTarget.Others, new object[] { num });
				this.OnIngredientAdd(num);
			}
			componentInParent.StartRespawnTimer(0f);
		}
		if (componentInParent.IngredientTypeSO != null && this.splashParticle)
		{
			this.splashParticle.Play();
		}
	}

	// Token: 0x06003CE3 RID: 15587 RVA: 0x0014BEC4 File Offset: 0x0014A0C4
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		this.currentIngredients.Clear();
	}

	// Token: 0x17000590 RID: 1424
	// (get) Token: 0x06003CE4 RID: 15588 RVA: 0x0014BEDD File Offset: 0x0014A0DD
	// (set) Token: 0x06003CE5 RID: 15589 RVA: 0x0014BF07 File Offset: 0x0014A107
	[Networked]
	[NetworkedWeaved(0, 4)]
	private unsafe MagicCauldron.MagicCauldronData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MagicCauldron.MagicCauldronData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MagicCauldron.MagicCauldronData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003CE6 RID: 15590 RVA: 0x0014BF32 File Offset: 0x0014A132
	public override void WriteDataFusion()
	{
		this.Data = new MagicCauldron.MagicCauldronData(this.currentStateElapsedTime, this.currentRecipeIndex, this.currentState, this.ingredientIndex);
	}

	// Token: 0x06003CE7 RID: 15591 RVA: 0x0014BF58 File Offset: 0x0014A158
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.CurrentStateElapsedTime, this.Data.CurrentRecipeIndex, this.Data.CurrentState, this.Data.IngredientIndex);
	}

	// Token: 0x06003CE8 RID: 15592 RVA: 0x0014BFA4 File Offset: 0x0014A1A4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.currentStateElapsedTime);
		stream.SendNext(this.currentRecipeIndex);
		stream.SendNext(this.currentState);
		stream.SendNext(this.ingredientIndex);
	}

	// Token: 0x06003CE9 RID: 15593 RVA: 0x0014C004 File Offset: 0x0014A204
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float num = (float)stream.ReceiveNext();
		int num2 = (int)stream.ReceiveNext();
		MagicCauldron.CauldronState cauldronState = (MagicCauldron.CauldronState)stream.ReceiveNext();
		int num3 = (int)stream.ReceiveNext();
		this.ReadDataShared(num, num2, cauldronState, num3);
	}

	// Token: 0x06003CEA RID: 15594 RVA: 0x0014C05C File Offset: 0x0014A25C
	private void ReadDataShared(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
	{
		MagicCauldron.CauldronState cauldronState = this.currentState;
		this.currentStateElapsedTime = stateElapsedTime;
		this.currentRecipeIndex = recipeIndex;
		this.currentState = state;
		this.ingredientIndex = ingredientIndex;
		if (cauldronState != this.currentState)
		{
			this.ChangeState(this.currentState);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.recipeCollecting && ingredientIndex != ingredientIndex && ingredientIndex >= 0 && ingredientIndex < this.allIngredients.Length)
		{
			this.UpdateCauldronColor(this.allIngredients[ingredientIndex].color);
		}
	}

	// Token: 0x06003CEC RID: 15596 RVA: 0x0014C159 File Offset: 0x0014A359
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06003CED RID: 15597 RVA: 0x0014C171 File Offset: 0x0014A371
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x06003CEE RID: 15598 RVA: 0x0014C188 File Offset: 0x0014A388
	[NetworkRpcWeavedInvoker(1, 1, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnIngredientAdd@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int num3 = num2;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((MagicCauldron)behaviour).RPC_OnIngredientAdd(num3, rpcInfo);
	}

	// Token: 0x04004D7E RID: 19838
	public List<MagicCauldron.Recipe> recipes = new List<MagicCauldron.Recipe>();

	// Token: 0x04004D7F RID: 19839
	public float maxTimeToAddAllIngredients = 30f;

	// Token: 0x04004D80 RID: 19840
	public float summonWitchesDuration = 20f;

	// Token: 0x04004D81 RID: 19841
	public float recipeFailedDuration = 5f;

	// Token: 0x04004D82 RID: 19842
	public float cooldownDuration = 30f;

	// Token: 0x04004D83 RID: 19843
	public MagicIngredientType[] allIngredients;

	// Token: 0x04004D84 RID: 19844
	public GameObject flyingWitchesContainer;

	// Token: 0x04004D85 RID: 19845
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004D86 RID: 19846
	public AudioClip ingredientAddedAudio;

	// Token: 0x04004D87 RID: 19847
	public AudioClip recipeFailedAudio;

	// Token: 0x04004D88 RID: 19848
	public ParticleSystem bubblesParticle;

	// Token: 0x04004D89 RID: 19849
	public ParticleSystem successParticle;

	// Token: 0x04004D8A RID: 19850
	public ParticleSystem splashParticle;

	// Token: 0x04004D8B RID: 19851
	public Color CauldronActiveColor;

	// Token: 0x04004D8C RID: 19852
	public Color CauldronFailedColor;

	// Token: 0x04004D8D RID: 19853
	[Tooltip("only if we are using the time of day event")]
	public Color CauldronNotReadyColor;

	// Token: 0x04004D8E RID: 19854
	private readonly List<NoncontrollableBroomstick> witchesComponent = new List<NoncontrollableBroomstick>();

	// Token: 0x04004D8F RID: 19855
	private readonly List<MagicIngredientType> currentIngredients = new List<MagicIngredientType>();

	// Token: 0x04004D90 RID: 19856
	private float currentStateElapsedTime;

	// Token: 0x04004D91 RID: 19857
	private MagicCauldron.CauldronState currentState;

	// Token: 0x04004D92 RID: 19858
	[SerializeField]
	private Renderer rendr;

	// Token: 0x04004D93 RID: 19859
	private Color cauldronColor;

	// Token: 0x04004D94 RID: 19860
	private Color currentColor;

	// Token: 0x04004D95 RID: 19861
	private int currentRecipeIndex;

	// Token: 0x04004D96 RID: 19862
	private int ingredientIndex;

	// Token: 0x04004D97 RID: 19863
	private float waitTimeToSummonWitches = 2f;

	// Token: 0x04004D98 RID: 19864
	[Space]
	[SerializeField]
	private MagicCauldronLiquid _liquid;

	// Token: 0x04004D99 RID: 19865
	private MagicCauldron.IngrediantFXContext reusableFXContext = new MagicCauldron.IngrediantFXContext();

	// Token: 0x04004D9A RID: 19866
	private MagicCauldron.IngredientArgs reusableIngrediantArgs = new MagicCauldron.IngredientArgs();

	// Token: 0x04004D9B RID: 19867
	public bool testLevitationAlwaysOn;

	// Token: 0x04004D9C RID: 19868
	public float levitationRadius;

	// Token: 0x04004D9D RID: 19869
	public float levitationSpellDuration;

	// Token: 0x04004D9E RID: 19870
	public float levitationStrength;

	// Token: 0x04004D9F RID: 19871
	public float levitationDuration;

	// Token: 0x04004DA0 RID: 19872
	public float levitationBlendOutDuration;

	// Token: 0x04004DA1 RID: 19873
	public float levitationBonusStrength;

	// Token: 0x04004DA2 RID: 19874
	public float levitationBonusOffAtYSpeed;

	// Token: 0x04004DA3 RID: 19875
	public float levitationBonusFullAtYSpeed;

	// Token: 0x04004DA4 RID: 19876
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 4)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private MagicCauldron.MagicCauldronData _Data;

	// Token: 0x02000913 RID: 2323
	private enum CauldronState
	{
		// Token: 0x04004DA6 RID: 19878
		notReady,
		// Token: 0x04004DA7 RID: 19879
		ready,
		// Token: 0x04004DA8 RID: 19880
		recipeCollecting,
		// Token: 0x04004DA9 RID: 19881
		recipeActivated,
		// Token: 0x04004DAA RID: 19882
		summoned,
		// Token: 0x04004DAB RID: 19883
		failed,
		// Token: 0x04004DAC RID: 19884
		cooldown
	}

	// Token: 0x02000914 RID: 2324
	[Serializable]
	public struct Recipe
	{
		// Token: 0x04004DAD RID: 19885
		public List<MagicIngredientType> recipeIngredients;

		// Token: 0x04004DAE RID: 19886
		public AudioClip successAudio;
	}

	// Token: 0x02000915 RID: 2325
	private class IngredientArgs : FXSArgs
	{
		// Token: 0x04004DAF RID: 19887
		public int key;
	}

	// Token: 0x02000916 RID: 2326
	private class IngrediantFXContext : IFXContextParems<MagicCauldron.IngredientArgs>
	{
		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06003CF0 RID: 15600 RVA: 0x0014C1E8 File Offset: 0x0014A3E8
		FXSystemSettings IFXContextParems<MagicCauldron.IngredientArgs>.settings
		{
			get
			{
				return this.playerSettings;
			}
		}

		// Token: 0x06003CF1 RID: 15601 RVA: 0x0014C1F0 File Offset: 0x0014A3F0
		void IFXContextParems<MagicCauldron.IngredientArgs>.OnPlayFX(MagicCauldron.IngredientArgs args)
		{
			this.fxCallBack(args.key);
		}

		// Token: 0x04004DB0 RID: 19888
		public FXSystemSettings playerSettings;

		// Token: 0x04004DB1 RID: 19889
		public MagicCauldron.IngrediantFXContext.Callback fxCallBack;

		// Token: 0x02000917 RID: 2327
		// (Invoke) Token: 0x06003CF4 RID: 15604
		public delegate void Callback(int key);
	}

	// Token: 0x02000918 RID: 2328
	[NetworkStructWeaved(4)]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	private struct MagicCauldronData : INetworkStruct
	{
		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06003CF7 RID: 15607 RVA: 0x0014C203 File Offset: 0x0014A403
		// (set) Token: 0x06003CF8 RID: 15608 RVA: 0x0014C20B File Offset: 0x0014A40B
		public float CurrentStateElapsedTime { readonly get; set; }

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x06003CF9 RID: 15609 RVA: 0x0014C214 File Offset: 0x0014A414
		// (set) Token: 0x06003CFA RID: 15610 RVA: 0x0014C21C File Offset: 0x0014A41C
		public int CurrentRecipeIndex { readonly get; set; }

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06003CFB RID: 15611 RVA: 0x0014C225 File Offset: 0x0014A425
		// (set) Token: 0x06003CFC RID: 15612 RVA: 0x0014C22D File Offset: 0x0014A42D
		public MagicCauldron.CauldronState CurrentState { readonly get; set; }

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06003CFD RID: 15613 RVA: 0x0014C236 File Offset: 0x0014A436
		// (set) Token: 0x06003CFE RID: 15614 RVA: 0x0014C23E File Offset: 0x0014A43E
		public int IngredientIndex { readonly get; set; }

		// Token: 0x06003CFF RID: 15615 RVA: 0x0014C247 File Offset: 0x0014A447
		public MagicCauldronData(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
		{
			this.CurrentStateElapsedTime = stateElapsedTime;
			this.CurrentRecipeIndex = recipeIndex;
			this.CurrentState = state;
			this.IngredientIndex = ingredientIndex;
		}
	}
}
