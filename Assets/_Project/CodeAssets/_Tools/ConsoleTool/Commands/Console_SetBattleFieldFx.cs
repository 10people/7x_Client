using UnityEngine;
using System.Collections;
using System;

public class Console_SetBattleFieldFx {

	#region Set Fx Command

	public static void SetAttackFx( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		bool t_param_1_show = false;
		
		try{
			t_param_1_show = bool.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		ExecSetAttackFx( t_param_1_show );
	}
	
	public static void SetSkillFx( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		bool t_param_1_show = false;
		
		try{
			t_param_1_show = bool.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		ExecSetSkillFx( t_param_1_show );
	}

	public static void setBloodLabel( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		bool t_param_1_show = false;
		
		try{
			t_param_1_show = bool.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		ExecSetBloodlabel( t_param_1_show );
	}
	
	#endregion



	#region Support

	private static bool m_enable_attack_fx 		= true;
	
	private static bool m_enable_skill_fx 		= true;

	private static bool m_enable_bloodlabel 	= true;
	
	private static void ExecSetAttackFx( bool p_enable ){
		m_enable_attack_fx = p_enable;
	}
	
	private static void ExecSetSkillFx( bool p_enable ){
		m_enable_skill_fx = p_enable;
	}

	private static void ExecSetBloodlabel( bool p_enable ){
		m_enable_bloodlabel = p_enable;
	}

	// attack fx switcher
	public static bool IsEnableAttackFx(){
		return m_enable_attack_fx;
	}

	// skill fx switcher
	public static bool IsEnableSkillFx(){
		return m_enable_skill_fx;
	}

	// bloodlabel switcher
	public static bool IsEnableBloodLabel(){
		return m_enable_bloodlabel;
	}

	#endregion
}
