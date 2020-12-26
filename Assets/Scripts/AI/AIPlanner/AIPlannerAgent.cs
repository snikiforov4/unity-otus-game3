using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using UnityEngine.AI.Planner.DomainLanguage.TraitBased;
using Generated.AI.Planner.StateRepresentation;

public class AIPlannerAgent : MonoBehaviour
{
    PlayerHealth health;
    PlayerAmmo ammo;
    BotUtility botUtility;
    TraitComponent traitComponent;

    void Start()
    {
        botUtility = GetComponent<BotUtility>();
        health = botUtility.GetComponent<PlayerHealth>();
        ammo = botUtility.GetComponent<PlayerAmmo>();
        traitComponent = GetComponent<TraitComponent>();

        int uniqueId = (TryGetComponent<AIPlannerTarget>(out var target) ? target.UniqueId : -1);
        ITraitData agent = traitComponent.GetTraitData<Agent>();
        agent.SetValue("UniqueId", uniqueId);

        UpdateParams();
    }

    void Update()
    {
        UpdateParams();
    }

    void UpdateParams()
    {
        ITraitData agent = traitComponent.GetTraitData<Agent>();
        agent.SetValue("Health", health.health);
        agent.SetValue("Ammo", ammo.ammo);
        agent.SetValue("Navigating", botUtility.IsNavigating());
    }

    public IEnumerator NavigateTo(GameObject target)
    {
        if (botUtility.NavigateTo(target.transform)) {
            do {
                yield return null;
            } while (botUtility.IsNavigating());
        }
    }

    public IEnumerator NavigateToEnemy()
    {
        var target = botUtility.FindClosestPlayer();
        if (botUtility.NavigateTo(target)) {
            do {
                yield return null;
            } while (botUtility.IsNavigating() && botUtility.GetDistanceToClosestEnemy() >= 20.0f);
        }
    }

    public IEnumerator AttackEnemy()
    {
        var target = botUtility.FindClosestPlayer();
        if (botUtility.Attack(target))
            yield return new WaitForSeconds(1.0f);
    }
}
