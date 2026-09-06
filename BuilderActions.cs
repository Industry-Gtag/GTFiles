using System;
using UnityEngine;

// Token: 0x02000629 RID: 1577
public class BuilderActions
{
	// Token: 0x06002734 RID: 10036 RVA: 0x000CF5F4 File Offset: 0x000CD7F4
	public static BuilderAction CreateAttachToPlayer(int cmdId, int pieceId, Vector3 localPosition, Quaternion localRotation, int actorNumber, bool leftHand)
	{
		return new BuilderAction
		{
			type = BuilderActionType.AttachToPlayer,
			localCommandId = cmdId,
			pieceId = pieceId,
			playerActorNumber = actorNumber,
			localPosition = localPosition,
			localRotation = localRotation,
			isLeftHand = leftHand
		};
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x000CF644 File Offset: 0x000CD844
	public static BuilderAction CreateAttachToPlayerRollback(int cmdId, BuilderPiece piece)
	{
		return BuilderActions.CreateAttachToPlayer(cmdId, piece.pieceId, piece.transform.localPosition, piece.transform.localRotation, piece.heldByPlayerActorNumber, piece.heldInLeftHand);
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x000CF674 File Offset: 0x000CD874
	public static BuilderAction CreateDetachFromPlayer(int cmdId, int pieceId, int actorNumber)
	{
		return new BuilderAction
		{
			type = BuilderActionType.DetachFromPlayer,
			localCommandId = cmdId,
			pieceId = pieceId,
			playerActorNumber = actorNumber
		};
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x000CF6AC File Offset: 0x000CD8AC
	public static BuilderAction CreateAttachToPiece(int cmdId, int pieceId, int parentPieceId, int attachIndex, int parentAttachIndex, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int actorNumber, int timeStamp)
	{
		return new BuilderAction
		{
			type = BuilderActionType.AttachToPiece,
			localCommandId = cmdId,
			pieceId = pieceId,
			parentPieceId = parentPieceId,
			attachIndex = attachIndex,
			parentAttachIndex = parentAttachIndex,
			bumpOffsetx = bumpOffsetX,
			bumpOffsetz = bumpOffsetZ,
			twist = twist,
			playerActorNumber = actorNumber,
			timeStamp = timeStamp
		};
	}

	// Token: 0x06002738 RID: 10040 RVA: 0x000CF720 File Offset: 0x000CD920
	public static BuilderAction CreateAttachToPieceRollback(int cmdId, BuilderPiece piece, int actorNumber)
	{
		byte pieceTwist = piece.GetPieceTwist();
		sbyte b;
		sbyte b2;
		piece.GetPieceBumpOffset(pieceTwist, out b, out b2);
		return BuilderActions.CreateAttachToPiece(cmdId, piece.pieceId, piece.parentPiece.pieceId, piece.attachIndex, piece.parentAttachIndex, b, b2, pieceTwist, actorNumber, piece.activatedTimeStamp);
	}

	// Token: 0x06002739 RID: 10041 RVA: 0x000CF76C File Offset: 0x000CD96C
	public static BuilderAction CreateDetachFromPiece(int cmdId, int pieceId, int actorNumber)
	{
		return new BuilderAction
		{
			type = BuilderActionType.DetachFromPiece,
			localCommandId = cmdId,
			pieceId = pieceId,
			playerActorNumber = actorNumber
		};
	}

	// Token: 0x0600273A RID: 10042 RVA: 0x000CF7A4 File Offset: 0x000CD9A4
	public static BuilderAction CreateMakeRoot(int cmdId, int pieceId)
	{
		return new BuilderAction
		{
			type = BuilderActionType.MakePieceRoot,
			localCommandId = cmdId,
			pieceId = pieceId
		};
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x000CF7D4 File Offset: 0x000CD9D4
	public static BuilderAction CreateDropPiece(int cmdId, int pieceId, Vector3 localPosition, Quaternion localRotation, Vector3 velocity, Vector3 angVelocity, int actorNumber)
	{
		return new BuilderAction
		{
			type = BuilderActionType.DropPiece,
			localCommandId = cmdId,
			pieceId = pieceId,
			localPosition = localPosition,
			localRotation = localRotation,
			velocity = velocity,
			angVelocity = angVelocity,
			playerActorNumber = actorNumber
		};
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x000CF830 File Offset: 0x000CDA30
	public static BuilderAction CreateDropPieceRollback(int cmdId, BuilderPiece rootPiece, int actorNumber)
	{
		Vector3 vector = rootPiece.transform.position;
		Quaternion quaternion = rootPiece.transform.rotation;
		Vector3 vector2 = Vector3.zero;
		Vector3 vector3 = Vector3.zero;
		Rigidbody component = rootPiece.GetComponent<Rigidbody>();
		if (component != null)
		{
			vector = component.position;
			quaternion = component.rotation;
			vector2 = component.linearVelocity;
			vector3 = component.angularVelocity;
		}
		return BuilderActions.CreateDropPiece(cmdId, rootPiece.pieceId, vector, quaternion, vector2, vector3, actorNumber);
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x000CF8A4 File Offset: 0x000CDAA4
	public static BuilderAction CreateAttachToShelfRollback(int cmdId, BuilderPiece piece, int shelfID, bool isConveyor, int timestamp = 0, float splineTime = 0f)
	{
		return new BuilderAction
		{
			type = BuilderActionType.AttachToShelf,
			localCommandId = cmdId,
			pieceId = piece.pieceId,
			attachIndex = shelfID,
			parentAttachIndex = timestamp,
			isLeftHand = isConveyor,
			velocity = new Vector3(splineTime, 0f, 0f),
			localPosition = piece.transform.localPosition,
			localRotation = piece.transform.localRotation
		};
	}
}
