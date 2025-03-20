using System;
using big_hw_1.models;

namespace big_hw_1.factories
{
	public class CategoryFactory
	{
        public static Category Create(string name, models.Type type)
        {
            return new Category(Guid.NewGuid(), name, type);
        }

        public static Category CreateWithDefaultName(models.Type type)
        {
            return new Category(Guid.NewGuid(), "default_name_" + Guid.NewGuid().ToString(), type);
        }
    }
}

