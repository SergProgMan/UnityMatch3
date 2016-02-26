using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
	public static int h = 12;
	public static int w = 10;

	public int [, ] grid = new int [w, h];
	public int [, ] canMatchGrid = new int [w, h];

	public Transform gem;

	private bool canMatch = false;
	private bool findMatches = false;
	private bool swapEffect = false; // need to be add to Swap fuction!!!

	static Color[] Colors = new Color[] {
		Color.red,
		Color.cyan,
		Color.green,
		Color.white,
		Color.yellow,
		Color.blue,
		Color.gray
	};

	// Use this for initialization
	void Start () {
		GenerateGrid();
	}

	// Update is called once per frame
	void Update () {
		if(Gem.select && Gem.moveTo) {
			if(CheckIfNear()) {
				Swap();
			}
			else {
				Gem.select = null;
				Gem.moveTo = null;
			}
		}
	}

	bool CheckIfNear()
	{
		Gem sel = Gem.select.gameObject.GetComponent<Gem>();
		Gem mov = Gem.moveTo.gameObject.GetComponent<Gem>();
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

	void GenerateGrid() {
		for (int y = 0; y < h; y++) {
			for (int x = 0; x < w; x++) {
				Transform cloneGem = (Transform)Instantiate(gem, new Vector3(x, y, 0), Quaternion.identity) as Transform;

				int randomColor = Mathf.RoundToInt(Random.Range(0, Colors.Length));

				while(MatchesOnSpawn(x, y, randomColor))
				{
					randomColor = Mathf.RoundToInt(Random.Range(0, Colors.Length));
				}

				cloneGem.GetComponent<Renderer>().material.color = Colors[randomColor];
				grid [x, y] = randomColor;
				Gem b = cloneGem.gameObject.AddComponent<Gem>();
				b.x = x;
				b.y = y;
				b.ID = randomColor;
			}
		}

		LoockForPosibleMatch();
		if(canMatch)
		{
		}

		if(!canMatch)
		{
			GenerateGrid();
		}
	}

	bool MatchesOnSpawn (int x, int y, int randomColor) //check matching on spawn
	{
		if (x-2>= 0) {
			if (grid[x-1, y] == randomColor && grid[x-2, y] == randomColor) {
				return true;
			}
		}

		if (y-2>= 0) {
			if(grid[x, y-1] == randomColor && grid[x, y-2] == randomColor) {
				return true;
			}
		}

		return false;
	}

	void LoockForPosibleMatch ()	{
		canMatch = false;
		for (int y = 0; y< grid.GetLength(1); y++) {
			for (int x = 0; x < grid.GetLength(0); x++) {
				if (x+1 < grid.GetLength(0))// horizontal, 2+1
				{
					if(grid[x, y] == grid[x+1, y])
					{
						CheckIfInArrayAndId(x-1, y+1, grid[x, y]);	//left up
						CheckIfInArrayAndId(x-2, y, grid[x, y]);	//left left
						CheckIfInArrayAndId(x-1, y-1, grid[x, y]);	//left down
						CheckIfInArrayAndId(x+2, y+1, grid[x, y]);	//right up
						CheckIfInArrayAndId(x+3, y, grid[x, y]);	//right right
						CheckIfInArrayAndId(x+2, y+1, grid[x, y]);	//right down
					}
				}

				if (x+2 < grid.GetLength(0))//horizontal, middle
				{
					if (grid[x, y] == grid[x+2, y])
					{
						CheckIfInArrayAndId(x+1, y+1, grid[x, y]);	//up
						CheckIfInArrayAndId(x+1, y-1, grid[x, y]);	//down
					}
				}

				if(y+1 < grid.GetLength(1))	//vertical, 2+1
				{
					if(grid[x, y] == grid[x, y+1])
					{
						CheckIfInArrayAndId(x-1, y+2, grid[x, y]);	//up left
						CheckIfInArrayAndId(x, y+3, grid[x, y]);	//up up
						CheckIfInArrayAndId(x+1, y+2, grid[x, y]);	//up right
						CheckIfInArrayAndId(x-1, y-1, grid[x, y]);	//down left
						CheckIfInArrayAndId(x, y-2, grid[x, y]);	//down down
						CheckIfInArrayAndId(x+1, y-1, grid[x, y]);	//down right
					}
				}

				if(y+2 < grid.GetLength(1))	//vertical, middle
				{
					if(grid[x, y] == grid[x, y+2])
					{
						CheckIfInArrayAndId(x-1, y, grid[x, y]);	//left
						CheckIfInArrayAndId(x+1, y, grid[x, y]);	//right
					}
				}
			}
		}

		if(!canMatch) {
			GameOver();
		}
	}

	void CheckIfInArrayAndId(int x, int y, int iD) {
		if(x<grid.GetLength(0) && x>= 0 && y<grid.GetLength(1) && y>= 0 && grid[x, y] == iD) {
			{
				canMatch = true;
				canMatchGrid [x, y] = 1;
			}
		}
	}

	void Swap() {
		PrintField(0);

		Gem sel = Gem.select.gameObject.GetComponent<Gem>();
		Gem mov = Gem.moveTo.gameObject.GetComponent<Gem>();

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

		grid[sel.x, sel.y] = sel.ID;
		grid[mov.x, mov.y] = mov.ID;

		if (swapEffect) {
			swapEffect = false;
			Gem.select = null;
			Gem.moveTo = null;
		}
		else if (!swapEffect) {
			swapEffect = true;
			CheckMatches();
		}
	}

	void CheckMatches()
	{
		PrintField(1);
		Gem [] allS = FindObjectsOfType(typeof(Gem))as Gem[];

		for (int y = 0; y<grid.GetLength(1); y++) {
			for (int x = 0; x<grid.GetLength(0); x++) {
				int countRight = x+1; //number of horizontal matches
				while (countRight<grid.GetLength(0) && grid[x, y] == grid[countRight, y]) {
					countRight++;
				}

				int foundMatchesX = countRight - x;
				if (foundMatchesX > 2) {
					for (int i = 0; i<foundMatchesX; i++) {
						foreach(Gem s in allS) {
							if (s.x == x+i && s.y == y) {
								//s.scoreValue = countRight*50;
								s.matched = true;
								findMatches = true;
							}
						}
					}
				}

				int countUp = y + 1; //number of vertical matches
				while (countUp<grid.GetLength(1) && grid[x, y] == grid[x, countUp]) {
					countUp++;
				}

				int foundMatchesY = countUp - y;
				if(foundMatchesY>2) {
					for (int i = 0; i<foundMatchesY; i++) {
						foreach (Gem s in allS) {
							if (s.x == x && s.y == y+i) {
								//s.scoreValue = countUp*50;
								s.matched = true;
								findMatches = true;
							}
						}
					}
				}
			}
		}

		PrintMatchedBalls();

		if(findMatches) {
			findMatches = false;
			swapEffect = false;
			Gem.select = null;
			Gem.moveTo = null;
			DeleteMatched();
		}
		else if (swapEffect) {
			Swap();
		}
		else if (!swapEffect) {
			Gem.select = null;
			Gem.moveTo = null;
			LoockForPosibleMatch();
			if(!canMatch) {
				GameOver();
			}
		}
	}

	void GameOver() {
		Debug.Log ("GameOver!");
	}

	void DeleteMatched() {
		PrintField(2);
		Gem [] allS = FindObjectsOfType(typeof(Gem))as Gem[];
		foreach (Gem s in allS) {
			if (s.matched) {
				grid [s.x, s.y] = 8;
				s.DestroyBall();
//				s.StartCoroutine(s.DestroyBlock());
			}
		}

		MoveDown();
	}
	
	void MoveDown (){
		PrintField(3);
		Gem [] allS = FindObjectsOfType(typeof(Gem))as Gem[];
		for (int x=0; x<grid.GetLength(0); x++){
			for (int y=0; y<grid.GetLength(1); y++){
				if (grid[x,y] == 8){
					foreach (Gem s in allS){
						if(s.x==x && s.y > y){
							s.readyToMove = true;
							s.moveDown ++;
						}
					}
				}
			}
		}
		PrintMoveDown();
		foreach (Gem s in allS){
			if (s.readyToMove){
				s.MoveByY();
				//s.StartCoroutine(s.MoveDown());
				grid[s.x,s.y-s.moveDown] = s.ID;
				s.y -= s.moveDown;

				for (int i=0; i< s.moveDown; i++){
					grid[s.x,grid.GetLength(1) -1 - i]=8;
				}
				s.moveDown = 0;
				s.readyToMove = false;
			}
		}

		Respawn();
	}
	

	void Respawn(){
		PrintField(4);
		for (int x=0; x<grid.GetLength(0); x++){
			for (int y=0; y<grid.GetLength(1); y++){
				if (grid[x,y]==8){
					Transform cloneBall = (Transform)Instantiate(gem, new Vector3(x,y,0), Quaternion.identity) as Transform;
					int randomColor = Mathf.RoundToInt(Random.Range(0,Colors.Length));
					cloneBall.GetComponent<Renderer>().material.color = Colors[randomColor];
					grid [x,y]=randomColor;
					Gem b = cloneBall.gameObject.AddComponent<Gem>();
					b.x = x;
					b.y = y;
					b.ID = randomColor;
					//	b.fallEfect = true;
					//	b.dY = matchBoard[x,y];
				}
			}
		}
		//PrintField();
		CheckMatches();
	}

	void PrintField(int x)
	{
		CheckBoard();
		string str = "";
		int i, j;
		for (j = h - 1; j >= 0; j--)
		{
			for (i = 0; i <w; i++)
				str += " " + grid [i, j];
			str += "\n";
		}
		if (x==0){
			Debug.Log("<color=red> Swap </color>");
		}
		if (x==1){
			Debug.Log("<color=red> CheckMatches </color>");
		}
		if (x==2){
			Debug.Log("<color=red> DeleteMatched </color>");
		}
		if (x==3){
			Debug.Log("<color=red> MoveDown </color>");
		}
		if (x==4){
			Debug.Log("<color=red> Respawn </color>");
		}
		Debug.Log(str);
	}

	void PrintGemsId(){
		
	}

	void PrintMatchedBalls()
	{
		Debug.Log("<color=green> PrintMatchedBalls</color>");
		Gem [] allS = FindObjectsOfType(typeof(Gem))as Gem[];
		string str = "";

		for(int y=grid.GetLength(1)-1; y>=0; y--){
			for(int x=0; x<grid.GetLength(0); x++){
				foreach (Gem s in allS){
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
		Gem [] allS = FindObjectsOfType(typeof(Gem))as Gem[];
		string str = "";

		for(int y=grid.GetLength(1)-1; y>=0; y--){
			for(int x=0; x<grid.GetLength(0); x++){
				foreach (Gem s in allS){
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
		Gem [] allS = FindObjectsOfType(typeof(Gem))as Gem[];
		for(int x=0; x<grid.GetLength(0); x++){
			for(int y=0; y<grid.GetLength(1); y++){
				foreach (Gem s in allS){
					if (s.x == x && s.y == y){
						if(grid[x,y]!= s.ID && grid[x,y]!=8){
							Debug.Log("<color=yellow> Grid[</color>"+x+" "+y+ "]=" +grid[x,y]+"but Sphere="+s.ID);	
							//grid[x,y]=s.ID;
						}
					}
				}
			}
		}
	}


	
	}
