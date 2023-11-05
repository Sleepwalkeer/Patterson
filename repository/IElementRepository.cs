using Patterson.model;

namespace Patterson.repository
{
    internal interface IElementRepository
    {
        string[] GetAllElementNames();

        Element FindElementByName(string name);
    }
}
