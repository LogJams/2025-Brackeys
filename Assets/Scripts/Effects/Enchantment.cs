using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EFFECTS {
    fire, harmless, heal
}

public enum TARGETS{
    self, target
}


[Serializable]
public struct Curse {
    public ATTRIBUTE trigger;
    public TARGETS target;
    public EFFECTS effect;
}


[CreateAssetMenu(fileName = "Enchantment", menuName = "Scriptable Objects/Enchantment")]
public class Enchantment : ScriptableObject {

    public TARGETS target;
    public EFFECTS effect;
    public ATTRIBUTE attribute;

    public List<Curse> curses;

}
