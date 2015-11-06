using UnityEngine;
using System.Collections;

public class SettingUpSelectCountryManagerment : MonoBehaviour 
{
    public GameObject m_BackKuang;
 
	void Start () 
    {
	
	}
    public void SelectedShow(bool show)
    {
      m_BackKuang.SetActive(show);
    }
	
 
}
