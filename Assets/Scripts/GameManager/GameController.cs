using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private int tickPerYear = 5;

    [SerializeField]
    private UIController uiController;

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

    public void Inteaction()
    {
        if (uiController.PlanetEvent)
        {
            uiController.PlanetEvent = false;
            PlanetEnergy += 0.1f;
            PlanetWater += 0.1f;
            PlanetTemp += 0.1f;
            PlanetCoins += 5;
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
        if (!tutorial)
        {
            // Nos subscribimos al evento OnTick, para ejecutar logica del juego en cada Tick
            TimeTickSystem.OnTick += delegate(object sender, TimeTickSystem.OnTickEventArgs e)
            {
                if (e.tick % tickPerYear == 0)
                {
                    Debug.Log("--- TICK ---");
                    ProcessYear(1);
                }
            };
        }
    }

    private void ProcessYear(int years)
    {
        //Debug.Log("Years passed: " + years);
        for (int i = 0; i < years; i++)
        {
            PlanetYear++;
            if (PlanetYear % 5 == 0)
            {
                PlanetEnergy -= 0.01f;
                PlanetWater -= 0.01f;
                PlanetTemp -= 0.01f;

                if (PlanetEnergy > 0.25f && PlanetWater > 0.25f && PlanetTemp > 0.25f)
                {
                    PlanetPopulation =
                        PlanetPopulation + UnityEngine.Random.Range(0, PlanetPopulation);
                }

                uiController.PlanetEvent = true;
            }
        }

        FindDialogue(PlanetPopulation);

        SaveGameData();
    }

    private void FindDialogue(int population)
    {
        if (!tutorial)
        {
            int populationLvl = 0;
            List<newsDialogues> avalibleDialogue = new List<newsDialogues>();

            if (population == 2)
            {
                populationLvl = 0;
            }
            else if (population == 4)
            {
                populationLvl = 1;
            }
            else if (population > 4 && population <= 10)
            {
                populationLvl = 2;
            }
            else if (population > 10 && population <= 200)
            {
                populationLvl = 3;
            }
            else if (population > 200)
            {
                populationLvl = 4;
            }

            foreach (newsDialogues newDialogue in dialogueList.newsDialogues)
            {
                if (newDialogue.name.ToUpper().Equals("POPULATIONLVL" + populationLvl))
                {
                    avalibleDialogue.Add(newDialogue);
                }
            }
            uiController.ShowDialogue(
                avalibleDialogue[(int)UnityEngine.Random.Range(0, avalibleDialogue.Count)].line
            );
        }
        else
        {
            StartCoroutine("WaitingForInput");
        }
    }

    IEnumerator WaitingForInput()
    {
        for (int i = 0; i < dialogueList.cutsceneDialogue.Length; i++)
        {
            var cutsceneDialogue = dialogueList.cutsceneDialogue[i];
            if (cutsceneDialogue.interaction == "WaitForBasicInput")
            {
                uiController.ShowDialogue(cutsceneDialogue.line);
                yield return new WaitUntil(IsScreenClicked);
                screenClicked = false;
            }
            else if (cutsceneDialogue.interaction == "WaitForBlessingInput")
            {
                uiController.ShowDialogue(cutsceneDialogue.line);
                yield return new WaitUntil(IsBlessingClicked);
                screenClicked = false;
            }
            else if (cutsceneDialogue.interaction == "WaitForPunishmentInput")
            {
                uiController.ShowDialogue(cutsceneDialogue.line);
                yield return new WaitUntil(IsPunishmentClicked);
                screenClicked = false;
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
        return screenClicked;
    }

    private bool IsPunishmentClicked()
    {
        return screenClicked;
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
        Debug.Log("--- SAVE ---");
        //Debug.Log("Archivo creado en " + Application.persistentDataPath + "/gameDataContainer.dat");
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
        Debug.Log("--- LOAD ---");
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
            Debug.Log(
                "Ticks passed: " + ((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear)
            );
            uiController.LoadPopup = (
                (int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear
            ).ToString();

            ProcessYear(((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear));
        }
        else
        {
            Debug.Log("--- FAILED LOAD ---");
            Debug.Log(
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
        tutorial = true;
        PlanetYear = 0;
        PlanetPopulation = 2;
        PlanetCoins = 0;

        PlanetEnergy = 1;
        PlanetWater = 1;
        PlanetTemp = 1;
        ProcessYear(0);
        SaveGameData();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Debug.Log("Has focus");
            LoadGameData();
        }
        else
        {
            Debug.Log("Lost focus");
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
            Debug.Log("Pausado");
            SaveGameData();
        }
        else
        {
            //Debug.Log("Resumed");
            //LoadGameData();
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Quitado");
        SaveGameData();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada");
        LoadGameData();
    }
}
