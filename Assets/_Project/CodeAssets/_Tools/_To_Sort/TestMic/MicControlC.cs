using UnityEngine;
using System.Collections;
 
[RequireComponent(typeof(AudioSource))]
public class MicControlC : MonoBehaviour {
 
	public string selectedDevice = "";
    public float sensitivity = 100;
    public float loudness = 0;//dont touch
	public float sourceVolume = 80;//Between 0 and 100
	public bool buttonToSpeak = false;
	//
	private int minFreq, maxFreq;
	private bool micSelected = false;
	private float mTimer, mRefTime = 10;
	private int amountSamples = 256;//increase to get better average, but will decrease performance. Best to leave it
 
    void Start() {
		GetComponent<AudioSource>().loop = true; // Set the AudioClip to loop
        GetComponent<AudioSource>().mute = false; // Mute the sound, we don't want the player to hear it
		selectedDevice = Microphone.devices[0].ToString();
		GetMicCaps();
    }
 
	void OnGUI() {
		MicDeviceGUI(10,10, 300, 150, 160, 0);
	}
 
	public void MicDeviceGUI (float left, float top, float width, float height, float buttonSpaceTop, float buttonSpaceLeft) {
		if (Microphone.devices.Length > 1 && micSelected == false)//If there is more than one device, choose one.
			for (int i = 0; i < Microphone.devices.Length; ++i)
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
 
	void GetMicCaps () {
		Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);//Gets the frequency of the device
		if ((minFreq + maxFreq) == 0)//These 2 lines of code are mainly for windows computers
			maxFreq = 44100;
	}
 
	public void StartMicrophone () {
		GetComponent<AudioSource>().clip = Microphone.Start(selectedDevice, true, 10, maxFreq);//Starts recording
        while (!(Microphone.GetPosition(selectedDevice) > 0)){} // Wait until the recording has started
        GetComponent<AudioSource>().Play(); // Play the audio source!
	}
 
	public void StopMicrophone () {
		GetComponent<AudioSource>().Stop();//Stops the audio
		Microphone.End(selectedDevice);//Stops the recording of the device	
	}
 
	void AudioVolume () {
		if (sourceVolume > 100)
			sourceVolume = 100;
		if (sourceVolume < 0)
			sourceVolume = 0;
		GetComponent<AudioSource>().volume = (sourceVolume/100);
	}
 
    void Update() {
		mTimer += Time.deltaTime;
		AudioVolume();
        loudness = GetAveragedVolume() * sensitivity * (sourceVolume/10);
		if (buttonToSpeak == true) {
			if (Input.GetKeyDown(KeyCode.T)) //Push to talk
				StartMicrophone();
			if (Input.GetKey(KeyCode.T)) { //Cleans ram while being able to talk
				if (mTimer >= mRefTime) {
					StopMicrophone();
					StartMicrophone();
					mTimer = 0;
				}
			}
			if (Input.GetKeyUp(KeyCode.T))
					StopMicrophone();	
		}
		if (buttonToSpeak == false && micSelected == true && !Microphone.IsRecording(selectedDevice))
			StartMicrophone();
		if (buttonToSpeak == false && micSelected == true) {//Cleans the ram 
			if (mTimer >= mRefTime) {
				StopMicrophone();
				StartMicrophone();
				mTimer = 0;
			}
		}
		if (Input.GetKeyDown(KeyCode.G))
			micSelected = false;
 
    }
 
	float GetAveragedVolume() {
        float[] data = new float[amountSamples];
        float a = 0;
        GetComponent<AudioSource>().GetOutputData(data,0);
        foreach(float s in data) {
            a += Mathf.Abs(s);
        }
        return a/amountSamples;
    }
}