using UnityEngine;
using System.Collections;
using System;

public class Console_Component {

	#region Component
	
	public static void ComponentCount( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		string t_param_1_mono_type = "";
		
		string t_param_2 = "";
		
		string t_param_3 = "";
		
		try{
			t_param_1_mono_type = p_params[ 1 ];
			
			if( p_params.Length >= 3 ){
				t_param_2 = p_params[ 2 ];
			}
			
			if( p_params.Length >= 4 ){
				t_param_3 = p_params[ 3 ];
			}
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		System.Type t_type = null;

		t_type = ConsoleTool.GetComponentType( t_param_1_mono_type );
		
		if( t_type != null ){
			UnityEngine.Object[] t_objects = GameObject.FindObjectsOfType( t_type );
			
			int t_count = 0;
			
			for( int i = 0; i < t_objects.Length; i++ ){
				if( t_type == typeof(UISprite) ){
					UISprite t_sprite = (UISprite)t_objects[ i ];
					
					if( !IsWHCompareStatisfy( t_sprite.width, t_sprite.height, t_param_2, t_param_3 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogUISprite( t_sprite );
					}
				}
				else if( t_type == typeof(UITexture) ){
					UITexture t_tex = (UITexture)t_objects[ i ];
					
					if( !IsWHCompareStatisfy( t_tex.width, t_tex.height, t_param_2, t_param_3 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogUITexutre( t_tex );
					}
				}
				else if( t_type == typeof(ParticleSystem) ){
					ParticleSystem t_ps = (ParticleSystem)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_param_2, t_ps.gameObject.name ) ){
						continue;
					}
					
					{
						//						ComponentHelper.LogParticleSystem( t_ps );
					}
				}
				
				GameObjectHelper.LogGameObjectHierarchy( ( (Component)( t_objects[ i ] ) ).gameObject, t_count + "" );
				
				t_count++;
			}
			
			Debug.Log( t_type + " count: " + t_count );
		}
		else{
			Debug.LogError( "type not found: " + t_type );
		}
	}
	
	#endregion



	#region Destroy Component
	
	public static void DestroyComponent( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		string t_param_1_mono_type = "";
		
		string t_param_2 = "";
		
		try{
			t_param_1_mono_type = p_params[ 1 ];
			
			if( p_params.Length >= 3 ){
				t_param_2 = p_params[ 2 ];
			}
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		System.Type t_type = null;
		
		t_type = ConsoleTool.GetComponentType ( t_param_1_mono_type );
		
		if( t_type != null ){
			UnityEngine.Object[] t_objects = GameObject.FindObjectsOfType( t_type );
			
			int t_count = 0;
			
			for( int i = 0; i < t_objects.Length; i++ ){
				Component t_component = (Component)( t_objects[ i ] );
				
				if( t_type == typeof(UISprite) ){
					UISprite t_sprite = (UISprite)t_objects[ i ];

					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_sprite.spriteName, t_param_2 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogUISprite( t_sprite );
					}
				}
				else if( t_type == typeof(UITexture) ){
					UITexture t_tex = (UITexture)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_tex.mainTexture.name, t_param_2 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogUITexutre( t_tex );
					}
				}
				else if( t_type == typeof(UILabel) ){
					UILabel t_label = (UILabel)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_label.gameObject.name, t_param_2 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogUILabel( t_label );
					}
				}
				else if( t_type == typeof(ParticleSystem) ){
					ParticleSystem t_ps = (ParticleSystem)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_ps.gameObject.name, t_param_2 ) ){
						continue;
					}
					
					{
						//						ComponentHelper.LogParticleSystem( t_ps );
					}
				}
				else if( t_type == typeof(Camera) ){
					Camera t_cam = (Camera)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_cam.gameObject.name, t_param_2 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogCamera( t_cam );
					}
				}
				
				{
					GameObjectHelper.LogGameObjectHierarchy( ( (Component)( t_objects[ i ] ) ).gameObject, t_count + "" );
					
					Debug.Log( t_count + 
					          " Destroy " + GameObjectHelper.GetGameObjectHierarchy( ( t_component ).gameObject ) +
					          " Component " + t_type  );

					MonoBehaviour.Destroy( t_component );
				}
				
				t_count++;
			}
			
			Debug.Log( t_type + " count: " + t_count );
		}
		else{
			Debug.LogError( "type not found: " + t_type );
		}
	}
	
	#endregion
	
	
	
	#region Enable Disable Component
	
	public static void EnableComponent( string[] p_params ){
		EnableDisableComponent ( p_params, true );
	}
	
	public static void DisableComponent( string[] p_params ){
		EnableDisableComponent ( p_params, false );
	}
	
	private static void EnableDisableComponent( string[] p_params, bool p_is_enable ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		string t_param_1_mono_type = "";
		
		string t_param_2 = "";
		
		try{
			t_param_1_mono_type = p_params[ 1 ];
			
			if( p_params.Length >= 3 ){
				t_param_2 = p_params[ 2 ];
			}
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		System.Type t_type = null;
		
		t_type = ConsoleTool.GetComponentType( t_param_1_mono_type );
		
		if( t_type != null ){
			UnityEngine.Object[] t_objects = GameObject.FindObjectsOfType( t_type );
			
			int t_count = 0;
			
			for( int i = 0; i < t_objects.Length; i++ ){
				Component t_component = (Component)( t_objects[ i ] );
				
				if( t_type == typeof(UISprite) ){
					UISprite t_sprite = (UISprite)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_sprite.spriteName, t_param_2 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogUISprite( t_sprite );
					}
				}
				else if( t_type == typeof(UITexture) ){
					UITexture t_tex = (UITexture)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_tex.mainTexture.name, t_param_2 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogUITexutre( t_tex );
					}
				}
				else if( t_type == typeof(UILabel) ){
					UILabel t_label = (UILabel)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_label.gameObject.name, t_param_2 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogUILabel( t_label );
					}
				}
				else if( t_type == typeof(Camera) ){
					Camera t_cam = (Camera)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_cam.gameObject.name, t_param_2 ) ){
						continue;
					}
					
					{
						ComponentHelper.LogCamera( t_cam );
					}
				}
				else if( t_type == typeof(ParticleSystem) ){
					ParticleSystem t_ps = (ParticleSystem)t_objects[ i ];
					
					if( !string.IsNullOrEmpty( t_param_2 ) && !StringHelper.IsLowerEqual( t_ps.gameObject.name, t_param_2 ) ){
						continue;
					}
					
					{
						Renderer t_renderer = t_ps.GetComponent<Renderer>();
						
						t_renderer.enabled = p_is_enable;
					}
					
					{
						//						GameObjectHelper.LogGameObjectHierarchy( t_ps.gameObject, t_count + "" );
						
						//						GameObjectHelper.LogGameObjectTransform( t_ps.gameObject, "" );
						
						//						ComponentHelper.LogParticleSystem( t_ps );
					}
				}
				
				{
					GameObjectHelper.LogGameObjectHierarchy( t_component.gameObject, t_count + "" );
					
					Debug.Log( ( p_is_enable ? "Enable " : "Disable " ) + t_count + 
					          ": " + t_type + 
					          " " + GameObjectHelper.GetGameObjectHierarchy( ( t_component ).gameObject ) );
					
					if( t_component is Behaviour ){
						Debug.Log( "Set Behaviour: " + p_is_enable + " " + 
						          t_component.GetType() + " - " + 
						          typeof(Behaviour) );
						
						( (Behaviour)t_component ).enabled = p_is_enable;
					}
					else if( t_component is ParticleSystem ){
						
					}
					else{
						Debug.LogError( "Component type not found: " + 
						               t_component.GetType() + " - " + 
						               typeof(Behaviour) );
					}
				}
				
				t_count++;
			}
			
			Debug.Log( t_type + " count: " + t_count );
		}
		else{
			Debug.LogError( "type not found: " + t_type );
		}
	}
	
	#endregion



	#region Utilities

	/// p_ops: < or >
	/// p_w_x_h: Wxh
	public static bool IsWHCompareStatisfy( float p_w, float p_h, string p_ops, string p_w_x_h ){
		if( string.IsNullOrEmpty( p_ops ) ){
			return true;
		} 
		
		if( string.IsNullOrEmpty( p_w_x_h ) ){
			return true;
		}
		
		string[] t_w_h = p_w_x_h.Split( new char[]{'x'} );
		
		if( t_w_h.Length != 2 ){
			Debug.LogError( "WxH error: " + p_w_x_h );
			
			return false;
		}
		
		float t_w = 0;
		
		float t_h = 0;
		
		try{
			t_w = float.Parse( t_w_h[ 0 ] );
			
			t_h = float.Parse( t_w_h[ 1 ] );
		}
		catch( Exception e ){
			Debug.LogError( "Error, params error: " + e );
			
			return false;
		}
		
		if( StringHelper.IsLowerEqual( p_ops, "<" ) ){
			if( p_w < t_w && p_h < t_h ){
				return true;
			}
		}
		else if( StringHelper.IsLowerEqual( p_ops, ">" ) ){
			if( p_w > t_w && p_h > t_h ){
				return true;
			}
		}
		else{
			Debug.Log( "compare error: " + p_ops );
		}
		
		return false;
	}

	#endregion
}
