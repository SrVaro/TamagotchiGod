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
    private float _food = 1;
    public float PlanetFood {
        get {return _food;}
        set {
            _food = Mathf.Clamp(value, 0, 1);
            uiController.Food = _food;
        }
    }

    private float _water = 1;
    public float PlanetWater {
        get {return _water;}
        set {
            _water = Mathf.Clamp(value, 0, 1);
            uiController.Water = _water;
        }
    }

    private float _temp = 1;
    public float PlanetTemp {
        get {return _temp;}
        set {
            _temp = Mathf.Clamp(value, 0, 1);
            uiController.Temp = _temp;
        }
    }

    private int _year;
    public int PlanetYear {
        get {return _year;}
        set {
            _year = value;
            uiController.yearText = "Year " + PlanetYear.ToString();
        }
    }
    
    private int _population;
    public int PlanetPopulation {
        get {return _population;}
        set {
            _population = value;
            uiController.populationText = PlanetPopulation.ToString();
        }
    }
    
    private int _coins;
    public int PlanetCoins {
        get {return _coins;}
        set {
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

    public void Inteaction() {
        if (uiController.PlanetEvent) {
            uiController.PlanetEvent = false;
            PlanetFood += 0.1f;
            PlanetWater += 0.1f;
            PlanetTemp += 0.1f;
            PlanetCoins += 5;
        }
        
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake() {
        Application.targetFrameRate = 60;
        dialogueList = jsonReader.Lines;
        //LoadGameData();
        TickSystemInit();
    }

    private void TickSystemInit() {
        // Nos subscribimos al evento OnTick, para ejecutar logica del juego en cada Tick
        TimeTickSystem.OnTick += delegate(object sender, TimeTickSystem.OnTickEventArgs e) {
            if(e.tick % tickPerYear == 0) {
                Debug.Log("--- TICK ---");
                ProcessYear(1);
            }
        };
    }

    private void ProcessYear(int years) {
        //Debug.Log("Years passed: " + years);
        for (int i = 0; i < years; i++)
        {
            PlanetYear++;
            if(PlanetYear % 5 == 0) {
                PlanetFood -= 0.01f;
                PlanetWater -= 0.01f;
                PlanetTemp -= 0.01f;
                
                if (PlanetFood > 0.25f && PlanetWater > 0.25f && PlanetTemp > 0.25f) {
                    PlanetPopulation = PlanetPopulation + UnityEngine.Random.Range(0, PlanetPopulation);
                }

                uiController.PlanetEvent = true;
            }
        }

        uiController.ShowDialogue(FindDialogue(PlanetPopulation));
        
        SaveGameData();
    }   

    private string FindDialogue(int population){
        int populationLvl = 0;
        List<newsDialogues> avalibleDialogue = new List<newsDialogues>();

         if(population == 2) {
            populationLvl = 0;
        } else if (population == 4) {
            populationLvl = 1;
        } else if (population > 4 && population <= 10) {
            populationLvl = 2;
        } else if (population > 10 && population <= 200) {
            populationLvl = 3;
        } else if (population > 200) {
            populationLvl = 4;
        }

        foreach (newsDialogues newDialogue in dialogueList.newsDialogues)
        {
            if(newDialogue.name.ToUpper().Equals("POPULATIONLVL" + populationLvl)) {
                avalibleDialogue.Add(newDialogue); 
            }
        }

        return avalibleDialogue[(int)UnityEngine.Random.Range(0, avalibleDialogue.Count)].line;
    }

    public void SaveGameData() {
        Debug.Log("--- SAVE ---");
        //Debug.Log("Archivo creado en " + Application.persistentDataPath + "/gameDataContainer.dat");
        // Se crea/sustituye el archivo del sistema (Windows/Android/IOs/Etc...)
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/gameDataContainer.dat", FileMode.Create);
        GameDataContainer gd = new GameDataContainer();

        // Se guardan las variables en el archivo
        gd.savedTime = DateTime.Now;
        gd.year = PlanetYear;
        gd.food = PlanetFood;
        gd.water = PlanetWater;
        gd.temp = PlanetTemp;
        gd.population = PlanetPopulation;

        bf.Serialize(file, gd);
        file.Close();
    }

    public void LoadGameData() {
        Debug.Log("--- LOAD ---");
        // Si no existe el archivo se crea uno nuevo y se inicializan las estadisticas (Primera vez que se inicia el juego)
        dialogueList = jsonReader.Lines;

        if (File.Exists(Application.persistentDataPath + "/gameDataContainer.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameDataContainer.dat", FileMode.Open);
            GameDataContainer gd = (GameDataContainer)bf.Deserialize(file);
            file.Close();

            // Se cargan los datos del fichero en las variables del juego
            DateTime savedTime = gd.savedTime;
            PlanetYear = gd.year;
            PlanetFood = gd.food;
            PlanetWater = gd.water;
            PlanetTemp = gd.temp;
            PlanetPopulation = gd.population;

            // Se convierte del tiempo que se ha recuperado del fichero a ticks del juego que han transcurrido desde esa fecha hasta ahora
            Debug.Log("Ticks passed: " + ((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear));
            uiController.LoadPopup = ((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear).ToString();

            ProcessYear(((int)(DateTime.Now - savedTime).TotalSeconds / tickPerYear));
        } else {
            Debug.Log("--- FAILED LOAD ---");
            Debug.Log("No existe el archivo en " + Application.persistentDataPath + "/gameDataContainer.dat");
            InitializeGameData();
        }
    }

    public void InitializeGameData() {
            PlanetYear = 0;
            PlanetPopulation = 2;
            PlanetCoins = 0;
            
            PlanetFood = 1;
            PlanetWater = 1;
            PlanetTemp = 1;
            SaveGameData();
    }    

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) {
            Debug.Log("Has focus");
            LoadGameData();
        } else {
            Debug.Log("Lost focus");
            SaveGameData();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) {
            Debug.Log("Pausado");
            SaveGameData();
        } else {
            //Debug.Log("Resumed");
            //LoadGameData();
        }
    }
    

    void OnApplicationQuit() {
        Debug.Log("Quitado");
        SaveGameData();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada");
        LoadGameData();
    }

}


