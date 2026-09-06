using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020008FC RID: 2300
public class HauntedObject : MonoBehaviour
{
	// Token: 0x06003C50 RID: 15440 RVA: 0x001495D8 File Offset: 0x001477D8
	private void Awake()
	{
		this.lurkerGhost = GameObject.FindGameObjectWithTag("LurkerGhost");
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.wanderingGhost = GameObject.FindGameObjectWithTag("WanderingGhost");
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.animators = base.transform.GetComponentsInChildren<Animator>();
	}

	// Token: 0x06003C51 RID: 15441 RVA: 0x00149694 File Offset: 0x00147894
	private void OnDestroy()
	{
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
	}

	// Token: 0x06003C52 RID: 15442 RVA: 0x0014971F File Offset: 0x0014791F
	private void Start()
	{
		this.initialPos = base.transform.position;
		this.passedTime = 0f;
		this.lightPassedTime = 0f;
	}

	// Token: 0x06003C53 RID: 15443 RVA: 0x00149748 File Offset: 0x00147948
	private void TriggerEffects(GameObject go)
	{
		if (base.gameObject != go)
		{
			return;
		}
		if (this.rattle)
		{
			base.StartCoroutine(this.Shake());
		}
		if (this.audioSource && this.hauntedSound)
		{
			this.audioSource.GTPlayOneShot(this.hauntedSound, 1f);
		}
		if (this.FBXprefab)
		{
			ObjectPools.instance.Instantiate(this.FBXprefab, base.transform.position, true);
		}
		if (this.TurnOffLight != null)
		{
			base.StartCoroutine(this.TurnOff());
		}
		foreach (Animator animator in this.animators)
		{
			if (animator)
			{
				animator.SetTrigger(HauntedObject._animHaunted);
			}
		}
	}

	// Token: 0x06003C54 RID: 15444 RVA: 0x0014981A File Offset: 0x00147A1A
	private IEnumerator Shake()
	{
		while (this.passedTime < this.duration)
		{
			this.passedTime += Time.deltaTime;
			base.transform.position = new Vector3(this.initialPos.x + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.y + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.z);
			yield return null;
		}
		this.passedTime = 0f;
		yield break;
	}

	// Token: 0x06003C55 RID: 15445 RVA: 0x00149829 File Offset: 0x00147A29
	private IEnumerator TurnOff()
	{
		this.TurnOffLight.gameObject.SetActive(false);
		while (this.lightPassedTime < this.TurnOffDuration)
		{
			this.lightPassedTime += Time.deltaTime;
			yield return null;
		}
		this.TurnOffLight.SetActive(true);
		this.lightPassedTime = 0f;
		yield break;
	}

	// Token: 0x04004CF7 RID: 19703
	private static readonly int _animHaunted = Animator.StringToHash("Haunted");

	// Token: 0x04004CF8 RID: 19704
	private const string _lurkerGhost = "LurkerGhost";

	// Token: 0x04004CF9 RID: 19705
	private const string _wanderingGhost = "WanderingGhost";

	// Token: 0x04004CFA RID: 19706
	[Tooltip("If this box is checked, then object will rattle when hunted")]
	public bool rattle;

	// Token: 0x04004CFB RID: 19707
	public float speed = 60f;

	// Token: 0x04004CFC RID: 19708
	public float amount = 0.01f;

	// Token: 0x04004CFD RID: 19709
	public float duration = 1f;

	// Token: 0x04004CFE RID: 19710
	[FormerlySerializedAs("FBX")]
	public GameObject FBXprefab;

	// Token: 0x04004CFF RID: 19711
	[Tooltip("Use to turn off a game object like candle flames when hunted")]
	public GameObject TurnOffLight;

	// Token: 0x04004D00 RID: 19712
	public float TurnOffDuration = 2f;

	// Token: 0x04004D01 RID: 19713
	private Vector3 initialPos;

	// Token: 0x04004D02 RID: 19714
	private float passedTime;

	// Token: 0x04004D03 RID: 19715
	private float lightPassedTime;

	// Token: 0x04004D04 RID: 19716
	private GameObject lurkerGhost;

	// Token: 0x04004D05 RID: 19717
	private GameObject wanderingGhost;

	// Token: 0x04004D06 RID: 19718
	private Animator[] animators;

	// Token: 0x04004D07 RID: 19719
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004D08 RID: 19720
	[FormerlySerializedAs("rattlingSound")]
	public AudioClip hauntedSound;
}
