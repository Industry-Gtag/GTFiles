using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// Token: 0x02000AE3 RID: 2787
public static class AESHMAC
{
	// Token: 0x0600477A RID: 18298 RVA: 0x00181738 File Offset: 0x0017F938
	public static byte[] NewKey()
	{
		byte[] array = new byte[32];
		AESHMAC.gRNG.GetBytes(array);
		return array;
	}

	// Token: 0x0600477B RID: 18299 RVA: 0x00181759 File Offset: 0x0017F959
	public static string SimpleEncrypt(string plaintext, byte[] key, byte[] auth, byte[] salt = null)
	{
		if (string.IsNullOrEmpty(plaintext))
		{
			throw new ArgumentNullException("plaintext");
		}
		return Convert.ToBase64String(AESHMAC.SimpleEncrypt(Encoding.UTF8.GetBytes(plaintext), key, auth, salt));
	}

	// Token: 0x0600477C RID: 18300 RVA: 0x00181788 File Offset: 0x0017F988
	public static string SimpleDecrypt(string ciphertext, byte[] key, byte[] auth, int saltLength = 0)
	{
		if (string.IsNullOrWhiteSpace(ciphertext))
		{
			throw new ArgumentNullException("ciphertext");
		}
		byte[] array = AESHMAC.SimpleDecrypt(Convert.FromBase64String(ciphertext), key, auth, saltLength);
		if (array != null)
		{
			return Encoding.UTF8.GetString(array);
		}
		return null;
	}

	// Token: 0x0600477D RID: 18301 RVA: 0x001817C7 File Offset: 0x0017F9C7
	public static string SimpleEncryptWithKey(string plaintext, string key, byte[] salt = null)
	{
		if (string.IsNullOrEmpty(plaintext))
		{
			throw new ArgumentNullException("plaintext");
		}
		return Convert.ToBase64String(AESHMAC.SimpleEncryptWithKey(Encoding.UTF8.GetBytes(plaintext), key, salt));
	}

	// Token: 0x0600477E RID: 18302 RVA: 0x001817F4 File Offset: 0x0017F9F4
	public static string SimpleDecryptWithKey(string ciphertext, string key, int saltLength = 0)
	{
		if (string.IsNullOrWhiteSpace(ciphertext))
		{
			throw new ArgumentNullException("ciphertext");
		}
		byte[] array = AESHMAC.SimpleDecryptWithKey(Convert.FromBase64String(ciphertext), key, saltLength);
		if (array != null)
		{
			return Encoding.UTF8.GetString(array);
		}
		return null;
	}

	// Token: 0x0600477F RID: 18303 RVA: 0x00181834 File Offset: 0x0017FA34
	public static byte[] SimpleEncrypt(byte[] plaintext, byte[] key, byte[] auth, byte[] salt = null)
	{
		if (key == null || key.Length != 32)
		{
			throw new ArgumentException(string.Format("Key must be {0} bits", 256), "key");
		}
		if (auth == null || auth.Length != 32)
		{
			throw new ArgumentException(string.Format("Auth must be {0} bits", 256), "auth");
		}
		if (plaintext == null || plaintext.Length < 1)
		{
			throw new ArgumentNullException("plaintext");
		}
		if (salt == null)
		{
			salt = new byte[0];
		}
		byte[] iv;
		byte[] array;
		using (AesManaged aesManaged = new AesManaged
		{
			KeySize = 256,
			BlockSize = 128,
			Mode = CipherMode.CBC,
			Padding = PaddingMode.PKCS7
		})
		{
			aesManaged.GenerateIV();
			iv = aesManaged.IV;
			using (ICryptoTransform cryptoTransform = aesManaged.CreateEncryptor(key, iv))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
						{
							binaryWriter.Write(plaintext);
						}
					}
					array = memoryStream.ToArray();
				}
			}
		}
		byte[] array3;
		using (HMACSHA256 hmacsha = new HMACSHA256(auth))
		{
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				using (BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2))
				{
					binaryWriter2.Write(salt);
					binaryWriter2.Write(iv);
					binaryWriter2.Write(array);
					binaryWriter2.Flush();
					byte[] array2 = hmacsha.ComputeHash(memoryStream2.ToArray());
					binaryWriter2.Write(array2);
				}
				array3 = memoryStream2.ToArray();
			}
		}
		return array3;
	}

	// Token: 0x06004780 RID: 18304 RVA: 0x00181A40 File Offset: 0x0017FC40
	public static byte[] SimpleDecrypt(byte[] ciphertext, byte[] key, byte[] auth, int saltLength = 0)
	{
		if (key == null || key.Length != 32)
		{
			throw new ArgumentException(string.Format("Key must be {0} bits", 256), "key");
		}
		if (auth == null || auth.Length != 32)
		{
			throw new ArgumentException(string.Format("Auth must be {0} bits", 256), "auth");
		}
		if (ciphertext == null || ciphertext.Length == 0)
		{
			throw new ArgumentNullException("ciphertext");
		}
		byte[] array3;
		using (HMACSHA256 hmacsha = new HMACSHA256(auth))
		{
			byte[] array = new byte[hmacsha.HashSize / 8];
			byte[] array2 = hmacsha.ComputeHash(ciphertext, 0, ciphertext.Length - array.Length);
			int num = 16;
			if (ciphertext.Length < array.Length + saltLength + num)
			{
				array3 = null;
			}
			else
			{
				Array.Copy(ciphertext, ciphertext.Length - array.Length, array, 0, array.Length);
				int num2 = 0;
				for (int i = 0; i < array.Length; i++)
				{
					num2 |= (int)(array[i] ^ array2[i]);
				}
				if (num2 != 0)
				{
					array3 = null;
				}
				else
				{
					using (AesManaged aesManaged = new AesManaged
					{
						KeySize = 256,
						BlockSize = 128,
						Mode = CipherMode.CBC,
						Padding = PaddingMode.PKCS7
					})
					{
						byte[] array4 = new byte[num];
						Array.Copy(ciphertext, saltLength, array4, 0, array4.Length);
						using (ICryptoTransform cryptoTransform = aesManaged.CreateDecryptor(key, array4))
						{
							using (MemoryStream memoryStream = new MemoryStream())
							{
								using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
								{
									using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
									{
										binaryWriter.Write(ciphertext, saltLength + array4.Length, ciphertext.Length - saltLength - array4.Length - array.Length);
									}
								}
								array3 = memoryStream.ToArray();
							}
						}
					}
				}
			}
		}
		return array3;
	}

	// Token: 0x06004781 RID: 18305 RVA: 0x00181C9C File Offset: 0x0017FE9C
	public static byte[] SimpleEncryptWithKey(byte[] plaintext, string key, byte[] salt = null)
	{
		if (salt == null)
		{
			salt = new byte[0];
		}
		if (string.IsNullOrWhiteSpace(key) || key.Length < 12)
		{
			throw new ArgumentException(string.Format("Key must be at least {0} characters", 12), "key");
		}
		if (plaintext == null || plaintext.Length == 0)
		{
			throw new ArgumentNullException("plaintext");
		}
		byte[] array = new byte[16 + salt.Length];
		Array.Copy(salt, array, salt.Length);
		int num = salt.Length;
		byte[] bytes;
		using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, 8, 10000))
		{
			byte[] salt2 = rfc2898DeriveBytes.Salt;
			bytes = rfc2898DeriveBytes.GetBytes(32);
			Array.Copy(salt2, 0, array, num, salt2.Length);
			num += salt2.Length;
		}
		byte[] bytes2;
		using (Rfc2898DeriveBytes rfc2898DeriveBytes2 = new Rfc2898DeriveBytes(key, 8, 10000))
		{
			byte[] salt3 = rfc2898DeriveBytes2.Salt;
			bytes2 = rfc2898DeriveBytes2.GetBytes(32);
			Array.Copy(salt3, 0, array, num, salt3.Length);
		}
		return AESHMAC.SimpleEncrypt(plaintext, bytes, bytes2, array);
	}

	// Token: 0x06004782 RID: 18306 RVA: 0x00181DB4 File Offset: 0x0017FFB4
	public static byte[] SimpleDecryptWithKey(byte[] ciphertext, string key, int saltLength = 0)
	{
		if (string.IsNullOrWhiteSpace(key) || key.Length < 12)
		{
			throw new ArgumentException(string.Format("Key must be at least {0} characters", 12), "key");
		}
		if (ciphertext == null || ciphertext.Length == 0)
		{
			throw new ArgumentNullException("ciphertext");
		}
		byte[] array = new byte[8];
		byte[] array2 = new byte[8];
		Array.Copy(ciphertext, saltLength, array, 0, array.Length);
		Array.Copy(ciphertext, saltLength + array.Length, array2, 0, array2.Length);
		byte[] array3 = AESHMAC.Rfc2898DeriveBytes(key, array, 10000, 32);
		byte[] array4 = AESHMAC.Rfc2898DeriveBytes(key, array2, 10000, 32);
		return AESHMAC.SimpleDecrypt(ciphertext, array3, array4, array.Length + array2.Length + saltLength);
	}

	// Token: 0x06004783 RID: 18307 RVA: 0x00181E5C File Offset: 0x0018005C
	private static byte[] Rfc2898DeriveBytes(string password, byte[] salt, int iterations, int numBytes)
	{
		byte[] bytes;
		using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
		{
			bytes = rfc2898DeriveBytes.GetBytes(numBytes);
		}
		return bytes;
	}

	// Token: 0x04005A09 RID: 23049
	private static readonly RandomNumberGenerator gRNG = RandomNumberGenerator.Create();

	// Token: 0x04005A0A RID: 23050
	public const int BlockBitSize = 128;

	// Token: 0x04005A0B RID: 23051
	public const int KeyBitSize = 256;

	// Token: 0x04005A0C RID: 23052
	public const int SaltBitSize = 64;

	// Token: 0x04005A0D RID: 23053
	public const int Iterations = 10000;

	// Token: 0x04005A0E RID: 23054
	public const int MinPasswordLength = 12;
}
