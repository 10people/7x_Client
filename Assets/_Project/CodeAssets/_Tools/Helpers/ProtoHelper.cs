#define DEBUG_PROTO_HELPER

using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using qxmobile.protobuf;


/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.9.9
 * @since:		Unity 5.1.2
 * Function:	Cache proto object to make ui experience more comfortable.
 * 
 * Notes:
 * None.
 */ 
public class ProtoHelper : Singleton<ProtoHelper>{

	#region Mono

	void OnDestroy(){
		base.OnDestroy();
	}

	#endregion


	#region Send Proto

	/// From LiangXiao.
	public static void SendQXMessage( object value, int protoIndex ){
		MemoryStream memStream = new MemoryStream();
		
		QiXiongSerializer qxSer = new QiXiongSerializer();

		qxSer.Serialize(memStream, value);

		byte[] t_protof = memStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage( (short)protoIndex, ref t_protof );
	}

	/// From LiangXiao.
	public static void SendQXMessage(int protoIndex){
		SocketTool.Instance().SendSocketMessage((short)protoIndex);
	}

	/// From LiangXiao.
	/// Modified, remove index check, remove object.
	public static object DeserializeProto( object targetObject, QXBuffer p_message ){
		MemoryStream memStream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );

		QiXiongSerializer qxSer = new QiXiongSerializer();

		qxSer.Deserialize( memStream, targetObject, targetObject.GetType() );

		return targetObject;
	}

	#endregion


	#region Cache Proto Objects

	private Dictionary<string, System.Object> m_cached_proto_objects = new Dictionary<string, System.Object>();

	/// Cache object with specified key, get it when show the same UI next time.
	/// 
	/// Params:
	/// 1.key of th cached object, suggest named by "ClassName.ProtoObjectName.Index";
	/// 2.the object to cache;
	public void CacheProtoObject( string p_cache_key, System.Object p_object_to_cache ){
		if( m_cached_proto_objects.ContainsKey ( p_cache_key ) ) {
			Debug.LogError( "Key already contaiend, please remove it first: " + p_cache_key );

			return;
		}

		m_cached_proto_objects[ p_cache_key ] = p_object_to_cache;
	}

	/// Remove cached ProtoObject by hand.
	public void RemoveCachedProtoObject( string p_cache_key ){
		if ( m_cached_proto_objects.ContainsKey ( p_cache_key  ) ){
			m_cached_proto_objects.Remove( p_cache_key );
		}
	}

	/// Get Cached ProtoObject and remove it from the cache.
	/// 
	/// Notes:
	/// 1.remove the cache to prevent potential bugs.
	public System.Object GetCachedProtoObject( string p_cached_key ){
		if ( m_cached_proto_objects.ContainsKey ( p_cached_key ) ) {
			System.Object t_cached_object = m_cached_proto_objects[ p_cached_key ];

			RemoveCachedProtoObject( p_cached_key );

			return t_cached_object;
		}

		return null;
	}

	#endregion
}
