using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TaskForwardManagerment : MonoBehaviour
{
    public TaskLayerManager m_TaskLayerManager;
    public List<GameObject> listButton = new List<GameObject>();
    [HideInInspector]
    public int m_ButtonIndex = 0;
 
	void Start () 
    {
	
	}

    public  void ShowNeedInfo()
    {
        m_TaskLayerManager.ChangeLayer(listButton[m_ButtonIndex]);
    }
	

}
