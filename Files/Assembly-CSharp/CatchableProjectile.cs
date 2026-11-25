using UnityEngine;

[RequireComponent(typeof(SlingshotProjectile))]
public class CatchableProjectile : MonoBehaviour
{
	[Tooltip("The throwable the catcher gets in their hand")]
	[GorillaSoundLookup]
	public int materialIndex;

	[Tooltip("How close the hand has to pass to the projectile, before player scale is applied")]
	public float catchRadius = 0.15f;

	public bool canCatchOwnProjectile = true;

	private SlingshotProjectile projectile;

	private Vector3 previousPosition;

	private bool wasLaunched;

	private void Awake()
	{
		projectile = GetComponent<SlingshotProjectile>();
	}

	private void LateUpdate()
	{
		if (!projectile.IsLaunched || !EquipmentInteractor.hasInstance)
		{
			wasLaunched = false;
			return;
		}
		Vector3 position = base.transform.position;
		if (!wasLaunched)
		{
			wasLaunched = true;
			previousPosition = position;
		}
		if ((!canCatchOwnProjectile && projectile.projectileOwner == NetworkSystem.Instance.LocalPlayer) || (!TryCatch(isLeftHand: true, previousPosition, position) && !TryCatch(isLeftHand: false, previousPosition, position)))
		{
			previousPosition = position;
		}
	}

	private bool TryCatch(bool isLeftHand, Vector3 from, Vector3 to)
	{
		EquipmentInteractor instance = EquipmentInteractor.instance;
		if (!(isLeftHand ? instance.isLeftGrabbing : instance.isRightGrabbing))
		{
			return false;
		}
		if ((isLeftHand ? instance.leftHandHeldEquipment : instance.rightHandHeldEquipment) != null)
		{
			return false;
		}
		Vector3 position = (isLeftHand ? instance.leftHand : instance.rightHand).transform.position;
		float num = catchRadius * VRRig.LocalRig.scaleFactor;
		if (DistanceSqToSegment(position, from, to) > num * num)
		{
			return false;
		}
		SnowballMaker snowballMaker = (isLeftHand ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance);
		if (snowballMaker == null || !snowballMaker.TryCreateSnowball(materialIndex, out var _))
		{
			return false;
		}
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
		RoomSystem.SendProjectileCaught(projectile.projectileOwner, projectile.myProjectileCount, to);
		projectile.Deactivate();
		return true;
	}

	private static float DistanceSqToSegment(Vector3 point, Vector3 a, Vector3 b)
	{
		Vector3 vector = b - a;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = ((sqrMagnitude > 0f) ? Mathf.Clamp01(Vector3.Dot(point - a, vector) / sqrMagnitude) : 0f);
		return (a + vector * num - point).sqrMagnitude;
	}
}
