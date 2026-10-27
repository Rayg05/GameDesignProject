using UnityEngine;
[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialouge")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public Sprite dialogueBox; // background for the dialogue panel, leave empty to keep the panel's own
    public string[] dialogueLines;
    public float typingSpeed = 0.05f;


}
