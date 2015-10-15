using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Highscore : MonoBehaviour {

	public static int score;
	public static int highscore;
	Text text;

	void Start () 
	{
		text = GetComponent <Text> ();
		highscore = PlayerPrefs.GetInt("highScore",0);

	}
	
	// Update is called once per frame
	void Update () 
	{
		text.text = "HighScore: " + highscore;
		if (score > highscore)
		{
			highscore = score;
			PlayerPrefs.SetInt("highScore", score);
			PlayerPrefs.Save();
		}
	}

}
