using Unity.MLAgents;

public class PlayingAcademy : Academy
{
    private GenerateMap generateMap;
    private PlayerManager playerManager;
    
    public override void AcademyReset()
    {
        generateMap.ResetMap();
        playerManager.ResetPlayers();
        GameManager.Instance.StartFreshGame();
    }

    public override void AcademyStep()
    {
        // this is not necessary?
    }
}