using System;
using CustomMapSupport;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A50 RID: 2640
public class CustomMapsGorillaZipline : GorillaZipline
{
	// Token: 0x060043C3 RID: 17347 RVA: 0x00168BDC File Offset: 0x00166DDC
	public bool GenerateZipline(global::CustomMapSupport.BezierSpline splineRef)
	{
		this.spline = base.GetComponent<global::BezierSpline>();
		if (this.spline.IsNull())
		{
			return false;
		}
		this.spline.BuildSplineFromPoints(splineRef.GetControlPoints(), this.ConvertControlPointModes(splineRef.GetControlPointModes()), splineRef.Loop);
		if (this.segmentsRoot == null)
		{
			return false;
		}
		this.ziplineDistance = 0f;
		float num = 0f;
		int num2 = 0;
		Transform transform = null;
		while (num < 1f)
		{
			float num3 = this.segmentDistance;
			if (num2 == 0)
			{
				num3 /= 2f;
			}
			base.FindTFromDistance(ref num, num3, 5000);
			if (num < 1f || this.spline.Loop)
			{
				Vector3 point = this.spline.GetPoint(num);
				GameObject gameObject = Object.Instantiate<GameObject>(this.segmentPrefab);
				gameObject.transform.SetParent(this.segmentsRoot);
				gameObject.transform.position = point;
				gameObject.transform.LookAt(point + this.spline.GetDirection(num));
				gameObject.transform.position -= gameObject.transform.forward * 0.5f;
				if (num2 > 0)
				{
					transform.LookAt(gameObject.transform);
				}
				gameObject.GetComponent<GorillaClimbableRef>().climb = this.slideHelper;
				this.ziplineDistance += this.segmentDistance;
				transform = gameObject.transform;
			}
			num2++;
		}
		return true;
	}

	// Token: 0x060043C4 RID: 17348 RVA: 0x00168D5E File Offset: 0x00166F5E
	protected override void OnBeforeClimb(GorillaHandClimber hand, GorillaClimbableRef climbRef)
	{
		this.slideHelper.gameObject.SetActive(true);
		base.OnBeforeClimb(hand, climbRef);
	}

	// Token: 0x060043C5 RID: 17349 RVA: 0x00168D7C File Offset: 0x00166F7C
	private global::BezierControlPointMode[] ConvertControlPointModes(global::CustomMapSupport.BezierControlPointMode[] refModes)
	{
		global::BezierControlPointMode[] array = new global::BezierControlPointMode[refModes.Length];
		for (int i = 0; i < refModes.Length; i++)
		{
			switch (refModes[i])
			{
			case global::CustomMapSupport.BezierControlPointMode.Free:
				array[i] = global::BezierControlPointMode.Free;
				break;
			case global::CustomMapSupport.BezierControlPointMode.Aligned:
				array[i] = global::BezierControlPointMode.Aligned;
				break;
			case global::CustomMapSupport.BezierControlPointMode.Mirrored:
				array[i] = global::BezierControlPointMode.Mirrored;
				break;
			}
		}
		return array;
	}

	// Token: 0x060043C6 RID: 17350 RVA: 0x00168DC9 File Offset: 0x00166FC9
	protected override void Start()
	{
		GorillaClimbable slideHelper = this.slideHelper;
		slideHelper.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(slideHelper.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
	}

	// Token: 0x060043C7 RID: 17351 RVA: 0x00168DF4 File Offset: 0x00166FF4
	public void Init(GTObjectPlaceholder ziplinePlaceholder)
	{
		if (ziplinePlaceholder.PlaceholderObject != GTObject.ZipLine)
		{
			return;
		}
		this.segmentDistance = ziplinePlaceholder.ziplineSegmentGenerationOffset;
		this.spline = base.gameObject.GetComponent<global::BezierSpline>();
		if (this.spline == null)
		{
			this.spline = base.gameObject.AddComponent<global::BezierSpline>();
		}
		this.spline.BuildSplineFromPoints(ziplinePlaceholder.spline.GetControlPoints(), this.ConvertControlPointModes(ziplinePlaceholder.spline.GetControlPointModes()), ziplinePlaceholder.spline.Loop);
		for (int i = 0; i < ziplinePlaceholder.ziplineSegments.Count; i++)
		{
			ziplinePlaceholder.ziplineSegments[i].transform.SetParent(this.segmentsRoot, true);
			ziplinePlaceholder.ziplineSegments[i].AddComponent<GorillaClimbableRef>().climb = this.slideHelper;
			this.ziplineDistance += this.segmentDistance;
		}
	}
}
