#pragma strict

@script RequireComponent (AudioSource);
//if true a menu will apear ingame with all the microphones
var SelectIngame:boolean=false;
//if false the below will override and set the mic selected in the editor
 //Select the microphone you want to use (supported up to 6 to choose from). If the device has number 1 in the console, you should select default as it is the first defice to be found.
enum Devices {DefaultDevice, Second, Third, Fourth, Fifth, Sixth}
 
var InputDevice : Devices;
private var selectedDevice:String;
 
 
 
 
var audioSource:AudioSource;
//The maximum amount of sample data that gets loaded in, best is to leave it on 256, unless you know what you are doing. A higher number gives more accuracy but 
//lowers performance allot, it is best to leave it at 256.
var amountSamples:float=256;
static var loudness:float;
var sensitivity:float=0.4;
var sourceVolume:float=100;
private var minFreq: int;
private var maxFreq: int;
 
var Mute:boolean=true;
var debug:boolean=false;
var ShowDeviceName:boolean=false;
private var micSelected:boolean=false; 

private var mTimer:float=0;
private var mRefTime:float=10; 
private var recording:boolean=true; 


function Start () {
 
if(!audioSource){
  audioSource = GetComponent(AudioSource);
	} 
 
var i=0;
//count amount of devices connected
for(device in Microphone.devices){
i++;
if(ShowDeviceName){
Debug.Log ("Devices number "+i+" Name"+"="+device);
 
}
}


if(SelectIngame==false){
//select the device if possible else give error
if(InputDevice==Devices.DefaultDevice){
if(i>=1){
selectedDevice= Microphone.devices[0];
}
else{
Debug.LogError ("No device detected on this slot. Check input connection");
}
 
}
 
 
if(InputDevice==Devices.Second){
if(i>=2){
selectedDevice= Microphone.devices[1];
}
else{
Debug.LogError ("No device detected on this slot. Check input connection");
}
 
}
 
 
 
if(InputDevice==Devices.Third){
if(i>=3){
selectedDevice= Microphone.devices[2];
}
else{
Debug.LogError ("No device detected on this slot. Check input connection");
return;
}
}
 
 
if(InputDevice==Devices.Fourth){
if(i>=4){
selectedDevice= Microphone.devices[2];
}
else{
Debug.LogError ("No device detected on this slot. Check input connection");
return;
}
}
if(InputDevice==Devices.Fifth){
if(i>=5){
selectedDevice= Microphone.devices[2];
}
else{
Debug.LogError ("No device detected on this slot. Check input connection");
return;
}
}
 
if(InputDevice==Devices.Sixth){
if(i>=6){
selectedDevice= Microphone.devices[2];
}
else{
Debug.LogError ("No device detected on this slot. Check input connection");
return;
}
}
 
}



//detect the selected microphone
GetComponent.<AudioSource>().clip = Microphone.Start(selectedDevice, true, 10, 44100);
//loop the playing of the recording so it will be realtime
GetComponent.<AudioSource>().loop = true;
//if you only need the data stream values  check Mute, if you want to hear yourself ingame don't check Mute. 
GetComponent.<AudioSource>().mute = Mute;

//don't do anything until the microphone started up
while (!(Microphone.GetPosition(selectedDevice) > 0)){
if(debug){
Debug.Log("Awaiting connection");
}
}
if(debug){
Debug.Log("Connected");
}
 
//Put the clip on play so the data stream gets ingame on realtime
GetComponent.<AudioSource>().Play();
recording=true; 
}
 
 
 
//apply the mic input data stream to a float;
function FixedUpdate () {
			

if(Microphone.IsRecording(selectedDevice)){
  loudness = GetDataStream()*sensitivity*(sourceVolume/10);

  }
   if(debug){
   Debug.Log(loudness);
  }
  
  //the source volume
  if (sourceVolume > 100){
       sourceVolume = 100;
 }
 
  if (sourceVolume < 0){
   sourceVolume = 0;
   }
  GetComponent.<AudioSource>().volume = (sourceVolume/100);
 

 
 //data lag prevention
 
  //set timer for refreshing memory. this prevents data overload and crashing of memory
 mTimer += Time.deltaTime;
 //refresh the memory
if (micSelected == true && recording){
 if (mTimer >= mRefTime) {
				StopMicrophone();
				StartMicrophone();
				mTimer = 0;
			}
			  
	 }
	 

}
 
 
	function GetDataStream(){
	if(Microphone.IsRecording(selectedDevice)){
 
  		var dataStream: float[]  = new float[amountSamples];
    
    	var audioValue: float = 0;
    
    	GetComponent.<AudioSource>().GetOutputData(dataStream,0);
 
    	for(var i in dataStream){
    		audioValue += Mathf.Abs(i);
		}
   		return audioValue/amountSamples;
	}
 
 	return 0.0f;
}
 
 
 
 
 
//select device ingame
 
    function OnGUI () {
 if(SelectIngame==true){
        if (Microphone.devices.Length > 0 && micSelected == false)//If there is more than one device, choose one.
             for (var i:int= 0; i < Microphone.devices.Length; ++i)
                 if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), Microphone.devices[i].ToString())) {
                     StopMicrophone();
                     selectedDevice = Microphone.devices[i].ToString();
                     GetMicCaps();
                    StartMicrophone();
                     micSelected = true;
 
                }
 
        if (Microphone.devices.Length < 1 && micSelected == false) {//If there is only 1 decive make it default
             selectedDevice = Microphone.devices[0].ToString();
            GetMicCaps();
             micSelected = true;
 
        }
 }
 
 
 
 			  //if time is below this stop microphone and renable when above, this prevents memory leaking when the game is paused.
		 if(recording && Time.timeScale<=0.00000001){
		 StopMicrophone();
		 recording=false;
		 Debug.Log("mic stopped");
		 }
		
		 if(!recording && Time.timeScale>=0.00000001){
		   StartMicrophone();
	       recording=true;
		  Debug.Log("restarted Mic");
		 }
 
    }
 
 
 
    //for the above control the mic start or stop
 

 public function StartMicrophone () {
         GetComponent.<AudioSource>().clip = Microphone.Start(selectedDevice, true, 10, maxFreq);//Starts recording
         while (!(Microphone.GetPosition(selectedDevice) > 0)){} // Wait until the recording has started
         GetComponent.<AudioSource>().Play(); // Play the audio source!
 
    }
 
 
 
 public function StopMicrophone () {
         GetComponent.<AudioSource>().Stop();//Stops the audio
         Microphone.End(selectedDevice);//Stops the recording of the device  
 
    }
 
 
      function GetMicCaps () {
         Microphone.GetDeviceCaps(selectedDevice,  minFreq,  maxFreq);//Gets the frequency of the device
         if ((minFreq + maxFreq) == 0)//These 2 lines of code are mainly for windows computers
             maxFreq = 44100;
 
    }
    
    
   
    
   
    
    //Create a gui button in another script that calls to this script
        public function MicDeviceGUI (left:float , top:float, width:float, height:float, buttonSpaceTop:float, buttonSpaceLeft:float) {
	if (Microphone.devices.Length > 1 && micSelected == false)//If there is more than one device, choose one.
		for (var i:int=0; i < Microphone.devices.Length; ++i)
			if (GUI.Button(new Rect(left + (buttonSpaceLeft * i), top + (buttonSpaceTop * i), width, height), Microphone.devices[i].ToString())) {
				StopMicrophone();
				selectedDevice = Microphone.devices[i].ToString();
				GetMicCaps();
				StartMicrophone();
				micSelected = true;
			}
	if (Microphone.devices.Length < 2 && micSelected == false) {//If there is only 1 decive make it default
		selectedDevice = Microphone.devices[0].ToString();
		GetMicCaps();
		micSelected = true;
	}
    }
 
    