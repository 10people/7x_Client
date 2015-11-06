using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.3.30
 * @since:		Unity 4.5.3
 * Function:	Encryption&Decryption functions.
 * 
 * Notes:
 * 1.Rijindael is much slow, only use it for most sensitive data, such as account/password/protocols.
 * 2.Simple Encryption just use xor is enough.
 */
public class EncryptTool{

	#region Simple Encryption

	public static int Encrypt( int p_origin_int ){
		return XorEncryption.EncryptInt( p_origin_int );
	}

	public static int Decrypt( int p_encrypted_int ){
		return XorEncryption.DecryptInt( p_encrypted_int );
	}

	public static float Encrypt( float p_origin_float ){
		return XorEncryption.EncryptFloat( p_origin_float );
	}

	public static float Decrypt( float p_encrypted_float ){
		return XorEncryption.DecryptFloat( p_encrypted_float );
	}

	public static long Encrypt( long p_origin_long ){
		return XorEncryption.EncryptLong( p_origin_long );
	}

	public static long Decrypt( long p_encrypted_long ){
		return XorEncryption.DecryptLong( p_encrypted_long );
	}

	public static string Encrypt( string p_origin_string ){
		return Base64Encryption.EncryptString( p_origin_string );
	}

	public static string Decrypt( string p_encrypted_string ){
		return Base64Encryption.DecryptString( p_encrypted_string );
	}

	#endregion


	public class RijindaelEncryption{

		#region Rijindael Init
		
		public static RijndaelManaged m_rijindael = new RijndaelManaged();
		
		private static bool m_initiated = false;
		
		private static void InitRijindael(){
			if( !m_initiated ){
				m_initiated = true;
				
				ResetRijindael();
			}
			
		}
		
		public static void ResetRijindael(){
			bool t_to_save = false;
			
			{
				string t_key = PlayerPrefs.GetString( ConstInGame.CONST_ENCRYPT_RJDL_KEY, "" );
				
				if( string.IsNullOrEmpty( t_key ) ){
					m_rijindael.GenerateKey();
					
					byte[] t_bytes = m_rijindael.Key;
					
					string t_encrypted_str = Get64StringFromBytes( t_bytes );
					
					PlayerPrefs.SetString( ConstInGame.CONST_ENCRYPT_RJDL_KEY, t_encrypted_str );
					
					t_to_save = true;
				}
				else{
					byte[] t_bytes = GetBytesFrom64( t_key );
					
					m_rijindael.Key = t_bytes;
				}
			}
			
			{
				string t_iv = PlayerPrefs.GetString( ConstInGame.CONST_ENCRYPT_RJDL_IV, "" );
				
				if( string.IsNullOrEmpty( t_iv ) ){
					m_rijindael.GenerateIV();
					
					byte[] t_bytes = m_rijindael.IV;
					
					string t_encrypted_str = Get64StringFromBytes( t_bytes );
					
					PlayerPrefs.SetString( ConstInGame.CONST_ENCRYPT_RJDL_IV, t_encrypted_str );
					
					t_to_save = true;
				}
				else{
					byte[] t_bytes = GetBytesFrom64( t_iv );
					
					m_rijindael.IV = t_bytes;
				}
			}
			
			if( t_to_save ){
				PlayerPrefs.Save();
			}
		}
		
		#endregion
		
		
		
		#region Rijindael int
		
		public static string EncryptInt( int p_int ){
			string t_str = Base64Encryption.EncryptInt( p_int );
			
			return EncryptString( t_str );
		}
		
		public static int DecryptInt( string cipherText ){
			string t_string = DecryptString( cipherText );

			return Base64Encryption.DecryptInt( t_string );
		}
		
		#endregion



		#region Rijindael long
		
		public static string EncryptLong( long p_long ){
			string t_str = Base64Encryption.EncryptLong( p_long );
			
			return EncryptString( t_str );
		}
		
		public static long DecryptLong( string cipherText ){
			string t_string = DecryptString( cipherText );
			
			return Base64Encryption.DecryptLong( t_string );
		}
		
		#endregion
		
		
		
		#region Rijindael float
		
		public static string EncryptFloat( float p_float ){
			string t_str = Base64Encryption.EncryptFloat( p_float );
			
			return EncryptString( t_str );
		}
		
		public static float DecryptFloat( string cipherText ){
			string t_string = DecryptString( cipherText );
			
			return Base64Encryption.DecryptFloat( t_string );
		}
		
		#endregion
		
		
		
		#region Rijindael String
		
		public static string EncryptString( string plainText ){
			byte[] t_bytes = EncryptStringToBytes( plainText );
			
			return Get64StringFromBytes( t_bytes );
		}
		
		public static string DecryptString( string cipherText ){
			byte[] bytes = GetBytesFrom64( cipherText );
			
			return DecryptStringFromBytes( bytes );
		}
		
		#endregion
		
		
		
		#region Rijindael byte[]
		
		public static string EncryptBytes( byte[] p_bytes ){
			string t_str = Get64StringFromBytes( p_bytes );
			
			return EncryptString( t_str );
		}
		
		public static byte[] DecryptBytes( string cipherText ){
			string t_str = DecryptString( cipherText );
			
			return GetBytesFrom64( t_str );
		}
		
		#endregion
		
		
		
		#region Rijindael Utilities
		
		private static byte[] EncryptStringToBytes( string p_origin_text ){
			InitRijindael();
			
			byte[] p_encrypted_bytes;
			
			using ( RijndaelManaged t_rijindael = new RijndaelManaged() ){
				if ( p_origin_text == null || p_origin_text.Length <= 0 ){
					Debug.LogError( "plainText" );
				}
				
				if ( m_rijindael.Key == null || m_rijindael.Key.Length <= 0 ){
					Debug.LogError( "Key" );
				}
				
				if ( m_rijindael.IV == null || m_rijindael.IV.Length <= 0 ){
					Debug.LogError( "IV" );
				}
				
				t_rijindael.Key = m_rijindael.Key;
				
				t_rijindael.IV = m_rijindael.IV;
				
				ICryptoTransform encryptor = t_rijindael.CreateEncryptor( t_rijindael.Key, t_rijindael.IV );
				
				using ( MemoryStream t_stream = new MemoryStream() ){
					using ( CryptoStream t_crypt_stream = new CryptoStream( t_stream, encryptor, CryptoStreamMode.Write ) ){
						using ( StreamWriter swEncrypt = new StreamWriter( t_crypt_stream ) ){
							swEncrypt.Write( p_origin_text );
						}
						
						p_encrypted_bytes = t_stream.ToArray();
					}
				}
			}
			
			return p_encrypted_bytes;
			
		}
		
		private static string DecryptStringFromBytes( byte[] p_cipher_bytes ){
			InitRijindael();
			
			string t_origin_text = null;
			
			using( RijndaelManaged t_rijindael = new RijndaelManaged() ){
				if (p_cipher_bytes == null || p_cipher_bytes.Length <= 0){
					Debug.LogError( "cipherText" );
				}
				
				if ( m_rijindael.Key == null || m_rijindael.Key.Length <= 0 ){
					Debug.LogError( "Key" );
				}
				
				if ( m_rijindael.IV == null || m_rijindael.IV.Length <= 0 ){
					Debug.LogError( "IV" );
				}
				
				t_rijindael.Key = m_rijindael.Key;
				
				t_rijindael.IV = m_rijindael.IV;
				
				ICryptoTransform decryptor = t_rijindael.CreateDecryptor( t_rijindael.Key, t_rijindael.IV );
				
				using ( MemoryStream t_mem_stream = new MemoryStream( p_cipher_bytes ) ){
					using ( CryptoStream t_crypto_stream = new CryptoStream( t_mem_stream, decryptor, CryptoStreamMode.Read ) ){
						using ( StreamReader t_stream_reader = new StreamReader( t_crypto_stream ) ){
							t_origin_text = t_stream_reader.ReadToEnd();
						}
					}
				}
			}
			
			return t_origin_text;
		}
		
		#endregion
	}

	
	
	
	public class Base64Encryption{

		#region int
		public static string EncryptInt( int p_int ){
			byte[] t_bytes = System.BitConverter.GetBytes( p_int );
			
			string t_str = Get64StringFromBytes( t_bytes );

			return t_str;
		}

		public static int DecryptInt( string p_encrypted_string ){
			byte[] bytes = GetBytesFrom64( p_encrypted_string );
			
			return System.BitConverter.ToInt32( bytes, 0 );
		}

		#endregion



		#region long
		public static string EncryptLong( long p_long ){
			byte[] t_bytes = System.BitConverter.GetBytes( p_long );
			
			string t_str = Get64StringFromBytes( t_bytes );
			
			return t_str;
		}
		
		public static long DecryptLong( string p_encrypted_string ){
			byte[] bytes = GetBytesFrom64( p_encrypted_string );
			
			return System.BitConverter.ToInt64( bytes, 0 );
		}
		
		#endregion



		#region float

		public static string EncryptFloat( float p_float ){
			byte[] t_bytes = System.BitConverter.GetBytes( p_float );
			
			return Get64StringFromBytes( t_bytes );
		}
		
		public static float DecryptFloat( string p_encrypted_string ){
			byte[] bytes = GetBytesFrom64( p_encrypted_string );
			
			return System.BitConverter.ToSingle( bytes, 0 );
		}

		#endregion



		#region string

		public static string EncryptString( string p_string ){
			byte[] t_bytes = Encoding.ASCII.GetBytes( p_string );

			return Get64StringFromBytes( t_bytes );
		}

		public static string DecryptString( string p_encrypt_string ){
			byte[] t_bytes = GetBytesFrom64( p_encrypt_string );

			return Encoding.ASCII.GetString( t_bytes );
		}

		#endregion
	}



	public class XorEncryption{

		#region Xor Init
		
		private static bool m_xor_initiated = false;
		
		private static void InitXor(){
			if( !m_xor_initiated ){
				m_xor_initiated = true;
				
				m_int_encrypt_key = Random.Range( int.MinValue, int.MaxValue );
				
				m_long_encrypt_key = Random.Range( int.MinValue, int.MaxValue );
				
				InitXorFloat();
			}
		}
		
		#endregion
		
		
		
		#region Xor Int
		
		public static int m_int_encrypt_key = 0;
		
		public static int DecryptInt( int p_encrypted_int ){
			InitXor();
			
			int t_value = p_encrypted_int ^ m_int_encrypt_key;
			
			return t_value;
		}
		
		public static int EncryptInt( int p_origin_int ){
			InitXor();
			
			int t_value = p_origin_int ^ m_int_encrypt_key;
			
			return t_value;
		}
		
		#endregion



		#region Xor Float
		
		private static int m_float_encrypt_key = 0;
		
		public static void InitXorFloat(){
			float t_float = Random.Range( float.MinValue, float.MaxValue );
			
			byte[] t_key_bytes = System.BitConverter.GetBytes( t_float );
			
			m_float_encrypt_key = System.BitConverter.ToInt32( t_key_bytes, 0 );
		}
		
		public static float DecryptFloat( float p_encrypted_float ){
			InitXor();
			
			byte[] t_encrypted_bytes = System.BitConverter.GetBytes( p_encrypted_float );
			
			int t_encrypted_int = System.BitConverter.ToInt32( t_encrypted_bytes, 0 );
			
			{
				int t_origin_int = t_encrypted_int ^ m_float_encrypt_key;
				
				byte[] t_origin_bytes = System.BitConverter.GetBytes( t_origin_int );
				
				return System.BitConverter.ToSingle( t_origin_bytes, 0 );
			}
		}
		
		public static float EncryptFloat( float p_origin_float ){
			InitXor();
			
			byte[] t_origin_bytes = System.BitConverter.GetBytes( p_origin_float );
			
			int t_origin_int = System.BitConverter.ToInt32( t_origin_bytes, 0 );
			
			{
				int t_encrypted_int = t_origin_int ^ m_float_encrypt_key;
				
				byte[] t_encrypt_bytes = System.BitConverter.GetBytes( t_encrypted_int );
				
				return System.BitConverter.ToSingle( t_encrypt_bytes, 0 );
			}
		}
		
		#endregion

		
		
		#region Xor Long
		
		public static long m_long_encrypt_key = 0;
		
		public static long DecryptLong( long p_encrypted_long ){
			InitXor();
			
			long t_value = p_encrypted_long ^ m_long_encrypt_key;
			
			return t_value;
		}
		
		public static long EncryptLong( long p_origin_long ){
			InitXor();
			
			long t_value = p_origin_long ^ m_long_encrypt_key;
			
			return t_value;
		}
		
		#endregion

	}



	#region Utilities

	public static byte[] GetBytesFrom64( string p_64_string ){
		return System.Convert.FromBase64String( p_64_string );
	}

	public static string Get64StringFromBytes( byte[] p_bytes ){
		return System.Convert.ToBase64String( p_bytes );
	}


	#endregion
}