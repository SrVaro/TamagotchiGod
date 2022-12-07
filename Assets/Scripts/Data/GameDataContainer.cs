using System;
using System.Collections.Generic;

[Serializable]
public class GameDataContainer
{
    public DateTime savedTime;
    public int year;
    public float food;
    public float water;
    public float temp;
    public int population;
    public Dictionary<string, bool> dialogueVariables = new Dictionary<string, bool>();
}
