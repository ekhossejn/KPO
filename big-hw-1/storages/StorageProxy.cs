using System;
using big_hw_1.models;

namespace big_hw_1.storages
{
	public class StorageProxy<T> : IStorage<T> where T : class, IModel
	{
		private readonly IStorage<T> _storage;
		private Dictionary<Guid, T> _cache = new();

		public StorageProxy(IStorage<T> storage)
    	{
        	_storage = storage;
        	_cache = _storage.Get().ToDictionary(x => x.Id);
    	}

		public void Add(T model) {
			_storage.Add(model);
            if (_cache.ContainsKey(model.Id))
            {
                throw new ArgumentException("Duplicated operation id");
            }
            _cache[model.Id] = model;
		}

        public T? Get(Guid id) => _cache.TryGetValue(id, out var res) ? res : null;

        public IEnumerable<T> Get() => _cache.Values;

		public void Delete(Guid id) {
			_storage.Delete(id);
			_cache.Remove(id);
		}

		public void Replace(T model) {
			_storage.Replace(model);
			if (!_cache.ContainsKey(model.Id))
            {
                throw new ArgumentException("Unknown operation id");
            }
            _cache[model.Id] = model;
		}
	}
}
