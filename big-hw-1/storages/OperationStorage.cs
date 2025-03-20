using System;
using big_hw_1.models;

namespace big_hw_1.storages
{
	public class OperationStorage
	{
        private readonly Dictionary<Guid, Operation> _operationStorage = new();

        public void Add(Operation operation)
        {
            if (_operationStorage.ContainsKey(operation.Id))
            {
                throw new ArgumentException("Duplicated operation id");
            }
            _operationStorage[operation.Id] = operation;
        }

        public Operation? Get(Guid id) => _operationStorage.TryGetValue(id, out var res) ? res : null;

        public IEnumerable<Operation> Get() => _operationStorage.Values;

        public void Delete(Guid id)
        {
            _operationStorage.Remove(id);
        }

        public void Replace(Operation operation)
        {
            if (!_operationStorage.ContainsKey(operation.Id))
            {
                throw new ArgumentException("Unknown operation id");
            }
            _operationStorage[operation.Id] = operation;
        }
    }
}

