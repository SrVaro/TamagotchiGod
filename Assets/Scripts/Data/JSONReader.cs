using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    [SerializeField]
    private TextAsset textJSON;

    [SerializeField]
    private TextAsset eventVariablesJSON;

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
    private Dictionary<string, bool> _eventVariables = null;
    public Dictionary<string, bool> eventVariables
    {
        get
        {
            if (_eventVariables == null)
                _eventVariables = JsonConvert.DeserializeObject<Dictionary<string, bool>>(
                    eventVariablesJSON.text
                );
            return _eventVariables;
        }
        set { _eventVariables = value; }
    }
}

[System.Serializable]
public class DialogueList
{
    public List<Dialogue> tutorialDialogue;
    public List<Dialogue> populationDialogue;
    public List<Dialogue> eventDialogue;
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
