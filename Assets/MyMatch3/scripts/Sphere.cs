using UnityEngine;
using System.Collections;

public class Sphere : MonoBehaviour {

	public int ID;
	public int x;
	public int y;

	public int dY;
	public int scoreValue;

	public bool matched;
	public bool destroyed;

	public bool readyToMove;
	public bool fallEfect;

	private Vector3 myScale;
//	private float startTime;

	public static Transform select;
	public static Transform moveTo;




	void Start()
	{
		myScale = transform.localScale;
		//startTime = Time.time;
		if (fallEfect)
		{
			StartCoroutine ("FallDown");
			fallEfect = false;


		}

	}

	void Update ()
	{

	}

	void OnMouseOver()
	{
		transform.localScale = new Vector3(myScale.x+0.2f, myScale.y+0.2f,myScale.z+0.2f);
		if (Input.GetMouseButtonDown(0))
		{
			if(!select)
			{
				select = transform;
				transform.localScale = new Vector3(myScale.x+0.2f, myScale.y+0.2f,myScale.z+0.2f);
			}
			else if(select != transform && !moveTo)
			{
				moveTo = transform;
			}
		}
	}

	void OnMouseExit ()
	{
		transform.localScale = myScale;
	}

	public IEnumerator MoveDown (int downY)
	{
		Vector3 correctPosition = transform.position;
		Vector3 newPosition = new Vector3 (transform.position.x, transform.position.y - downY, transform.position.z);
		float time  = 0;

		while(time<1)
		{
			time += Time.deltaTime*4;
			transform.position = Vector3.Lerp (correctPosition, newPosition, time);
			yield return null;
		}

		readyToMove =false;
		CheckPlace();
	}

	public IEnumerator FallDown ()
	{
	


		Vector3 newPosition = transform.position;
		Vector3 correctPosition = new Vector3 (transform.position.x, transform.position.y+dY, transform.position.z);
		float time = 0;

		while (time<1)
		{
			time += Time.fixedDeltaTime*4;
			transform.position= Vector3.Lerp (correctPosition, newPosition, time);
			yield return null;
		}
		//Grid.CheckMatchAfterSpawn();


	}

	public IEnumerator DestroyBlock()
	{
		
		Vector3 correctPosition = transform.localScale;

		float time = 0;

		while (time<1)
		{
			time += Time.deltaTime*4;
			transform.localScale = Vector3.Lerp (correctPosition, Vector3.zero, time);
			yield return null;
		}

		ScoreManager.score += scoreValue;
		Grid.canCheckMatch =true;
		Destroy (gameObject);


	}
	public void CheckPlace()
	{
		Vector3 dPos = transform.position;
		Vector3 cPos= new Vector3(x,y,0);
		transform.position = Vector3.Lerp (dPos,cPos,1);
		//Grid.CheckMatchAfterSpawn();
	}
	}

