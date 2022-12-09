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

    #region STATS
    private float _energy = 0;
    public float PlanetEnergy
    {
        get { return _energy; }
        set
        {
            _energy = Mathf.Clamp(value, 0, 1);
            uiController.Energy = _energy;
        }
    }

    private float _water = 0;
    public float PlanetWater
    {
        get { return _water; }
        set
        {
            _water = Mathf.Clamp(value, 0, 1);
            uiController.Water = _water;
        }
    }

    private float _faith = 0;
    public float PlanetFaith
    {
        get { return _faith; }
        set
        {
            _faith = value;
            uiController.faithText = PlanetFaith.ToString();
        }
    }

    private float _culture = 0;
    public float PlanetCulture
    {
        get { return _faith; }
        set
        {
            _culture = value;
            uiController.cultureText = PlanetCulture.ToString();
        }
    }

    private float _souls = 0;
    public float PlanetSouls
    {
        get { return _souls; }
        set
        {
            _souls = value;
            uiController.soulText = PlanetSouls.ToString();
        }
    }

    private float _science = 0;
    public float PlanetScience
    {
        get { return _science; }
        set
        {
            _science = value;
            uiController.scienceText = PlanetScience.ToString();
        }
    }

    private int _year = 0;
    public int PlanetYear
    {
        get { return _year; }
        set
        {
            _year = value;
            uiController.yearText = "Year " + PlanetYear.ToString();
        }
    }

    private int _population = 0;
    public int PlanetPopulation
    {
        get { return _population; }
        set
        {
            _population = value;

            uiController.populationText = PlanetPopulation.ToString();
        }
    }

    private int _coins = 0;
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

    private bool _paused = false;
    public bool PauseGame
    {
        get { return _paused; }
        set { _paused = value; }
    }

    public void Interaction()
    {
        if (uiController.PlanetEvent)
        {
            uiController.PlanetEvent = false;
            uiController.ActionFocus = true;
            PlanetCoins += 5;

            PickRandomDialogue(GetUnlockedDialogues(dialogueList.eventDialogue), true);
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
        Application.targetFrameRate = 60;
        inputManager = InputManager.Instance;
        dialogueList = jsonReader.dialogueList;
        //LoadGameData();
        TickSystemInit();
    }

    private void TickSystemInit()
    {
        // Nos subscribimos al evento OnTick, para ejecutar logica del juego en cada Tick
        TimeTickSystem.OnTick += delegate(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (!PauseGame)
            {
                // Debug.Log("--- TICK --- " + e.tick % tickPerYear);
                if ((e.tick % tickPerYear) == 0)
                {
                    // Debug.Log("--- YEAR ---");
                    ProcessYear(1);
                }
            }
        };
    }

    private void ProcessYear(int years)
    {
        // Debug.Log("Years passed: " + years);
        for (int i = 0; i < years; i++)
        {
            PlanetYear++;
            if (PlanetYear % 5 == 0)
            {
                uiController.PlanetEvent = true;
            }
        }

        PickRandomDialogue(GetUnlockedDialogues(dialogueList.populationDialogue), false);

        SaveGameData();
    }

    private List<Dialogue> GetUnlockedDialogues(List<Dialogue> dialogues)
    {
        List<Dialogue> avalibleDialogues = new List<Dialogue>();
        foreach (var populationDialogue in dialogues)
        {
            if (populationDialogue.name.Contains('-'))
            {
                string[] aux = populationDialogue.name.Split('-');
                bool dialogueUnlocked = true;
                foreach (var elem in aux)
                {
                    if (eventVariables.ContainsKey(elem))
                    {
                        if (!eventVariables[elem])
                        {
                            dialogueUnlocked = false;
                        }
                    }
                    else
                    {
                        eventVariables.Add(populationDialogue.name, false);
                        dialogueUnlocked = false;
                    }
                }

                if (dialogueUnlocked)
                    avalibleDialogues.Add(populationDialogue);
            }
            else
            {
                if (eventVariables.ContainsKey(populationDialogue.name))
                {
                    if (eventVariables[populationDialogue.name])
                    {
                        avalibleDialogues.Add(populationDialogue);
                    }
                }
                else
                {
                    eventVariables.Add(populationDialogue.name, false);
                }
            }
        }
        return avalibleDialogues;
    }

    private void PickRandomDialogue(List<Dialogue> dialogues, bool evt)
    {
        int population = PlanetPopulation;

        if (!evt)
        {
            var rndDialogueList = dialogues[(int)UnityEngine.Random.Range(0, dialogues.Count)];
            var rndLine = rndDialogueList.linePool[
                (int)UnityEngine.Random.Range(0, rndDialogueList.linePool.Length)
            ];

            uiController.ShowDialogue(rndLine);
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
        PauseGame = true;
        screenClicked = false;
        blessingClicked = false;
        punishmentClicked = false;

        for (int i = 0; i < eventDialogues.Count; i++)
        {
            var eventElement = eventDialogues[i];

            ////Debug.Log("Line actual: " + eventElement.evtLine);

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

                    uiController.ActionFocus = false;
                }
                else if (interaction == "WaitForBlessingInput")
                {
                    uiController.ShowDialogue(eventElement.evtLine);
                    yield return new WaitUntil(IsBlessingClicked);
                    blessingClicked = false;
                    uiController.PointerAtBlessing = false;
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
                    uiController.ActionFocus = true;
                    uiController.CutsceneBackground = false;
                    uiController.PointerAtBlessing = true;
                }
                else if (interaction == "PointAtPopulation")
                {
                    uiController.CutsceneBackground = false;
                    uiController.PointerAtPopulation = true;
                }
                else if (interaction == "CharacterLeft")
                {
                    uiController.CutsceneBackground = true;
                    uiController.CharacterRight = false;
                    uiController.CharacterLeft = true;
                }
                else if (interaction == "CharacterRight")
                {
                    uiController.CutsceneBackground = true;
                    uiController.CharacterLeft = false;
                    uiController.CharacterRight = true;
                }
            }
        }

        PauseGame = false;
        endTutorial();
    }

    public void Actions(string action)
    {
        ////Debug.Log(action);
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
        }
        else if (aux[0] == "PopulationDecriment")
        {
            PlanetPopulation -= int.Parse(aux[1]);
        }
        else if (aux[0] == "CultureIncrement")
        {
            PlanetCulture += int.Parse(aux[1]);
        }
        else if (aux[0] == "CultureDecriment")
        {
            PlanetCulture -= int.Parse(aux[1]);
        }
        else if (aux[0] == "ScienceIncrement")
        {
            PlanetScience += int.Parse(aux[1]);
        }
        else if (aux[0] == "ScienceDecriment")
        {
            PlanetScience -= int.Parse(aux[1]);
        }
        else if (aux[0] == "unlock")
        {
            if (eventVariables.ContainsKey(aux[1]))
            {
                eventVariables[aux[1]] = true;
            }
            else
            {
                eventVariables.Add(aux[1], true);
            }
        }
    }

    public void Click(Vector2 mousePos, float time)
    {
        screenClicked = true;
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
        blessingClicked = true;
    }

    public void Punishment()
    {
        punishmentClicked = true;
    }

    public void SaveGameData()
    {
        //Debug.Log("--- SAVE ---");
        ////Debug.Log("Archivo creado en " + Application.persistentDataPath + "/gameDataContainer.dat");
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
        //Debug.Log("--- LOAD ---");
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
            //Debug.Log(
            //     "Ticks passed: " + ((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear)
            //);
            uiController.LoadPopup = (
                (int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear
            ).ToString();

            ProcessYear(((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear));
        }
        else
        {
            //Debug.Log("--- FAILED LOAD ---");
            //Debug.Log(
            //    "No existe el archivo en "
            //        + Application.persistentDataPath
            //       + "/gameDataContainer.dat"
            //);
            InitializeGameData();
        }
    }

    /*
     *  Reset Game State to 0
     *  Used when the the application is opened the first time (No save data) and for //Debugging reasons
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
        PauseGame = true;
        uiController.CutsceneBackground = true;
        StartCoroutine("RunEventDialogue", dialogueList.tutorialDialogue);
    }

    void endTutorial()
    {
        PauseGame = false;
        uiController.CharacterLeft = false;
        uiController.CharacterRight = false;
        uiController.PointerAtBlessing = false;
        uiController.CutsceneBackground = false;
        uiController.PointerAtPopulation = false;
        uiController.ActionFocus = false;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            //Debug.Log("App Status: Has focus");
            LoadGameData();
        }
        else
        {
            //Debug.Log("App Status: Lost focus");
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
            //Debug.Log("App Status: Paused");
            SaveGameData();
        }
        else
        {
            ////Debug.Log("Resumed");
            //LoadGameData();
        }
    }

    void OnApplicationQuit()
    {
        //Debug.Log("App Status: Closed");
        SaveGameData();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("App Status: Opened");
        LoadGameData();
    }
}
