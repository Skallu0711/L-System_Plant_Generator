using System.Collections.Generic;
using UnityEngine;

public class RulePicker
{
    private List<string> rulesList = new List<string>
    {
         "[F[+FX][*+FX][/+FX]]",
        // "[*+FX]X[+FX][/+F-FX]",
        // "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]"
        
        //"[FFF+[FFF+[FFF+[FFF+[FFF]"
    };

    public string PickRandomRule() => rulesList[Random.Range(0, rulesList.Count)];
    
}