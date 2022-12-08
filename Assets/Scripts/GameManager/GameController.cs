using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using NoSuchStudio.Common;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private int tickPerYear = 5;

    [SerializeField]
    private UIController uiController;

    [SerializeField]
    private GameObject cutsceneBackground;

    [SerializeField]
    private GameObject pointerAtBlessing;

    [SerializeField]
    private GameObject pointerAtPopulation;

    [SerializeField]
    private GameObject characterLeft;

    [SerializeField]
    private GameObject characterRight;

    #region STATS
    private float _energy = 1;
    public float PlanetEnergy
    {
        get { return _energy; }
        set
        {
            _energy = Mathf.Clamp(value, 0, 1);
            uiController.Energy = _energy;
        }
    }

    private float _water = 1;
    public float PlanetWater
    {
        get { return _water; }
        set
        {
            _water = Mathf.Clamp(value, 0, 1);
            uiController.Water = _water;
        }
    }

    private float _faith = 1;
    public float PlanetFaith
    {
        get { return _faith; }
        set
        {
            _faith = value;
            uiController.faithText = PlanetFaith.ToString();
        }
    }

    private float _culture = 1;
    public float PlanetCulture
    {
        get { return _faith; }
        set
        {
            _culture = value;
            uiController.cultureText = PlanetCulture.ToString();
        }
    }

    private float _souls = 1;
    public float PlanetSouls
    {
        get { return _souls; }
        set
        {
            _souls = value;
            uiController.soulText = PlanetSouls.ToString();
        }
    }

    private float _science = 1;
    public float PlanetScience
    {
        get { return _science; }
        set
        {
            _science = value;
            uiController.scienceText = PlanetScience.ToString();
        }
    }

    private int _year;
    public int PlanetYear
    {
        get { return _year; }
        set
        {
            _year = value;
            uiController.yearText = "Year " + PlanetYear.ToString();
        }
    }

    private int _population;
    public int PlanetPopulation
    {
        get { return _population; }
        set
        {
            _population = value;
            if (_population == 1)
            {
                growRate = 1;
            }
            else if (_population >= 2 && _population <= 5)
            {
                growRate = 2;
            }
            else if (_population > 5 && _population <= 20)
            {
                growRate = 5;
            }

            uiController.populationText = PlanetPopulation.ToString();
        }
    }

    private int _coins;
    public int PlanetCoins
    {
        get { return _coins; }
        set
        {
            _coins = value;
            uiController.coinsText = PlanetCoins.ToString();
        }
    }
    #endregion

    #region JSON DATA
    [SerializeField]
    private JSONReader jsonReader;

    public DialogueList dialogueList = new DialogueList();
    private Dictionary<string, bool> eventVariables = new Dictionary<string, bool>();
    #endregion

    private InputManager inputManager;

    private bool screenClicked;
    private bool blessingClicked;
    private bool punishmentClicked;
    private bool paused = false;

    private int growRate = 1;

    //private float cooldownTime = 0;

    //[SerializeField]
    //private float clickCooldown = 3;

    public void Interaction()
    {
        if (uiController.PlanetEvent)
        {
            uiController.PlanetEvent = false;
            //PlanetEnergy += 0.1f;
            //PlanetWater += 0.1f;
            //PlanetTemp += 0.1f;
            PlanetCoins += 5;

            List<Dialogue> avalibleDialogues = new List<Dialogue>();
            foreach (var eventDialogue in dialogueList.eventDialogue)
            {
                if (eventVariables[eventDialogue.name])
                {
                    avalibleDialogues.Add(eventDialogue);
                }
            }
            FindDialogue(avalibleDialogues, true);
            /*
            if (growRate == 1)
                PlanetPopulation += 1;
            else
                PlanetPopulation += UnityEngine.Random.Range(0, growRate);
            */
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        inputManager.OnStartTouch += Click;
        //inputManager.OnEndTouch += Release;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        inputManager = InputManager.Instance;
        Application.targetFrameRate = 60;
        dialogueList = jsonReader.dialogueList;
        //LoadGameData();
        TickSystemInit();
    }

    private void TickSystemInit()
    {
        // Nos subscribimos al evento OnTick, para ejecutar logica del juego en cada Tick
        TimeTickSystem.OnTick += delegate(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (!paused)
            {
                //this.LogLog("--- TICK --- " + e.tick % tickPerYear);

                if ((e.tick % tickPerYear) == 0)
                {
                    //this.LogLog("--- YEAR ---");
                    ProcessYear(1);
                }
            }
        };
    }

    private void ProcessYear(int years)
    {
        //this.LogLog("Years passed: " + years);
        for (int i = 0; i < years; i++)
        {
            PlanetYear++;
            if (PlanetYear % 5 == 0)
            {
                //PlanetEnergy -= 0.01f;
                PlanetWater -= 0.01f;
                //PlanetFe -= 0.01f;

                //if (PlanetEnergy > 0.25f && PlanetWater > 0.25f && PlanetFe > 0.25f)
                //{
                //    PlanetPopulation += UnityEngine.Random.Range(0, PlanetPopulation);
                //}

                uiController.PlanetEvent = true;
            }
        }

        List<Dialogue> avalibleDialogues = new List<Dialogue>();
        foreach (var populationDialogue in dialogueList.populationDialogue)
        {
            if (eventVariables[populationDialogue.name])
            {
                avalibleDialogues.Add(populationDialogue);
                Debug.Log("Population dialogue " + populationDialogue.name);
            }
        }

        FindDialogue(avalibleDialogues, false);

        SaveGameData();
    }

    private void FindDialogue(List<Dialogue> dialogues, bool evt)
    {
        int population = PlanetPopulation;

        if (!evt)
        {
            foreach (Dialogue dialogue in dialogues)
            {
                if (!dialogue.name.Contains('-'))
                {
                    var rndLine = dialogue.linePool[
                        (int)UnityEngine.Random.Range(0, dialogue.linePool.Length)
                    ];

                    uiController.ShowDialogue(rndLine);
                }
                else
                {
                    string[] populationRange = dialogue.name.Split("-");
                    if (
                        population >= int.Parse(populationRange[0])
                        && population <= int.Parse(populationRange[1])
                    )
                    {
                        //this.LogLog(
                        //    "Population Range: " + populationRange[0] + ", " + populationRange[1]
                        //);

                        var rndLine = dialogue.linePool[
                            (int)UnityEngine.Random.Range(0, dialogue.linePool.Length)
                        ];

                        uiController.ShowDialogue(rndLine);
                    }
                }
            }
        }
        else
        {
            Dialogue randomEvent = dialogues[(int)UnityEngine.Random.Range(0, dialogues.Count)];
            List<Dialogue> auxDialogueList = new List<Dialogue>();
            for (int i = 0; i < dialogueList.eventDialogue.Count; i++)
            {
                if (randomEvent.name == dialogueList.eventDialogue[i].name)
                {
                    eventVariables[dialogueList.eventDialogue[i].name] = false;
                    dialogueList.eventDialogue.RemoveAt(i);
                }
            }
            auxDialogueList.Add(randomEvent);
            StartCoroutine("RunEventDialogue", auxDialogueList);
        }
    }

    IEnumerator RunEventDialogue(List<Dialogue> eventDialogues)
    {
        paused = true;
        //bless
        screenClicked = false;
        blessingClicked = false;
        punishmentClicked = false;

        for (int i = 0; i < eventDialogues.Count; i++)
        {
            var eventElement = eventDialogues[i];

            //Debug.Log("Line actual: " + eventElement.evtLine);

            foreach (var interaction in eventElement.interactions)
            {
                if (interaction == "WaitForBasicInput")
                {
                    uiController.ShowDialogue(eventElement.evtLine);
                    yield return new WaitUntil(IsScreenClicked);
                    screenClicked = false;
                }
                else if (interaction == "WaitForActionInput")
                {
                    uiController.ShowDialogue(eventElement.evtLine);
                    yield return new WaitUntil(IsActionClicked);
                    if (blessingClicked)
                    {
                        blessingClicked = false;
                        uiController.ShowDialogue(eventElement.blessing[0]);
                        yield return new WaitUntil(IsScreenClicked);
                        screenClicked = false;
                        for (int j = 1; j < eventElement.blessing.Length; j++)
                        {
                            Actions(eventElement.blessing[j]);
                        }
                    }
                    else if (punishmentClicked)
                    {
                        punishmentClicked = false;
                        uiController.ShowDialogue(eventElement.punishment[0]);
                        yield return new WaitUntil(IsScreenClicked);
                        screenClicked = false;
                        for (int j = 1; j < eventElement.punishment.Length; j++)
                        {
                            Actions(eventElement.punishment[j]);
                        }
                    }
                }
                else if (interaction == "WaitForBlessingInput")
                {
                    uiController.ShowDialogue(eventElement.evtLine);
                    yield return new WaitUntil(IsBlessingClicked);
                    blessingClicked = false;
                    pointerAtBlessing.SetActive(false);
                    PlanetPopulation = 1;
                }
                else if (interaction == "WaitForPunishmentInput")
                {
                    uiController.ShowDialogue(eventElement.evtLine);
                    yield return new WaitUntil(IsPunishmentClicked);
                    punishmentClicked = false;
                }
                else if (interaction == "PointAtBlessing")
                {
                    cutsceneBackground.SetActive(false);
                    pointerAtBlessing.SetActive(true);
                }
                else if (interaction == "PointAtPopulation")
                {
                    cutsceneBackground.SetActive(false);
                    pointerAtPopulation.SetActive(true);
                }
                else if (interaction == "CharacterLeft")
                {
                    cutsceneBackground.SetActive(true);
                    characterRight.SetActive(false);
                    characterLeft.SetActive(true);
                }
                else if (interaction == "CharacterRight")
                {
                    cutsceneBackground.SetActive(true);
                    characterLeft.SetActive(false);
                    characterRight.SetActive(true);
                }
            }
        }

        paused = false;
        endTutorial();
    }

    public void Actions(string action)
    {
        Debug.Log(action);
        string[] aux = action.Split('-');
        if (aux[0] == "FeIncrement")
        {
            PlanetFaith += int.Parse(aux[1]);
        }
        else if (aux[0] == "FeDecriment")
        {
            PlanetFaith -= int.Parse(aux[1]);
        }
        else if (aux[0] == "PopulationIncrement")
        {
            PlanetPopulation += int.Parse(aux[1]);
        }else if (aux[0] == "PopulationDecriment")
        {
            PlanetPopulation -= int.Parse(aux[1]);
        }else if (aux[0] == "CultureIncrement")
        {
            PlanetCulture += int.Parse(aux[1]);
        }else if (aux[0] == "CultureDecriment")
        {
            PlanetCulture -= int.Parse(aux[1]);
        }else if (aux[0] == "ScienceIncrement")
        {
            PlanetScience += int.Parse(aux[1]);
        }else if (aux[0] == "ScienceDecriment")
        {
            PlanetScience -= int.Parse(aux[1]);
        } else if(aux[0] == "unlock") {
            Debug.Log("unlock: " + eventVariables[aux[1]]);
            eventVariables[aux[1]] = true;
        }
    }

    public void Click(Vector2 mousePos, float time)
    {
        //if (time > cooldownTime)
        //{
        screenClicked = true;
        //cooldownTime = time + clickCooldown;
        //}
    }

    private bool IsScreenClicked()
    {
        return screenClicked;
    }

    private bool IsBlessingClicked()
    {
        return blessingClicked;
    }

    private bool IsPunishmentClicked()
    {
        return punishmentClicked;
    }

    private bool IsActionClicked()
    {
        return blessingClicked || punishmentClicked;
    }

    public void Blessing()
    {
        //if ((PlanetEnergy -= 0.25f) >= 0)
        //{
        blessingClicked = true;
        //PlanetEnergy -= 0.25f;
        //}
    }

    public void Punishment()
    {
        //if ((PlanetEnergy -= 0.25f) >= 0)
        //{
        punishmentClicked = true;
        //PlanetEnergy -= 0.25f;
        //}
    }

    public void SaveGameData()
    {
        //this.LogLog("--- SAVE ---");
        ////this.LogLog("Archivo creado en " + Application.persistentDataPath + "/gameDataContainer.dat");
        // Se crea/sustituye el archivo del sistema (Windows/Android/IOs/Etc...)
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(
            Application.persistentDataPath + "/gameDataContainer.dat",
            FileMode.Create
        );
        GameDataContainer gd = new GameDataContainer();

        // Se guardan las variables en el archivo
        gd.savedTime = DateTime.Now;
        gd.year = PlanetYear;
        gd.energy = PlanetEnergy;
        gd.water = PlanetWater;
        gd.faith = PlanetFaith;
        gd.culture = PlanetCulture;
        gd.science = PlanetScience;
        gd.souls = PlanetSouls;

        gd.population = PlanetPopulation;
        gd.dialogueVariables = eventVariables;
        bf.Serialize(file, gd);
        file.Close();
    }

    public void LoadGameData()
    {
        //this.LogLog("--- LOAD ---");
        // Si no existe el archivo se crea uno nuevo y se inicializan las estadisticas (Primera vez que se inicia el juego)
        dialogueList = jsonReader.dialogueList;

        if (File.Exists(Application.persistentDataPath + "/gameDataContainer.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(
                Application.persistentDataPath + "/gameDataContainer.dat",
                FileMode.Open
            );
            GameDataContainer gd = (GameDataContainer)bf.Deserialize(file);
            file.Close();

            // Se cargan los datos del fichero en las variables del juego
            DateTime savedTime = gd.savedTime;
            PlanetYear = gd.year;
            PlanetEnergy = gd.energy;
            PlanetWater = gd.water;
            PlanetFaith = gd.faith;
            PlanetCulture = gd.culture;
            PlanetScience = gd.science = PlanetScience;
            PlanetSouls = gd.souls;
            PlanetPopulation = gd.population;
            eventVariables = gd.dialogueVariables;

            // Se convierte del tiempo que se ha recuperado del fichero a ticks del juego que han transcurrido desde esa fecha hasta ahora
            //this.LogLog(
            //     "Ticks passed: " + ((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear)
            //);
            uiController.LoadPopup = (
                (int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear
            ).ToString();

            ProcessYear(((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear));
        }
        else
        {
            //this.LogLog("--- FAILED LOAD ---");
            //this.LogLog(
            //    "No existe el archivo en "
            //        + Application.persistentDataPath
            //       + "/gameDataContainer.dat"
            //);
            InitializeGameData();
        }
    }

    /*
     *  Reset Game State to 0
     *  Used when the the application is opened the first time (No save data) and for debugging reasons
     */
    public void InitializeGameData()
    {
        PlanetYear = 0;
        PlanetPopulation = 0;
        PlanetCoins = 0;

        PlanetEnergy = 0;
        PlanetWater = 0;
        PlanetFaith = 0;
        PlanetCulture = 0;
        PlanetScience = 0;
        PlanetSouls = 0;
        uiController.PlanetEvent = false;
        eventVariables = jsonReader.eventVariables;

        //ProcessYear(0);
        SaveGameData();

        startTutorial();
    }

    void startTutorial()
    {
        paused = true;
        cutsceneBackground.SetActive(true);
        StartCoroutine("RunEventDialogue", dialogueList.tutorialDialogue);
    }

    void endTutorial()
    {
        paused = false;
        characterLeft.SetActive(false);
        characterRight.SetActive(false);
        pointerAtBlessing.SetActive(false);
        cutsceneBackground.SetActive(false);
        pointerAtPopulation.SetActive(false);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            //this.LogLog("App Status: Has focus");
            LoadGameData();
        }
        else
        {
            //this.LogLog("App Status: Lost focus");
            SaveGameData();
        }
    }

    /*
     * Save Game Status when the app pauses (Lost focus, second plane, etc...)
     */
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            //this.LogLog("App Status: Paused");
            SaveGameData();
        }
        else
        {
            ////this.LogLog("Resumed");
            //LoadGameData();
        }
    }

    void OnApplicationQuit()
    {
        //this.LogLog("App Status: Closed");
        SaveGameData();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //this.LogLog("App Status: Opened");
        LoadGameData();
    }
}
