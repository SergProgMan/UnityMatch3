using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public static int h = 12;
	public static int w = 10;

	public Transform ball;
	
	private bool findMatches = false;

  static Color[] Colors = new Color[]{
		Color.red,
		Color.cyan,
		Color.green,
		Color.white,
		Color.yellow,
		Color.blue,
		Color.gray
	};

	public int [,] grid = new int [w,h];
	
	void Start()
	{
		CreateBalls();
	}


	void FixedUpdate()
	{
		CheckMatchAfterSpawn();

		    
		if(Sphere.select && Sphere.moveTo)
		{
			if(CheckIfNear()==true)
			{

				if (!swapEfect)
				{
					swapEfect = true;
					StartCoroutine(Swap(true));
				}
			}
				
			
			else 
				{
				Sphere.select = null;
				Sphere.moveTo = null;
				}
		}


	}


	public void CreateBalls()
	{
		for (int y = 0; y < h; y++) {
			for (int x = 0; x < w; x++) {
				Transform cloneBall = (Transform)Instantiate(ball, new Vector3(x,y,0), Quaternion.identity) as Transform;
				
				int randomColor = Mathf.RoundToInt(Random.Range(0,Colors.Length));
				
				while(MatchesOnSpawn(x,y,randomColor))
				{
					randomColor = Mathf.RoundToInt(Random.Range(0,Colors.Length));
				}
				
				cloneBall.renderer.material.color = Colors[randomColor];
				grid [x,y]=randomColor;
				Sphere b = cloneBall.gameObject.AddComponent<Sphere>();
				b.x = x;
				b.y = y;
				b.ID = randomColor;
			}
		}
	}

	public bool MatchesOnSpawn (int x, int y, int randomColor) //checking matching on spawn
	{
		if (x-2>=0){
			if (grid[x-1,y]==randomColor && grid[x-2,y]==randomColor ){
			return true;
			}
		}
		
		if (y-2>=0){
			if(grid[x-2,y]==randomColor && grid[x,y-2]==randomColor){
				return true;
			}
		}
		
		return false;
	}

	bool CheckIfNear()
	{
		Sphere sel = Sphere.select.gameObject.GetComponent<Sphere>();
		Sphere mov = Sphere.moveTo.gameObject.GetComponent<Sphere>();
		if (sel.x-1 == mov.x && sel.y == mov.y)
			return true;
		if (sel.x+1 == mov.x && sel.y == mov.y)
			return true;
		if (sel.x == mov.x && sel.y+1 == mov.y)
			return true;
		if (sel.x == mov.x && sel.y-1 == mov.y)
			return true;
		Debug.Log ("What are you trying to select?");
		return false;
	}
	
	IEnumerator Swap(bool match3)
	{
		Sphere sel = Sphere.select.gameObject.GetComponent<Sphere>();
		Sphere mov = Sphere.moveTo.gameObject.GetComponent<Sphere>();

		Vector3 selTempPos = sel.transform.position;
		Vector3 movTempPos = mov.transform.position;
		float time = 0;

		while (time<1)
		{
			time+=Time.deltaTime*5;
			sel.transform.position = Vector3.SLerp(selTempPos, movTempPos, time);
			mov.transform.position = Vector3.SLerp(movTempPos, selTempPos, time);
			yield return null;
		}

		int tempX = sel.x;
		int tempY = sel.y;

		sel.x = mov.x;
		sel.y = mov.y;

		mov.x = tempX;
		mov.y = tempY;

		board[sel.x,sel.y]=sel.ID;
		board[mov.x,mov.y]=mov.ID;

		if (match3 == true)
		{
			if (CheckMatch() == true)
			{
				swapEfect = false;
				Sphere.select = null;
				Sphere.moveTo = null;
			}

			else 
			{
				StartCoroutine(Swap(false));
				Sphere.select = null;
				Sphere.moveTo = null;

			}
		}
		else 
		{
			swapEfect = false;
		}
	}



	public void CheckMatchAfterSpawn()
	{
		if(canCheckMatch)
		{
			TestBoard();

		if (CheckMatchHorizontal() | CheckMatchVertical())
		{
			DeleteMatched();
		}
		}
	}


	public void CheckMatch()
	{
		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
	
		for (int y=0; y<=grid.GetLength(1); y++){
			for (int x=0; x<=grid.GetLength(0); x++){
				int countRight=0; //number of horizontal matches
				while (countRight<grid.GetLength(0) && grid[x,y] == grid[x+countRight+1,y]){
					countRight++;
				}
				if (countRight >= 2){
					for (int i=0; i<=countRight; i++){
						foreach(Sphere s in allS){
							if (s.x == x+i && s.y ==y){
								//s.scoreValue = foundMatches*50;
								s.matched =true;
								findMatches = true;
							}
						}
					}
				}
				
				int countUp=0; //number of vertical matches
				while (countUp<grid.GetLength(1) && grid[x,y] == grid [x, y+countUp+1]){
					countUp++;	
				}
				if(countUp>=2){
					for (int i=0; i<=countUp; i++){
						foreach (Sphere s in allS){
							if (s.x = x && s.y == y+i){
							s.scoreValue = foundMatches*50;
							s.matched =true;
							findMatches = true;	
							}
						}
					}
				}
			}
		}
	
	}


	

	public void DeleteMatched()
	{
		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
		foreach (Sphere s in allS)
		{
			if (s.matched)
			{
				board [s.x,s.y] = 77;
				s.StartCoroutine(s.DestroyBlock());
			}
		}
		MoveY ();
	}

	public void MoveY ()
	{
		canCheckMatch =false;

		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
		int moveDownY = 0;
		for (int x=0; x<w; x++)
		{
			for (int y=h-1; y>=0; y--)
			{
				if (board[x,y]==77)
				{
					canCheckMatch = false;
					foreach (Sphere b in allS)
					{
						if(b.x==x && b.y > y)
						{
							b.y -=1;
							b.readyToMove = true;

						}
					}
					moveDownY ++;

				}
			}

			foreach (Sphere s in allS)
			{
				if (s.readyToMove){
					s.StartCoroutine(s.MoveDown(moveDownY));
					s.readyToMove =false;
					canCheckMatch =true;
					board[s.x,s.y] = s.ID;


				}

			}
			MarkEmpty(x, moveDownY);

			moveDownY = 0;


		}
		Respawn();

	//	CheckMatchAfterSpawn();

	}



	public void MarkEmpty (int x, int downY)
	{
		for (int i=0; i<downY; i++)
		{
			board [x, h-1-i] = 77;
			matchBoard [x, h-1-i] = downY;

		}

	}

	public void TestBoard()
	{


		Sphere[] allb = FindObjectsOfType(typeof(Sphere)) as Sphere[];
		for(int x=0; x<board.GetLength(0); x++){
			for(int y=0; y<board.GetLength(1); y++){
				foreach(Sphere b in allb){
					if(b.x == x && b.y ==y){
						if(board[x,y]!=b.ID){//Found a mistake
							board[x,y] = b.ID;//Fix the mistake
						}
					}
				}
			}
		}
	}


	public void Respawn(){

		canCheckMatch =false;

		for (int x=0; x<board.GetLength(0); x++){
			for (int y=0; y<board.GetLength(1); y++){
				if (board[x,y]==77){
					Transform cloneBall = (Transform)Instantiate(ball, new Vector3(x,y,0), Quaternion.identity) as Transform;
					int randomColor = Mathf.RoundToInt(Random.Range(0,Colors.Length));
					cloneBall.renderer.material.color = Colors[randomColor];
					board [x,y]=randomColor;
					Sphere b = cloneBall.gameObject.AddComponent<Sphere>();
					b.x = x;
					b.y = y;
					b.ID = randomColor;
					b.fallEfect = true;
					b.dY = matchBoard[x,y];

				}
			}
		}
	}

	public void PrintField()
	{
		string str = "";
		int i, j;
		for (j = h - 1; j >= 0; j--)
		{
			for (i = 0; i <w; i++)
				str += " " + board [i, j];
			str += "\n";
		}
		Debug.Log(str);
	}

	/*public void LoockForPosibleMatch ()
	{	
		TestBoard();
		Sphere[] allb = FindObjectsOfType(typeof(Sphere)) as Sphere[];
		// horizontal, 2+1
		for (int y=0; y<= board.GetLength(1); y++)
		{
			for (int x=0; x <= board.GetLength(0); x++)
			{
				foreach(Sphere b in allb)
				{
					if (board[x,y]=board[x+1,y])
					{
						if (x>=board.GetLength(0)-2 && board[x,y] == board[x-2,y])
						{

						}
				}
			}
		}

		}
	}*/

}
	
