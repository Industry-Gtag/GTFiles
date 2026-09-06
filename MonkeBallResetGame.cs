using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000611 RID: 1553
public class MonkeBallResetGame : MonoBehaviourTick
{
	// Token: 0x060026D2 RID: 9938 RVA: 0x000CDC44 File Offset: 0x000CBE44
	private void Awake()
	{
		this._resetButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
		if (this._resetButton == null)
		{
			this._buttonOrigin = this._resetButton.transform.position;
		}
	}

	// Token: 0x060026D3 RID: 9939 RVA: 0x000CDC91 File Offset: 0x000CBE91
	public override void Tick()
	{
		if (this._cooldown)
		{
			this._cooldownTimer -= Time.deltaTime;
			if (this._cooldownTimer <= 0f)
			{
				this.ToggleButton(false, -1);
				this._cooldown = false;
			}
		}
	}

	// Token: 0x060026D4 RID: 9940 RVA: 0x000CDCCC File Offset: 0x000CBECC
	public void ToggleReset(bool toggle, int teamId, bool force = false)
	{
		if (teamId < -1 || teamId >= this.teamMaterials.Length)
		{
			return;
		}
		if (toggle)
		{
			this.ToggleButton(true, teamId);
			this._cooldown = false;
			return;
		}
		if (force)
		{
			this.ToggleButton(false, -1);
			return;
		}
		this._cooldown = true;
		this._cooldownTimer = 3f;
	}

	// Token: 0x060026D5 RID: 9941 RVA: 0x000CDD1C File Offset: 0x000CBF1C
	private void ToggleButton(bool toggle, int teamId)
	{
		this._resetButton.enabled = toggle;
		this.allowedTeamId = teamId;
		if (!toggle || teamId == -1)
		{
			this.button.sharedMaterial = this.neutralMaterial;
			return;
		}
		this.button.sharedMaterial = this.teamMaterials[teamId];
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x000CDD68 File Offset: 0x000CBF68
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestResetGame();
	}

	// Token: 0x04003241 RID: 12865
	[SerializeField]
	private GorillaPressableButton _resetButton;

	// Token: 0x04003242 RID: 12866
	public Renderer button;

	// Token: 0x04003243 RID: 12867
	public Vector3 buttonPressOffset;

	// Token: 0x04003244 RID: 12868
	private Vector3 _buttonOrigin = Vector3.zero;

	// Token: 0x04003245 RID: 12869
	[Space]
	public Material[] teamMaterials;

	// Token: 0x04003246 RID: 12870
	public Material neutralMaterial;

	// Token: 0x04003247 RID: 12871
	public int allowedTeamId = -1;

	// Token: 0x04003248 RID: 12872
	[SerializeField]
	private TextMeshPro _resetLabel;

	// Token: 0x04003249 RID: 12873
	private bool _cooldown;

	// Token: 0x0400324A RID: 12874
	private float _cooldownTimer;
}
