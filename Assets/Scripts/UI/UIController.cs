using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject _planetEvent;
    public bool PlanetEvent {
        get {return _planetEvent.activeSelf; }
        set {
            _planetEvent.SetActive(value);
            if (PlanetEvent) {
                LeanTween.cancel(_planetEvent);
                _planetEvent.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                LeanTween.scale(_planetEvent, new Vector3(0.3f, 0.3f), 0.3f).setLoopPingPong();
            } else {
                LeanTween.cancel(_planetEvent);
            }
        }
    }

    [SerializeField]
    private TextMeshProUGUI _yearText;
    public string yearText {
        get {return _yearText.text;}
        set {
            _yearText.text = value;
        }
    }

    [SerializeField]
    private TextMeshProUGUI _populationText;
    public string populationText {
        get {return _populationText.text;}
        set {
            _populationText.text = "Population: " + FormatPopulation(value);
        }
    }

    [SerializeField]
    private TextMeshProUGUI _coinsText;
    public string coinsText { 
        get {return _coinsText.text;}
        set {
            _coinsText.text = value + "<sprite=\"coin\" name=\"coin\">";
        }
    }

    [SerializeField]
    private TextMeshProUGUI textLabel;

    [SerializeField]
    private TextMeshProUGUI _loadText;
    [SerializeField]
    private GameObject _loadPopup;
    public string LoadPopup {
        get { return _loadText.text; }
        set { 
            _loadText.text = "Game Loaded \n Years Passed: " + value;
            Debug.Log("Game Loaded \n Years Passed: " + value);
            StartCoroutine(showPopUp());
        }
    }

    [SerializeField]
    private GameObject _shopPanel;
    public bool ShopPanel {
        get {return _shopPanel.activeSelf;}
        set {
            _shopPanel.SetActive(value);
        }
    }
    
    private TypewriterEffect typewriterEffect;

    private Coroutine typewriterCoroutine = null;

    private string FormatPopulation(string population) {
        int populationNumber = int.Parse(population.Trim());
        double digitos = Math.Floor(Math.Log10(populationNumber) + 1);
        string finalValue = "";

        if(digitos >= 7) {
            finalValue = population.Substring(0, population.Length - 6) + "M";
        } else if (digitos >= 8) {
            finalValue = population.Substring(0, population.Length - 7) + "M";
        } else {
            finalValue = population;
        }

        
        return finalValue;
    }

    public void ShowDialogue(string dialogueArray) {
        if (textLabel.text != dialogueArray) {
            textLabel.text = "";
            typewriterCoroutine = StartCoroutine(StepThroughDialogue(dialogueArray));
        }
    }

    private IEnumerator StepThroughDialogue(string dialogueArray) {
        yield return typewriterEffect.Run(dialogueArray, textLabel);

    }

    private IEnumerator showPopUp() {
        _loadPopup.SetActive(true);
        yield return new WaitForSeconds(2);
        _loadPopup.SetActive(false);
    }

    public Image _foodMat;
    public float Food {
        get {return _foodMat.material.GetFloat("_Delta");}
        set {
            _foodMat.material.SetFloat("_Delta", value);
        }
    }
    public Image _waterMat;
    public float Water {
        get {return _waterMat.material.GetFloat("_Delta");}
        set {
            _waterMat.material.SetFloat("_Delta", value);
        }
    }
    public Image _tempMat;
    public float Temp {
        get {return _tempMat.material.GetFloat("_Delta");}
        set {
            _tempMat.material.SetFloat("_Delta", value);
        }
    }


    void Awake() {   
        typewriterEffect = GetComponent<TypewriterEffect>();
    }

    public void LoadMinigame() {
        SceneManager.LoadScene("Minigame", LoadSceneMode.Single);
    }
}
