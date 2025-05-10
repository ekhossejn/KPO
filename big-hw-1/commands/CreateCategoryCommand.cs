using System;
using big_hw_1.facades;

namespace big_hw_1.commands
{
	public class CreateCategoryCommand : ICommand
	{
        private readonly CategoryFacade _categoryFacade;
        private readonly string _categoryName;
        readonly models.Type _type;
        
        public CreateCategoryCommand(CategoryFacade categoryFacade, string categoryName, models.Type type) {
            _categoryFacade = categoryFacade;
            _categoryName = categoryName;
            _type = type;
        }

        public void Execute() {
            _categoryFacade.Create(_categoryName, _type);
        }
    }
}

