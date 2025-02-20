using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EFFECTS {
    fire, harmless, heal, crit, confident, ethereal
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

[Serializable]
public struct TemporaryAttributes {
    public ATTRIBUTE trigger;
    public EFFECTS effect;
}

[CreateAssetMenu(fileName = "Enchantment", menuName = "Scriptable Objects/Enchantment")]
public class Enchantment : ScriptableObject {

    [Header("Weilder Attributes")]
    public List<TemporaryAttributes> benefits;

    [Header("Attack Effects")]
    public TARGETS target;
    public EFFECTS effect;
    public ATTRIBUTE attribute;
    [Header("Curse Effects")]
    public List<Curse> curses;

}
