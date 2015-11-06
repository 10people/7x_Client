using UnityEngine;
using System.Collections;

public class BezierShower : MonoBehaviour
{
    public int WhichRoute;

    private float t_para = 0;
    private bool isopen;
    // Use this for initialization
    void Start()
    {

        CartRouteTemplate.LoadTemplates();
    }

    // Update is called once per frame
    void Update()
    {
        if (isopen)
        {
            if (t_para < 0 || t_para > 1.5)
            {
                t_para = 0;
            }
            t_para += Time.deltaTime * 0.2f;

            Vector2 temp = BezierUtility.GetBezierPoint(t_para, CartRouteTemplate.Templates[WhichRoute - 1].Position);
            if (temp != Vector2.zero)
            {
                transform.position = new Vector3(temp.x, -10, temp.y);
            }
        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (GUILayout.Button("Do"))
        {
            isopen = true;
        }
    }
#endif

}
