using System;
namespace big_hw_1.models
{
	public class Category
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public Type Type { get; private set; }

		public Category(Guid id, string name, Type type)
		{
			Id = id;
			Name = name;
			Type = type;
		}

		public void ChangeName(string newName)
		{
			Name = newName;
		}

		public void ChangeType(Type newType)
		{
			Type = newType;
		}
	}
}

