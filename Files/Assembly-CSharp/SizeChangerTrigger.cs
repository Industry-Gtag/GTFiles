using UnityEngine;

public class SizeChangerTrigger : MonoBehaviour, IBuilderPieceComponent
{
	public delegate void SizeChangerTriggerEvent(Collider other);

	private Collider myCollider;

	public bool builderEnterTrigger;

	public bool builderExitOnEnterTrigger;

	public event SizeChangerTriggerEvent OnEnter;

	public event SizeChangerTriggerEvent OnExit;

	private void Awake()
	{
		myCollider = GetComponent<Collider>();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (OnEnter != null)
		{
			OnEnter(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (OnExit != null)
		{
			OnExit(other);
		}
	}

	public Vector3 ClosestPoint(Vector3 position)
	{
		return myCollider.ClosestPoint(position);
	}

	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	public void OnPieceDestroy()
	{
	}

	public void OnPiecePlacementDeserialized()
	{
	}

	public void OnPieceActivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	public void OnPieceDeactivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}
}
