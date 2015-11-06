#pragma strict

function Update () {
//scales the gameObject heigt based on input stream gathered from MicControl.loudness
transform.localScale=Vector3(1,MicControl.loudness,1);
}