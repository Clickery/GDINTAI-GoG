using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    


    private BoardManager boardRef;//tiles
    private Game gameRef;//pieces

   





    // Start is called before the first frame update
    void Start()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Board");
        this.boardRef = temp.GetComponent<BoardManager>();
        if (this.boardRef == null)
        {
            Debug.LogError("Script: PlayerController, variable 'boardRef' is null!");
        }

        temp = GameObject.FindGameObjectWithTag("GameController");
        this.gameRef = temp.GetComponent<Game>();
        if (this.gameRef == null)
        {
            Debug.LogError("Script: PlayerController, variable 'gameRef' is null!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if ((PersistentData.instance.GetState() == PersistentData.GameState.Versus) && (PersistentData.instance.GetTurn() == "Enemy"))
        {
            this.AiDoTurn();
        }
    }

    private void AiDoTurn()
    {
        GamePiece[] deepCopy = new GamePiece[42];
        bool aiTurn = true;
        List<KeyValuePair<GamePiece, Vector2Int>> moveList = new List<KeyValuePair<GamePiece, Vector2Int>>();

        // Deep Copy all pieces to create state
        this.DeepCopyParentNode(deepCopy);
        // Get all ai possible moves
        this.GetAiPossibleMoves(deepCopy, moveList);


        int randomMove = 0;
        if (moveList.Count > 0)
        {
            randomMove = Random.Range(0, moveList.Count - 1);
            //Debug.Log("moveList size: " + moveList.Count);
            //Debug.Log(randomMove);
            //Debug.Log(moveList[randomMove].Key.GetName() + ", " + moveList[randomMove].Value);


            this.EvaluateBoard(deepCopy, moveList);
            this.MovePiece(moveList[randomMove].Key, moveList[randomMove].Value);



            //to debug available moves
            /*for (int i = 0; i < moveList.Count; i++)
            {
                Debug.Log("name " + moveList[i].Key.GetName() + ", move to " + moveList[i].Value + ", life status: " + moveList[i].Key.IsPieceAlive());
                
            }
            Debug.Log("////////////////////////////////////////////////////////////////////////");*/
        }
        else
        {
            Debug.Log("No moves available");
        }

        PersistentData.instance.SetTurn("Player");

        for(int i = 0; i < 42; i++)
        {
            Destroy(deepCopy[i].gameObject);
        }
    }

    //Monte Carlo Search
    private void EvaluateBoard(GamePiece[] deepCopy, List<KeyValuePair<GamePiece, Vector2Int>> moveList)
    {
        int highestScore = 0;
        int offense_coeff = 1;
        int open_coeff = 2;
        int defense_coeff = 3;
        int flagSafety_coeff = 4;
        GamePiece[] tempCopy = new GamePiece[42];


        //evaluate board on all possible moves
        for (int i = moveList.Count - 1; i < moveList.Count; i++)
        {
            Debug.Log("EVALUATE!");
            int score = 5;
            //deepcopy
            for (int j = 0; j < 42; j++)
            {
                tempCopy[j] = (GamePiece)Instantiate(deepCopy[j]);
                //getCoords
                tempCopy[j].SetCoords(deepCopy[j].GetCoords().x, deepCopy[j].GetCoords().y);
                //setName
                tempCopy[j].SetName(deepCopy[j].GetName());
                //setowner
                tempCopy[j].SetOwner(deepCopy[j].GetOwner());
                //set alive boolean to true
                tempCopy[j].SetLifeStatus(deepCopy[j].IsPieceAlive());
                //set points
                tempCopy[j].SetPoint(deepCopy[j].GetPoint());
            }


            //edit move to create boardstate
            for (int j = 0; j < 42; j++)
            {
                if (moveList[i].Key.GetCoords().x == tempCopy[j].GetCoords().x && moveList[i].Key.GetCoords().y == tempCopy[j].GetCoords().y)
                {
                    this.MoveEvaluatingPiece(tempCopy[j], moveList[i].Value, tempCopy, tempCopy);
                    //Debug.Log("MoveList: " + moveList[i].Value);
                    //Debug.Log("tempDeepCopy: " + tempCopy[j].GetCoords());
                    break;
                }
            }

            //calculate offense
            score += this.CalculateOffense(tempCopy);
            
            //calculate Openness

            //calculate Defense

            //clean garbage
            for (int j = 0; j < 42; j++)
            {
                Destroy(tempCopy[j].gameObject);
            }
        }

    }

    private int CalculateOffense(GamePiece[] deepCopy)
    {
        int score = 0;

        //check how many playerpiece are alive
        for(int i = 0; i < 42; i++)
        {
            //count dead enemies add points
            if(!deepCopy[i].IsPieceAlive() && deepCopy[i].GetOwner() == "Player")
            {
                score++;
            }

            //if(deepCopy)

        }

        return score;
    }


    private void MoveEvaluatingPiece(GamePiece pieceToMove, Vector2Int moveCoord, GamePiece[] opposingPieces , GamePiece[] aiPieces)
    {
        GamePiece playerPiece = null;
        bool isFound = false;

        //move piece to moveCoord  
        int x = moveCoord.x;
        int y = moveCoord.y;
        pieceToMove.SetCoords(x, y);
                
        //check if move is a battle move
        for (int i = 21; i < 42; i++)
        {
            playerPiece = opposingPieces[i];
            if (playerPiece.GetCoords().x == moveCoord.x && playerPiece.GetCoords().y == moveCoord.y)//find player piece is same coord as "move"
            {
                isFound = true;
                break;
            }
        }

        //if battle move
        if (isFound)
        {
            int playerAliveCount = 0;
            int aiAliveCount = 0;
            for(int i = 0; i < 42; i++)
            {
                if (i < 21 && aiPieces[i].IsPieceAlive())
                    aiAliveCount++;
                else
                    playerAliveCount++;
            }

            //if player has less piece or when equal assume we lose
            if (playerAliveCount < aiAliveCount || playerAliveCount == aiAliveCount)
                pieceToMove.GetKilled();
            else
                playerPiece.GetKilled();

        }
    }

    private void MovePiece(GamePiece pieceToMove, Vector2Int moveCoord)
    {
        GamePiece aiPiece = null;
        GamePiece playerPiece = null;
        bool isFound = false;


       
        for (int i = 0; i < 21; i++)
        {
            aiPiece = this.gameRef.enemy[i].GetComponent<GamePiece>();
            if (aiPiece.GetCoords().x == pieceToMove.GetCoords().x && aiPiece.GetCoords().y == pieceToMove.GetCoords().y)// find piece to move
            {
                int x = moveCoord.x;
                int y = moveCoord.y;
                float xPos = this.boardRef.GetTile(x, y).transform.position.x;
                float yPos = this.boardRef.GetTile(x, y).transform.position.y;
                //Debug.Log("x: " + x + ", y: " + y);
                //Debug.Log(aiPiece.GetName() + ", " + xPos + " " + yPos);
                float zPos = -1;
                this.gameRef.enemy[i].GetComponent<GamePiece>().SetCoords(x, y);
                this.gameRef.enemy[i].transform.position = new Vector3(xPos, yPos, zPos);
                break;
            }
        }

        //check if move is a battle move
        for (int i = 0; i < 21; i++)
        {
            playerPiece = this.gameRef.player[i].GetComponent<GamePiece>();
            if (playerPiece.GetCoords().x == moveCoord.x && playerPiece.GetCoords().y == moveCoord.y)//find player piece is same coord as "move"
            {
                isFound = true;
                break;
            }
        }
        
        
        //if battle move
        if(isFound)
        {
            //Debug.Log(playerPiece.GetName() + ", coord: " + playerPiece.GetCoords());
            //Debug.Log(aiPiece.GetName() + ", coord: " + aiPiece.GetCoords());
            //arbiter
            if (aiPiece.GetPoint() == playerPiece.GetPoint())
            {
                if (aiPiece.GetName() == "enemy_Flag" && playerPiece.GetName() == "player_Flag")
                    aiPiece.GetKilled();
                else
                {
                    playerPiece.GetKilled();
                    aiPiece.GetKilled();
                }
            }
            else if (aiPiece.GetName() == "enemy_Spy")
            {
                if (playerPiece.GetName() == "player_Private")
                    aiPiece.GetKilled();
                else
                    playerPiece.GetKilled();
            }
            else if (aiPiece.GetName() == "enemy_Private" && playerPiece.GetName() == "player_Spy")
            {
                playerPiece.GetKilled();
            }
            else if (aiPiece.GetPoint() > playerPiece.GetPoint())
            {
                playerPiece.GetKilled();
            }
            else if (aiPiece.GetPoint() < playerPiece.GetPoint())
            {
                aiPiece.GetKilled();
            }  
        }
    }

    private void GetAiPossibleMoves(GamePiece[] deepCopy, List<KeyValuePair<GamePiece, Vector2Int>> moveList)
    {
        Stack<KeyValuePair<GamePiece, Vector2Int>> tempMoveStack = new Stack<KeyValuePair<GamePiece, Vector2Int>>();

        for (int i = 0; i < 21; i++)
        {
            GamePiece gp = deepCopy[i].GetComponent<GamePiece>();
            int x = gp.GetCoords().x;
            int y = gp.GetCoords().y;
            string name = deepCopy[i].GetName();
            //check if up is valid
            if (gp.IsPieceAlive() && (y + 1 < 8) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y + 1)))
            {
                tempMoveStack.Push(new KeyValuePair<GamePiece, Vector2Int>(deepCopy[i], new Vector2Int(x, y + 1)));
                //Debug.Log(name + " UP: " + !this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y + 1)));
            }
            //check if left is valid
            if (gp.IsPieceAlive() && (x - 1 > -1) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x - 1, y)))
            {
                tempMoveStack.Push(new KeyValuePair<GamePiece, Vector2Int>(deepCopy[i], new Vector2Int(x - 1, y)));
                //Debug.Log(name + " LEFT: " + !this.CheckForAllyPiece(deepCopy, new Vector2Int(x - 1, y)));
            }
            //check if right is valid
            if (gp.IsPieceAlive() && (x + 1 < 9) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x + 1, y)))
            {
                tempMoveStack.Push(new KeyValuePair<GamePiece, Vector2Int>(deepCopy[i], new Vector2Int(x + 1, y)));
                //Debug.Log(name + " RIGHT: " + !this.CheckForAllyPiece(deepCopy, new Vector2Int(x + 1, y)));
            }
            //check if down is valid
            if (gp.IsPieceAlive() && (y - 1 > -1) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y - 1)))
            {
                tempMoveStack.Push(new KeyValuePair<GamePiece, Vector2Int>(deepCopy[i], new Vector2Int(x, y - 1)));
                //Debug.Log(name + " DOWN: " + !this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y - 1)));
            }
        }


        while (tempMoveStack.Count > 0)
        {
            moveList.Add(tempMoveStack.Pop());
        }
        
    }

    //check if "ally" pieces is in said coordinates in reference of a gamepiece
    private bool CheckForAllyPiece(GamePiece[] deepCopy, Vector2Int coord)
    {
        bool isFound = false;
        for(int i = 0; i < 21; i++)
        {
            if(deepCopy[i].GetCoords() == coord)
            {
                isFound = true;  
            }
        }

        return isFound;
    }


    private void DeepCopyParentNode(GamePiece[] deepCopy)//optimization purposes
    {
        for(int i = 0; i < 42; i++)
        {
            //FIND A WAY TO DEEP COPY!!!!!

            if(i < 21)
            {
                deepCopy[i] = (GamePiece)Instantiate(this.gameRef.enemy[i].GetComponent<GamePiece>());
                
                //getCoords
                deepCopy[i].SetCoords(this.gameRef.enemy[i].GetComponent<GamePiece>().GetCoords().x, this.gameRef.enemy[i].GetComponent<GamePiece>().GetCoords().y);
                //setName
                deepCopy[i].SetName(this.gameRef.enemy[i].GetComponent<GamePiece>().GetName());
                //setowner
                deepCopy[i].SetOwner(this.gameRef.enemy[i].GetComponent<GamePiece>().GetOwner());
                //set alive boolean to true
                deepCopy[i].SetLifeStatus(this.gameRef.enemy[i].GetComponent<GamePiece>().IsPieceAlive());
                //set points
                deepCopy[i].SetPoint(this.gameRef.enemy[i].GetComponent<GamePiece>().GetPoint());
            } 
            else
            {
                deepCopy[i] = (GamePiece)Instantiate(this.gameRef.player[i - 21].GetComponent<GamePiece>());

                //getCoords
                deepCopy[i].SetCoords(this.gameRef.player[i - 21].GetComponent<GamePiece>().GetCoords().x, this.gameRef.player[i - 21].GetComponent<GamePiece>().GetCoords().y);
                //setName
                deepCopy[i].SetName(this.gameRef.player[i - 21].GetComponent<GamePiece>().GetName());
                //setowner
                deepCopy[i].SetOwner(this.gameRef.player[i - 21].GetComponent<GamePiece>().GetOwner());
                //set alive boolean to true
                deepCopy[i].SetLifeStatus(this.gameRef.player[i - 21].GetComponent<GamePiece>().IsPieceAlive());
                //set points
                deepCopy[i].SetPoint(this.gameRef.player[i - 21].GetComponent<GamePiece>().GetPoint());

            }

            deepCopy[i].EmptySprites();//optimization purposes
            Destroy(deepCopy[i].GetComponent<DragDrop>());//optimization purposes
            Destroy(deepCopy[i].GetComponent<SpriteRenderer>());//optimization purposes
            Destroy(deepCopy[i].GetComponent<BoxCollider2D>());//optimization purposes
        }    
    }



}
