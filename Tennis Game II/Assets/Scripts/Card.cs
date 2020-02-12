using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{

    public class Attack { //attacks that effect how the ball is returned
        public delegate void effect_attack(Card_Attack attack);  
    }
    public class Support { //moves that increase or decreases stats of the player or opponent, respectively 
        public delegate Stats effect_support(Card_Support support); 
    }
    public class Defense { //moves that enable you to recieve the ball easier
        public delegate void effect_defense(Card_Support defense); 
    }
}
