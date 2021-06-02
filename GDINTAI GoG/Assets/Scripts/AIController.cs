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
        List<KeyValuePair<GamePiece, Vector2Int>> moveList = new List<KeyValuePair<GamePiece, Vector2Int>>();

        // Deep Copy all pieces to create state
        this.DeepCopyParentNode(deepCopy);
        // Get all ai possible moves
        this.GetAiPossibleMoves(deepCopy, moveList);

        
        if (moveList.Count > 0)
        {
            int aiMoveIndex = 0;

            aiMoveIndex = this.EvaluateBoard(deepCopy, moveList);
            Debug.Log(moveList[aiMoveIndex].Key.GetName() + ", " + moveList[aiMoveIndex].Value);
            this.MovePiece(moveList[aiMoveIndex].Key, moveList[aiMoveIndex].Value);

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
    private int EvaluateBoard(GamePiece[] deepCopy, List<KeyValuePair<GamePiece, Vector2Int>> moveList)
    {
        int[] scoreTally = new int[moveList.Count];
        int offense_coeff = 6;
        int open_coeff = 5;
        int defense_coeff = 3;

        GamePiece[] tempCopy = new GamePiece[42];

        //evaluate board on all possible moves
        for (int i = 0; i < moveList.Count; i++)
        {
            Debug.Log("EVALUATE!");
            int score = 0;
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
                    this.MoveEvaluatingPiece(tempCopy[j], moveList[i].Value, tempCopy);
                    //Debug.Log("MoveList: " + moveList[i].Value);
                    //Debug.Log("tempDeepCopy: " + tempCopy[j].GetCoords());
                    
                }
            }

            //calculate offense
            score += this.CalculateOffense(tempCopy, offense_coeff);
            //calculate Openness
            score += this.CalculateOpen(tempCopy, open_coeff);
            //calculate Defense
            score += this.CalculateDefense(tempCopy, defense_coeff);
            //calculate Flag Safety
            score += this.CalculateFlagSafety(tempCopy);
            Debug.Log("index: " + i + ", " + moveList[i].Key.GetName() + "move to coord: " + moveList[i].Value + ", score: " + score);
            //tally

            scoreTally[i] = score;

            //clean garbage
            for (int j = 0; j < 42; j++)
            {
                Destroy(tempCopy[j].gameObject);
            }
        }

        

        int tempScore = -99999;
        int tempIndex = -99;
        for(int i = 0; i < scoreTally.Length; i++)
        {
            if (scoreTally[i] > tempScore)
            {
                tempIndex = i;
                tempScore = scoreTally[i];
            }
            //Debug.Log(i + ", " + tempScore);

        }
        Debug.Log(tempIndex + ", " + tempScore);
        Debug.Log("///////////////////////////");

        return tempIndex;
    }

    private int CalculateOffense(GamePiece[] deepCopy, int offense_Coeff)
    {
        int score = 0;

        for(int i = 0; i < 42; i++)
        {
            //count dead enemies add points
            if(!deepCopy[i].IsPieceAlive() && deepCopy[i].GetOwner() == "Player")
            {
                score += (1 * offense_Coeff);
            }
            

            //score based on how close ai pieces to player zone
            if (deepCopy[i].IsPieceAlive() && deepCopy[i].GetOwner() == "Enemy")
            {
                score += ((Mathf.Abs(deepCopy[i].GetCoords().y - 7)) * offense_Coeff);
               
            }

        }
        //capture points
        for (int i = 0; i < 21; i++)
        {
            int x = deepCopy[i].GetCoords().x;
            int y = deepCopy[i].GetCoords().y;
            for (int j = 0; j < 3; j++)
            {
                //check if up is valid
                if (deepCopy[i].IsPieceAlive() && (y + (1 + j) < 8) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y + (1 + j)), "Player"))
                {
                    score += (9 - j) * offense_Coeff;
                }
                //check if left is valid
                if (deepCopy[i].IsPieceAlive() && (x - (1 + j) > -1) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x - (1 + j), y), "Player"))
                {
                    score += (9 - j) * offense_Coeff;
                }
                //check if right is valid
                if (deepCopy[i].IsPieceAlive() && (x + (1 + j) < 9) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x + (1 + j), y), "Player"))
                {
                    score += (9 - j) * offense_Coeff;
                }
                //check if down is valid
                if (deepCopy[i].IsPieceAlive() && (y - (1 + j) > -1) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y - (1 + j)), "Player"))
                {
                    score += (9 - j) * offense_Coeff;
                }
            } 
        }

        return score;
    }
    //add score based on possible moves
    private int CalculateOpen(GamePiece[] deepCopy, int open_Coeff)
    {
        int score = 0;

        for (int i = 0; i < 21; i++)
        {
            int x = deepCopy[i].GetCoords().x;
            int y = deepCopy[i].GetCoords().y;
            //check if up is valid
            if (deepCopy[i].IsPieceAlive() && (y + 1 < 8) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y + 1), "Enemy"))
            {
                score += 1 * open_Coeff;
            }
            //check if left is valid
            else if (deepCopy[i].IsPieceAlive() && (x - 1 > -1) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x - 1, y), "Enemy"))
            {
                score += 1 * open_Coeff;
            }
            //check if right is valid
            else if (deepCopy[i].IsPieceAlive() && (x + 1 < 9) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x + 1, y), "Enemy"))
            {
                score += 1 * open_Coeff;
            }
            //check if down is valid
            if (deepCopy[i].IsPieceAlive() && (y - 1 > -1) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y - 1), "Enemy"))
            {
                score += 1 * open_Coeff;
            }
            
            
        }

        return score;
    }

    private int CalculateDefense(GamePiece[] deepCopy, int defense_Coeff)
    {
        int score = 0;

        for (int i = 21; i < 42; i++)
        {
            int x = deepCopy[i].GetCoords().x;
            int y = deepCopy[i].GetCoords().y;
            //check if up is valid
            if (deepCopy[i].IsPieceAlive() && (y + 1 < 8) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y + 1), "Enemy"))
            {
                score -= 1 * defense_Coeff;
            }
            //check if left is valid
            if (deepCopy[i].IsPieceAlive() && (x - 1 > -1) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x - 1, y), "Enemy"))
            {
                score -= 1 * defense_Coeff;
            }
            //check if right is valid
            if (deepCopy[i].IsPieceAlive() && (x + 1 < 9) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x + 1, y), "Enemy"))
            {
                score -= 1 * defense_Coeff;
            }
            //check if down is valid
            if (deepCopy[i].IsPieceAlive() && (y - 1 > -1) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y - 1), "Enemy"))
            {
                score -= 1 * defense_Coeff;

            }
        }
        return score;
    }

    private int CalculateFlagSafety(GamePiece[] deepCopy)
    {
        int score = 0;

        
        int x = deepCopy[20].GetCoords().x;
        int y = deepCopy[20].GetCoords().y;

        for(int i = 0; i < 3; i++)
        {
            //check if up is valid
            if (deepCopy[20].IsPieceAlive() && (y + (i + 1) < 8) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y + (i + 1)), "Player"))
            {
                score -= 900 - (i * 250);
            }
            //check if left is valid
            if (deepCopy[20].IsPieceAlive() && (x - (i + 1) > -1) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x - (i + 1), y), "Player"))
            {
                score -= 900 - (i * 250);
            }
            //check if right is valid
            if (deepCopy[20].IsPieceAlive() && (x + (i + 1) < 9) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x + (i + 1), y), "Player"))
            {
                score -= 900 - (i * 250);
            }
            //check if down is valid
            if (deepCopy[20].IsPieceAlive() && (y - (i + 1) > -1) && this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y - (i + 1)), "Player"))
            {
                score -= 900 - (i * 250);
            }
        }


        return score;
    }

    private void MoveEvaluatingPiece(GamePiece pieceToMove, Vector2Int moveCoord, GamePiece[] opposingPieces)
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
                Debug.Log("enemy found!");
                isFound = true;
                break;
            }
        }

        //if battle move
        if (isFound)
        {
            if (pieceToMove.GetPoint() > 10)
                playerPiece.GetKilled();
            else
                pieceToMove.GetKilled();


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
            if (gp.IsPieceAlive() && (y + 1 < 8) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y + 1), "Enemy"))
            {
                tempMoveStack.Push(new KeyValuePair<GamePiece, Vector2Int>(deepCopy[i], new Vector2Int(x, y + 1)));
                //Debug.Log(name + " UP: " + !this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y + 1)));
            }
            //check if left is valid
            if (gp.IsPieceAlive() && (x - 1 > -1) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x - 1, y), "Enemy"))
            {
                tempMoveStack.Push(new KeyValuePair<GamePiece, Vector2Int>(deepCopy[i], new Vector2Int(x - 1, y)));
                //Debug.Log(name + " LEFT: " + !this.CheckForAllyPiece(deepCopy, new Vector2Int(x - 1, y)));
            }
            //check if right is valid
            if (gp.IsPieceAlive() && (x + 1 < 9) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x + 1, y), "Enemy"))
            {
                tempMoveStack.Push(new KeyValuePair<GamePiece, Vector2Int>(deepCopy[i], new Vector2Int(x + 1, y) ));
                //Debug.Log(name + " RIGHT: " + !this.CheckForAllyPiece(deepCopy, new Vector2Int(x + 1, y)));
            }
            //check if down is valid
            if (gp.IsPieceAlive() && (y - 1 > -1) && !this.CheckForAllyPiece(deepCopy, new Vector2Int(x, y - 1), "Enemy"))
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
    private bool CheckForAllyPiece(GamePiece[] deepCopy, Vector2Int coord, string whatToSearch)
    {
        bool isFound = false;
        int startingNum = 0;
        int maxNum = 0;

        if(whatToSearch == "Player")
        {
            startingNum = 21;
            maxNum = 42;
        }
        else if(whatToSearch == "Enemy")
        {
            startingNum = 0;
            maxNum = 21;
        }
        else if(whatToSearch == "Everyone")
        {
            startingNum = 0;
            maxNum = 42;
        }


        for(int i = startingNum; i < maxNum; i++)
        {
            if(deepCopy[i].GetCoords() == coord)
            {
                isFound = true;
                break;
            }
        }

        return isFound;
    }

    private void DeepCopyParentNode(GamePiece[] deepCopy)//optimization purposes
    {
        for(int i = 0; i < 42; i++)
        {
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
