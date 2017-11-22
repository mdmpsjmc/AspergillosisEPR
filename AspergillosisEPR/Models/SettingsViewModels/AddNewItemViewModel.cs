using AspergillosisEPR.Extensions;

namespace AspergillosisEPR.Models.SettingsViewModels
{
    public class AddNewItemViewModel
    {
        public string Title
        {
            get
            {
                return ItemKlass.SplitCamelCase();
            }
        }
        public string ItemKlass { get; set; }
        public string Underscore
        {
            get
            {
                return ItemKlass.ToUnderscore();
            }
        }
        public string Controller { get; set; }
    }
}
