using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	
	public static int h = 14;
	public static int w = 10;
	
	public int [,] grid = new int [w,h];
	public int [,] canMatchGrid = new int [w,h];
	
	public Transform ball;

	private bool canMatch = false;
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
		GameOver
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

			case State.GameOver:
			GameOver();
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
		PrintMatchedBalls();
		CheckBoard();

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
		LoockForPosibleMatch();
		if(canMatch)
		{
		SwitchState(State.Select);	
		}
		if(!canMatch)
		{
			GenerateGrid();
		}
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
								s.scoreValue = countRight*50;
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
							s.scoreValue = countUp*50;
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
			LoockForPosibleMatch();
			if(canMatch){
			SwitchState(State.Select);
			}
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
		PrintMoveDown();
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

	void PrintMatchedBalls()
	{
		Debug.Log("<color=green> PrintMatchedBalls</color>");
		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
		string str = "";

		for(int x=0; x<grid.GetLength(0); x++){
			for(int y=0; y<grid.GetLength(1); y++){
				foreach (Sphere s in allS){
					if (s.x == x && s.y == y && s.matched){
						str += "1 ";
					}
					if (s.x == x && s.y == y && !s.matched){
						str += "0 ";
					}
				}
			}
			str += "\n";
		
	}
		Debug.Log(str);
	}
		void PrintMoveDown()
		{
		Debug.Log("<color=green> PrintMoveDown</color>");
			Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
			string str = "";

			for(int x=0; x<grid.GetLength(0); x++){
				for(int y=0; y<grid.GetLength(1); y++){
					foreach (Sphere s in allS){
						if (s.x == x && s.y == y){
						str += " " +s.moveDown;
						}
					}
				}
				str += "\n";
				
			}
		Debug.Log(str);
	}

	void CheckBoard()
	{
		Debug.Log("<color=green> CheckBoard</color>");
		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
		for(int x=0; x<grid.GetLength(0); x++){
			for(int y=0; y<grid.GetLength(1); y++){
				foreach (Sphere s in allS){
					if (s.x == x && s.y == y){
						if(grid[x,y]!= s.ID && grid[x,y]!=777){
							Debug.Log("<color=red> Grid</color>"+x+" "+y+ "=" +grid[x,y]+"but Sphere"+s.ID);							
						}
					}
				}
			}
		}
	}

	void LoockForPosibleMatch ()	{	
		for (int y=0; y< grid.GetLength(1); y++){
			for (int x=0; x < grid.GetLength(0); x++){
					if (x+1 < grid.GetLength(0))// horizontal, 2+1
					{	
					if(grid[x,y]==grid[x+1,y])
					{
						CheckIfInArrayAndId(x-1,y+1,grid[x,y]);	//left up
						CheckIfInArrayAndId(x-2,y,grid[x,y]);	//left left
						CheckIfInArrayAndId(x-1,y-1,grid[x,y]);	//left down
						CheckIfInArrayAndId(x+2,y+1,grid[x,y]);	//right up
						CheckIfInArrayAndId(x+3,y,grid[x,y]);	//right right
						CheckIfInArrayAndId(x+2,y+1,grid[x,y]);	//right down
					}
			}

					if (x+2 < grid.GetLength(0))//horizontal, middle
					{
					if (grid[x,y]==grid[x+2,y])
					{
						CheckIfInArrayAndId(x+1,y+1,grid[x,y]);	//up
						CheckIfInArrayAndId(x+1,y-1,grid[x,y]);	//down
					}
					}

				if(y+1 < grid.GetLength(1))	//vertical, 2+1
					{
					if(grid[x,y]==grid[x,y+1])
					{
						CheckIfInArrayAndId(x-1,y+2,grid[x,y]);	//up left
						CheckIfInArrayAndId(x,y+3,grid[x,y]);	//up up
						CheckIfInArrayAndId(x+1,y+2,grid[x,y]);	//up right
						CheckIfInArrayAndId(x-1,y-1,grid[x,y]);	//down left
						CheckIfInArrayAndId(x,y-2,grid[x,y]);	//down down
						CheckIfInArrayAndId(x+1,y-1,grid[x,y]);	//down right
					}
				}

				if(y+2 < grid.GetLength(1))	//vertical, middle
					{
					if(grid[x,y]==grid[x,y+2])
					{
						CheckIfInArrayAndId(x-1,y,grid[x,y]);	//left
						CheckIfInArrayAndId(x+1,y,grid[x,y]);	//right
					}
				}
			}
		}
		if(!canMatch){
			SwitchState(State.GameOver);
		}
	}
	
	void CheckIfInArrayAndId(int x,int y, int iD){ 
		if(x<grid.GetLength(0) && x>=0 && y<grid.GetLength(1) && y>=0 && grid[x,y]==iD){
			{
					canMatch=true;
					canMatchGrid [x,y]=1;
			}
		}	
	}

	void GameOver()
	{
		
	}
}
	
