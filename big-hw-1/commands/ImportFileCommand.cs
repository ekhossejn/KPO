using System;
using big_hw_1.facades;
using big_hw_1.models;
using big_hw_1.storages;
using big_hw_1.importers;

namespace big_hw_1.commands
{
	public class ImportFileCommand : ICommand
	{
        private readonly FileImporter _importer;
        private readonly string _filePath;

        public ImportFileCommand(FileImporter importer, string filePath) {
            _importer = importer;
            _filePath = filePath;
        }

        public void Execute() {
            _importer.ImportFile(_filePath);
        }
    }
}
