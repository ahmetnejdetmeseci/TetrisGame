using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPiece : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackinPiece;

    public Tilemap tilemap {  get; private set; }

    public Vector3Int[] cells { get; private set; }

    public Vector3Int position {  get; private set; }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap> ();
        this.cells = new Vector3Int[4];
    }

    //It is important in games to keep track of the main piece movement.
    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackinPiece.cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = this.trackinPiece.position;

        int currentRow = position.y;

        //not to exceed the bottom -1 
        int bottom = -this.board.boardSize.y / 2 - 1;

        //we need to clear the trackingPiece not to occupy the actual Piece's position
        //otherwise if statement will always return false!

        this.board.Clear(this.trackinPiece);

        for( int row = currentRow; row >= bottom; row-- ) 
        {
            position.y = row;

            if(this.board.IsValidPosition(this.trackinPiece, position))
            {
                this.position = position;
            }
            else
            {
                break;
            }
        }

        this.board.Set(this.trackinPiece);
    }

    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, this.tile);
        }
    }

}
