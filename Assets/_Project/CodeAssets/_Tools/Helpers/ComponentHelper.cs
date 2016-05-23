//#define DEBUG_MONO_HELPER

using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class ComponentHelper{
	
	#region Register Global Component

	/// <summary>
	/// Register global components which never destroyed here.
	/// </summary>
	public static void RegisterGlobalComponents(){
		#if DEBUG_GAME_OBJECT_HELPER
		Debug.Log( "RegisterGlobalComponents()" );
		#endif

		GameObject t_dont_destroy = GameObjectHelper.GetDontDestroyOnLoadGameObject();

		ComponentHelper.AddIfNotExist( t_dont_destroy, typeof(UtilityTool) );

		ComponentHelper.AddIfNotExist( t_dont_destroy, typeof(PushAndNotificationHelper) );

		ComponentHelper.AddIfNotExist( t_dont_destroy, typeof(ConsoleTool) );

		ComponentHelper.AddIfNotExist(t_dont_destroy, typeof(InGameLog));

		ComponentHelper.AddIfNotExist(t_dont_destroy, typeof(UIWindowTool));

		{
			FileHelper.RegisterLog();
			JunZhuData.Instance ();
//			DebugHelper.RegisterLog();
		}
	}

	#endregion



	#region Global Class Update

	/// Non-Mono class update could be here, invoked in UtilityTool.
	public static void GlobalClassUpdate(){
		TimeHelper.UpdateLastRealtimeSinceStartup();

		UIRootAutoActivator.Instance().ManualUpdate();

		ModelAutoActivator.Instance().ManualUpdate();

		ObjectHelper.OnUpdate();

		SocketHelper.UpdateErrorBoxes();

		ScheduleHelper.OnUpdate();

		//			SoundHelper.OnUpdate();
	}

	#endregion



	#region Find Camera

	public static Camera GetCameraInSelfOrChildren( GameObject p_gb ){
		if( p_gb == null ){
			return null;
		}
		
		Camera t_com = p_gb.GetComponent<Camera>();
		
		if( t_com != null ){
			return t_com;
		}

		Camera[] t_cams = p_gb.GetComponentsInChildren<Camera>();

		if( t_cams.Length == 0 ){
			return null;
		}

		if( t_cams.Length > 1 ){
			Debug.Log( "Many Cameras here." );

			for( int i = 0; i < t_cams.Length; i++ ){
				GameObjectHelper.LogGameObjectHierarchy( t_cams[ i ].gameObject,  i +" : " );
			}
		}

		return t_cams[ 0 ];
	}

	public static Camera GetCameraInSelfOrParent( GameObject p_gb ){
		if( p_gb == null ){
			return null;
		}

		Camera t_com = p_gb.GetComponent<Camera>();

		if( t_com != null ){
			return t_com;
		}

		t_com = p_gb.GetComponentInParent<Camera>();

		return t_com;
	}

	#endregion



	#region Add & Destroy

	/// <summary>
	/// Destroy mono.
	/// 
	/// Notes:
	/// 1.if in PlayMode, destroy it;
	/// 2.if in EditMode, destroyimmediately, since destroy is notusable in EditMode;
	/// </summary>
	public static void Destroy( Component p_target ){
		if( p_target == null ){
			#if DEBUG_MONO_HELPER
			Debug.Log( "HideAndDestroy( " + p_target.name + " )" );
			#endif
			
			return;
		}

		if( Application.isPlaying ){
			GameObject.Destroy( p_target );
		}
		else{
			GameObject.DestroyImmediate( p_target );
		}
	}

	/// <summary>
	/// Destroies mono immediately.
	/// </summary>
	/// <param name="p_target">P_target.</param>
	public static void DestroyImmediate( Component p_target ){
		GameObject.DestroyImmediate (p_target);
	}

	public static Component AddComponet( GameObject p_gb, System.Type p_type ){
		if( p_gb == null ){
			Debug.LogError( "Error, gb is null: " + p_gb );

			return null;
		}

		Component t_com = p_gb.AddComponent( p_type );

		return t_com;
	}

	/// Add type to a gameobject, if not exist on it.
	public static Component AddIfNotExist( Component p_com, System.Type p_type ){
		if( p_com == null ){
			Debug.LogError( "Error, component is null: " + p_com );

			return null;
		}

		if( p_com.gameObject == null ){
			Debug.LogError( "Error, gb is null: " + p_com );

			return null;
		}

		Component t_com = p_com.gameObject.GetComponent( p_type );

		if( t_com == null ) {
			t_com = p_com.gameObject.AddComponent( p_type );
		}

		return t_com;
	}

	/// Add type to a gameobject, if not exist on it.
	public static Component AddIfNotExist( GameObject p_gb, System.Type p_type ){
		if( p_gb == null ){
			Debug.LogError( "Error, gb is null: " + p_gb );

			return null;
		}

		Component t_com = p_gb.GetComponent( p_type );
		
		if( t_com == null ) {
			t_com = p_gb.AddComponent( p_type );
		}

		return t_com;
	}

	public static Component RemoveIfExist( GameObject p_gb, System.Type p_type ){
		Component[] t_coms = p_gb.GetComponents( p_type );

		for( int i = 0; i < t_coms.Length; i++ ){
			if( t_coms[ i ] != null ){
				Destroy( t_coms[ i ] );
			}
		}
		if( t_coms.Length > 0 ){
			return t_coms[ 0 ];	
		}
		else{
			return null;
		}
	}

	#endregion



	#region Log Component

	public static void LogUISprite( UISprite p_sprite, string p_prefix = "" ){
		if( p_sprite == null ){
			Debug.Log( "UISprite is null." );

			return;
		}

		Debug.Log( p_prefix + " " + p_sprite.spriteName + " : " + p_sprite.type + " - " + p_sprite.gameObject.activeInHierarchy );

		Debug.Log( "Hierarchy: " + GameObjectHelper.GetGameObjectHierarchy( p_sprite.gameObject ) );

		Debug.Log( "w.h: " + p_sprite.width + ", " + p_sprite.height + " - " + 
			p_sprite.atlas.name + " - " + 
			"atlas.pixelSize: " + p_sprite.atlas.pixelSize +
			"atlas.w.h: " + p_sprite.atlas.AtlasWidth + ", " + p_sprite.atlas.AtlasHeight );

		Vector4 t_draw = p_sprite.drawingDimensions;

		Debug.Log( "drawRegion: " + t_draw );
	}

	public static void LogUISpriteData( UISpriteData p_spriteData, string p_prefix = "" ){
		if( p_spriteData == null ){
			Debug.Log( "UISpriteData is null." );

			return;
		}

		Debug.Log( p_prefix + " " + p_spriteData.name );

		p_spriteData.Log();
	}

	public static void LogUITexture( UITexture p_tex, string p_prefix = "" ){
		Debug.Log( p_prefix + " " + p_tex.name + " : " + p_tex.width + ", " + p_tex.height + " - " + 
		          p_tex.mainTexture.name + " : " + p_tex.mainTexture.width + ", " + p_tex.mainTexture.height );
	}

	public static void LogUILabel( UILabel p_label ){
		Debug.Log( p_label.width + ", " + p_label.height + ": " + p_label.text );
	}

	public static void LogCamera( Camera p_cam, string p_prefix = "" ){
		Debug.Log( "------ " + p_prefix + " ------" );

		if( p_cam == null ){
			return;
		}

		Debug.Log( "clearFlags: " + p_cam.clearFlags );

		Debug.Log( "cullingMask: " + p_cam.cullingMask );

		Debug.Log( "projectionMatrix: " + p_cam.projectionMatrix );

		Debug.Log( "fieldOfView: " + p_cam.fieldOfView );

		Debug.Log( "nearClipPlane: " + p_cam.nearClipPlane );

		Debug.Log( "farClipPlane: " + p_cam.farClipPlane );

		Debug.Log( "depth: " + p_cam.depth );
	}

	public static void LogParticleSystem( ParticleSystem p_ps, string p_prefix = "" ){
		if( p_ps == null ){
			return;
		}

		Debug.Log( "------ " + p_prefix + " ------" );

		Debug.Log( "Alive: " + p_ps.IsAlive() );

		Debug.Log( "isPlaying: " + p_ps.isPlaying );

		Debug.Log( "isPaused: " + p_ps.isPaused );

		Debug.Log( "isStopped: " + p_ps.isStopped );

		Debug.Log( "particleCount: " + p_ps.particleCount );

		Debug.Log( "startColor: " + p_ps.startColor );
	}

	public static void LogTexture( Texture p_tex, string p_prefix = "" ){
		if( p_tex == null ){
			return;
		}
		
		Debug.Log( "w,h: " + p_tex.name + " : " + p_tex.width + ", " + p_tex.height );

//		#if UNITY_EDITOR
//		Debug.Log( "Path: " + AssetDatabase.GetAssetPath( p_tex ) );
//		#endif
	}

	public static void LogTexture2D( Texture2D p_tex, string p_prefix = "" ){
		if( p_tex == null ){
			return;
		}

		Debug.Log( "------ " + p_prefix + " ------" );

		#if UNITY_EDITOR
		Debug.Log( "Path: " + AssetDatabase.GetAssetPath( p_tex ) );
		#endif

		Debug.Log( "w,h: " + p_tex.name + " : " + p_tex.width + ", " + p_tex.height );

		Debug.Log( "format: " + p_tex.format );

//		Debug.Log( "texelSize: " + p_tex.texelSize );

//		Debug.Log( "filterMode: " + p_tex.filterMode );

//		Debug.Log( "wrapMode: " + p_tex.wrapMode );
	}

	public static void LogShader( Shader p_shader, string p_prefix = "" ){
		if( p_shader == null ){
			Debug.Log( "Shader is null." );

			return;
		}

		Debug.Log( "------ " + p_prefix + " ------" );

		Debug.Log( "Shader: " + p_shader );
		
		Debug.Log( "Shader.Name: " + p_shader.name );
		
		Debug.Log( "Shader.String: " + p_shader.ToString() );
	}

	public static void LogAnimator( Animator p_animator, string p_prefix = "" ){
		if( p_animator == null ){
			Debug.Log( "Animator is null." );
			
			return;
		}
		
		Debug.Log( p_prefix + " " + p_animator + " " +
		          " " + p_animator.name + " " + 
		          " " + p_animator.runtimeAnimatorController );
		
		#if UNITY_EDITOR
		Debug.Log( "Path: " + AssetDatabase.GetAssetPath( p_animator ) );
		#endif
	}

	public static void LogAnimationClip( AnimationClip p_clip, string p_prefix = "" ){
		if( p_clip == null ){
			Debug.Log( "AnimationClip is null." );

			return;
		}

		Debug.Log( p_prefix + " " + p_clip + " " +
		          " " + p_clip.name + " " + 
		          " " + p_clip.length );

		#if UNITY_EDITOR
		Debug.Log( "Path: " + AssetDatabase.GetAssetPath( p_clip ) );
		#endif
	}

	public static void LogAnimatorController( RuntimeAnimatorController p_controller, string p_prefix = "" ){
		if( p_controller == null ){
			Debug.Log( "AnimatorController is null." );
			
			return;
		}

		Debug.Log( p_prefix + " " + p_controller + " " +
		          " " + p_controller.name + " " +
		          " Len: " + p_controller.animationClips.Length );

		AnimationClip[] t_clips = p_controller.animationClips;

		for( int i = 0; i < t_clips.Length; i++ ){
			LogAnimationClip( t_clips[ i ], i + "" );
		}
	}

	public static void LogAnimationState( AnimationState p_state, string p_prefix = "" ){
		if( p_state == null ){
			Debug.Log( "AnimationState is null." );
			
			return;
		}
		
		Debug.Log( p_prefix + " " + p_state + " " +
		          " " + p_state.name + " " + 
		          " " + p_state.clip + " " +
		          " " + p_state.length + " " +
		          " " + p_state.normalizedTime + " " + 
		          " " + p_state.time );
	}

	public static void LogUIAtlas( UIAtlas p_atlas, string p_prefix = "" ){
		if( p_atlas == null ){
			Debug.Log( "Atlas is null." );

			return;
		}

		Debug.Log( p_prefix + " " + "Atlas: " + p_atlas + " " + 
		          " " + p_atlas.name + " " + 
		          " " + p_atlas.texture + " " +
		          " " + p_atlas.AtlasWidth + ", " + p_atlas.AtlasHeight );

#if UNITY_EDITOR
		Debug.Log( "Path: " + AssetDatabase.GetAssetPath( p_atlas ) );
#endif
	}

	public static void LogSkinnedMeshRenderer( SkinnedMeshRenderer p_renderer, string p_prefix = "" ){
		if( p_renderer == null ){
			Debug.Log( "SkinnedMeshRenderer is null." );
			
			return;
		}
		
		Debug.Log( p_prefix + " " + p_renderer + ", " + p_renderer.name );
		
		#if UNITY_EDITOR
		Debug.Log( "Path: " + AssetDatabase.GetAssetPath( p_renderer ) );
		#endif
	}

	public static void LogRenderer( Renderer p_renderer, string p_prefix = "" ){
		if( p_renderer == null ){
			Debug.Log( "Renderer is null." );
			
			return;
		}
		
		Debug.Log( p_prefix + " " + p_renderer + ", " + p_renderer.name );
		
		#if UNITY_EDITOR
		Debug.Log( "Path: " + AssetDatabase.GetAssetPath( p_renderer ) );
		#endif
	}

	public static void LogAudioSource( AudioSource p_source, string p_prefix = "", string t_surfix = ""  ){
		if( p_source == null ){
			Debug.Log( "AudioSource is Null: " + p_source );

			return;
		}

		if( p_source.clip == null ){
			Debug.Log( "Clip is Null: " + p_source );

			return;
		}

		Debug.Log( p_prefix + " " + p_source + ": " + 
			"IsPlaying: " + p_source.isPlaying + ", " +
			"Gb.Active: " + p_source.gameObject.activeInHierarchy + "," +
			"Enable: " + p_source.enabled + ", " +
			"Volume: " + p_source.volume + ", " +
			p_source.time + " / " + p_source.clip.length + " " + t_surfix );
	}

	#if UNITY_EDITOR
	public static void LogMovieTexture( MovieTexture p_movie, string p_prefix = "", string t_surfix = ""  ){
		if( p_movie == null ){
			Debug.Log( "MovieTexture is null." );
		}

		Debug.Log( p_prefix + "  " +  
			"playing: " + p_movie.isPlaying + " - " +
			"duration: " + p_movie.duration + " - " +
			"isReadyToPlay: " + p_movie.isReadyToPlay + " - " +
			" " + t_surfix );
	}
	#endif

	#endregion

	
	
	#region Collider
	
	/// Clear All Colliders under p_gb and its' children.
	public static void ClearColliders( GameObject p_gb ){
		if ( p_gb == null ){
			Debug.LogWarning("Error in ClearColliders, p_gb = null.");
			
			return;
		}
		
		int t_child_count = p_gb.transform.childCount;
		
		{
			for ( int i = 0; i < t_child_count; i++ ){
				Transform t_child = p_gb.transform.GetChild(i);
				
				ClearColliders(t_child.gameObject);
			}
			
			{
				Collider2D[] t_colliders = p_gb.GetComponents<Collider2D>();

				for( int i = t_colliders.Length - 1; i >= 0; i-- ){
					Collider2D t_collider = t_colliders[i];
					
					Destroy(t_collider);
				}
			}
			
			{
				Collider[] t_colliders = p_gb.GetComponents<Collider>();
				
				for (int i = t_colliders.Length - 1; i >= 0; i--)
				{
					Collider t_collider = t_colliders[i];
					
					Destroy(t_collider);
				}
			}
		}
	}
	
	/// Disable All Colliders under p_gb and its' children.
	public static void DisableColliders(GameObject p_gb)
	{
		if (p_gb == null)
		{
			Debug.LogWarning("Error in DisableColliders, p_gb = null.");
			
			return;
		}
		
		int t_child_count = p_gb.transform.childCount;
		
		{
			for (int i = 0; i < t_child_count; i++)
			{
				Transform t_child = p_gb.transform.GetChild(i);
				
				DisableColliders(t_child.gameObject);
			}
			
			{
				Collider2D[] t_colliders = p_gb.GetComponents<Collider2D>();
				
				for (int i = 0; i < t_colliders.Length; i++)
				{
					Collider2D t_collider = t_colliders[i];
					
					t_collider.enabled = false;
				}
			}
			
			{
				Collider[] t_colliders = p_gb.GetComponents<Collider>();
				
				for (int i = 0; i < t_colliders.Length; i++)
				{
					Collider t_collider = t_colliders[i];
					
					t_collider.enabled = false;
				}
			}
			
			{
				iTween.Stop(p_gb);
			}
		}
	}

	#endregion



	#region Clean

	public static void UnloadUseless( bool p_clean_anim = false ){
//		#if UNITY_EDITOR || UNITY_STANDALONE
//		CleanAtlas();
//		#endif

//		#if UNITY_ANDROID
//		if( p_clean_clip ){
//			CleanAnimation();
//		}
//		#endif

//		#if UNITY_ANDROID
//		if( p_clean_anim ){
//			CleanAnimator();
//		}
//		#endif

		Resources.UnloadUnusedAssets();
	}

	private static void CleanAnimator(){
		List<Animator> t_anims = new List<Animator>();
		
		{
			UnityEngine.Object[] t_objects = GameObject.FindObjectsOfType( typeof(Animator) );
			
			int t_count = 0;
			
			for( int i = 0; i < t_objects.Length; i++ ){
				Animator t_anim = (Animator)t_objects[ i ];
				
				if( t_anim == null ){
					continue;
				}

				if( t_anims.Contains( t_anim ) ){
					continue;
				}
				
				{
					t_anims.Add( t_anim );
					
					t_count++;
					
					t_objects[ i ] = null;
					
					t_anim = null;
				}
			}
			
			t_objects = null;
		}

		{
			UnityEngine.Object[] t_objects = Resources.FindObjectsOfTypeAll( typeof(Animator) );
			
			int t_count = 0;
			
			for( int i = 0; i < t_objects.Length; i++ ){
				Animator t_anim = (Animator)t_objects[ i ];
				
				if( t_anim == null ){
					continue;
				}
				
				if( t_anims.Contains( t_anim ) ){
					continue;
				}
				
				{
					t_objects[ i ] = null;

					try{
						LogAnimator( t_anim );

						Resources.UnloadAsset( t_anim );
					}
					catch( Exception e ){

					}
					
					t_anim = null;
					
					t_count++;
				}
			}
			
			t_objects = null;
		}
	}

	private static void CleanAnimation(){
		List<AnimationClip> t_clips = new List<AnimationClip>();

		{
			UnityEngine.Object[] t_objects = GameObject.FindObjectsOfType( typeof(Animation) );
			
			int t_count = 0;
			
			for( int i = 0; i < t_objects.Length; i++ ){
				Animation t_anim = (Animation)t_objects[ i ];
				
				if( t_anim == null ){
					continue;
				}

				foreach ( AnimationState state in t_anim ) {
					if( state.clip == null ){
						continue;
					}

					if( t_clips.Contains( state.clip ) ){
						continue;
					}

					{
						t_clips.Add( state.clip );
						
						t_count++;
					}
				}

				if( t_anim.clip == null ){
					continue;
				}

				if( t_clips.Contains( t_anim.clip ) ){
					continue;
				}

				{
					t_clips.Add( t_anim.clip );
					
					t_count++;

					t_objects[ i ] = null;
					
					t_anim = null;
				}
			}

			t_objects = null;
		}

		{
			UnityEngine.Object[] t_objects = GameObject.FindObjectsOfType( typeof(Animator) );
			
			int t_count = 0;
			
			for( int i = 0; i < t_objects.Length; i++ ){
				Animator t_animator = (Animator)t_objects[ i ];
				
				if( t_animator == null ){
					continue;
				}
				
				if( t_animator.runtimeAnimatorController == null ){
					continue;
				}

				if( t_animator.runtimeAnimatorController.animationClips == null ){
					continue;
				}

				for( int j = 0; j < t_animator.runtimeAnimatorController.animationClips.Length; j++ ){
					if( t_animator.runtimeAnimatorController.animationClips[ j ] == null ){
						continue;
					}

					if( t_clips.Contains( t_animator.runtimeAnimatorController.animationClips[ j ] ) ){
						continue;
					}

					{
						t_clips.Add( t_animator.runtimeAnimatorController.animationClips[ j ] );
						
						t_count++;
					}
				}

				t_animator = null;

				t_objects[ i ] = null;
			}

			t_objects = null;
		}

		{
			UnityEngine.Object[] t_objects = Resources.FindObjectsOfTypeAll( typeof(AnimationClip) );
			
			int t_count = 0;
			
			for( int i = 0; i < t_objects.Length; i++ ){
				AnimationClip t_clip = (AnimationClip)t_objects[ i ];

				if( t_clip == null ){
					continue;
				}

				if( t_clips.Contains( t_clip ) ){
					continue;
				}

				{
					t_objects[ i ] = null;

					Resources.UnloadAsset( t_clip );

					t_clip = null;
					
					t_count++;
				}
			}

			t_objects = null;
		}
	}

	private static void CleanAtlas(){
		List<UIAtlas> t_atlases = new List<UIAtlas>();
		
		{
			UnityEngine.Object[] t_objects = Resources.FindObjectsOfTypeAll( typeof(UISprite) );
			
			int t_count = 0;
			
			for( int i = 0; i < t_objects.Length; i++ ){
				UISprite t_sprite = (UISprite)t_objects[ i ];
				
				if( t_sprite == null ){
					continue;
				}
				
				if( t_sprite.atlas == null ){
					continue;
				}
				
				if( t_atlases.Contains( t_sprite.atlas ) ){
					continue;
				}
				
				{
					t_atlases.Add( t_sprite.atlas );
					
					t_count++;

					t_sprite = null;

					t_objects[ i ] = null;
				}
			}

			t_objects = null;
		}
		
		{
			UnityEngine.Object[] t_objects = Resources.FindObjectsOfTypeAll( typeof(UIAtlas) );
			
			int t_count = 0;
			
			for( int i = t_objects.Length - 1; i >= 0; i-- ){
				UIAtlas t_atlas = (UIAtlas)t_objects[ i ];
				
				if( t_atlas == null ){
					continue;
				}
				
				if( t_atlas.texture == null ){
					continue;
				}
				
				if( t_atlases.Contains( t_atlas ) ){
					continue;
				}
				
				{
					Texture t_tex = t_atlas.texture;

					t_atlas = null;

					t_objects[ i ] = null;

					Resources.UnloadAsset( t_tex );
					
					t_count++;
				}
			}

			t_objects = null;
		}
	}

	#endregion



	#region iTween

	public static void StopITweens(GameObject p_gb)
	{
		if (p_gb == null)
		{
			Debug.LogWarning("Error in ClearColliders, p_gb = null.");
			
			return;
		}
		
		int t_child_count = p_gb.transform.childCount;
		
		{
			for (int i = 0; i < t_child_count; i++)
			{
				Transform t_child = p_gb.transform.GetChild(i);
				
				StopITweens(t_child.gameObject);
			}
			
			{
				iTween.Stop(p_gb);
			}
		}
	}

	#endregion



	#region Monos

	/// Clear All Monos under p_gb and its' children.
	public static void ClearMonos( GameObject p_gb, bool p_clear_children = true ){
		if ( p_gb == null ){
			Debug.LogWarning("Error in ClearMonos, p_gb = null.");
			
			return;
		}
		
		int t_child_count = p_gb.transform.childCount;
		
		{
			if( p_clear_children ){
				for ( int i = 0; i < t_child_count; i++ ){
					Transform t_child = p_gb.transform.GetChild( i );

					ClearMonos( t_child.gameObject, p_clear_children );
				}
			}
			
			{
				MonoBehaviour[] t_monos = p_gb.GetComponents<MonoBehaviour>();
				
				for( int i = t_monos.Length - 1; i >= 0; i-- ){
					MonoBehaviour t_mono = t_monos[ i ];
					
					Destroy( t_mono );
				}
			}
		}
	}

	/// Clear All Monos under p_gb and its' children.
	public static void ClearComponents( GameObject p_gb, bool p_clear_children = true ){
		if ( p_gb == null ){
			Debug.LogWarning("Error in ClearMonos, p_gb = null.");

			return;
		}

		int t_child_count = p_gb.transform.childCount;

		{
			if( p_clear_children ){
				for ( int i = 0; i < t_child_count; i++ ){
					Transform t_child = p_gb.transform.GetChild( i );

					ClearMonos( t_child.gameObject, p_clear_children );
				}
			}

			{
				Component[] t_coms = p_gb.GetComponents<Component>();

				for( int i = t_coms.Length - 1; i >= 0; i-- ){
					Component t_com = t_coms[ i ];

					Destroy( t_com );
				}
			}
		}
	}

	/// Clears All Monos without NGUI under p_gb and its' children.
	public static void ClearMonosWithoutNGUI(GameObject p_gb)
	{
		if (p_gb == null)
		{
			Debug.LogWarning("Error in ClearMonosWithoutNGUI, p_gb = null.");
			
			return;
		}
		
		int t_child_count = p_gb.transform.childCount;
		
		{
			for (int i = 0; i < t_child_count; i++)
			{
				Transform t_child = p_gb.transform.GetChild(i);
				
				ClearMonosWithoutNGUI(t_child.gameObject);
			}
			
			Component[] t_monos = p_gb.GetComponentsInChildren(typeof(MonoBehaviour));
			
			for (int i = 0; i < t_monos.Length; i++)
			{
				MonoBehaviour t_mono = (MonoBehaviour)t_monos[i];
				
				if (t_mono is UIWidget ||
				    t_mono is UIAnchor ||
				    t_mono is UIWidgetContainer ||
				    t_mono is UIFont ||
				    t_mono is UIAtlas)
				{
					continue;
				}
				else
				{
					t_mono.enabled = false;
					
					Destroy(t_mono);
				}
			}
		}
	}

	#endregion



	#region Mesh & PS

	public static void DisableAllVisibleObject( GameObject p_gb ){
		if (p_gb == null) {
			return;
		}

		Renderer[] t_renderers = p_gb.GetComponentsInChildren<Renderer>();
		
		for( int i = 0; i < t_renderers.Length; i++ ){
			t_renderers[ i ].enabled = false;
		}
	}

	#endregion



	#region Materials

	public static Material NewMaterial( string p_shader_name ){
		Shader t_shader = Shader.Find( p_shader_name );

		if( t_shader == null ){
			Debug.LogError( "Shader is null: " + p_shader_name );

			return null;
		}

		Material t_mat = new Material( t_shader );

		return t_mat;
	}
	
	public static void LogMaterial( Material p_mat, string p_prefix = "" ){
		if( p_mat == null ){
			return;
		}
		
		Debug.Log( p_prefix + "Mat: " + p_mat.name );

		#if UNITY_EDITOR
		Debug.Log( "Path: " + AssetDatabase.GetAssetPath( p_mat ) );
		#endif
		
//		LogShader( p_mat.shader );

		if( p_mat.mainTexture != null ){
			LogTexture( p_mat.mainTexture );
		}
	}

	public static void LogMaterials<T>( GameObject p_gb, string p_prefix = "" ) where T : Renderer{
		if( p_gb == null ){
			return;
		}

		T[] t_renderers = p_gb.GetComponentsInChildren<T>();
		
		for( int i = 0; i < t_renderers.Length; i++ ){
			T t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			for ( int j = 0; j < t_renderer.materials.Length; j++ ){
				Material t_mat = t_renderer.materials[ j ];
				
				Debug.Log( p_prefix + " " + i + ": " + t_mat + " - " + t_mat.shader.name );
			}
		}
	}

	public static Material GetMaterialWithShader<T>( GameObject p_gb ) where T : Renderer{
		if( p_gb == null ){
//			Debug.Log( "p_object is null." );

			return null;
		}

		T[] t_renderers = p_gb.GetComponentsInChildren<T>();

		for( int i = 0; i < t_renderers.Length; i++ ){
			T t_renderer = t_renderers[i];

			Material[] t_mats = t_renderer.materials;

			for ( int j = 0; j < t_renderer.materials.Length; j++ ){
				Material t_mat = t_renderer.materials[ j ];

				if ( t_mat == null ){
					continue;
				}

				return t_mat;
			}
		}

		return null;
	}

	public static Material GetMaterialWithShader<T>( GameObject p_gb, Shader p_shader ) where T : Renderer{
		if( p_gb == null ){
//			Debug.Log( "p_object is null." );
			
			return null;
		}
		
		if( p_shader == null ){
//			Debug.Log( "p_origin_shader is null." );
			
			return null;
		}
		
		T[] t_renderers = p_gb.GetComponentsInChildren<T>();
		
		for( int i = 0; i < t_renderers.Length; i++ ){
			T t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			for ( int j = 0; j < t_renderer.materials.Length; j++ ){
				Material t_mat = t_renderer.materials[ j ];
				
				if ( t_mat == null ){
					continue;
				}
				
				if ( t_mat.shader == p_shader ){
					return t_mat;
				}
			}
		}

		return null;
	}

	public static Material[] GetMaterialsWithShader<T>( GameObject p_gb, Shader p_shader ) where T : Renderer{
		List<Material> m_list = new List<Material>();

		if( p_gb == null ){
			//			Debug.Log( "p_object is null." );
			
			return null;
		}
		
		if( p_shader == null ){
			//			Debug.Log( "p_origin_shader is null." );
			
			return null;
		}
		
		T[] t_renderers = p_gb.GetComponentsInChildren<T>();
		
		for( int i = 0; i < t_renderers.Length; i++ ){
			T t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			for ( int j = 0; j < t_renderer.materials.Length; j++ ){
				Material t_mat = t_renderer.materials[ j ];
				
				if ( t_mat == null ){
					continue;
				}
				
				if ( t_mat.shader == p_shader ){
					m_list.Add( t_mat );
				}
			}
		}
		
		return m_list.ToArray();
	}

	public static Material GetFirstMaterial( GameObject p_gb ){
		Material t_mat = ComponentHelper.GetMaterialWithShader<MeshRenderer>( p_gb );

		if( t_mat == null ){
			t_mat = ComponentHelper.GetMaterialWithShader<SkinnedMeshRenderer>( p_gb );
		}

		return t_mat;
	}

	#endregion



	#region UIWidget

	public static bool IsNGUIWidget( UIWidget p_widget ){
		if( p_widget == null ){
			Debug.Log( "Widget is null: " + p_widget );

			return false;
		}

		if( p_widget is UITexture ||
			p_widget is UISprite ||
			p_widget is UILabel ){
			return true;
		}

		return false;
	}

	public static UIWidget GetActiveWidget( GameObject p_gb ){
		UIWidget[] t_widgets = p_gb.GetComponents<UIWidget>();

		for( int i = 0; i < t_widgets.Length; i++ ){
			if( t_widgets[ i ] != null ){
				if( t_widgets[ i ].enabled ){
					return t_widgets[ i ];
				}
			}
		}

		return null;
	}

	public static void ShiftWidgetDepth( GameObject p_gb, int p_offset ){
		UIWidget[] t_widgets = p_gb.GetComponentsInChildren<UIWidget>();

		for( int i = 0; i < t_widgets.Length; i++ ){
			UIWidget t_widget = t_widgets[ i ];

			if( t_widget == null ){
				continue;
			}

			if( IsNGUIWidget( t_widget ) ){
				t_widget.depth += p_offset;
			}
		}
	}

	public static int GetNGUIMinDepth( GameObject p_gb ){
		UIWidget[] t_widgets = p_gb.GetComponentsInChildren<UIWidget>();

		if( t_widgets == null || t_widgets.Length == 0 ){
			return int.MinValue;
		}

		int t_min = t_widgets[ 0 ].depth;

		for( int i = 1; i < t_widgets.Length; i++ ){
			UIWidget t_widget = t_widgets[ i ];

			if( t_widget == null ){
				continue;
			}

			if( IsNGUIWidget( t_widget ) ){
				if( t_widget.depth < t_min ){
					t_min = t_widget.depth;
				}
			}
		}

		return t_min;
	}

	public static int GetNGUIMaxDepth( GameObject p_gb ){
		UIWidget[] t_widgets = p_gb.GetComponentsInChildren<UIWidget>();

		if( t_widgets == null || t_widgets.Length == 0 ){
			return int.MinValue;
		}

		int t_max = t_widgets[ 0 ].depth;

		for( int i = 1; i < t_widgets.Length; i++ ){
			UIWidget t_widget = t_widgets[ i ];

			if( t_widget == null ){
				continue;
			}

			if( IsNGUIWidget( t_widget ) ){
				if( t_widget.depth > t_max ){
					t_max = t_widget.depth;
				}
			}
		}

		return t_max;
	}

	#endregion



	#region Common

	public static void ClearComponents<T>( GameObject p_gb ) where T : Component{
		if ( p_gb == null ){
			Debug.LogWarning("Error in ClearType, p_gb = null.");
			
			return;
		}

		int t_child_count = p_gb.transform.childCount;
		
		{
			for ( int i = 0; i < t_child_count; i++ ){
				Transform t_child = p_gb.transform.GetChild( i );
				
				ClearComponents<T>( t_child.gameObject );
			}
			
			{
				Component[] t_coms = p_gb.GetComponents<T>();
				
				for( int i = t_coms.Length - 1; i >= 0; i-- ){
					Component t_com = t_coms[ i ];
					
					Destroy( t_com );
				}
			}
		}
	}

	#endregion



	#region Log

	public static void LogGameObjectComponents( GameObject p_gb ){
		if( p_gb == null ){
			Debug.Log( "GameObject is null." );

			return;
		}

		MonoBehaviour[] t_monos = p_gb.GetComponents<MonoBehaviour>();

		for( int i = 0; i < t_monos.Length; i++ ){
			if( t_monos[ i ] == null ){
				continue;
			}

			Debug.Log( i + ": " + t_monos[ i ] + " - " + t_monos[ i ].GetType() );
		}
	}

	#endregion



	#region Utilities

	public static void HideComponent( Component p_com ){
		if( p_com == null ){
			Debug.LogError( "Error, p_gb = null." );

			return;
		}

		p_com.hideFlags = HideFlags.HideInInspector;
	}

	#endregion
}
