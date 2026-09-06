using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200145D RID: 5213
	public class QuaternionUtil
	{
		// Token: 0x0600836B RID: 33643 RVA: 0x002A5D44 File Offset: 0x002A3F44
		public static float Magnitude(Quaternion q)
		{
			return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
		}

		// Token: 0x0600836C RID: 33644 RVA: 0x002A5D82 File Offset: 0x002A3F82
		public static float MagnitudeSqr(Quaternion q)
		{
			return q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
		}

		// Token: 0x0600836D RID: 33645 RVA: 0x002B0534 File Offset: 0x002AE734
		public static Quaternion Normalize(Quaternion q)
		{
			float num = 1f / QuaternionUtil.Magnitude(q);
			return new Quaternion(num * q.x, num * q.y, num * q.z, num * q.w);
		}

		// Token: 0x0600836E RID: 33646 RVA: 0x002B0574 File Offset: 0x002AE774
		public static Quaternion AxisAngle(Vector3 axis, float angle)
		{
			float num = 0.5f * angle;
			float num2 = Mathf.Sin(num);
			float num3 = Mathf.Cos(num);
			return new Quaternion(num2 * axis.x, num2 * axis.y, num2 * axis.z, num3);
		}

		// Token: 0x0600836F RID: 33647 RVA: 0x002B05B4 File Offset: 0x002AE7B4
		public static Vector3 GetAxis(Quaternion q)
		{
			Vector3 vector = new Vector3(q.x, q.y, q.z);
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return Vector3.left;
			}
			return vector / magnitude;
		}

		// Token: 0x06008370 RID: 33648 RVA: 0x002A5EDF File Offset: 0x002A40DF
		public static float GetAngle(Quaternion q)
		{
			return 2f * Mathf.Acos(Mathf.Clamp(q.w, -1f, 1f));
		}

		// Token: 0x06008371 RID: 33649 RVA: 0x002B05F8 File Offset: 0x002AE7F8
		public static Quaternion FromAngularVector(Vector3 v)
		{
			float magnitude = v.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return Quaternion.identity;
			}
			v /= magnitude;
			float num = 0.5f * magnitude;
			float num2 = Mathf.Sin(num);
			float num3 = Mathf.Cos(num);
			return new Quaternion(num2 * v.x, num2 * v.y, num2 * v.z, num3);
		}

		// Token: 0x06008372 RID: 33650 RVA: 0x002B0658 File Offset: 0x002AE858
		public static Vector3 ToAngularVector(Quaternion q)
		{
			Vector3 axis = QuaternionUtil.GetAxis(q);
			return QuaternionUtil.GetAngle(q) * axis;
		}

		// Token: 0x06008373 RID: 33651 RVA: 0x002B0678 File Offset: 0x002AE878
		public static Quaternion Pow(Quaternion q, float exp)
		{
			Vector3 axis = QuaternionUtil.GetAxis(q);
			float num = QuaternionUtil.GetAngle(q) * exp;
			return QuaternionUtil.AxisAngle(axis, num);
		}

		// Token: 0x06008374 RID: 33652 RVA: 0x002B069A File Offset: 0x002AE89A
		public static Quaternion Integrate(Quaternion q, Quaternion v, float dt)
		{
			return QuaternionUtil.Pow(v, dt) * q;
		}

		// Token: 0x06008375 RID: 33653 RVA: 0x002B06AC File Offset: 0x002AE8AC
		public static Quaternion Integrate(Quaternion q, Vector3 omega, float dt)
		{
			omega *= 0.5f;
			Quaternion quaternion = new Quaternion(omega.x, omega.y, omega.z, 0f) * q;
			return QuaternionUtil.Normalize(new Quaternion(q.x + quaternion.x * dt, q.y + quaternion.y * dt, q.z + quaternion.z * dt, q.w + quaternion.w * dt));
		}

		// Token: 0x06008376 RID: 33654 RVA: 0x002A5F6D File Offset: 0x002A416D
		public static Vector4 ToVector4(Quaternion q)
		{
			return new Vector4(q.x, q.y, q.z, q.w);
		}

		// Token: 0x06008377 RID: 33655 RVA: 0x002B0730 File Offset: 0x002AE930
		public static Quaternion FromVector4(Vector4 v, bool normalize = true)
		{
			if (normalize)
			{
				float sqrMagnitude = v.sqrMagnitude;
				if (sqrMagnitude < MathUtil.Epsilon)
				{
					return Quaternion.identity;
				}
				v /= Mathf.Sqrt(sqrMagnitude);
			}
			return new Quaternion(v.x, v.y, v.z, v.w);
		}

		// Token: 0x06008378 RID: 33656 RVA: 0x002B0784 File Offset: 0x002AE984
		public static void DecomposeSwingTwist(Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
		{
			Vector3 vector = new Vector3(q.x, q.y, q.z);
			if (vector.sqrMagnitude < MathUtil.Epsilon)
			{
				Vector3 vector2 = q * twistAxis;
				Vector3 vector3 = Vector3.Cross(twistAxis, vector2);
				if (vector3.sqrMagnitude > MathUtil.Epsilon)
				{
					float num = Vector3.Angle(twistAxis, vector2);
					swing = Quaternion.AngleAxis(num, vector3);
				}
				else
				{
					swing = Quaternion.identity;
				}
				twist = Quaternion.AngleAxis(180f, twistAxis);
				return;
			}
			Vector3 vector4 = Vector3.Project(vector, twistAxis);
			twist = new Quaternion(vector4.x, vector4.y, vector4.z, q.w);
			twist = QuaternionUtil.Normalize(twist);
			swing = q * Quaternion.Inverse(twist);
		}

		// Token: 0x06008379 RID: 33657 RVA: 0x002B0860 File Offset: 0x002AEA60
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float t, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			return QuaternionUtil.Sterp(a, b, twistAxis, t, out quaternion, out quaternion2, mode);
		}

		// Token: 0x0600837A RID: 33658 RVA: 0x002B087C File Offset: 0x002AEA7C
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float t, out Quaternion swing, out Quaternion twist, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			return QuaternionUtil.Sterp(a, b, twistAxis, t, t, out swing, out twist, mode);
		}

		// Token: 0x0600837B RID: 33659 RVA: 0x002B0890 File Offset: 0x002AEA90
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float tSwing, float tTwist, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			return QuaternionUtil.Sterp(a, b, twistAxis, tSwing, tTwist, out quaternion, out quaternion2, mode);
		}

		// Token: 0x0600837C RID: 33660 RVA: 0x002B08B0 File Offset: 0x002AEAB0
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float tSwing, float tTwist, out Quaternion swing, out Quaternion twist, QuaternionUtil.SterpMode mode)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			QuaternionUtil.DecomposeSwingTwist(b * Quaternion.Inverse(a), twistAxis, out quaternion, out quaternion2);
			if (mode == QuaternionUtil.SterpMode.Nlerp || mode != QuaternionUtil.SterpMode.Slerp)
			{
				swing = Quaternion.Lerp(Quaternion.identity, quaternion, tSwing);
				twist = Quaternion.Lerp(Quaternion.identity, quaternion2, tTwist);
			}
			else
			{
				swing = Quaternion.Slerp(Quaternion.identity, quaternion, tSwing);
				twist = Quaternion.Slerp(Quaternion.identity, quaternion2, tTwist);
			}
			return twist * swing;
		}

		// Token: 0x0200145E RID: 5214
		public enum SterpMode
		{
			// Token: 0x04009474 RID: 38004
			Nlerp,
			// Token: 0x04009475 RID: 38005
			Slerp
		}
	}
}
