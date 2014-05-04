﻿using System.Collections.Generic;
using System.ServiceModel;
using SymbolService.Models;

namespace RESTSymbolService
{
    [ServiceContract]
    public interface ISymbolService
    {
        [OperationContract]
        string LoadSectors(List<Sector> sectors);

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
