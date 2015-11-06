using UnityEngine;
using System.Collections;

public class BuyTiLiUIManager : MonoBehaviour
{


    float my_locScolx;

    bool IsStart;
    public GameObject childgb;
    Vector3 startpos = new Vector3(-272, 280, 0);
    Vector3 endpos = new Vector3(0, 0, 0);
    void Start()
    {
        my_locScolx = 0;

        IsStart = true;
        transform.localPosition = startpos;
        transform.localScale = Vector3.one;

        movegobj();
    }

    void Update()
    {

        if (IsStart)
        {
            if (my_locScolx < 1)
            {
                my_locScolx += 2f * Time.deltaTime;
            }
            else
            {
                my_locScolx = 1;
            }
            childgb.transform.localScale = new Vector3(my_locScolx, my_locScolx, 1);
        }
        else
        {
            if (my_locScolx >= 0)
            {
                my_locScolx -= 2f * Time.deltaTime;
            }

            childgb.transform.localScale = new Vector3(my_locScolx, my_locScolx, 1);
            if (my_locScolx < 0)
            {
                Debug.Log("销毁UI");
                Destroy(this.gameObject);
            }
        }


    }
    void movegobj()
    {
        TweenPosition.Begin(this.gameObject, 0.5f, endpos);
    }
    public void ClosethisUI()
    {
        if (IsStart)
        {
            MapData.mapinstance.IsCloseGuid = false;
            IsStart = false;
            TweenPosition.Begin(this.gameObject, 0.5f, startpos);
        }

    }
}
