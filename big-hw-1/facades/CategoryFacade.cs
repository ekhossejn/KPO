using System;
using big_hw_1.storages;
using big_hw_1.factories;
using big_hw_1.models;
namespace big_hw_1.facades
{
	public class CategoryFacade
	{
        private readonly CategoryStorage _categoryStorage;

        public CategoryFacade(CategoryStorage categoryStorage)
        {
            _categoryStorage = categoryStorage;
        }

        public IEnumerable<Category> Get() => _categoryStorage.Get();

        public Category? Get(Guid id) => _categoryStorage.Get(id);

        public void Create(string name, models.Type type)
        {
            var category = CategoryFactory.Create(name, type);
            _categoryStorage.Add(category);
        }

        public void ChangeName(Guid id, string name)
        {
            var category = _categoryStorage.Get(id) ?? throw new ArgumentException("Unknown Category id");
            category.ChangeName(name);
            _categoryStorage.Replace(category);
        }

        public void ChangeType(Guid id, models.Type type)
        {
            var category = _categoryStorage.Get(id) ?? throw new ArgumentException("Unknown Category id");
            category.ChangeType(type);
            _categoryStorage.Replace(category);
        }

        public void Delete(Guid id)
        {
            _categoryStorage.Delete(id);
        }
	}
}

