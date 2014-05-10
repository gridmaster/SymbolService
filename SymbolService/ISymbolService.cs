using System.ServiceModel;
using SymbolService.Models.Requests;

namespace SymbolService
{
    [ServiceContract]
    public interface ISymbolService
    {
        [OperationContract]
        string LoadSectors(SectorRequest sectors);

        [OperationContract]
        string LoadDailySectors(BasicRequest basicRequest);

        [OperationContract]
        string LoadIndustries(IndustryRequest industries);

        [OperationContract]
        string LoadDailyIndustries(BasicRequest basicRequest);

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
