using UnityEngine;

namespace GorillaTag.Gravity;

public class SplinePlanetZone : PlanetZone
{
	[SerializeField]
	private CatmullRomSpline spline;

	protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
	{
		float closestEvaluationOnSpline = spline.GetClosestEvaluationOnSpline(worldPosition, out var _);
		Vector3 vector = spline.Evaluate(closestEvaluationOnSpline);
		return worldPosition - vector;
	}
}
