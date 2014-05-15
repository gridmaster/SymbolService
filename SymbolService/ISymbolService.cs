using System.ServiceModel;
using SymbolService.Models;
using SymbolService.Models.Requests;
using SymbolService.Models.ViewModels;

namespace SymbolService
{
    [ServiceContract]
    public interface ISymbolService
    {
        [OperationContract]
        Sectors GetSectors();

        [OperationContract]
        string LoadSectors(SectorRequest sectors);

        [OperationContract]
        string LoadDailySectors(BasicRequest basicRequest);

        [OperationContract]
        Industries GetIndustries();

        [OperationContract]
        IndustryView IndustryWithSectorName();

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
