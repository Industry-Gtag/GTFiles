using System;
using System.IO;
using K4os.Compression.LZ4;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x02001380 RID: 4992
	public static class ChunkIO
	{
		// Token: 0x06007D01 RID: 32001 RVA: 0x0028DF0E File Offset: 0x0028C10E
		public static string PathFor(int3 id)
		{
			return Path.Combine(ChunkIO.Root, string.Format("{0}_{1}_{2}.vox", id.x, id.y, id.z));
		}

		// Token: 0x06007D02 RID: 32002 RVA: 0x0028DF48 File Offset: 0x0028C148
		public static void SaveChunk(ChunkDTO dto)
		{
			string text = ChunkIO.PathFor(dto.Id);
			Debug.Log(string.Format("Saving chunk {0} to {1}", dto.Id, text));
			ChunkIO.Save(text, in dto);
		}

		// Token: 0x06007D03 RID: 32003 RVA: 0x0028DF84 File Offset: 0x0028C184
		public static bool TryLoadChunk(int3 id, out ChunkDTO dto)
		{
			string text = ChunkIO.PathFor(id);
			if (!File.Exists(ChunkIO.PathFor(id)))
			{
				dto = default(ChunkDTO);
				return false;
			}
			dto = ChunkIO.Load(ChunkIO.PathFor(id), Allocator.Persistent);
			if (dto.IsValid)
			{
				Debug.Log(string.Format("Loaded chunk {0} from {1}", id, text));
			}
			else
			{
				Debug.Log(string.Format("Chunk {0} at {1} magic or version mismatch.", id, text));
			}
			return dto.IsValid;
		}

		// Token: 0x06007D04 RID: 32004 RVA: 0x0028DFFC File Offset: 0x0028C1FC
		public static void Save(string path, in ChunkDTO chunk)
		{
			if (!Directory.Exists(ChunkIO.Root))
			{
				Directory.CreateDirectory(ChunkIO.Root);
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, false))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					ChunkIO.WriteChunk(binaryWriter, in chunk);
				}
			}
		}

		// Token: 0x06007D05 RID: 32005 RVA: 0x0028E070 File Offset: 0x0028C270
		public static byte[] SerializeChunk(in ChunkDTO chunk)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream(4096))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ChunkIO.WriteChunk(binaryWriter, in chunk);
					binaryWriter.Flush();
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		// Token: 0x06007D06 RID: 32006 RVA: 0x0028E0D8 File Offset: 0x0028C2D8
		private static void WriteChunk(BinaryWriter bw, in ChunkDTO chunk)
		{
			bw.Write(1448040524U);
			bw.Write(5);
			bw.Write(chunk.WorldId);
			bw.Write(chunk.Id.x);
			bw.Write(chunk.Id.y);
			bw.Write(chunk.Id.z);
			bw.Write(chunk.Size.x);
			bw.Write(chunk.Size.y);
			bw.Write(chunk.Size.z);
			bw.Write(chunk.Dimensions.x);
			bw.Write(chunk.Dimensions.y);
			bw.Write(chunk.Dimensions.z);
			ChunkIO.WriteNativeArray(bw, chunk.Density);
			ChunkIO.WriteNativeArray(bw, chunk.Material);
		}

		// Token: 0x06007D07 RID: 32007 RVA: 0x0028E1B4 File Offset: 0x0028C3B4
		public static ChunkDTO Load(string path, Allocator alloc = Allocator.Persistent)
		{
			ChunkDTO chunkDTO;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					chunkDTO = ChunkIO.ReadChunk(binaryReader, alloc);
				}
			}
			return chunkDTO;
		}

		// Token: 0x06007D08 RID: 32008 RVA: 0x0028E214 File Offset: 0x0028C414
		public static bool TryDeserializeChunk(in byte[] data, out ChunkDTO dto)
		{
			dto = ChunkIO.DeserializeChunk(in data, Allocator.Persistent);
			return dto.IsValid;
		}

		// Token: 0x06007D09 RID: 32009 RVA: 0x0028E22C File Offset: 0x0028C42C
		public static ChunkDTO DeserializeChunk(in byte[] data, Allocator alloc = Allocator.Persistent)
		{
			ChunkDTO chunkDTO;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					chunkDTO = ChunkIO.ReadChunk(binaryReader, Allocator.Persistent);
				}
			}
			return chunkDTO;
		}

		// Token: 0x06007D0A RID: 32010 RVA: 0x0028E284 File Offset: 0x0028C484
		private static ChunkDTO ReadChunk(BinaryReader br, Allocator alloc = Allocator.Persistent)
		{
			int num = (int)br.ReadUInt32();
			int num2 = br.ReadInt32();
			if (num != 1448040524 || num2 != 5)
			{
				return default(ChunkDTO);
			}
			int num3 = br.ReadInt32();
			int3 @int = new int3(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
			int3 int2 = new int3(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
			int3 int3 = new int3(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
			NativeArray<byte> nativeArray = ChunkIO.ReadNativeArray(br, alloc);
			NativeArray<byte> nativeArray2 = ChunkIO.ReadNativeArray(br, alloc);
			return new ChunkDTO
			{
				WorldId = num3,
				Id = @int,
				Size = int2,
				Dimensions = int3,
				Density = nativeArray,
				Material = nativeArray2
			};
		}

		// Token: 0x06007D0B RID: 32011 RVA: 0x0028E358 File Offset: 0x0028C558
		private static void WriteNativeArray(BinaryWriter bw, NativeArray<byte> src)
		{
			byte[] array = LZ4Pickler.Pickle(src.ToArray(), LZ4Level.L00_FAST);
			bw.Write(array.Length);
			bw.Write(array);
		}

		// Token: 0x06007D0C RID: 32012 RVA: 0x0028E384 File Offset: 0x0028C584
		private static NativeArray<byte> ReadNativeArray(BinaryReader br, Allocator alloc = Allocator.Persistent)
		{
			int num = br.ReadInt32();
			byte[] array = LZ4Pickler.Unpickle(br.ReadBytes(num));
			int num2 = array.Length;
			NativeArray<byte> nativeArray = new NativeArray<byte>(num2, alloc, NativeArrayOptions.ClearMemory);
			NativeArray<byte>.Copy(array, nativeArray, num2);
			return nativeArray;
		}

		// Token: 0x06007D0D RID: 32013 RVA: 0x0028E3BA File Offset: 0x0028C5BA
		public static void DeleteWorld()
		{
			if (Directory.Exists(ChunkIO.Root))
			{
				Directory.Delete(ChunkIO.Root, true);
			}
			Debug.Log("All chunks deleted.");
		}

		// Token: 0x04008FDB RID: 36827
		public const uint MAGIC = 1448040524U;

		// Token: 0x04008FDC RID: 36828
		public const int VERSION = 5;

		// Token: 0x04008FDD RID: 36829
		private static readonly string Root = Path.Combine(Application.persistentDataPath, "WorldSaves");
	}
}
