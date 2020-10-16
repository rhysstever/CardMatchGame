using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enums
public enum CardType
{
    Air,
    Water,
    Earth,
    Fire
}

public enum CardValue
{
    Zero,
    One,
    Two,
    Three
}

public class Card
{
	// Fields
	private CardValue value;
    private CardType type;
    private int row;
    private int column;
	private bool isActive;

	// Properties
	public CardValue Value { get { return value; } }
	public CardType Type { get { return type; } }
	public int Row
	{
		get { return row; }
		set { row = value; }
	}
	public int Column
	{
		get { return column; }
		set { column = value; }
	}
	public bool IsActive
	{
		get { return isActive; }
		set { isActive = value; }
	}

	// Constructors
	/// <summary>
	/// Creates a Card object by the given indexes for each field
	/// </summary>
	/// <param name="valueIndex">The index for the value enum</param>
	/// <param name="typeIndex">The index for the type enum</param>
	public Card(int valueIndex, int typeIndex)
	{
		value = (CardValue)valueIndex;
		type = (CardType)typeIndex;
		row = 0;
		column = 0;
		isActive = true;
	}
	/// <summary>
	/// Creates a Card object by the given enum values
	/// </summary>
	/// <param name="value">The value of the card</param>
	/// <param name="type">The type of the card</param>
	public Card(CardValue value, CardType type)
	{
		this.value = value;
		this.type = type;
		row = 0;
		column = 0;
		isActive = true;
	}

	// Methods
    public override string ToString()
    {
        return "card" + value + "Of" + type;
    }
}