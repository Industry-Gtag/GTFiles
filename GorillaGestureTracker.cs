using System;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class GorillaGestureTracker : MonoBehaviour
{
	// Token: 0x06001278 RID: 4728 RVA: 0x00062B1C File Offset: 0x00060D1C
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x00062B24 File Offset: 0x00060D24
	private void Setup()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			this._rig = base.GetComponentInChildren<VRRig>();
		}
		if (this._rig.AsNull<VRRig>() == null)
		{
			return;
		}
		this._rigTransform = this._rig.transform;
		this._vrNodes[1] = this._rig.rightHand;
		this._vrNodes[5] = this._rig.rightThumb;
		this._vrNodes[6] = this._rig.rightIndex;
		this._vrNodes[7] = this._rig.rightMiddle;
		this._vrNodes[8] = this._rig.leftHand;
		this._vrNodes[12] = this._rig.leftThumb;
		this._vrNodes[13] = this._rig.leftIndex;
		this._vrNodes[14] = this._rig.leftMiddle;
		foreach (Transform transform in this._rig.mainSkin.bones)
		{
			string name = transform.name;
			if (name.Contains("head", StringComparison.OrdinalIgnoreCase))
			{
				this._bones[0] = transform;
			}
			else if (name.Contains("hand.R", StringComparison.OrdinalIgnoreCase))
			{
				this._bones[1] = transform;
			}
			else if (name.Contains("thumb.03.R", StringComparison.OrdinalIgnoreCase))
			{
				this._bones[5] = transform;
			}
			else if (name.Contains("f_index.02.R", StringComparison.OrdinalIgnoreCase))
			{
				this._bones[6] = transform;
			}
			else if (name.Contains("f_middle.02.R", StringComparison.OrdinalIgnoreCase))
			{
				this._bones[7] = transform;
			}
			else if (name.Contains("hand.L", StringComparison.OrdinalIgnoreCase))
			{
				this._bones[8] = transform;
			}
			else if (name.Contains("thumb.03.L", StringComparison.OrdinalIgnoreCase))
			{
				this._bones[12] = transform;
			}
			else if (name.Contains("f_index.02.L", StringComparison.OrdinalIgnoreCase))
			{
				this._bones[13] = transform;
			}
			else if (name.Contains("f_middle.02.L", StringComparison.OrdinalIgnoreCase))
			{
				this._bones[14] = transform;
			}
		}
		this._matchesR = new bool[this._gestures.Length];
		this._matchesL = new bool[this._gestures.Length];
		this._setupDone = true;
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x00062D58 File Offset: 0x00060F58
	private void FixedUpdate()
	{
		this.PollNodes();
		this.PollGestures();
	}

	// Token: 0x0600127B RID: 4731 RVA: 0x00062D68 File Offset: 0x00060F68
	private void PollGestures()
	{
		if (this._gestures == null)
		{
			return;
		}
		int num = this._gestures.Length;
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < num; i++)
		{
			this.PollGesture(1, i, deltaTime, ref this._matchesR);
			this.PollGesture(8, i, deltaTime, ref this._matchesL);
		}
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x00062DB8 File Offset: 0x00060FB8
	private void PollNodes()
	{
		this.PollFace(0);
		this.PollHandAxes(1);
		int num;
		this.PollThumb(5, out num);
		int num2;
		this.PollIndex(6, out num2);
		int num3;
		this.PollMiddle(7, out num3);
		this.PollHandAxes(8);
		int num4;
		this.PollThumb(12, out num4);
		int num5;
		this.PollIndex(13, out num5);
		int num6;
		this.PollMiddle(14, out num6);
		this._flexes[1] = num + 1 + (num2 + 1) + (num3 + 1);
		this._flexes[8] = num4 + 1 + (num5 + 1) + (num6 + 1);
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x00062E3C File Offset: 0x0006103C
	private void PollThumb(int i, out int flex)
	{
		VRMapThumb vrmapThumb = (VRMapThumb)this._vrNodes[i];
		Transform transform = this._bones[i];
		float num = 0f;
		bool flag = vrmapThumb.primaryButtonTouch || vrmapThumb.secondaryButtonTouch;
		bool flag2 = vrmapThumb.primaryButtonPress || vrmapThumb.secondaryButtonPress;
		if (flag)
		{
			num = 0.1f;
		}
		if (flag2)
		{
			num = 1f;
		}
		flex = -1;
		if (flag2)
		{
			flex = 1;
		}
		else if (!flag)
		{
			flex = 0;
		}
		Vector3 position = transform.position;
		Vector3 up = transform.up;
		this._positions[i] = position;
		this._normals[i] = up;
		this._inputs[i] = num;
		this._flexes[i] = flex;
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x00062EEC File Offset: 0x000610EC
	private void PollIndex(int i, out int flex)
	{
		VRMapIndex vrmapIndex = (VRMapIndex)this._vrNodes[i];
		Transform transform = this._bones[i];
		float num = Mathf.Clamp01(vrmapIndex.triggerValue / 0.88f);
		flex = -1;
		if (num.Approx(0f, 1E-06f))
		{
			flex = 0;
		}
		if (num.Approx(1f, 1E-06f))
		{
			flex = 1;
		}
		Vector3 position = transform.position;
		Vector3 up = transform.up;
		this._positions[i] = position;
		this._normals[i] = up;
		this._inputs[i] = num;
		this._flexes[i] = flex;
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x00062F88 File Offset: 0x00061188
	private void PollMiddle(int i, out int flex)
	{
		VRMapMiddle vrmapMiddle = (VRMapMiddle)this._vrNodes[i];
		Transform transform = this._bones[i];
		float gripValue = vrmapMiddle.gripValue;
		flex = -1;
		if (gripValue.Approx(0f, 1E-06f))
		{
			flex = 0;
		}
		if (gripValue.Approx(1f, 1E-06f))
		{
			flex = 1;
		}
		Vector3 position = transform.position;
		Vector3 up = transform.up;
		this._positions[i] = position;
		this._normals[i] = up;
		this._inputs[i] = gripValue;
		this._flexes[i] = flex;
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x0006301C File Offset: 0x0006121C
	private void PollGesture(int hand, int i, float dt, ref bool[] results)
	{
		results[i] = false;
		GorillaHandGesture gorillaHandGesture = this._gestures[i];
		if (!gorillaHandGesture.track)
		{
			return;
		}
		GestureNode[] nodes = gorillaHandGesture.nodes;
		int num = 0;
		int num2 = 0;
		this.TrackHand(hand, (GestureHandNode)nodes[0], ref num, ref num2);
		this.TrackHandAxis(hand + 1, nodes[1], ref num, ref num2);
		this.TrackHandAxis(hand + 2, nodes[2], ref num, ref num2);
		this.TrackHandAxis(hand + 3, nodes[3], ref num, ref num2);
		this.TrackDigit(hand + 4, (GestureDigitNode)nodes[4], ref num, ref num2);
		this.TrackDigit(hand + 5, (GestureDigitNode)nodes[5], ref num, ref num2);
		this.TrackDigit(hand + 6, (GestureDigitNode)nodes[6], ref num, ref num2);
		results[i] = num == num2;
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x000630D8 File Offset: 0x000612D8
	private void TrackHand(int hand, GestureHandNode node, ref int tracked, ref int matches)
	{
		if (!node.track)
		{
			return;
		}
		GestureHandState state = node.state;
		if ((state & GestureHandState.IsLeft) == GestureHandState.IsLeft)
		{
			tracked++;
			if (hand == 8)
			{
				matches++;
			}
		}
		if ((state & GestureHandState.IsRight) == GestureHandState.IsRight)
		{
			tracked++;
			if (hand == 1)
			{
				matches++;
			}
		}
		if ((state & GestureHandState.Open) == GestureHandState.Open)
		{
			tracked++;
			if (this._flexes[hand] == 3)
			{
				matches++;
			}
		}
		if ((state & GestureHandState.Closed) == GestureHandState.Closed)
		{
			tracked++;
			if (this._flexes[hand] == 6)
			{
				matches++;
			}
		}
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x00063164 File Offset: 0x00061364
	private void TrackHandAxis(int axis, GestureNode node, ref int tracked, ref int matches)
	{
		if (!node.track)
		{
			return;
		}
		GestureAlignment alignment = node.alignment;
		Vector3 vector = this._normals[axis];
		Vector3 vector2 = this._normals[0];
		float num = Vector3.Dot(vector, Vector3.up);
		float num2 = -num;
		float num3 = Vector3.Dot(vector, vector2);
		float num4 = -num3;
		if ((alignment & GestureAlignment.WorldUp) == GestureAlignment.WorldUp)
		{
			tracked++;
			if (num > 1E-05f)
			{
				matches++;
			}
		}
		if ((alignment & GestureAlignment.WorldDown) == GestureAlignment.WorldDown)
		{
			tracked++;
			if (num2 > 1E-05f)
			{
				matches++;
			}
		}
		if ((alignment & GestureAlignment.TowardFace) == GestureAlignment.TowardFace)
		{
			tracked++;
			if (num3 > 1E-05f)
			{
				matches++;
			}
		}
		if ((alignment & GestureAlignment.AwayFromFace) == GestureAlignment.AwayFromFace)
		{
			tracked++;
			if (num4 > 1E-05f)
			{
				matches++;
			}
		}
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x00063244 File Offset: 0x00061444
	private void TrackDigit(int digit, GestureDigitNode node, ref int tracked, ref int matches)
	{
		if (!node.track)
		{
			return;
		}
		GestureAlignment alignment = node.alignment;
		GestureDigitFlexion flexion = node.flexion;
		Vector3 vector = this._normals[digit];
		Vector3 vector2 = this._normals[0];
		int num = this._flexes[digit];
		bool flag = num == 0;
		bool flag2 = num == 1;
		bool flag3 = num == -1;
		float num2 = Vector3.Dot(vector, Vector3.up);
		float num3 = -num2;
		float num4 = Vector3.Dot(vector, vector2);
		float num5 = -num4;
		if ((alignment & GestureAlignment.WorldUp) == GestureAlignment.WorldUp)
		{
			tracked++;
			if (num2 > 1E-05f)
			{
				matches++;
			}
		}
		if ((alignment & GestureAlignment.WorldDown) == GestureAlignment.WorldDown)
		{
			tracked++;
			if (num3 > 1E-05f)
			{
				matches++;
			}
		}
		if ((alignment & GestureAlignment.TowardFace) == GestureAlignment.TowardFace)
		{
			tracked++;
			if (num4 > 1E-05f)
			{
				matches++;
			}
		}
		if ((alignment & GestureAlignment.AwayFromFace) == GestureAlignment.AwayFromFace)
		{
			tracked++;
			if (num5 > 1E-05f)
			{
				matches++;
			}
		}
		if ((flexion & GestureDigitFlexion.Bent) == GestureDigitFlexion.Bent)
		{
			tracked++;
			if (flag3)
			{
				matches++;
			}
		}
		if ((flexion & GestureDigitFlexion.Open) == GestureDigitFlexion.Open)
		{
			tracked++;
			if (flag)
			{
				matches++;
			}
		}
		if ((flexion & GestureDigitFlexion.Closed) == GestureDigitFlexion.Closed)
		{
			tracked++;
			if (flag2)
			{
				matches++;
			}
		}
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x00063398 File Offset: 0x00061598
	private void PollFace(int index)
	{
		Transform transform = this._bones[index];
		this._positions[index] = transform.TransformPoint(this._faceBasisOffset);
		this._normals[index] = this._faceBasisAngles * transform.forward;
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x000633E4 File Offset: 0x000615E4
	private void PollHandAxes(int hand)
	{
		bool flag = hand == 1;
		bool flag2 = hand == 8;
		int num = hand + 1;
		int num2 = hand + 2;
		int num3 = hand + 3;
		Transform transform = this._bones[hand];
		Vector3 handBasisAngles = this._handBasisAngles;
		if (flag2)
		{
			handBasisAngles.z *= -1f;
		}
		Quaternion quaternion = transform.rotation * Quaternion.Euler(handBasisAngles);
		this._positions[hand] = transform.position;
		this._normals[num] = quaternion * Vector3.right * (flag ? 1f : (-1f));
		this._normals[num2] = quaternion * Vector3.forward;
		this._normals[num3] = quaternion * Vector3.up;
	}

	// Token: 0x0400167B RID: 5755
	[SerializeField]
	private VRRig _rig;

	// Token: 0x0400167C RID: 5756
	[SerializeField]
	private Transform _rigTransform;

	// Token: 0x0400167D RID: 5757
	public const int N_FACE = 0;

	// Token: 0x0400167E RID: 5758
	public const int R_HAND = 1;

	// Token: 0x0400167F RID: 5759
	public const int R_PALM = 2;

	// Token: 0x04001680 RID: 5760
	public const int R_WRIST = 3;

	// Token: 0x04001681 RID: 5761
	public const int R_DIGITS = 4;

	// Token: 0x04001682 RID: 5762
	public const int R_THUMB = 5;

	// Token: 0x04001683 RID: 5763
	public const int R_INDEX = 6;

	// Token: 0x04001684 RID: 5764
	public const int R_MIDDLE = 7;

	// Token: 0x04001685 RID: 5765
	public const int L_HAND = 8;

	// Token: 0x04001686 RID: 5766
	public const int L_PALM = 9;

	// Token: 0x04001687 RID: 5767
	public const int L_WRIST = 10;

	// Token: 0x04001688 RID: 5768
	public const int L_DIGITS = 11;

	// Token: 0x04001689 RID: 5769
	public const int L_THUMB = 12;

	// Token: 0x0400168A RID: 5770
	public const int L_INDEX = 13;

	// Token: 0x0400168B RID: 5771
	public const int L_MIDDLE = 14;

	// Token: 0x0400168C RID: 5772
	public const int N_SIZE = 15;

	// Token: 0x0400168D RID: 5773
	[Space]
	[SerializeField]
	private Vector3 _handBasisAngles = new Vector3(0f, 2f, 341f);

	// Token: 0x0400168E RID: 5774
	[Space]
	[SerializeField]
	private Vector3 _faceBasisOffset = new Vector3(0f, 0.1f, 0.136f);

	// Token: 0x0400168F RID: 5775
	[SerializeField]
	private Quaternion _faceBasisAngles = Quaternion.Euler(-8f, 0f, 0f);

	// Token: 0x04001690 RID: 5776
	[Space]
	[SerializeField]
	private bool _debug;

	// Token: 0x04001691 RID: 5777
	[NonSerialized]
	private bool _setupDone;

	// Token: 0x04001692 RID: 5778
	public static uint TickRate = 24U;

	// Token: 0x04001693 RID: 5779
	[Space]
	[SerializeField]
	private Transform[] _bones = new Transform[15];

	// Token: 0x04001694 RID: 5780
	[NonSerialized]
	private VRMap[] _vrNodes = new VRMap[15];

	// Token: 0x04001695 RID: 5781
	[NonSerialized]
	private float[] _inputs = new float[15];

	// Token: 0x04001696 RID: 5782
	[NonSerialized]
	private int[] _flexes = new int[15];

	// Token: 0x04001697 RID: 5783
	[NonSerialized]
	private Vector3[] _normals = new Vector3[15];

	// Token: 0x04001698 RID: 5784
	[NonSerialized]
	private Vector3[] _positions = new Vector3[15];

	// Token: 0x04001699 RID: 5785
	[Space]
	[SerializeField]
	private GorillaHandGesture[] _gestures = new GorillaHandGesture[0];

	// Token: 0x0400169A RID: 5786
	[NonSerialized]
	private bool[] _matchesR = new bool[0];

	// Token: 0x0400169B RID: 5787
	[NonSerialized]
	private bool[] _matchesL = new bool[0];

	// Token: 0x0400169C RID: 5788
	private const int H_BENT = 0;

	// Token: 0x0400169D RID: 5789
	private const int H_OPEN = 3;

	// Token: 0x0400169E RID: 5790
	private const int H_CLOSED = 6;

	// Token: 0x0400169F RID: 5791
	private const int N_HAND = 0;

	// Token: 0x040016A0 RID: 5792
	private const int A_PALM = 1;

	// Token: 0x040016A1 RID: 5793
	private const int A_WRIST = 2;

	// Token: 0x040016A2 RID: 5794
	private const int A_DIGITS = 3;

	// Token: 0x040016A3 RID: 5795
	private const int D_THUMB = 4;

	// Token: 0x040016A4 RID: 5796
	private const int D_INDEX = 5;

	// Token: 0x040016A5 RID: 5797
	private const int D_MIDDLE = 6;
}
