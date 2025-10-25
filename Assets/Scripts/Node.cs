using UnityEngine;

public enum NodeName
{
    FirstNode = 1,
    SecondNode = 2,
    ThirdNode = 3,
    FourthNode = 4,
    FifthNode = 5,
    SixthNode = 6,
    SeventhNode = 7,
    EighthNode = 8,
    NinthNode = 9
}

public sealed class Node : MonoBehaviour
{
    [field: SerializeField]
    public NodeName NodeName { get; private set; } = NodeName.FirstNode;

    public bool Occupied { get; private set; } = false;
    private Chip occupant;
   
    public void SetOccupied(Chip chip)
    {
        Occupied = true;
        occupant = chip;
    }

    public void Clear()
    {
        Occupied = false;
        occupant = null;
    }
}



