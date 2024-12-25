using TMPro;
using UnityEngine;

public class LogSystem : MonoBehaviour
{
    public GameObject LogContent;
    public GameObject LogPrefab;

    public void InsertLog(string log)
    {
        GameObject addLog = Instantiate(LogPrefab, Vector3.zero, Quaternion.identity);

        TextMeshProUGUI logtext = addLog.GetComponentInChildren<TextMeshProUGUI>();
        logtext.text = log;
        addLog.transform.SetParent(LogContent.transform, false);
    }
}
