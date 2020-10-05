using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public enum TitleType
    {
        EMPTY,
        NORMAL,
        COUNT,
    };

    [System.Serializable]
    public struct TitlePrefab
    {
        public TitleType type;
        public GameObject prefab;
    };

    public int Columns;
    public int Rows;
    public float fillTime;

    public TitlePrefab[] tilePrefabs;
    public GameObject backgroundPrefab;

    public Score score;

    private Dictionary<TitleType, GameObject> tilePrefabDict;

    private GameTitle[,] tiles;

    private bool inverse = false;

    private GameTitle pressedTile;
    private GameTitle enteredTile;


    void Start()
    {
        Columns = MenuSettings.Colums;
        Rows = MenuSettings.Rows;


        tilePrefabDict = new Dictionary<TitleType, GameObject>();

        for (int i = 0; i < tilePrefabs.Length; i++)
        {
            if (!tilePrefabDict.ContainsKey(tilePrefabs[i].type))
            {
                tilePrefabDict.Add(tilePrefabs[i].type, tilePrefabs[i].prefab);
            }
        }

        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                GameObject background =
                    (GameObject) Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        tiles = new GameTitle[Columns, Rows];
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                SpawnNewPiece(x, y, TitleType.EMPTY);
            }
        }

        StartCoroutine(Fill());

        score.ResetCount();
    }


    public IEnumerator Fill()
    {
        bool needsRefill = true;

        while (needsRefill)
        {
            yield return new WaitForSeconds(fillTime);

            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }

            needsRefill = ClearAllValidMatches();
        }
    }

    public bool FillStep()
    {
        bool movedPiece = false;
        for (int y = Rows - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < Columns; loopX++)
            {
                int x = loopX;

                if (inverse)
                {
                    x = Columns - 1 - loopX;
                }

                GameTitle title = tiles[x, y];
                if (title.IsMovable())
                {
                    GameTitle titleBelow = tiles[x, y + 1];
                    if (titleBelow.Type == TitleType.EMPTY)
                    {
                        Destroy(titleBelow.gameObject);
                        title.MovableComponent.Move(x, y + 1, fillTime);
                        tiles[x, y + 1] = title;
                        SpawnNewPiece(x, y, TitleType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                }

                                if (diagX >= 0 && diagX < Columns)
                                {
                                    GameTitle diagonalTitle = tiles[diagX, y + 1];

                                    if (diagonalTitle.Type == TitleType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameTitle titleAbove = tiles[diagX, aboveY];

                                            if (titleAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!titleAbove.IsMovable() && titleAbove.Type != TitleType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalTitle.gameObject);
                                            title.MovableComponent.Move(diagX, y + 1, fillTime);
                                            tiles[diagX, y + 1] = title;
                                            SpawnNewPiece(x, y, TitleType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < Columns; x++)
        {
            GameTitle titleBelow = tiles[x, 0];

            if (titleBelow.Type == TitleType.EMPTY)
            {
                Destroy(titleBelow.gameObject);
                GameObject newPiece = (GameObject) Instantiate(tilePrefabDict[TitleType.NORMAL],
                    GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;
                tiles[x, 0] = newPiece.GetComponent<GameTitle>();
                tiles[x, 0].Init(x, -1, this, TitleType.NORMAL); //
                tiles[x, 0].MovableComponent.Move(x, 0, fillTime);
                tiles[x, 0].VegetablesComponent
                    .SetColor((VegetablesTitle.VegetablesType) Random.Range(0, tiles[x, 0].VegetablesComponent.NumColors));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - Columns / 2.0f + x,
            transform.position.y + Rows / 2.0f - y);
    }

    public GameTitle SpawnNewPiece(int x, int y, TitleType type)
    {
        GameObject newPiece =
            (GameObject) Instantiate(tilePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        tiles[x, y] = newPiece.GetComponent<GameTitle>();
        tiles[x, y].Init(x, y, this, type);

        return tiles[x, y];
    }

    public bool IsAdjacent(GameTitle piece1, GameTitle piece2)
    {
        return (piece1.X == piece2.X && (int) Mathf.Abs(piece1.Y - piece2.Y) == 1)
               || (piece1.Y == piece2.Y && (int) Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    public void SwapPieces(GameTitle piece1, GameTitle piece2)
    {
        if (piece1.IsMovable() && piece2.IsMovable())
        {
            tiles[piece1.X, piece1.Y] = piece2;
            tiles[piece2.X, piece2.Y] = piece1;

            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

                ClearAllValidMatches();

                StartCoroutine(Fill());
            }
            else
            {
                tiles[piece1.X, piece1.Y] = piece1;
                tiles[piece2.X, piece2.Y] = piece2;
            }
        }
    }

    public void PressPiece(GameTitle title)
    {
        pressedTile = title;
    }

    public void EnterPiece(GameTitle title)
    {
        enteredTile = title;
    }

    public void ReleasePiece()
    {
        if (IsAdjacent(pressedTile, enteredTile))
        {
            SwapPieces(pressedTile, enteredTile);
        }
    }

    public List<GameTitle> GetMatch(GameTitle title, int newX, int newY)
    {
        if (title.IsColored())
        {
            VegetablesTitle.VegetablesType vegetables = title.VegetablesComponent.Vegetables;
            List<GameTitle> horizontalPieces = new List<GameTitle>();
            List<GameTitle> verticalPieces = new List<GameTitle>();
            List<GameTitle> matchingPieces = new List<GameTitle>();

            horizontalPieces.Add(title);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < Columns; xOffset++)
                {
                    int x;

                    if (dir == 0)
                    {
                        x = newX - xOffset;
                    }
                    else
                    {
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= Columns)
                    {
                        break;
                    }

                    if (tiles[x, newY].IsColored() && tiles[x, newY].VegetablesComponent.Vegetables == vegetables)
                    {
                        horizontalPieces.Add(tiles[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < Rows; yOffset++)
                        {
                            int y;

                            if (dir == 0)
                            {
                                // Up
                                y = newY - yOffset;
                            }
                            else
                            {
                                // Down
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= Rows)
                            {
                                break;
                            }

                            if (tiles[horizontalPieces[i].X, y].IsColored() &&
                                tiles[horizontalPieces[i].X, y].VegetablesComponent.Vegetables == vegetables)
                            {
                                verticalPieces.Add(tiles[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }

                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(title);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < Rows; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    {
                        // Up
                        y = newY - yOffset;
                    }
                    else
                    {
                        // Down
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= Rows)
                    {
                        break;
                    }

                    if (tiles[newX, y].IsColored() && tiles[newX, y].VegetablesComponent.Vegetables == vegetables)
                    {
                        verticalPieces.Add(tiles[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < Columns; xOffset++)
                        {
                            int x;

                            if (dir == 0)
                            {
                                // Left
                                x = newX - xOffset;
                            }
                            else
                            {
                                // Right
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= Columns)
                            {
                                break;
                            }

                            if (tiles[x, verticalPieces[i].Y].IsColored() &&
                                tiles[x, verticalPieces[i].Y].VegetablesComponent.Vegetables == vegetables)
                            {
                                horizontalPieces.Add(tiles[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }

                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }

        return null;
    }

    public bool ClearAllValidMatches()
    {
        Debug.Log("ClearAllValidMatches");
        bool needsRefill = false;

        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                if (tiles[x, y].IsClearable())
                {
                    List<GameTitle> match = GetMatch(tiles[x, y], x, y);

                    if (match != null)
                    {
                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }

    public bool ClearPiece(int x, int y)
    {
        if (tiles[x, y].IsClearable() && !tiles[x, y].ClearableComponent.IsBeingCleared)
        {
            tiles[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, TitleType.EMPTY);
            score.AddPoint(1);
            return true;
        }

        return false;
    }
}