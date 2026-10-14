using UnityEngine;
[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialouge")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public float typingSpeed = 0.05f;


}
