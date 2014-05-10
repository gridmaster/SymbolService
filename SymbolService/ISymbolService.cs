using System.ServiceModel;
using RESTSymbolService;
using SymbolService.Models.Requests;

namespace SymbolService
{
    [ServiceContract]
    public interface ISymbolService
    {
        [OperationContract]
        string LoadSectors(SectorRequest sectors);

        [OperationContract]
        string LoadIndustries(IndustryRequest industries);

        [OperationContract]
        string GetSymbols();

        [OperationContract]
        string GetPeople();

        [OperationContract]
        string GetPerson(int id);

        [OperationContract]
        Person InsertPerson(Person[] person);
  
        [OperationContract]
        Person UpdatePerson(int id, Person person);

        [OperationContract]
        void DeletePerson(int id);
    }
}
