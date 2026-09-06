using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000422 RID: 1058
public abstract class ArcadeGame : MonoBehaviour
{
	// Token: 0x06001915 RID: 6421 RVA: 0x0008E1CF File Offset: 0x0008C3CF
	protected virtual void Awake()
	{
		this.InitializeMemoryStreams();
	}

	// Token: 0x06001916 RID: 6422 RVA: 0x0008E1D7 File Offset: 0x0008C3D7
	public void InitializeMemoryStreams()
	{
		if (!this.memoryStreamsInitialized)
		{
			this.netStateMemStream = new MemoryStream(this.netStateBuffer, true);
			this.netStateMemStreamAlt = new MemoryStream(this.netStateBufferAlt, true);
			this.memoryStreamsInitialized = true;
		}
	}

	// Token: 0x06001917 RID: 6423 RVA: 0x0008E20C File Offset: 0x0008C40C
	public void SetMachine(ArcadeMachine machine)
	{
		this.machine = machine;
	}

	// Token: 0x06001918 RID: 6424 RVA: 0x0008E215 File Offset: 0x0008C415
	protected bool getButtonState(int player, ArcadeButtons button)
	{
		return this.playerInputs[player].HasFlag(button);
	}

	// Token: 0x06001919 RID: 6425 RVA: 0x0008E230 File Offset: 0x0008C430
	public void OnInputStateChange(int player, ArcadeButtons buttons)
	{
		for (int i = 1; i < 256; i += i)
		{
			ArcadeButtons arcadeButtons = (ArcadeButtons)i;
			bool flag = buttons.HasFlag(arcadeButtons);
			bool flag2 = this.playerInputs[player].HasFlag(arcadeButtons);
			if (flag != flag2)
			{
				if (flag)
				{
					this.ButtonDown(player, arcadeButtons);
				}
				else
				{
					this.ButtonUp(player, arcadeButtons);
				}
			}
		}
		this.playerInputs[player] = buttons;
	}

	// Token: 0x0600191A RID: 6426
	public abstract byte[] GetNetworkState();

	// Token: 0x0600191B RID: 6427
	public abstract void SetNetworkState(byte[] obj);

	// Token: 0x0600191C RID: 6428 RVA: 0x0008E29C File Offset: 0x0008C49C
	protected static void WrapNetState(object ns, MemoryStream stream)
	{
		if (stream == null)
		{
			Debug.LogWarning("Null MemoryStream passed to WrapNetState");
			return;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		stream.SetLength(0L);
		stream.Position = 0L;
		binaryFormatter.Serialize(stream, ns);
	}

	// Token: 0x0600191D RID: 6429 RVA: 0x0008E2C8 File Offset: 0x0008C4C8
	protected static object UnwrapNetState(byte[] b)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(b);
		memoryStream.Position = 0L;
		object obj = binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		return obj;
	}

	// Token: 0x0600191E RID: 6430 RVA: 0x0008E300 File Offset: 0x0008C500
	protected void SwapNetStateBuffersAndStreams()
	{
		byte[] array = this.netStateBufferAlt;
		byte[] array2 = this.netStateBuffer;
		this.netStateBuffer = array;
		this.netStateBufferAlt = array2;
		MemoryStream memoryStream = this.netStateMemStreamAlt;
		MemoryStream memoryStream2 = this.netStateMemStream;
		this.netStateMemStream = memoryStream;
		this.netStateMemStreamAlt = memoryStream2;
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x0008E345 File Offset: 0x0008C545
	protected void PlaySound(int clipId, int prio = 3)
	{
		this.machine.PlaySound(clipId, prio);
	}

	// Token: 0x06001920 RID: 6432 RVA: 0x0008E354 File Offset: 0x0008C554
	protected bool IsPlayerLocallyControlled(int player)
	{
		return this.machine.IsPlayerLocallyControlled(player);
	}

	// Token: 0x06001921 RID: 6433
	protected abstract void ButtonUp(int player, ArcadeButtons button);

	// Token: 0x06001922 RID: 6434
	protected abstract void ButtonDown(int player, ArcadeButtons button);

	// Token: 0x06001923 RID: 6435
	public abstract void OnTimeout();

	// Token: 0x06001924 RID: 6436 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x04002442 RID: 9282
	[SerializeField]
	public Vector2 Scale = new Vector2(1f, 1f);

	// Token: 0x04002443 RID: 9283
	private ArcadeButtons[] playerInputs = new ArcadeButtons[4];

	// Token: 0x04002444 RID: 9284
	public AudioClip[] audioClips;

	// Token: 0x04002445 RID: 9285
	private ArcadeMachine machine;

	// Token: 0x04002446 RID: 9286
	protected static int NetStateBufferSize = 512;

	// Token: 0x04002447 RID: 9287
	protected byte[] netStateBuffer = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x04002448 RID: 9288
	protected byte[] netStateBufferAlt = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x04002449 RID: 9289
	protected MemoryStream netStateMemStream;

	// Token: 0x0400244A RID: 9290
	protected MemoryStream netStateMemStreamAlt;

	// Token: 0x0400244B RID: 9291
	public bool memoryStreamsInitialized;
}
