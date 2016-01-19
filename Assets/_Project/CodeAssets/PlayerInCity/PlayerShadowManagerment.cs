using UnityEngine;
using System.Collections;

public class PlayerShadowManagerment : MonoBehaviour
{
    public GameObject m_Shadow;
	void Start () 
    {
		m_Shadow.SetActive( Quality_Shadow.InCity_ShowSimpleShadow() );
	}

    void Update()
    {
        if (JunZhuData.Instance().m_LevelUpInfoSave)
        {
            JunZhuData.Instance().m_LevelUpInfoSave = false;
            Global.ResourcesDotLoad(EffectIdTemplate.getEffectTemplateByEffectId(100190).path, EffectLoadCallback);
        }
    }
    private void EffectLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject temp = (GameObject)Instantiate(p_object);
        temp.transform.parent = transform;
        temp.transform.localPosition = Vector3.zero;
        StartCoroutine(WaitForUpgaade(temp));
    }
    IEnumerator WaitForUpgaade(GameObject obj)
    {
        yield return new WaitForSeconds(1.4f);
        Destroy(obj);
    }
}
