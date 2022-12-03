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

    private float _temp = 1;
    public float PlanetTemp
    {
        get { return _temp; }
        set
        {
            _temp = Mathf.Clamp(value, 0, 1);
            uiController.Temp = _temp;
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
    #endregion

    private InputManager inputManager;

    private bool screenClicked;
    private bool blessingClicked;
    private bool punishmentClicked;
    private bool tutorial = false;

    private int growRate = 1;

    public void Interaction()
    {
        if (uiController.PlanetEvent)
        {
            uiController.PlanetEvent = false;
            PlanetEnergy += 0.1f;
            PlanetWater += 0.1f;
            PlanetTemp += 0.1f;
            PlanetCoins += 5;

            if (growRate == 1)
                PlanetPopulation += 1;
            else
                PlanetPopulation += UnityEngine.Random.Range(0, growRate);
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
        dialogueList = jsonReader.Lines;
        //LoadGameData();
        TickSystemInit();
    }

    private void TickSystemInit()
    {
        // Nos subscribimos al evento OnTick, para ejecutar logica del juego en cada Tick
        TimeTickSystem.OnTick += delegate(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (!tutorial)
            {
                this.LogLog("--- TICK --- " + e.tick % tickPerYear);

                if ((e.tick % tickPerYear) == 0)
                {
                    this.LogLog("--- YEAR ---");
                    ProcessYear(1);
                }
            }
        };
    }

    private void ProcessYear(int years)
    {
        this.LogLog("Years passed: " + years);
        for (int i = 0; i < years; i++)
        {
            PlanetYear++;
            if (PlanetYear % 5 == 0)
            {
                //PlanetEnergy -= 0.01f;
                PlanetWater -= 0.01f;
                PlanetTemp -= 0.01f;

                if (PlanetEnergy > 0.25f && PlanetWater > 0.25f && PlanetTemp > 0.25f)
                {
                    PlanetPopulation += UnityEngine.Random.Range(0, PlanetPopulation);
                }

                uiController.PlanetEvent = true;
            }
        }

        FindDialogue(PlanetPopulation);

        SaveGameData();
    }

    private void FindDialogue(int population)
    {
        foreach (newsDialogues newDialogue in dialogueList.newsDialogues)
        {
            string populationAux = newDialogue.name.Substring(10);
            if (populationAux.Length <= 2)
            {
                if (int.Parse(populationAux) == population)
                {
                    uiController.ShowDialogue(
                        newDialogue.line[(int)UnityEngine.Random.Range(0, newDialogue.line.Length)]
                    );
                }
            }
            else
            {
                string[] populationRange = populationAux.Split("-");
                this.LogLog("Population Range: " + populationRange[0] + ", " + populationRange[1]);
                if (
                    population >= int.Parse(populationRange[0])
                    && population <= int.Parse(populationRange[1])
                )
                {
                    uiController.ShowDialogue(
                        newDialogue.line[(int)UnityEngine.Random.Range(0, newDialogue.line.Length)]
                    );
                }
            }
        }
    }

    IEnumerator WaitingForInput()
    {
        screenClicked = false;
        blessingClicked = false;
        punishmentClicked = false;

        for (int i = 0; i < dialogueList.cutsceneDialogue.Length; i++)
        {
            var cutsceneDialogue = dialogueList.cutsceneDialogue[i];

            foreach (var interaction in cutsceneDialogue.interactions)
            {
                if (interaction == "WaitForBasicInput")
                {
                    uiController.ShowDialogue(cutsceneDialogue.line);
                    yield return new WaitUntil(IsScreenClicked);
                    screenClicked = false;
                }
                else if (interaction == "WaitForBlessingInput")
                {
                    uiController.ShowDialogue(cutsceneDialogue.line);
                    yield return new WaitUntil(IsBlessingClicked);
                    blessingClicked = false;
                    pointerAtBlessing.SetActive(false);
                    PlanetPopulation = 1;
                }
                else if (interaction == "WaitForPunishmentInput")
                {
                    uiController.ShowDialogue(cutsceneDialogue.line);
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
        endTutorial();
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

    public void Blessing()
    {
        if ((PlanetEnergy -= 0.25f) >= 0)
        {
            blessingClicked = true;
            PlanetEnergy -= 0.25f;
        }
    }

    public void Punishment()
    {
        if ((PlanetEnergy -= 0.25f) >= 0)
        {
            punishmentClicked = true;
            PlanetEnergy -= 0.25f;
        }
    }

    public void SaveGameData()
    {
        this.LogLog("--- SAVE ---");
        //this.LogLog("Archivo creado en " + Application.persistentDataPath + "/gameDataContainer.dat");
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
        gd.food = PlanetEnergy;
        gd.water = PlanetWater;
        gd.temp = PlanetTemp;
        gd.population = PlanetPopulation;

        bf.Serialize(file, gd);
        file.Close();
    }

    public void LoadGameData()
    {
        this.LogLog("--- LOAD ---");
        // Si no existe el archivo se crea uno nuevo y se inicializan las estadisticas (Primera vez que se inicia el juego)
        dialogueList = jsonReader.Lines;

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
            PlanetEnergy = gd.food;
            PlanetWater = gd.water;
            PlanetTemp = gd.temp;
            PlanetPopulation = gd.population;

            // Se convierte del tiempo que se ha recuperado del fichero a ticks del juego que han transcurrido desde esa fecha hasta ahora
            this.LogLog(
                "Ticks passed: " + ((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear)
            );
            uiController.LoadPopup = (
                (int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear
            ).ToString();

            ProcessYear(((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear));
        }
        else
        {
            this.LogLog("--- FAILED LOAD ---");
            this.LogLog(
                "No existe el archivo en "
                    + Application.persistentDataPath
                    + "/gameDataContainer.dat"
            );
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

        PlanetEnergy = 1;
        PlanetWater = 1;
        PlanetTemp = 1;
        //ProcessYear(0);
        SaveGameData();

        startTutorial();
    }

    void startTutorial()
    {
        tutorial = true;
        cutsceneBackground.SetActive(true);
        StartCoroutine("WaitingForInput");
    }

    void endTutorial()
    {
        tutorial = false;
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
            this.LogLog("App Status: Has focus");
            LoadGameData();
        }
        else
        {
            this.LogLog("App Status: Lost focus");
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
            this.LogLog("App Status: Paused");
            SaveGameData();
        }
        else
        {
            //this.LogLog("Resumed");
            //LoadGameData();
        }
    }

    void OnApplicationQuit()
    {
        this.LogLog("App Status: Closed");
        SaveGameData();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.LogLog("App Status: Opened");
        LoadGameData();
    }
}
