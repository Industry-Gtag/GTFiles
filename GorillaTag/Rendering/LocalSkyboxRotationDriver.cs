using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x020012B4 RID: 4788
	public class LocalSkyboxRotationDriver : MonoBehaviour
	{
		// Token: 0x06007845 RID: 30789 RVA: 0x0026EE29 File Offset: 0x0026D029
		private void LateUpdate()
		{
			if (this.rotationSource == null)
			{
				return;
			}
			Shader.SetGlobalMatrix(LocalSkyboxRotationDriver._LocalSkyboxRotation, Matrix4x4.Rotate(this.rotationSource.rotation));
		}

		// Token: 0x06007846 RID: 30790 RVA: 0x0026EE54 File Offset: 0x0026D054
		private void OnDisable()
		{
			Shader.SetGlobalMatrix(LocalSkyboxRotationDriver._LocalSkyboxRotation, Matrix4x4.identity);
		}

		// Token: 0x0400886C RID: 34924
		private static readonly int _LocalSkyboxRotation = Shader.PropertyToID("_LocalSkyboxRotation");

		// Token: 0x0400886D RID: 34925
		[Tooltip("The sky's rotation mirrors this Transform's world rotation. Only rotation is used - position and scale are ignored.")]
		[SerializeField]
		private Transform rotationSource;
	}
}
