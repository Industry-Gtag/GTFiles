using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005EE RID: 1518
public class PrimaryButtonWatcher : MonoBehaviour
{
	// Token: 0x060025D3 RID: 9683 RVA: 0x000C8C2B File Offset: 0x000C6E2B
	private void Awake()
	{
		if (this.primaryButtonPress == null)
		{
			this.primaryButtonPress = new PrimaryButtonEvent();
		}
		this.devicesWithPrimaryButton = new List<InputDevice>();
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x000C8C4C File Offset: 0x000C6E4C
	private void OnEnable()
	{
		List<InputDevice> list = new List<InputDevice>();
		InputDevices.GetDevices(list);
		foreach (InputDevice inputDevice in list)
		{
			this.InputDevices_deviceConnected(inputDevice);
		}
		InputDevices.deviceConnected += this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected += this.InputDevices_deviceDisconnected;
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x000C8CC8 File Offset: 0x000C6EC8
	private void OnDisable()
	{
		InputDevices.deviceConnected -= this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected -= this.InputDevices_deviceDisconnected;
		this.devicesWithPrimaryButton.Clear();
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x000C8CF8 File Offset: 0x000C6EF8
	private void InputDevices_deviceConnected(InputDevice device)
	{
		bool flag;
		if (device.TryGetFeatureValue(CommonUsages.primaryButton, out flag))
		{
			this.devicesWithPrimaryButton.Add(device);
		}
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x000C8D21 File Offset: 0x000C6F21
	private void InputDevices_deviceDisconnected(InputDevice device)
	{
		if (this.devicesWithPrimaryButton.Contains(device))
		{
			this.devicesWithPrimaryButton.Remove(device);
		}
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x000C8D40 File Offset: 0x000C6F40
	private void Update()
	{
		bool flag = false;
		foreach (InputDevice inputDevice in this.devicesWithPrimaryButton)
		{
			bool flag2 = false;
			flag = (inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out flag2) && flag2) || flag;
		}
		if (flag != this.lastButtonState)
		{
			this.primaryButtonPress.Invoke(flag);
			this.lastButtonState = flag;
		}
	}

	// Token: 0x04003171 RID: 12657
	public PrimaryButtonEvent primaryButtonPress;

	// Token: 0x04003172 RID: 12658
	private bool lastButtonState;

	// Token: 0x04003173 RID: 12659
	private List<InputDevice> devicesWithPrimaryButton;
}
