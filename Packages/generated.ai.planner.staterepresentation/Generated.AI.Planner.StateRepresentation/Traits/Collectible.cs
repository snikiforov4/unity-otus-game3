using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Generated.AI.Planner.StateRepresentation.Enums;

namespace Generated.AI.Planner.StateRepresentation
{
    [Serializable]
    public struct Collectible : ITrait, IEquatable<Collectible>
    {
        public const string FieldProvides = "Provides";
        public const string FieldAmount = "Amount";
        public const string FieldActive = "Active";
        public Generated.AI.Planner.StateRepresentation.Enums.Consumable Provides;
        public System.Int32 Amount;
        public System.Boolean Active;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(Provides):
                    Provides = (Generated.AI.Planner.StateRepresentation.Enums.Consumable)Enum.ToObject(typeof(Generated.AI.Planner.StateRepresentation.Enums.Consumable), value);
                    break;
                case nameof(Amount):
                    Amount = (System.Int32)value;
                    break;
                case nameof(Active):
                    Active = (System.Boolean)value;
                    break;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Collectible.");
            }
        }

        public object GetField(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(Provides):
                    return Provides;
                case nameof(Amount):
                    return Amount;
                case nameof(Active):
                    return Active;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Collectible.");
            }
        }

        public bool Equals(Collectible other)
        {
            return Provides == other.Provides && Amount == other.Amount && Active == other.Active;
        }

        public override string ToString()
        {
            return $"Collectible\n  Provides: {Provides}\n  Amount: {Amount}\n  Active: {Active}";
        }
    }
}
