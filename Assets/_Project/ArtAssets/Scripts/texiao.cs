using UnityEngine;
using System.Collections;

public class texiao : MonoBehaviour {


	public bool Ins;

	public float SubTime;



	void Start(){
		CalcLifeTime();

		//StartCoroutine("TeXiaoGo");

		StartCoroutine( TeXiaoGo_X() );
	}

	IEnumerator TeXiaoGo(){
		yield return new WaitForSeconds(1);
		while(true){
			while(Ins){
				GameObject texiaotemp = (GameObject)Instantiate(gameObject,transform.position,transform.rotation);
				texiaotemp.GetComponent<texiao>().Ins = false;
				texiaotemp.name = gameObject.name + "(test)";
				texiaotemp.AddComponent<texiao_Des>();
				yield return new WaitForSeconds(SubTime);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	IEnumerator TeXiaoGo_X(){
		yield return new WaitForSeconds(1);
		while( SubTime > 0 ){
			GameObject texiaotemp = (GameObject)Instantiate(gameObject,transform.position,transform.rotation);
			texiaotemp.GetComponent<texiao>().Ins = false;
			texiaotemp.name = gameObject.name + "(test)";
			texiaotemp.AddComponent<texiao_Des>();
			yield return new WaitForSeconds(SubTime);
		}
	}

	void CalcLifeTime(){
		//CalcLifeTimeLegacy();

		CalcLifeTimeShuriken();

		//CalcLifeTrail();
	}

	/*
	void CalcLifeTimeLegacy()
	{
		#if UNITY_EDITOR 
		//get all emitters we need to do scaling on
		ParticleEmitter[] emitters = GetComponentsInChildren<ParticleEmitter>();
		
		//get all animators we need to do scaling on
		ParticleAnimator[] animators = GetComponentsInChildren<ParticleAnimator>();
		
		//apply scaling to emitters
		foreach (ParticleEmitter emitter in emitters)
		{
			emitter.minSize *= scaleFactor;
			emitter.maxSize *= scaleFactor;
			emitter.worldVelocity *= scaleFactor;
			emitter.localVelocity *= scaleFactor;
			emitter.rndVelocity *= scaleFactor;

			//some variables cannot be accessed through regular script, we will acces them through a serialized object
			SerializedObject so = new SerializedObject(emitter);
			
			so.FindProperty("m_Ellipsoid").vector3Value *= scaleFactor;
			so.FindProperty("tangentVelocity").vector3Value *= scaleFactor;
			so.ApplyModifiedProperties();
		}
		
		//apply scaling to animators
		foreach (ParticleAnimator animator in animators)
		{
			animator.force *= scaleFactor;
			animator.rndForce *= scaleFactor;
		}
		#endif
	}
	*/

	void CalcLifeTimeShuriken()
	{
		#if UNITY_EDITOR 
		//get all shuriken systems we need to do scaling on
		ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
		
		foreach (ParticleSystem system in systems){
			if( SubTime < system.time ){
				SubTime = system.time;
			}

			if( SubTime < system.duration ){
				SubTime = system.duration;
			}

		}
		#endif

		Debug.Log( "Max life: " + SubTime );
	}
	
	/*	
	void CalcLifeTrail()
	{
		//get all animators we need to do scaling on
		TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();
		
		//apply scaling to animators
		foreach (TrailRenderer trail in trails)
		{
			trail.startWidth *= scaleFactor;
			trail.endWidth *= scaleFactor;
		}
	}
	*/

}
