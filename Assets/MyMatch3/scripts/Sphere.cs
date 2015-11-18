using UnityEngine;
using System.Collections;

public class Sphere : MonoBehaviour {

	public int ID;
	public int x;
	public int y;

	public int moveDown;
	public int scoreValue;

	public bool matched;

	private Vector3 myScale;

	public static Transform select;
	public static Transform moveTo;
	
	void Start(){
		myScale = transform.localScale;
		//StartCoroutine ("FallDown");
	}

	void Update (){

	}

	void OnMouseOver(){
		transform.localScale = new Vector3(myScale.x+0.2f, myScale.y+0.2f,myScale.z+0.2f);
		if (Input.GetMouseButtonDown(0)){
			if(!select){
				select = transform;
				transform.localScale = new Vector3(myScale.x+0.2f, myScale.y+0.2f,myScale.z+0.2f);
			}
			else if(select != transform && !moveTo)	{
				moveTo = transform;
			}
		}
	}

	void OnMouseExit (){
		transform.localScale = myScale;
	}

	public IEnumerator MoveDown (){
		Vector3 correctPosition = transform.position;
		Vector3 newPosition = new Vector3 (transform.position.x, transform.position.y - moveDown, transform.position.z);
	
		float time  = 0;

		while(time<1){
			time += Time.deltaTime*4;
			transform.position = Vector3.Lerp (correctPosition, newPosition, time);
			yield return null;
		}
	}

	public IEnumerator FallDown (){
		Vector3 newPosition = transform.position;
		Vector3 correctPosition = new Vector3 (transform.position.x, transform.position.y+dY, transform.position.z);
		float time = 0;

		while (time<1){
			time += Time.fixedDeltaTime*4;
			transform.position= Vector3.Lerp (correctPosition, newPosition, time);
			yield return null;
		}
	}

	public IEnumerator DestroyBlock(){
		
		Vector3 correctPosition = transform.localScale;

		float time = 0;

		while (time<1){
			time += Time.deltaTime*4;
			transform.localScale = Vector3.Lerp (correctPosition, Vector3.zero, time);
			yield return null;
		}
		
		ScoreManager.score += scoreValue;
		Destroy (gameObject);
	}
	
/*	public void CheckPlace(){
		Vector3 dPos = transform.position;
		Vector3 cPos= new Vector3(x,y,0);
		transform.position = Vector3.Lerp (dPos,cPos,1);
	}
	*/
}

