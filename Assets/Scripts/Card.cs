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
    One = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten
}

public class Card : MonoBehaviour
{
    // Fields set in inspector
    public CardValue value;
    public CardType type;
    public int row;
    public int column;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override string ToString()
    {
        return value + " of " + type;
    }
}