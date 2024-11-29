using UnityEngine;

public class Idk : MonoBehaviour
{
    StageManager stageManager;
    void Start()
    {
        stageManager = StageManager.instance;
        stageManager.GetSlotLine();
    }
}
