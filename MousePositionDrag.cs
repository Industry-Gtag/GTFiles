using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class MousePositionDrag : MonoBehaviour
{
	// Token: 0x06000090 RID: 144 RVA: 0x00004F62 File Offset: 0x00003162
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00004F74 File Offset: 0x00003174
	private void Update()
	{
		this.m_currFrameHasFocus = Application.isFocused;
		bool prevFrameHasFocus = this.m_prevFrameHasFocus;
		this.m_prevFrameHasFocus = this.m_currFrameHasFocus;
		if (!prevFrameHasFocus && !this.m_currFrameHasFocus)
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3 prevMousePosition = this.m_prevMousePosition;
		Vector3 vector = mousePosition - prevMousePosition;
		this.m_prevMousePosition = mousePosition;
		if (!prevFrameHasFocus)
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			base.transform.position += 0.02f * vector;
		}
	}

	// Token: 0x040000A8 RID: 168
	private bool m_currFrameHasFocus;

	// Token: 0x040000A9 RID: 169
	private bool m_prevFrameHasFocus;

	// Token: 0x040000AA RID: 170
	private Vector3 m_prevMousePosition;
}
