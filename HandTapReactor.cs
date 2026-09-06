using System;
using TagEffects;
using UnityEngine;

// Token: 0x020002E2 RID: 738
public class HandTapReactor : MonoBehaviour
{
	// Token: 0x060012CF RID: 4815 RVA: 0x00064291 File Offset: 0x00062491
	private void LeftDown(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.LeftDown, false);
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x000642A0 File Offset: 0x000624A0
	private void LeftUp(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.LeftUp, false);
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x000642B0 File Offset: 0x000624B0
	private void LeftGesture(IHandEffectsTrigger.Mode mode)
	{
		FlagEvents<HandTapReactor.TapType> flagEvents = this.handTapEvents;
		HandTapReactor.TapType tapType;
		switch (mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			tapType = HandTapReactor.TapType.LeftHighFive;
			break;
		case IHandEffectsTrigger.Mode.FistBump:
			tapType = HandTapReactor.TapType.LeftFistBump;
			break;
		case IHandEffectsTrigger.Mode.Tag3P:
			tapType = HandTapReactor.TapType.LeftTagThirdPerson;
			break;
		case IHandEffectsTrigger.Mode.Tag1P:
			tapType = HandTapReactor.TapType.LeftTagFirstPerson;
			break;
		default:
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(mode);
			break;
		}
		flagEvents.InvokeAll(tapType, false);
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x00064301 File Offset: 0x00062501
	private void RightDown(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.RightDown, false);
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x00064311 File Offset: 0x00062511
	private void RightUp(HandEffectContext ctx)
	{
		this.handTapEvents.InvokeAll(HandTapReactor.TapType.RightUp, false);
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x00064324 File Offset: 0x00062524
	private void RightGesture(IHandEffectsTrigger.Mode mode)
	{
		FlagEvents<HandTapReactor.TapType> flagEvents = this.handTapEvents;
		HandTapReactor.TapType tapType;
		switch (mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			tapType = HandTapReactor.TapType.RightHighFive;
			break;
		case IHandEffectsTrigger.Mode.FistBump:
			tapType = HandTapReactor.TapType.RightFistBump;
			break;
		case IHandEffectsTrigger.Mode.Tag3P:
			tapType = HandTapReactor.TapType.RightTagThirdPerson;
			break;
		case IHandEffectsTrigger.Mode.Tag1P:
			tapType = HandTapReactor.TapType.RightTagFirstPerson;
			break;
		default:
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(mode);
			break;
		}
		flagEvents.InvokeAll(tapType, false);
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x00064384 File Offset: 0x00062584
	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
			IHandEffectsTrigger[] componentsInChildren = this.myRig.GetComponentsInChildren<IHandEffectsTrigger>();
			if (componentsInChildren[0].RightHand)
			{
				this.rightHandTrigger = componentsInChildren[0];
				this.leftHandTrigger = componentsInChildren[1];
			}
			else
			{
				this.rightHandTrigger = componentsInChildren[1];
				this.leftHandTrigger = componentsInChildren[0];
			}
		}
		if (this.myRig != null)
		{
			this.myRig.LeftHandEffect.handTapDown += this.LeftDown;
			this.myRig.LeftHandEffect.handTapUp += this.LeftUp;
			IHandEffectsTrigger handEffectsTrigger = this.leftHandTrigger;
			handEffectsTrigger.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Combine(handEffectsTrigger.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.LeftGesture));
			this.myRig.RightHandEffect.handTapDown += this.RightDown;
			this.myRig.RightHandEffect.handTapUp += this.RightUp;
			IHandEffectsTrigger handEffectsTrigger2 = this.rightHandTrigger;
			handEffectsTrigger2.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Combine(handEffectsTrigger2.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.RightGesture));
		}
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x000644B8 File Offset: 0x000626B8
	private void OnDisable()
	{
		if (this.myRig != null)
		{
			this.myRig.LeftHandEffect.handTapDown -= this.LeftDown;
			this.myRig.LeftHandEffect.handTapUp -= this.LeftUp;
			IHandEffectsTrigger handEffectsTrigger = this.leftHandTrigger;
			handEffectsTrigger.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Remove(handEffectsTrigger.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.LeftGesture));
			this.myRig.RightHandEffect.handTapDown -= this.RightDown;
			this.myRig.RightHandEffect.handTapUp -= this.RightUp;
			IHandEffectsTrigger handEffectsTrigger2 = this.rightHandTrigger;
			handEffectsTrigger2.OnTrigger = (Action<IHandEffectsTrigger.Mode>)Delegate.Remove(handEffectsTrigger2.OnTrigger, new Action<IHandEffectsTrigger.Mode>(this.RightGesture));
		}
	}

	// Token: 0x040016E9 RID: 5865
	[SerializeField]
	private FlagEvents<HandTapReactor.TapType> handTapEvents;

	// Token: 0x040016EA RID: 5866
	private VRRig myRig;

	// Token: 0x040016EB RID: 5867
	private IHandEffectsTrigger leftHandTrigger;

	// Token: 0x040016EC RID: 5868
	private IHandEffectsTrigger rightHandTrigger;

	// Token: 0x020002E3 RID: 739
	[Flags]
	private enum TapType
	{
		// Token: 0x040016EE RID: 5870
		None = 0,
		// Token: 0x040016EF RID: 5871
		LeftDown = 1,
		// Token: 0x040016F0 RID: 5872
		LeftUp = 2,
		// Token: 0x040016F1 RID: 5873
		LeftHighFive = 4,
		// Token: 0x040016F2 RID: 5874
		LeftFistBump = 8,
		// Token: 0x040016F3 RID: 5875
		LeftTagFirstPerson = 16,
		// Token: 0x040016F4 RID: 5876
		LeftTagThirdPerson = 32,
		// Token: 0x040016F5 RID: 5877
		AllLeft = 63,
		// Token: 0x040016F6 RID: 5878
		RightDown = 64,
		// Token: 0x040016F7 RID: 5879
		RightUp = 128,
		// Token: 0x040016F8 RID: 5880
		RightHighFive = 256,
		// Token: 0x040016F9 RID: 5881
		RightFistBump = 512,
		// Token: 0x040016FA RID: 5882
		RightTagFirstPerson = 1024,
		// Token: 0x040016FB RID: 5883
		RightTagThirdPerson = 2048,
		// Token: 0x040016FC RID: 5884
		AllRight = 4032,
		// Token: 0x040016FD RID: 5885
		All = -1
	}
}
