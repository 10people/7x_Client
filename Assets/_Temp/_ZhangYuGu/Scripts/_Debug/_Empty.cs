//#define DEBUG_EMPTY

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _Empty : MonoBehaviour {
	
	#region Mono

	void Awake(){
//		Debug.Log( Time.realtimeSinceStartup + " _Empty.Awake() " + gameObject.name );

//		SparkleEffectItem.OpenSparkle( gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon, -1 );
	}

//	void Update(){
//		
//	}

	public int m_yuan_bao_count = 0;

	void OnClick(){
		Debug.Log( "BuyYuanBao.OnClick( " + m_yuan_bao_count + " )" );

		Bonjour.BuyYuanBao( m_yuan_bao_count );
	}

//	public float delay = 1.0f;

	// Use this for initialization
//	void Start () {
//	IEnumerator Start(){
//		Debug.Log( Time.realtimeSinceStartup + " _Empty.Start() " + gameObject.name );
//
//		if(delay > 0){
//			yield return StartCoroutine("TweenDelay");
//		}
//	}
//
//	IEnumerator TweenDelay(){
//		yield return new WaitForSeconds( 3.0f );
//
//		Debug.Log( "TweenDelay.Done()" );
//	}

//	void OnEnable(){
//		Debug.Log( "_Empty.OnEnable()" );
//
//		ModelAutoActivator.RegisterAutoActivator( m_model_gb, this );
//	}

//	public void SetModelActive( bool p_active ){
//		Debug.Log( "SetModelActiveListener( " + gameObject + " - " + p_active + " )" );
//	}

//	void OnDestroy(){
//		Debug.Log( "_Empty.OnDestroy()" );

//	}

	#endregion
}