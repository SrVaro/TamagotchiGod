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
    private GameController gameController;

    [SerializeField]
    private GameObject _happinessNeed;
    public bool HappinessNeed
    {
        get { return _happinessNeed.activeSelf; }
        set { _happinessNeed.SetActive(value); }
    }

    [SerializeField]
    private GameObject _sleepNeed;
    public bool SleepNeed
    {
        get { return _sleepNeed.activeSelf; }
        set { _sleepNeed.SetActive(value); }
    }

    [SerializeField]
    private GameObject _hygieneLowNeed;
    public bool HygieneLowNeed
    {
        get { return _hygieneLowNeed.activeSelf; }
        set { _hygieneLowNeed.SetActive(value); }
    }

    [SerializeField]
    private GameObject _hygieneMidNeed;
    public bool HygieneMidNeed
    {
        get { return _hygieneMidNeed.activeSelf; }
        set { _hygieneMidNeed.SetActive(value); }
    }

    [SerializeField]
    private GameObject _hygieneHighNeed;
    public bool HygieneHighNeed
    {
        get { return _hygieneHighNeed.activeSelf; }
        set { _hygieneHighNeed.SetActive(value); }
    }

    [SerializeField]
    private GameObject _actionFocus;
    public bool ActionFocus
    {
        get { return _actionFocus.activeSelf; }
        set { _actionFocus.SetActive(value); }
    }

    [SerializeField]
    private GameObject _cutsceneBackground;
    public bool CutsceneBackground
    {
        get { return _cutsceneBackground.activeSelf; }
        set { _cutsceneBackground.SetActive(value); }
    }

    [SerializeField]
    private GameObject _pointerAtBlessing;
    public bool PointerAtBlessing
    {
        get { return _pointerAtBlessing.activeSelf; }
        set { _pointerAtBlessing.SetActive(value); }
    }

    [SerializeField]
    private GameObject _pointerAtPopulation;
    public bool PointerAtPopulation
    {
        get { return _pointerAtPopulation.activeSelf; }
        set { _pointerAtPopulation.SetActive(value); }
    }

    [SerializeField]
    private GameObject _characterLeft;
    public bool CharacterLeft
    {
        get { return _characterLeft.activeSelf; }
        set { _characterLeft.SetActive(value); }
    }

    [SerializeField]
    private GameObject _characterRight;
    public bool CharacterRight
    {
        get { return _characterRight.activeSelf; }
        set { _characterRight.SetActive(value); }
    }

    [SerializeField]
    private GameObject _planetEvent;
    public bool PlanetEvent
    {
        get { return _planetEvent.activeSelf; }
        set
        {
            _planetEvent.SetActive(value);
            if (PlanetEvent)
            {
                LeanTween.cancel(_planetEvent);
                _planetEvent.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                LeanTween.scale(_planetEvent, new Vector3(0.3f, 0.3f), 0.3f).setLoopPingPong();
            }
            else
            {
                LeanTween.cancel(_planetEvent);
            }
        }
    }

    [SerializeField]
    private TextMeshProUGUI _yearText;
    public string yearText
    {
        get { return _yearText.text; }
        set { _yearText.text = value; }
    }

    [SerializeField]
    private TextMeshProUGUI _populationText;
    public string populationText
    {
        get { return _populationText.text; }
        set { _populationText.text = "Population: " + FormatPopulation(value); }
    }

    [SerializeField]
    private TextMeshProUGUI _coinsText;
    public string coinsText
    {
        get { return _coinsText.text; }
        set { _coinsText.text = value + " <sprite=0>"; }
    }

    [SerializeField]
    private TextMeshProUGUI _faithText;
    public string faithText
    {
        get { return _faithText.text; }
        set { _faithText.text = value + " <sprite=2>"; }
    }

    [SerializeField]
    private TextMeshProUGUI _cultureText;
    public string cultureText
    {
        get { return _cultureText.text; }
        set { _cultureText.text = value + " <sprite=4>"; }
    }

    [SerializeField]
    private TextMeshProUGUI _scienceText;
    public string scienceText
    {
        get { return _scienceText.text; }
        set { _scienceText.text = value + " <sprite=3>"; }
    }

    [SerializeField]
    private TextMeshProUGUI _soulText;
    public string soulText
    {
        get { return _soulText.text; }
        set { _soulText.text = value + " <sprite=1>"; }
    }

    [SerializeField]
    private TextMeshProUGUI textLabel;

    [SerializeField]
    private TextMeshProUGUI _loadText;

    [SerializeField]
    private GameObject _loadPopup;
    public string LoadPopup
    {
        get { return _loadText.text; }
        set
        {
            _loadText.text = "Game Loaded \n Years Passed: " + value;
            ////Debug.Log("Game Loaded \n Years Passed: " + value);
            StartCoroutine(showPopUp());
        }
    }

    [SerializeField]
    private GameObject _shopPanel;
    public bool ShopPanel
    {
        get { return _shopPanel.activeSelf; }
        set
        {
            _shopPanel.SetActive(value);
            if (ShopPanel)
                gameController.PauseGame = true;
            else
                gameController.PauseGame = false;
        }
    }

    private TypewriterEffect typewriterEffect;

    private Coroutine typewriterCoroutine = null;

    private string FormatPopulation(string population)
    {
        int populationNumber = int.Parse(population.Trim());
        double digitos = Math.Floor(Math.Log10(populationNumber) + 1);
        string finalValue = "";

        if (digitos >= 7)
        {
            finalValue = population.Substring(0, population.Length - 6) + "M";
        }
        else if (digitos >= 8)
        {
            finalValue = population.Substring(0, population.Length - 7) + "M";
        }
        else
        {
            finalValue = population;
        }

        return finalValue;
    }

    public void incrementHappiness()
    {
        if (gameController.PlanetEnergy >= 0.25f && gameController.PlanetEnergy > 0)
        {
            gameController.PlanetEnergy -= 0.25f;
            gameController.PlanetHappiness += 0.15f;
        }
    }

    public void incrementSleep()
    {
        if (gameController.PlanetEnergy >= 0.25f && gameController.PlanetSleep > 0)
        {
            gameController.PlanetSleep += 0.15f;
            gameController.PlanetEnergy -= 0.25f;
        }
    }

    public void incrementHygiene()
    {
        if (gameController.PlanetEnergy >= 0.25f && gameController.PlanetHygiene > 0)
        {
            gameController.PlanetHygiene += 1f;
            gameController.PlanetEnergy -= 0.25f;
        }
    }

    public void ShowDialogue(string dialogueArray)
    {
        if (gameController.PlanetEnergy >= 0)
        {
            textLabel.text = "";
            typewriterCoroutine = StartCoroutine(StepThroughDialogue(dialogueArray));
        }
    }

    private IEnumerator StepThroughDialogue(string dialogueArray)
    {
        //Debug.Log("Corutina empezada para el texto: " + dialogueArray);
        yield return typewriterEffect.Run(dialogueArray, textLabel);
    }

    private IEnumerator showPopUp()
    {
        _loadPopup.SetActive(true);
        yield return new WaitForSeconds(2);
        _loadPopup.SetActive(false);
    }

    public Image _energyMat;
    public float Energy
    {
        get { return _energyMat.material.GetFloat("_Delta"); }
        set { _energyMat.material.SetFloat("_Delta", value); }
    }
    public Image _sleepMat;
    public float Sleep
    {
        get { return _sleepMat.material.GetFloat("_Delta"); }
        set { _sleepMat.material.SetFloat("_Delta", value); }
    }
    public Image _hygieneMat;
    public float Hygiene
    {
        get { return _hygieneMat.material.GetFloat("_Delta"); }
        set { _hygieneMat.material.SetFloat("_Delta", value); }
    }

    public Image _happinessMat;
    public float Happiness
    {
        get { return _happinessMat.material.GetFloat("_Delta"); }
        set { _happinessMat.material.SetFloat("_Delta", value); }
    }

    void Awake()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
    }

    public void LoadMinigame()
    {
        SceneManager.LoadScene("Minigame", LoadSceneMode.Single);
    }
}
