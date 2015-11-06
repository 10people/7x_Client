using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BagLayerManager : MonoBehaviour {

    //public List<EventHandler> m_eventHandlerList = new List<EventHandler>();

	public UIBagLeft m_UIBagLeft;

	void Start()
	{
        //foreach(EventHandler tempHandler in m_eventHandlerList)
        //{
        //    tempHandler.m_handler += ChangeLayer;
        //}
	}

    //void ChangeLayer(GameObject tempObject)
    //{
    //    //Reset uiscrollview for possible error.
    //    var targetTransform = m_UIBagLeft.transform.parent;
    //    var springPanel = targetTransform.GetComponent<SpringPanel>();
    //    var panel = targetTransform.GetComponent<UIPanel>();
    //    if (springPanel != null)
    //    {
    //        Vector3 endPos = new Vector3(springPanel.transform.localPosition.x, -13, 0);
    //        SpringPanel.Begin(targetTransform.gameObject, endPos, springPanel.strength);
    //    }

    //    string tempString = tempObject.name;


    //    if(tempString == "Button_Equip")
    //    {

    //        m_UIBagLeft.m_itemType = UIBagLeft.ItemType.Equip;
    //    }
    //    else if(tempString == "Button_CaiLiao")
    //    {

    //        m_UIBagLeft.m_itemType = UIBagLeft.ItemType.Material;
    //    }
	
    //    m_UIBagLeft.InitWithLayer();

    //}
}
