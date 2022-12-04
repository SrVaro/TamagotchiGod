using System.Collections;
using TMPro;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    [SerializeField]
    private TextAsset textJSON;

    private DialogueList _dialogueList = null;
    public DialogueList dialogueList
    {
        get
        {
            if (_dialogueList == null)
                _dialogueList = JsonUtility.FromJson<DialogueList>(textJSON.text);
            return _dialogueList;
        }
        set { _dialogueList = value; }
    }
}

[System.Serializable]
public class DialogueList
{
    public Dialogue[] tutorialDialogue;
    public Dialogue[] populationDialogue;
    public Dialogue[] eventDialogue;
    
}

/* [System.Serializable]
public class noActionTexts
{
    public Lines[] line;
    public string name;
}

[System.Serializable]
public class cutsceneDialogue
{
    public Lines line;
    public string name;
    public string[] interactions;
}

[System.Serializable]
public class Lines
{
    public string line;
    public bool evt;
} */

[System.Serializable]
public class Dialogue
{
    public string name;
    public string[] linePool;

    public bool evt;
    public string evtLine;
    public string[] interactions;
    public string[] blessing;
    public string[] punishment;
}

[System.Serializable]
public enum Actions
{
    CharacterLeft,
    WaitForBasicInput,
    CharacterRight,
    PointAtBlessing,
    WaitForBlessingInput,
    PointAtPopulation,
    FeIncrement
}
