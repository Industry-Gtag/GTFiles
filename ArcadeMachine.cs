using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000423 RID: 1059
[NetworkBehaviourWeaved(128)]
public class ArcadeMachine : NetworkComponent
{
	// Token: 0x06001928 RID: 6440 RVA: 0x0008E3C4 File Offset: 0x0008C5C4
	protected override void Awake()
	{
		base.Awake();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06001929 RID: 6441 RVA: 0x0008E3D8 File Offset: 0x0008C5D8
	protected override void Start()
	{
		base.Start();
		if (this.arcadeGame != null && this.arcadeGame.Scale.x > 0f && this.arcadeGame.Scale.y > 0f)
		{
			this.arcadeGameInstance = global::UnityEngine.Object.Instantiate<ArcadeGame>(this.arcadeGame, this.screen.transform);
			this.arcadeGameInstance.transform.localScale = new Vector3(1f / this.arcadeGameInstance.Scale.x, 1f / this.arcadeGameInstance.Scale.y, 1f);
			this.screen.forceRenderingOff = true;
			this.arcadeGameInstance.SetMachine(this);
		}
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x0008E4A8 File Offset: 0x0008C6A8
	public void PlaySound(int soundId, int priority)
	{
		if (!this.audioSource.isPlaying || this.audioSourcePriority >= priority)
		{
			this.audioSource.GTStop();
			this.audioSourcePriority = priority;
			this.audioSource.clip = this.arcadeGameInstance.audioClips[soundId];
			this.audioSource.GTPlay();
			if (this.networkSynchronized && base.IsMine)
			{
				base.GetView.RPC("ArcadeGameInstance_OnPlaySound_RPC", RpcTarget.Others, new object[] { soundId });
			}
		}
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x0008E530 File Offset: 0x0008C730
	public bool IsPlayerLocallyControlled(int player)
	{
		return this.sticks[player].heldByLocalPlayer;
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x0008E540 File Offset: 0x0008C740
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		for (int i = 0; i < this.sticks.Length; i++)
		{
			this.sticks[i].Init(this, i);
		}
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x0008E57B File Offset: 0x0008C77B
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x0008E58C File Offset: 0x0008C78C
	[PunRPC]
	private void ArcadeGameInstance_OnPlaySound_RPC(int id, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || id > this.arcadeGameInstance.audioClips.Length || id < 0 || !this.soundCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.audioSource.GTStop();
		this.audioSource.clip = this.arcadeGameInstance.audioClips[id];
		this.audioSource.GTPlay();
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x0008E5FB File Offset: 0x0008C7FB
	public void OnJoystickStateChange(int player, ArcadeButtons buttons)
	{
		if (this.arcadeGameInstance != null)
		{
			this.arcadeGameInstance.OnInputStateChange(player, buttons);
		}
	}

	// Token: 0x06001930 RID: 6448 RVA: 0x0008E614 File Offset: 0x0008C814
	public bool IsControllerInUse(int player)
	{
		if (base.IsMine)
		{
			return this.playersPerJoystick[player] != null && Time.time < this.playerIdleTimeouts[player];
		}
		return (this.buttonsStateValue & (1 << player * 8)) != 0;
	}

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x06001931 RID: 6449 RVA: 0x0008E64C File Offset: 0x0008C84C
	[Networked]
	[Capacity(128)]
	[NetworkedWeaved(0, 128)]
	[NetworkedWeavedArray(128, 1, typeof(ElementReaderWriterByte))]
	public unsafe NetworkArray<byte> Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ArcadeMachine.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<byte>((byte*)(this.Ptr + 0), 128, ElementReaderWriterByte.GetInstance());
		}
	}

	// Token: 0x06001932 RID: 6450 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x0008E68C File Offset: 0x0008C88C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x0008E699 File Offset: 0x0008C899
	public void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.ReadPlayerDataPUN(player, stream, info);
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x0008E6A9 File Offset: 0x0008C8A9
	public void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.WritePlayerDataPUN(player, stream, info);
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x0008E6E0 File Offset: 0x0008C8E0
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		NetworkBehaviourUtils.InitializeNetworkArray<byte>(this.Data, this._Data, "Data");
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x0008E702 File Offset: 0x0008C902
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		NetworkBehaviourUtils.CopyFromNetworkArray<byte>(this.Data, ref this._Data);
	}

	// Token: 0x0400244C RID: 9292
	[SerializeField]
	private ArcadeGame arcadeGame;

	// Token: 0x0400244D RID: 9293
	[SerializeField]
	private ArcadeMachineJoystick[] sticks;

	// Token: 0x0400244E RID: 9294
	[SerializeField]
	private Renderer screen;

	// Token: 0x0400244F RID: 9295
	[SerializeField]
	private bool networkSynchronized = true;

	// Token: 0x04002450 RID: 9296
	[SerializeField]
	private CallLimiter soundCallLimit;

	// Token: 0x04002451 RID: 9297
	private int buttonsStateValue;

	// Token: 0x04002452 RID: 9298
	private AudioSource audioSource;

	// Token: 0x04002453 RID: 9299
	private int audioSourcePriority;

	// Token: 0x04002454 RID: 9300
	private ArcadeGame arcadeGameInstance;

	// Token: 0x04002455 RID: 9301
	private Player[] playersPerJoystick = new Player[4];

	// Token: 0x04002456 RID: 9302
	private float[] playerIdleTimeouts = new float[4];

	// Token: 0x04002457 RID: 9303
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 128)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private byte[] _Data;
}
