using UnityEngine;
using System.Collections.Generic;

public class VegetablesTitle : MonoBehaviour {

	public enum VegetablesType
	{
		BANANA,
		BEET,
		STRAWBERRY,
		EGGPLANT,
		LIMEWIRE,
		RASPBERRIES,
		ANY,
		COUNT
	};

	[System.Serializable]
	public struct ColorSprite
	{
		public VegetablesType vegetables;
		public Sprite sprite;
	};

	public ColorSprite[] colorSprites;

	private VegetablesType vegetables;

	public VegetablesType Vegetables
	{
		get { return vegetables; }
		set { SetColor (value); }
	}

	public int NumColors
	{
		get { return colorSprites.Length; }
	}

	private SpriteRenderer sprite;
	private Dictionary<VegetablesType, Sprite> colorSpriteDict;

	void Awake()
	{
		sprite = transform.Find ("piece").GetComponent<SpriteRenderer> ();

		colorSpriteDict = new Dictionary<VegetablesType, Sprite> ();

		for (int i = 0; i < colorSprites.Length; i++) {
			if (!colorSpriteDict.ContainsKey (colorSprites [i].vegetables)) {
				colorSpriteDict.Add (colorSprites [i].vegetables, colorSprites [i].sprite);
			}
		}
	}

	public void SetColor(VegetablesType vegetables)
	{
		this.vegetables = vegetables;

		if (colorSpriteDict.ContainsKey (vegetables)) {
			sprite.sprite = colorSpriteDict [vegetables];
		}
	}
}
