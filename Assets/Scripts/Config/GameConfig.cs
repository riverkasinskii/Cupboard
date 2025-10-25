using System;
using UnityEngine;

[Serializable]
public class GameConfig
{
    public int chipCount = 6;
    public int nodeCount = 9;
    public Vector2 firstNode = new(-500, 350);
    public Vector2 secondNode = new(0, 350);
    public Vector2 thirdNode = new(500, 350);
    public Vector2 fourthNode = new(-500, 0);
    public Vector2 fifthNode = new(0, 0);
    public Vector2 sixthNode = new(500, 0);
    public Vector2 seventhNode = new(-500, -350);
    public Vector2 eighthNode = new(0, -350);
    public Vector2 ninthNode = new(500, -350);
    public int[] initialChipsLocation = new int[6] { 1, 2, 3, 7, 8, 9 };
    public int[] finishChipsLocation = new int[6] { 7, 8, 9, 1, 2, 3 };
    public int countPairs = 8;
    public int[] firstPair = new int[2] { 1, 4 };
    public int[] secondPair = new int[2] { 2, 5 };
    public int[] thirdPair = new int[2] { 3, 6 };
    public int[] fourthPair = new int[2] { 4, 5 };
    public int[] fifthPair = new int[2] { 5, 6 };
    public int[] sixthPair = new int[2] { 4, 7 };
    public int[] seventhPair = new int[2] { 5, 8 };
    public int[] eighthPair = new int[2] { 6, 9 };
}
