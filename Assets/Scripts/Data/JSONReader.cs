using System.Collections;
using TMPro;
using UnityEngine;

public class JSONReader : MonoBehaviour
{

   [SerializeField]
    private TextAsset textJSON;

    private DialogueList _lines = null;
    public DialogueList Lines {
        get {
            if (_lines == null) _lines = JsonUtility.FromJson<DialogueList>(textJSON.text);
            return _lines;
        }
        set {
            _lines = value;
        }
    }

}


[System.Serializable]
public class DialogueList {
    public newsDialogues[] newsDialogues;
}

[System.Serializable]
public class newsDialogues {
    public string line;
    public string name;
}

