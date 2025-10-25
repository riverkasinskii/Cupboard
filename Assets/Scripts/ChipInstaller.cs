using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ChipInstaller : MonoBehaviour
{
    private int chipCount;
    private int[] initialChipsLocation = new int[6];
    private int[] finishChipsLocation = new int[6];
    private List<Transform> chips = new();

    public void Install()
    {
        chipCount = GameInitializer.Config.chipCount;
        initialChipsLocation = GameInitializer.Config.initialChipsLocation;
        finishChipsLocation = GameInitializer.Config.finishChipsLocation;

        chips = GetComponentsInChildren<Transform>().ToList();

        for (int i = 0; i < chips.Count; i++)
        {
            if (chipCount < i)
            {
                chips[i].gameObject.SetActive(false);
            }
        }
    }
}
