using UnityEngine;
using System.Collections;

public class TaskLabMoveEffert : MonoBehaviour {

    public  string content = "dddddddddddddddd";
    bool isSendContent = false;
    public GameObject m_MainObj;
	void Start () 
    {
        MoveEffert();
	}

    void MoveEffert()
    {
       transform.GetComponent<UILabel>().text = "";
       transform.GetComponent<TweenPosition>();
       transform.GetComponent<TweenPosition>().from = new Vector3(-80, 20, 0);
       transform.GetComponent<TweenPosition>().to = new Vector3(170, 20, 0);
       transform.GetComponent<TweenPosition>().duration = 0.4f;
       transform.GetComponent<TweenPosition>().enabled = true;
       EventDelegate.Add(transform.GetComponent<TweenPosition>().onFinished, SendContent);
    }
    void SendContent()
    {
        EventDelegate.Remove(transform.GetComponent<TweenPosition>().onFinished, SendContent);
       // Destroy(transform.GetComponent<TweenPosition>());
        if (isSendContent)
        {
            Destroy(transform.GetComponent<TweenPosition>());
            Destroy(transform.GetComponent<TaskLabMoveEffert>());
        }
        if (!isSendContent)
        {
            isSendContent = true;
            transform.GetComponent<UILabel>().text = MyColorData.getColorString(1, content);
     
            transform.GetComponent<TweenPosition>().from = new Vector3(170, 20, 0);
            transform.GetComponent<TweenPosition>().to = new Vector3(-80, 20, 0);
            transform.GetComponent<TweenPosition>().duration = 0.4f;
            transform.GetComponent<TweenPosition>().enabled = true;
            //Destroy(transform.GetComponent<TweenPosition>());
          //  Destroy(transform.GetComponent<TaskLabMoveEffert>());
            EventDelegate.Add(transform.GetComponent<TweenPosition>().onFinished, SendContent);
        }
    }
    void MoveBack()
    {
        EventDelegate.Remove(transform.GetComponent<TweenPosition>().onFinished, SendContent);
      
      
      
    }
 
}
