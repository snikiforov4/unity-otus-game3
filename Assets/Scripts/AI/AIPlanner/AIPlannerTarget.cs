using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using UnityEngine.AI.Planner.DomainLanguage.TraitBased;
using Generated.AI.Planner.StateRepresentation;

public class AIPlannerTarget : MonoBehaviour
{
    static int UniqueIdCounter;
    public int UniqueId { get; private set; }

    void Awake()
    {
        UniqueId = UniqueIdCounter++;
    }

    void Start()
    {
        var traitComponent = GetComponent<TraitComponent>();
        ITraitData target = traitComponent.GetTraitData<Target>();
        target.SetValue("UniqueId", UniqueId);
    }
}
