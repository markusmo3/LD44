using System;

namespace Kryz.CharacterStats
{

	[Serializable]
	public enum StatModType
	{
		Flat = 100,
		PercentAdd = 200,
		PercentMult = 300,
	}

	[Serializable]
	public class StatModifier : System.Object
	{
		public float Value;
		public StatModType Type = StatModType.Flat;
		public int Order;
		public object Source;

		public StatModifier(float value, StatModType type, int order, object source)
		{
			Value = value;
			Type = type;
			Order = order;
			Source = source;
		}

		public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }

		public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }

		public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
	}
}
