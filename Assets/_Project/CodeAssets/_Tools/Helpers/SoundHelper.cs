using UnityEngine;
using System.Collections;

public class SoundHelper {

	#region Play Sound

	public static void PlayFxSound( GameObject p_gb, string p_fx_path ){
		EffectIdTemplate t_template = EffectIdTemplate.getEffectTemplateByEffectPath( p_fx_path, false );
		
		if( t_template == null ){
			return;
		}
		
		if( t_template.HaveSound() ){
			SoundPlayEff spe = p_gb.AddComponent<SoundPlayEff>();
			
			spe.PlaySound( t_template.sound );
		}
	}

	#endregion
}
