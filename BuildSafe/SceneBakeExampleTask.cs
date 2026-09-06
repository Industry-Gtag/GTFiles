using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSafe
{
	// Token: 0x0200109C RID: 4252
	public class SceneBakeExampleTask : SceneBakeTask
	{
		// Token: 0x060069FD RID: 27133 RVA: 0x00221018 File Offset: 0x0021F218
		public override void OnSceneBake(Scene scene, SceneBakeMode mode)
		{
			for (int i = 0; i < 10; i++)
			{
				SceneBakeExampleTask.DuplicateAndRecolor(base.gameObject);
			}
			if (mode != SceneBakeMode.OnBuildPlayer)
			{
			}
		}

		// Token: 0x060069FE RID: 27134 RVA: 0x00221048 File Offset: 0x0021F248
		private static void DuplicateAndRecolor(GameObject target)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(target);
			gameObject.transform.position = Random.insideUnitSphere * 4f;
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			component.material = new Material(component.sharedMaterial)
			{
				color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f)
			};
		}
	}
}
