using UnityEngine;

public class QuestIcon : MonoBehaviour
{
    [Header("Icons")]
    public GameObject questAvailableIcon;
    public GameObject questActiveIcon;
    public GameObject questReadyIcon;

    private QuestNPC myNPC;

    private void Start()
    {
        myNPC = GetComponent<QuestNPC>();
    }
}