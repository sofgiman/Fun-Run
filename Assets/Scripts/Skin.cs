using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Represent skins nameds
public static class SkinsNames{
    public const string templeRunnerName ="Temple Runner";
    public const string zoeName ="Zoe";
    public const string gingerName = "Ginger";
    public const string blackberryName = "Blackberry";
}

// Class to represent the skin on lobby. Made from sprite idle and skin's name
public class Skin
{       private Sprite spriteIdle;  // the idle skin's image that will shown in lobby
        public Sprite SpriteIdle{
            get{
                return spriteIdle;
            }
        }
        private string name;  // skin's name
        public string Name{
            get {
                
                return name;
            }
        }
        // Constructor 
        public Skin(Sprite spriteIdle, string name)
        {
            this.spriteIdle = spriteIdle;
            this.name = name;
        }

}

