using Patterson.model;

namespace Patterson.repository
{
    internal interface IElementRepository
    {
        void Populate();

        string[] GetAllElementNames();

        Element FindElementByName(string name);
    }
}
