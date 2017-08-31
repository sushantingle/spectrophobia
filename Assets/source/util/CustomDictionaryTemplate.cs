using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dictionary template class.
/// Implemented to display dictionary in editor view.
/// TODO: Still need to work on editor ui customization.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// 

[System.Serializable]
public class DictionaryTemplate<TKey, TValue>
{
    [SerializeField] public TKey _key;
    [SerializeField] public TValue _value;
}

// need to implement
public class DictionaryUtility
{
    
};
