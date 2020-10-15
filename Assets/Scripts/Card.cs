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
        return "card" + value + "Of" + type;
    }
}