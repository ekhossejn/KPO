using System;
using big_hw_1.models;

namespace big_hw_1.storages
{
	public class CategoryStorage
	{
        private readonly Dictionary<Guid, Category> _categoryStorage = new();

        public void Add(Category category)
        {
            if (_categoryStorage.ContainsKey(category.Id))
            {
                throw new ArgumentException("Duplicated category id");
            }
            _categoryStorage[category.Id] = category;
        }

        public Category? Get(Guid id) => _categoryStorage.TryGetValue(id, out var res) ? res : null;

        public IEnumerable<Category> Get() => _categoryStorage.Values;

        public void Delete(Guid id)
        {
            _categoryStorage.Remove(id);    
        }

        public void Replace(Category category)
        {
            if (!_categoryStorage.ContainsKey(category.Id))
            {
                throw new ArgumentException("Unknown category id");
            }
            _categoryStorage[category.Id] = category;
        }
    }
}

