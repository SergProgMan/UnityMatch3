using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public static int h = 12;
	public static int w = 10;

	public Transform ball;

	public static bool canCheckMatch = false;
	public static bool swapEfect;

    static Color[] Colors = new Color[]{
		Color.red,
		Color.cyan,
		Color.green,
		Color.white,
		Color.yellow,
		Color.blue,
		Color.gray
	//	Color.magenta
	};

	public int [,] board = new int [w,h];
	public int [,] matchBoard = new int [w,h];
	

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
				int foundColor = Mathf.RoundToInt(Random.Range(0,Colors.Length));

				int randomColor = foundColor;

				while(!MatchesOnSpawn(x,y,foundColor) && foundColor == randomColor)
				{
					foundColor = Mathf.RoundToInt(Random.Range(0,Colors.Length));
					randomColor = foundColor;
				}
			

				cloneBall.renderer.material.color = Colors[randomColor];
				board [x,y]=randomColor;
				Sphere b = cloneBall.gameObject.AddComponent<Sphere>();
				b.x = x;
				b.y = y;
				b.ID = randomColor;
			}
		}
	}

	public bool MatchesOnSpawn (int x,int y, int foundColor)
	{
		int countLeft = x-1;
		while ( countLeft >=0 && foundColor == board[countLeft,y])
		{
			countLeft--;
		}			
		int foundMatchesLeft = x-countLeft;
		if (foundMatchesLeft>2)
		{
			return false;
		}
		
		int countDown = y-1;
		while ( countDown >=0 && foundColor == board[x,countDown])
		{
			countDown--;
		}			
		int foundMatchesDown = y-countDown;
		if (foundMatchesDown>2)
		{
			return false;
		}
		
		return true;
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
			sel.transform.position = Vector3.Lerp(selTempPos, movTempPos, time);
			mov.transform.position = Vector3.Lerp(movTempPos, selTempPos, time);
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

	public bool CheckMatch(){
		//Check for any mistake in the board
	//	testBoard ();

		TestBoard();
		//Get all blocks in scene

		Sphere [] allb = FindObjectsOfType(typeof(Sphere))as Sphere[];
		Sphere sel = Sphere.select.gameObject.GetComponent<Sphere>();
		Sphere mov = Sphere.moveTo.gameObject.GetComponent<Sphere>();
		//SELECTED BLOCK
		//Check how many blocks have same ID as our selected block(for each direction)
		int countU = 0; //Count Up
		int countD = 0; //Count Down
		int countL = 0; //Count Left
		int countR = 0; //Count RIght
		
		//Check how many same blocks have sam ID...
		//Left
		for(int l = sel.x-1; l>=0; l--){
			if(board[l,sel.y]==sel.ID){//If block have same ID
				countL++;
			}
			if(board[l,sel.y]!=sel.ID){//If block have same ID
				break;
			}
		}
		//Right
		for(int r = sel.x; r<board.GetLength(0); r++){
			if(board[r,sel.y]==sel.ID){//If block have same ID
				countR++;
			}
			if(board[r,sel.y]!=sel.ID){//If block have same ID
				break;
			}
		}
		//Down
		for(int d = sel.y-1; d>=0; d--){
			if(board[sel.x,d]==sel.ID){
				countD++;
			}
			if(board[sel.x,d]!=sel.ID){
				break;
			}
		}
		
		//Up
		for(int u = sel.y; u<board.GetLength(1); u++){
			if(board[sel.x,u]==sel.ID){
				countU++;
			}
			
			if(board[sel.x,u]!=sel.ID){
				break;
			}
		}
		
		//MOVE TO BLOCK
		int countUU = 0; //Count Up
		int countDD = 0; //Count Down
		int countLL = 0; //Count Left
		int countRR = 0; //Count RIght
		
		//Check how many same blocks have sam ID...
		//Left
		for(int l = mov.x-1; l>=0; l--){
			if(board[l,mov.y]==mov.ID){//If block have same ID
				countLL++;
			}
			if(board[l,mov.y]!=mov.ID){//If block have same ID
				break;
			}
		}
		//Right
		for(int r = mov.x; r<board.GetLength(0); r++){
			if(board[r,mov.y]==mov.ID){//If block have same ID
				countRR++;
			}
			if(board[r,mov.y]!=mov.ID){//If block have same ID
				break;
			}
		}
		//Down
		for(int d = mov.y-1; d>=0; d--){
			if(board[mov.x,d]==mov.ID){
				countDD++;
			}
			if(board[mov.x,d]!=mov.ID){
				break;
			}
		}
		
		//Up
		for(int u = mov.y; u<board.GetLength(1); u++){
			if(board[mov.x,u]==mov.ID){
				countUU++;
			}
			
			if(board[mov.x,u]!=mov.ID){
				break;
			}
		}
		
		
		
		
		//Check if there is 3+ match 
		if((countL+countR>=3 || countD+countU>=3) || (countLL+countRR>=3 || countDD+countUU>=3)){
			if(countL+countR>=3){
				//Destroy and mark empty block
				for(int cl = 0; cl<=countL; cl++){
					foreach(Sphere b in allb){
						if(b.x == sel.x-cl && b.y == sel.y){
							b.scoreValue = (countL+countR-1)*50;
							b.matched = true;

						}
					}
				}
				for(int cr = 0; cr<countR; cr++){
					foreach(Sphere b in allb){
						if(b.x == sel.x+cr && b.y == sel.y){
							b.scoreValue = (countL+countR-1)*50;
							b.matched = true;
						}
					}
				}
			}
			if(countD+countU>=3){
				for(int cd = 0; cd<=countD; cd++){
					foreach(Sphere b in allb){
						if(b.x == sel.x && b.y == sel.y - cd){
							b.scoreValue = (countD+countU-1)*50;
							b.matched = true;
						}
					}
				}
				for(int cu = 0; cu<countU; cu++){
					foreach(Sphere b in allb){
						if(b.x == sel.x && b.y == sel.y+cu){
							b.scoreValue = (countD+countU-1)*50;
							b.matched = true;
						}
					}
				}
			}
			
			
			
			if(countLL+countRR>=3){
				//Destroy and mark empty block
				for(int cl = 0; cl<=countLL; cl++){
					foreach(Sphere b in allb){
						if(b.x == mov.x-cl && b.y == mov.y){
							b.scoreValue = (countLL+countRR-1)*50;
							b.matched = true;
						}
					}
				}
				for(int cr = 0; cr<countRR; cr++){
					foreach(Sphere b in allb){
						if(b.x == mov.x+cr && b.y == mov.y){
							b.scoreValue = (countLL+countRR-1)*50;
							b.matched = true;
						}
					}
				}
			}
			if(countDD+countUU>=3){
				for(int cd = 0; cd<=countDD; cd++){
					foreach(Sphere b in allb){
						if(b.x == mov.x && b.y == mov.y - cd){
							b.scoreValue = (countDD+countUU-1)*50;
							b.matched = true;

						}
					}
				}
				for(int cu = 0; cu<countUU; cu++){
					foreach(Sphere b in allb){
						if(b.x == mov.x && b.y == mov.y+cu){
							b.scoreValue = (countDD+countUU-1)*50;
							b.matched = true;

						}
					}
				}
			}

			DeleteMatched();
			return true;

			
		}
		

		return false;
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


	public bool CheckMatchHorizontal()
	{

		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];
	
		for (int y = 0; y < h; y++)
		{
			for (int x = 0; x < w; x++){




				int countLeft = x+1;
				int foundColor = board[x,y];
				while ( countLeft < w && foundColor == board[countLeft,y])
				{
					countLeft++;
				}
				int foundMatches = countLeft - x;
				countLeft--;
			//	while ( foundMatches > 2 && x <= countLeft)
			//	{
			//		matchBoard[x++,y] = 8;
			//	}
				x = countLeft;

				if (foundMatches > 2){
					for (int cd =0; cd<foundMatches;cd++){
						foreach(Sphere s in allS){
							if (s.x == x-cd && s.y ==y)
							{
								s.scoreValue = (foundMatches-1)*50;
								s.matched =true;
							}
						}
					}
					return true;
			}


		}
	}
		return false;
	}


	public bool CheckMatchVertical()
	{
		Sphere [] allS = FindObjectsOfType(typeof(Sphere))as Sphere[];

		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				//start checking matches from the coordinate above
				int countUp = y+1;
				// store the found color so we don't get mixed up
				int foundColor = board[x, y];
				// matchfinding
				while ( countUp < h && foundColor == board[x, countUp])
				{
					countUp++;
				}
				// how mych did the matchfinding loop increment countUp == number of matches
				int foundMatches = countUp - y;
				// matchfinding loop exits when no match is found or when out of array bounds - so decrement.
				countUp--;
				// if enough matches
			//	while ( foundMatches > 2 && y <= countUp) // countUp < h done in matchfinding. no need to check y < h
			//	{
			//		matchBoard[x, y++] = 8;
			//	}
				y = countUp; // continue from last matching ball. for-loop will increment y++ next


				if (foundMatches > 2){
					for (int cd =0; cd<foundMatches;cd++){
						foreach(Sphere s in allS){
							if (s.x == x && s.y ==y-cd)
							{
								s.scoreValue = (foundMatches-1)*50;
								s.matched =true;

							}
						}
					}
					return true;
				}
			}
		}
		return false;
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
	