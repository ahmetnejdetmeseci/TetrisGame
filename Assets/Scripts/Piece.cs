using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board {  get; private set; }

    public TetrominoData data { get; private set; }

    public Vector3Int[] cells { get; private set; } 



    public Vector3Int position { get; private set; }

    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;

    public float lockDelay = 0.5f;


    private float stepTime;

    private float lockTime;


    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;

        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;


        if(this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for(int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }

    }


    private void Update()
    {
        this.board.Clear(this);

        this.lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }


        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if(Time.time >= this.stepTime) 
        {
            Step();
        }

        this.board.Set(this);

    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private void Step()
    {
        this.stepTime = Time.time + stepDelay;


        //when the piece hits the bottom Move will return false and lockTime will not be reset. So if statement becomes corrent and locks the piece
        Move(Vector2Int.down);

        if(this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }


    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    //In the move Part we call IsValid method from the Game Board which is Board class.!!!
    //We need to calculate the Borders and The Items already put in the cells 
    //
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if(valid)
        {
            this.position = newPosition;
            this.lockTime = 0f;
        }

        return valid;
    }


    private void Rotate(int direction)
    {

        int originalRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(this.rotationIndex, direction))
        {
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }

    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            //I and O rotations are different! That's why it is Vector3 not Vector3Int!
            Vector3 cell = this.cells[i];

            int x, y;

            //we consider rotation matrix!
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;

                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;

                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);

        }
    }



    //Not to exceed the walls of the game
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);
        
        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];
            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection) 
    {
        int wallKickIndex = rotationIndex * 2;

        if(rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    //not to exceed the indexes
    private int Wrap(int input, int min, int max)
    {
        if(input < min)
        {
            return max - (min - input) % (max - min);
        }else
        {
            return min + (input - min) % (max - min);
        }
    }
}
