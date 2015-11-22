using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	
	public static int h = 12;
	public static int w = 10;
	
	public int [,] grid = new int [w,h];

	public Transform ball;
	
	private bool findMatches = false;
	private bool swapEffect = false; // need to be add to Swap fuction!!!

	public enum State{
		GenerateGrid,
		Select,
		Swap,
		CheckMatches,
		DeleteMatched,
		MoveDown,
		Respawn,
		
		Debugging //for debugging
	};

	 static Color[] Colors = new Color[]{
		Color.red,
		Color.cyan,
		Color.green,
		Color.white,
		Color.yellow,
		Color.blue,
		Color.gray
	};

	State activeState;
		
	void Start(){
	activeState = State.GenerateGrid;
	}

	void Update(){
		RunEngine();
	}
	
	void RunEngine(){
		switch(activeState){
			case State.GenerateGrid:
			GenerateGrid();
			break;
			
			case State.Select:
			Select();
			break;
			
			case State.Swap:
			Swap();
			break;
			
			case State.CheckMatches:
			CheckMatches();
			break;
			
			case State.DeleteMatched:
			DeleteMatched();
			break;
			
			case State.MoveDown:
			MoveDown();
			break;
			
			case State.Respawn:
			Respawn();
			break;

		}
	}
	
	void SwitchState (State nextState){
		//activeState = nextState;
	
		Debugging(nextState);
	}
	
	void Debugging(State nextState){
		Debug.Log("activeState " +activeState);
		Debug.Log("nextState " +nextState);
		PrintField();
		//Debug.Break();
		activeState = nextState;
	}
	
	void GenerateGrid(){
		for (int y = 0; y < h; y++) {
			for (int x = 0; x < w; x++) {
				Transform cloneBall = (Transform)Instantiate(ball, new Vector3(x,y,0), Quaternion.identity) as Transform;
				
				int randomColor = Mathf.RoundToInt(Random.Range(0,Colors.Length));
				
				while(MatchesOnSpawn(x,y,randomColor))
				{
					randomColor = Mathf.RoundToInt(Random.Range(0,Colors.Length));
				}
				
				cloneBall.GetComponent<Renderer>().material.color = Colors[randomColor];
				grid [x,y]=randomColor;
				Sphere b = cloneBall.gameObject.AddComponent<Sphere>();
				b.x = x;
				b.y = y;
				b.ID = randomColor;
			}
		}
		SwitchState(State.Select);
	}

	bool MatchesOnSpawn (int x, int y, int randomColor) //check matching on spawn
	{
		if (x-2>=0){
			if (grid[x-1,y]==randomColor && grid[x-2,y]==randomColor ){
			return true;
			}
		}
		
		if (y-2>=0){
			if(grid[x,y-1]==randomColor && grid[x,y-2]==randomColor){
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
	
	void Select(){
		if(Sphere.select && Sphere.moveTo){
			if(CheckIfNear()){
				SwitchState(State.Swap);
			}
			else{
				Sphere.select = null;
				Sphere.moveTo = null;
			}
		}
	}
	
	void Swap(){
		Sphere sel = Sphere.select.gameObject.GetComponent<Sphere>();
		Sphere mov = Sphere.moveTo.gameObject.GetComponent<Sphere>();

		Vector3 selTempPos = sel.transform.position;
		Vector3 movTempPos = mov.transform.position;

		sel.transform.position = Vector3.Lerp(selTempPos, movTempPos, 1);
		mov.transform.position = Vector3.Lerp(movTempPos, selTempPos, 1);
		  
		
		int tempX = sel.x;
		int tempY = sel.y;

		sel.x = mov.x;
		sel.y = mov.y;

		mov.x = tempX;
		mov.y = tempY;

		grid[sel.x,sel.y]=sel.ID;
		grid[mov.x,mov.y]=mov.ID;
	
		if (swapEffect){
			swapEffect = false;
			Sphere.select = null;
			Sphere.moveTo = null;
			SwitchState(State.Select);
		}
		else if (!swapEffect){
			swapEffect = true;
			SwitchState(State.CheckMatches);
		}
	}

	void CheckMatches()
	{
		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
	
		for (int y=0; y<grid.GetLength(1); y++){
			for (int x=0; x<grid.GetLength(0); x++){
				int countRight=x+1; //number of horizontal matches
				while (countRight<grid.GetLength(0) && grid[x,y] == grid[countRight,y]){
					countRight++;
				}
				int foundMatchesX = countRight - x;
				if (foundMatchesX > 2){
					for (int i=0; i<foundMatchesX; i++){
						foreach(Sphere s in allS){
							if (s.x == x+i && s.y ==y){
								//s.scoreValue = countRight*50;
								s.matched =true;
								findMatches = true;
							}
						}
					}
				}
				
				int countUp = y + 1; //number of vertical matches
				while (countUp<grid.GetLength(1) && grid[x,y] == grid[x,countUp]){
					countUp++;	
				}
				int foundMatchesY = countUp - y;
				if(foundMatchesY>2){
					for (int i=0; i<foundMatchesY; i++){
						foreach (Sphere s in allS){
							if (s.x == x && s.y == y+i){
							//s.scoreValue = countUp*50;
							s.matched =true;
							findMatches = true;	
							}
						}
					}
				}
			}
		}
		
		if(findMatches){
			findMatches = false;
			swapEffect = false;
			Sphere.select = null;
			Sphere.moveTo = null;
			SwitchState(State.DeleteMatched);
		}
		else if (swapEffect){
			SwitchState(State.Swap);
		}
		else if (!swapEffect){
			Sphere.select = null;
			Sphere.moveTo = null;
			SwitchState(State.Select);
		}
	}

	void DeleteMatched(){
		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
		foreach (Sphere s in allS){
			if (s.matched){
				grid [s.x,s.y] = 777;
				s.DestroyBall();
			//	s.StartCoroutine(s.DestroyBlock());
			}
		}
		SwitchState(State.MoveDown);
	}

	void MoveDown (){
		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
		for (int x=0; x<grid.GetLength(0); x++){
			for (int y=0; y<grid.GetLength(1); y++){
				if (grid[x,y] == 777){
					foreach (Sphere s in allS){
						if(s.x==x && s.y > y){
							s.readyToMove = true;
							s.moveDown ++;
						}
					}
				}
			}
		}
		foreach (Sphere s in allS){
			if (s.readyToMove){
				//s.StartCoroutine(s.MoveDown());
				s.MoveByY();
				s.y -= s.moveDown;
				grid[s.x,s.y] = s.ID;
				for (int i=0; i< s.moveDown; i++){
					grid[s.x,grid.GetLength(1) -1 - i]=777;
				}
				s.moveDown = 0;
				s.readyToMove = false;
			}
		}
		SwitchState(State.Respawn);
	}
	
	void Respawn(){
		for (int x=0; x<grid.GetLength(0); x++){
			for (int y=0; y<grid.GetLength(1); y++){
				if (grid[x,y]==777){
					Transform cloneBall = (Transform)Instantiate(ball, new Vector3(x,y,0), Quaternion.identity) as Transform;
					int randomColor = Mathf.RoundToInt(Random.Range(0,Colors.Length));
					cloneBall.GetComponent<Renderer>().material.color = Colors[randomColor];
					grid [x,y]=randomColor;
					Sphere b = cloneBall.gameObject.AddComponent<Sphere>();
					b.x = x;
					b.y = y;
					b.ID = randomColor;
				//	b.fallEfect = true;
				//	b.dY = matchBoard[x,y];
				}
			}
		}
		SwitchState(State.CheckMatches);
	}

	void TestBoard(){
		Sphere[] allb = FindObjectsOfType(typeof(Sphere)) as Sphere[];
		for(int x=0; x<grid.GetLength(0); x++){
			for(int y=0; y<grid.GetLength(1); y++){
				foreach(Sphere b in allb){
					if(b.x == x && b.y ==y){
						if(grid[x,y]!=b.ID){//Found a mistake
							grid[x,y] = b.ID;//Fix the mistake
						}
					}
				}
			}
		}
	}




	void PrintField()
	{
		string str = "";
		int i, j;
		for (j = h - 1; j >= 0; j--)
		{
			for (i = 0; i <w; i++)
				str += " " + grid [i, j];
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
	
