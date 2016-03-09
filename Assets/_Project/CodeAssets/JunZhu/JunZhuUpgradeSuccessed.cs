using UnityEngine;
using System.Collections;

public class JunZhuUpgradeSuccessed : MonoBehaviour
{
    void Awake()
    {
	 	UI3DEffectTool.ShowTopLayerEffect( UI3DEffectTool.UIType.PopUI_2, gameObject, EffectIdTemplate.GetPathByeffectId(100020), null);
    }
 
	void Start () 
    {
     
	}

    void OnEnable()
    {
        StartCoroutine(WaitFor());
    }
    IEnumerator WaitFor()
    {
       yield return new WaitForSeconds(1.2f);
       this.gameObject.SetActive(false);
    }
}
