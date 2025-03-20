using System;
using big_hw_1.models;

namespace big_hw_1.storages
{
	public interface IStorage<T> where T : class, IModel
	{

		void Add(T model);

        T? Get(Guid id);

        IEnumerable<T> Get();

		void Delete(Guid id);

		void Replace(T model);
	}
}
