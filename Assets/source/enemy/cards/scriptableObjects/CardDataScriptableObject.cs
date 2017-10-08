using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "CardData")]
[System.Serializable]
public class CardDataScriptableObject : ScriptableObject {

    public CardDataBase card;
}
