using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public Transform camera;

	private float gravity = 20f;
	private float jump = 1f;
	private float gapPipe = 1f;
	Color32 birdColor = new Color32(128, 128, 128, 255);
	private string buttonText = "Reload for another variation";
	

	void Start () {
		isStartButtonPressed = false;
		Time.timeScale = 0.0f;

		Debug.Log ("Initializing Splitforce");
		UnitySplitForce.SFManager.Instance.initCallback = SplitforceInitialised;
		UnitySplitForce.SFManager.Init ("apmodiwqux", "qkqddjssssxodrnpeblvutztfpwgtkwjkpbndtgpshbrtdbwae", new Hashtable () {
			{"isDebug", true}
		});
	}

	
	void  SplitforceInitialised(bool isFailed, Hashtable additionalData) {
		var _didInit = !isFailed;
		
		// You can check if everything is correct
		if (isFailed)
		{
			Debug.Log("SPLITFORCE: Init FAILED" + additionalData.ToString(), gameObject);
			
		}
		else
		{
			Debug.Log("SPLITFORCE: Init SUCCESS", gameObject);
		}


		// Experiment GamePhysics
		UnitySplitForce.SFVariation v = UnitySplitForce.SFManager.Instance.initExperiment("GamePhysics");
		
		if (v != null) {
			this.birdColor = v.VariationData("color").DataToColor32();
			this.gravity = v.VariationData("gravity").DataToFloat(gravity);
			this.jump = v.VariationData("jump").DataToFloat(jump);
			this.gapPipe = v.VariationData("gap pipe").DataToFloat(gapPipe);
			Debug.Log("Setting SF stuff (color: " + this.birdColor + ")" );
		}
		
		
		GenerateWorld world = camera.GetComponent<GenerateWorld> ();
		world.gapPipe = this.gapPipe;
		
		world.maintenance (this.transform.position.x);
		this.renderer.material.color = this.birdColor;
		
		
		// Experiment #2
		string buttonText = "Default Text";
		UnitySplitForce.SFVariation b = UnitySplitForce.SFManager.Instance.initExperiment("Experiment #2");
		
		if (b != null) {
			this.buttonText = b.VariationData("Button Text").DataToString();
		}


	}

	

	
	// Update is called once per frame
	void Update () {
		updateScore();
		if(!isInView())
		{
			//restartGame();
		}
		if(Input.anyKeyDown)
		{
			if (!isDead) 
			{
				move();
			}
		}
	}
	
	private bool isInView()
	{
		Vector3 port = Camera.main.WorldToViewportPoint(transform.position);
		if((port.x < 1) && (port.x > 0) && (port.y < 1) && (port.y > 0) && port.z > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
		
	}

	private bool isStartButtonPressed;
	private bool isDead = false;

	public GUIText scoreLabel;

	void OnGUI()
	{
		if (!isStartButtonPressed)
		{

			if(GUI.Button(new Rect(Screen.width/2-35, Screen.height/2-11 ,70,22), "Start"))
			{
				Time.timeScale = 1.0f;
				isStartButtonPressed = true;
			}
		}

		if (isDead)
		{
			if(GUI.Button(new Rect(Screen.width/2-55, Screen.height/2-11 ,200,22), this.buttonText))
			{
				Application.OpenURL("index.html");
//				isDead = false;
//				isStartButtonPressed = false;
//				restartGame();			
			}

//			if(GUI.Button(new Rect(Screen.width/2+5, Screen.height/2-11 ,50,22), "Later"))
//			{
//				isDead = false;
//				isStartButtonPressed = false;
//				restartGame();			
//			}
		}
	}

	private void move()
	{
		rigidbody.velocity = new Vector3(0,0,0);
		rigidbody.AddForce (new Vector3(275,200  * this.jump,0), ForceMode.Force);
	}


	void OnTriggerEnter(Collider other)
	{

		isDead = true;
		UnitySplitForce.SFVariation v = UnitySplitForce.SFManager.Instance.getExperiment("GamePhysics");
		Debug.Log ("Dead :(");

		if (v != null) {
			Debug.Log ("Sending Goals");

			v.trackGoal("Ping");
			v.trackTime("Average Session");

			v.trackQuantifiedGoal("Average Score", int.Parse(scoreLabel.text));
			v.endVariation();
		}

	}

	private void restartGame()
	{
		Time.timeScale = 0.0f;
		isStartButtonPressed = false;
		Application.LoadLevel (Application.loadedLevelName);	
	}

	private void updateScore()
	{
		if (isDead)
		{
			return;
		}
		int score = (int) (transform.position.x / GenerateWorld.distanceBetweenObjects);
		if(score != (int.Parse(scoreLabel.text)) && score > 0)
		{
			scoreLabel.text = score.ToString();

		}
		
	}


}
