using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField]
    private ChipInstaller _chipInstaller;

    [SerializeField]
    private GraphBuilder _graphBuilder;

    public static GameConfig Config { get; private set; }

    private void Awake()
    {
        Config = ConfigLoader.Load();
        _chipInstaller.Install();
        _graphBuilder.Install();
    }
}

