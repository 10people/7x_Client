using UnityEngine;
using System.Collections;

public class EnterUnion : MonoBehaviour {

    public GameObject m_layer;

    void OnClick()
    {
        EnterUnionLayer();
    }

    void EnterUnionLayer()
    {
        GameObject tempObject = Instantiate(m_layer) as GameObject;

        tempObject.name = "UnionLayerObject";

        tempObject.transform.position = new Vector3(0,100,0);
    }
}
