using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SkinChosenSingleton : MonoBehaviour
{
    private SkinChosenSingleton(){}
    private static SkinChosenSingleton instance = null;
    public static SkinChosenSingleton Instance{
        get{
            if(instance == null){
                instance = new SkinChosenSingleton();
            }
            return instance;
        }
    }
    
    private static string chosenSkin;
    public static string ChosenSkin{
        get{
            return chosenSkin;
        }
        set{
            chosenSkin = value;
        }
    }
}
