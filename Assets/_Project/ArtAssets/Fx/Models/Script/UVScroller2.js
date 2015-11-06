
var targetMaterialSlot:int=0;
//var scrollThis:Material;
var speedY:float=0.5;
var speedX:float=0.0;
var speedY2:float=0.5;
var speedX2:float=0.0;
private var timeWentX:float=0;
private var timeWentY:float=0;
private var timeWentX2:float=0;
private var timeWentY2:float=0;
function Start () {

}

function Update () {
timeWentY += Time.deltaTime*speedY;
timeWentX += Time.deltaTime*speedX;
timeWentY2 += Time.deltaTime*speedY2;
timeWentX2 += Time.deltaTime*speedX2;


GetComponent.<Renderer>().materials[targetMaterialSlot].SetTextureOffset ("_MainTex", Vector2(timeWentX, timeWentY));
GetComponent.<Renderer>().materials[targetMaterialSlot].SetTextureOffset ("_Cutout", Vector2(timeWentX2, timeWentY2));


}