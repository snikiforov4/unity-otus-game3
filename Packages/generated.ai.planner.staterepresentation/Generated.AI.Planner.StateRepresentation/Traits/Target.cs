using System;
using Unity.Entities;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Generated.AI.Planner.StateRepresentation.Enums;

namespace Generated.AI.Planner.StateRepresentation
{
    [Serializable]
    public struct Target : ITrait, IEquatable<Target>
    {
        public const string FieldUniqueId = "UniqueId";
        public System.Int32 UniqueId;

        public void SetField(string fieldName, object value)
        {
            switch (fieldName)
            {
                case nameof(UniqueId):
                    UniqueId = (System.Int32)value;
                    break;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Target.");
            }
        }

        public object GetField(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(UniqueId):
                    return UniqueId;
                default:
                    throw new ArgumentException($"Field \"{fieldName}\" does not exist on trait Target.");
            }
        }

        public bool Equals(Target other)
        {
            return UniqueId == other.UniqueId;
        }

        public override string ToString()
        {
            return $"Target\n  UniqueId: {UniqueId}";
        }
    }
}
