using UnityEngine;

public class GameTitle : MonoBehaviour {

	private int x;
	private int y;

	public int X
	{
		get { return x; }
		set {
			if (IsMovable ()) {
				x = value;
			}
		}
	}

	public int Y
	{
		get { return y; }
		set {
			if (IsMovable ()) {
				y = value;
			}
		}
	}

	private Grid.TitleType type;

	public Grid.TitleType Type
	{
		get { return type; }
	}

	private Grid grid;

	public Grid GridRef
	{
		get { return grid; }
	}

	private MovableTitle movableComponent;

	public MovableTitle MovableComponent
	{
		get { return movableComponent; }
	}

	private VegetablesTitle vegetablesComponent;

	public VegetablesTitle VegetablesComponent
	{
		get { return vegetablesComponent; }
	}

	private ClearableTile clearableComponent;

	public ClearableTile ClearableComponent {
		get { return clearableComponent; }
	}

	void Awake()
	{
		movableComponent = GetComponent<MovableTitle> ();
		vegetablesComponent = GetComponent<VegetablesTitle> ();
		clearableComponent = GetComponent<ClearableTile> ();
	}

	public void Init(int _x, int _y, Grid _grid, Grid.TitleType _type)
	{
		x = _x;
		y = _y;
		grid = _grid;
		type = _type;
	}

	void OnMouseEnter()
	{
		grid.EnterPiece (this);
	}

	void OnMouseDown()
	{
		grid.PressPiece (this);
	}

	void OnMouseUp()
	{
		grid.ReleasePiece ();
	}

	public bool IsMovable()
	{
		return movableComponent != null;
	}

	public bool IsColored()
	{
		return vegetablesComponent != null;
	}

	public bool IsClearable()
	{
		return clearableComponent != null;
	}
}
