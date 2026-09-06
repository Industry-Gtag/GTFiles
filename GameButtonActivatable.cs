using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020006B9 RID: 1721
public class GameButtonActivatable : MonoBehaviour, IGameActivatable
{
	// Token: 0x06002B08 RID: 11016 RVA: 0x000E6FA0 File Offset: 0x000E51A0
	public bool CheckInput(XRNode xrNode, float sensitivity = 0.25f)
	{
		switch (this.inputButton)
		{
		case GameButtonActivatable.InputButton.Trigger:
			return ControllerInputPoller.TriggerFloat(xrNode) > sensitivity;
		case GameButtonActivatable.InputButton.ButtonA:
			return ControllerInputPoller.PrimaryButtonPress(xrNode);
		case GameButtonActivatable.InputButton.ButtonB:
			return ControllerInputPoller.SecondaryButtonPress(xrNode);
		case GameButtonActivatable.InputButton.Grip:
			return ControllerInputPoller.GripFloat(xrNode) > sensitivity;
		case GameButtonActivatable.InputButton.Joystick:
			return ControllerInputPoller.TriggerFloat(xrNode) > sensitivity;
		default:
			return false;
		}
	}

	// Token: 0x06002B09 RID: 11017 RVA: 0x000E7000 File Offset: 0x000E5200
	public bool CheckInput(float sensitivity = 0.25f)
	{
		int equippedSlotIndex = this.gameEntity.EquippedSlotIndex;
		if (equippedSlotIndex == -1 || !this.gameEntity.IsHeldOrSnappedByLocalPlayer)
		{
			return false;
		}
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		if (this.gameEntity.IsSnappedToHand)
		{
			int num;
			if (equippedSlotIndex != 2)
			{
				if (equippedSlotIndex != 3)
				{
					num = -1;
				}
				else
				{
					num = 1;
				}
			}
			else
			{
				num = 0;
			}
			int num2 = num;
			GameEntity gameEntity;
			IGameActivatable gameActivatable;
			if (gamePlayer.TryGetSlotEntity(num2, out gameEntity) && gameEntity.TryGetComponent<IGameActivatable>(out gameActivatable))
			{
				return false;
			}
			if (this.inputButton == GameButtonActivatable.InputButton.Trigger && GameTriggerInteractable.LocalInteractableTriggers.Count > 0)
			{
				Vector3 position = gamePlayer.GetHandTransform(num2).position;
				for (int i = 0; i < GameTriggerInteractable.LocalInteractableTriggers.Count; i++)
				{
					if (GameTriggerInteractable.LocalInteractableTriggers[i].PointWithinInteractableArea(position))
					{
						return false;
					}
				}
			}
		}
		return this.CheckInput(this.gameEntity.EquippedHandXRNode, sensitivity);
	}

	// Token: 0x040037D3 RID: 14291
	[SerializeField]
	public GameButtonActivatable.InputButton inputButton;

	// Token: 0x040037D4 RID: 14292
	public GameEntity gameEntity;

	// Token: 0x020006BA RID: 1722
	public enum InputButton
	{
		// Token: 0x040037D6 RID: 14294
		Trigger,
		// Token: 0x040037D7 RID: 14295
		ButtonA,
		// Token: 0x040037D8 RID: 14296
		ButtonB,
		// Token: 0x040037D9 RID: 14297
		Grip,
		// Token: 0x040037DA RID: 14298
		Joystick
	}
}
