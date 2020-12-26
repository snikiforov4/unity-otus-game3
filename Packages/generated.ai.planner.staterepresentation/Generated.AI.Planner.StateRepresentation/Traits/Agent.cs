using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Generated.AI.Planner.StateRepresentation.Enums;

namespace Generated.AI.Planner.StateRepresentation
{
    [Serializable]
    public struct Agent : ITrait, IEquatable<Agent>
    {
        public const string FieldUniqueId = "UniqueId";
        public const string FieldHealth = "Health";
        public const string FieldAmmo = "Ammo";
        public const string FieldNavigating = "Navigating";
        public System.Int32 UniqueId;
        public System.Int32 Health;
        public System.Int32 Ammo;
        public System.Boolean Navigating;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(UniqueId):
                    UniqueId = (System.Int32)value;
                    break;
                case nameof(Health):
                    Health = (System.Int32)value;
                    break;
                case nameof(Ammo):
                    Ammo = (System.Int32)value;
                    break;
                case nameof(Navigating):
                    Navigating = (System.Boolean)value;
                    break;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Agent.");
            }
        }

        public object GetField(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(UniqueId):
                    return UniqueId;
                case nameof(Health):
                    return Health;
                case nameof(Ammo):
                    return Ammo;
                case nameof(Navigating):
                    return Navigating;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Agent.");
            }
        }

        public bool Equals(Agent other)
        {
            return UniqueId == other.UniqueId && Health == other.Health && Ammo == other.Ammo && Navigating == other.Navigating;
        }

        public override string ToString()
        {
            return $"Agent\n  UniqueId: {UniqueId}\n  Health: {Health}\n  Ammo: {Ammo}\n  Navigating: {Navigating}";
        }
    }
}
