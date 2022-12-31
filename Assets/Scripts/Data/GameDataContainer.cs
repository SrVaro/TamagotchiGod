using System;
using System.Collections.Generic;

[Serializable]
public class GameDataContainer
{
    public DateTime savedTime;
    public int year;
    public float energy;
    public float happiness;
    public float hygiene;
    public float sleep;
    public float faith;
    public float culture;
    public float science;
    public float souls;
    public int population;
    public Dictionary<string, bool> dialogueVariables = new Dictionary<string, bool>();
}
