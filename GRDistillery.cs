using System;
using System.Globalization;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000770 RID: 1904
public class GRDistillery : MonoBehaviour
{
	// Token: 0x06003049 RID: 12361 RVA: 0x00105D20 File Offset: 0x00103F20
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
		this.sentientCoreDeposit.Init(reactor);
		this.cores = PlayerPrefs.GetInt("_grDistilleryCore", -1);
		if (this.cores == -1)
		{
			this.cores = 0;
		}
		this.RestoreStartTime();
		this.InitializeGauges();
	}

	// Token: 0x0600304A RID: 12362 RVA: 0x00105D70 File Offset: 0x00103F70
	private void SaveStartTime(DateTime time)
	{
		string text = time.ToString("O");
		PlayerPrefs.SetString("_grDistilleryStartTime", text);
		PlayerPrefs.Save();
	}

	// Token: 0x0600304B RID: 12363 RVA: 0x00105D9C File Offset: 0x00103F9C
	private void RestoreStartTime()
	{
		string @string = PlayerPrefs.GetString("_grDistilleryStartTime", string.Empty);
		if (@string != string.Empty)
		{
			this.startTime = DateTime.ParseExact(@string, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		}
	}

	// Token: 0x0600304C RID: 12364 RVA: 0x00105DE1 File Offset: 0x00103FE1
	public void StartResearch()
	{
		if (this.cores > 0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime();
			this.SaveStartTime(this.startTime);
			this.bProcessing = true;
			this.InitializeGauges();
		}
	}

	// Token: 0x0600304D RID: 12365 RVA: 0x00105E18 File Offset: 0x00104018
	public double CalculateRemaining()
	{
		return (double)this.secondsToResearchACore - (GorillaComputer.instance.GetServerTime() - this.startTime).TotalSeconds;
	}

	// Token: 0x0600304E RID: 12366 RVA: 0x00105E4C File Offset: 0x0010404C
	private void FirstUpdate()
	{
		double num = this.CalculateRemaining();
		while (this.cores > 0 && num < (double)(-(double)this.secondsToResearchACore))
		{
			if (num < (double)(-(double)this.secondsToResearchACore))
			{
				this.CompleteResearchingCore();
				num += (double)this.secondsToResearchACore;
			}
		}
		if (this.cores > 0 && num < 0.0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime().AddSeconds(num);
			num = this.CalculateRemaining();
			this.SaveStartTime(this.startTime);
		}
		if (this.cores > 0)
		{
			this.bProcessing = true;
			this.currentGaugeCore = this.cores - 1;
		}
		else
		{
			this.currentGaugeCore = 0;
		}
		if (this.cores >= 4)
		{
			this.depositDoor.transform.position = this.depositClosePosition.position;
		}
		else
		{
			this.depositDoor.transform.position = this.depositOpenPosition.position;
		}
		this.UpdateGauges();
	}

	// Token: 0x0600304F RID: 12367 RVA: 0x00105F44 File Offset: 0x00104144
	public void Update()
	{
		if (!this.firstUpdate)
		{
			this.FirstUpdate();
			this.firstUpdate = true;
		}
		this.UpdateDoorPosition();
		this.UpdateGauges();
		if (!this.bProcessing)
		{
			return;
		}
		this.remaingTime = this.CalculateRemaining();
		if (this.remaingTime <= 0.0)
		{
			this.CompleteResearchingCore();
		}
	}

	// Token: 0x06003050 RID: 12368 RVA: 0x00105FA0 File Offset: 0x001041A0
	private void UpdateDoorPosition()
	{
		if (this.cores >= 4)
		{
			this.depositDoor.transform.position = Vector3.MoveTowards(this.depositDoor.transform.position, this.depositClosePosition.transform.position, this.depositDoorCloseSpeed * Time.deltaTime);
			return;
		}
		this.depositDoor.transform.position = Vector3.MoveTowards(this.depositDoor.transform.position, this.depositOpenPosition.transform.position, this.depositDoorCloseSpeed * Time.deltaTime);
	}

	// Token: 0x06003051 RID: 12369 RVA: 0x0010603C File Offset: 0x0010423C
	private void CompleteResearchingCore()
	{
		this.cores = Math.Max(this.cores - 1, 0);
		this.currentGaugeCore = Math.Max(this.cores - 1, 0);
		PlayerPrefs.SetInt("_grDistilleryCore", this.cores);
		PlayerPrefs.Save();
		if (this.cores > 0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime().AddSeconds(this.remaingTime);
			this.SaveStartTime(this.startTime);
			this.remaingTime = this.CalculateRemaining();
		}
		if (this.cores == 0)
		{
			this.bProcessing = false;
		}
		this.UpdateGauges();
	}

	// Token: 0x06003052 RID: 12370 RVA: 0x001060DC File Offset: 0x001042DC
	public void DepositCore()
	{
		if (this.cores < this.maxCores)
		{
			this.cores++;
			if (!this.bFillingGauge)
			{
				this.bFillingGauge = true;
				this.fillTime = 0f;
			}
			PlayerPrefs.SetInt("_grDistilleryCore", this.cores);
			PlayerPrefs.Save();
			if (this.cores == 1)
			{
				this.StartResearch();
			}
		}
	}

	// Token: 0x06003053 RID: 12371 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void DebugFinishDistill()
	{
	}

	// Token: 0x06003054 RID: 12372 RVA: 0x00106144 File Offset: 0x00104344
	private void OnEnable()
	{
		if (this._applyMaterialgauge1)
		{
			this._applyMaterialgauge1.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge2)
		{
			this._applyMaterialgauge2.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge3)
		{
			this._applyMaterialgauge3.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge4)
		{
			this._applyMaterialgauge4.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		this.InitializeGauges();
	}

	// Token: 0x06003055 RID: 12373 RVA: 0x001061BC File Offset: 0x001043BC
	private void InitializeGauges()
	{
		for (int i = 0; i < this.gaugesFill.Length - 1; i++)
		{
			this.gaugesFill[i] = ((this.cores >= i + 1) ? this.gaugeFullFillAmount : this.gaugeEmptyFillAmount);
		}
		this.researchGaugeFill = this.gaugesFill[0];
		this.currentGaugeFillAmount = this.gaugeEmptyFillAmount;
	}

	// Token: 0x06003056 RID: 12374 RVA: 0x0010621C File Offset: 0x0010441C
	private void UpdateGauges()
	{
		for (int i = 0; i < this.gaugesFill.Length; i++)
		{
			if (i + 1 > this.cores)
			{
				this.gaugesFill[i] = this.gaugeEmptyFillAmount;
			}
		}
		if (this.bFillingGauge)
		{
			this.fillTime += Time.deltaTime;
			float num = this.fillTime / this.gaugeDrainTime;
			if (this.currentGaugeCore == this.cores - 1)
			{
				if (num > 1f)
				{
					this.bFillingGauge = false;
				}
				else
				{
					this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.currentGaugeFillAmount, Mathf.Lerp(this.gaugeEmptyFillAmount, this.gaugeFullFillAmount, (float)this.remaingTime / (float)this.secondsToResearchACore), num);
				}
			}
			else
			{
				this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.currentGaugeFillAmount, this.gaugeFullFillAmount, num);
			}
			if (this.bFillingGauge && num > 1f)
			{
				this.currentGaugeCore++;
				this.currentGaugeFillAmount = this.gaugeEmptyFillAmount;
				this.fillTime = 0f;
			}
		}
		else if (this.bProcessing)
		{
			this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.gaugeEmptyFillAmount, this.gaugeFullFillAmount, (float)this.remaingTime / (float)this.secondsToResearchACore);
			this.currentGaugeFillAmount = this.gaugesFill[this.currentGaugeCore];
		}
		this._applyMaterialgauge1.SetFloat("_LiquidFill", this.gaugesFill[0]);
		this._applyMaterialgauge1.Apply();
		this._applyMaterialgauge2.SetFloat("_LiquidFill", this.gaugesFill[1]);
		this._applyMaterialgauge2.Apply();
		this._applyMaterialgauge3.SetFloat("_LiquidFill", this.gaugesFill[2]);
		this._applyMaterialgauge3.Apply();
		this._applyMaterialgauge4.SetFloat("_LiquidFill", this.gaugesFill[3]);
		this._applyMaterialgauge4.Apply();
		this._applyMaterialCurrentResearch.SetFloat("_LiquidFill", this.researchGaugeFill);
		this._applyMaterialCurrentResearch.Apply();
	}

	// Token: 0x04003DB9 RID: 15801
	[SerializeField]
	private GRCurrencyDepositor sentientCoreDeposit;

	// Token: 0x04003DBA RID: 15802
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge1;

	// Token: 0x04003DBB RID: 15803
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge2;

	// Token: 0x04003DBC RID: 15804
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge3;

	// Token: 0x04003DBD RID: 15805
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge4;

	// Token: 0x04003DBE RID: 15806
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialCurrentResearch;

	// Token: 0x04003DBF RID: 15807
	[FormerlySerializedAs("emptyFillAmount")]
	public float gaugeEmptyFillAmount = 0.44f;

	// Token: 0x04003DC0 RID: 15808
	[FormerlySerializedAs("fullFillAmount")]
	public float gaugeFullFillAmount = 0.56f;

	// Token: 0x04003DC1 RID: 15809
	[SerializeField]
	private Transform depositClosePosition;

	// Token: 0x04003DC2 RID: 15810
	[SerializeField]
	private Transform depositOpenPosition;

	// Token: 0x04003DC3 RID: 15811
	[SerializeField]
	private GameObject depositDoor;

	// Token: 0x04003DC4 RID: 15812
	[SerializeField]
	private float depositDoorCloseSpeed = 0.5f;

	// Token: 0x04003DC5 RID: 15813
	[SerializeField]
	private TextMeshPro currentResearchPoints;

	// Token: 0x04003DC6 RID: 15814
	public float researchGaugeEmptyFillAmount = 0.44f;

	// Token: 0x04003DC7 RID: 15815
	public float researchGaugeFullFillAmount = 0.56f;

	// Token: 0x04003DC8 RID: 15816
	public int secondsToResearchACore;

	// Token: 0x04003DC9 RID: 15817
	public float gaugeDrainTime = 2f;

	// Token: 0x04003DCA RID: 15818
	public int maxCores = 4;

	// Token: 0x04003DCB RID: 15819
	public AudioSource feedbackSound;

	// Token: 0x04003DCC RID: 15820
	private DateTime startTime;

	// Token: 0x04003DCD RID: 15821
	private bool bProcessing;

	// Token: 0x04003DCE RID: 15822
	private int cores;

	// Token: 0x04003DCF RID: 15823
	private bool bFillingGauge;

	// Token: 0x04003DD0 RID: 15824
	private int currentGaugeCore;

	// Token: 0x04003DD1 RID: 15825
	private float currentGaugeFillAmount;

	// Token: 0x04003DD2 RID: 15826
	private double remaingTime;

	// Token: 0x04003DD3 RID: 15827
	private float fillTime;

	// Token: 0x04003DD4 RID: 15828
	private float[] gaugesFill = new float[4];

	// Token: 0x04003DD5 RID: 15829
	private float researchGaugeFill;

	// Token: 0x04003DD6 RID: 15830
	private bool firstUpdate;

	// Token: 0x04003DD7 RID: 15831
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x04003DD8 RID: 15832
	private const string grDistilleryCorePrefsKey = "_grDistilleryCore";

	// Token: 0x04003DD9 RID: 15833
	private const string grDistilleryStartTimePrefsKey = "_grDistilleryStartTime";
}
