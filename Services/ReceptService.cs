
using System;
using System.Collections.Generic;
using FoodExplorer;
using FoodExplorer.Controllers;
using FoodExplorer.Models;
using Neo4jClient;
using Microsoft.Extensions.Logging;

namespace FoodExplorer.Modules
{
    public class ReceptModule
    {
        private static IGraphClient _neo4JClient;
       
       private static ILogger _logger;

        public ReceptModule(IGraphClient graphClient, ILogger logger)
        {
            _neo4JClient = graphClient;
            _logger = logger;
        }

        public async IAsyncEnumerable<object> CreateRecept(int id, string naziv, string opis, int vremePr, string kategorija, string uputstvoPr)
        {
            var obj = new object();
            try
            {
                Dictionary<string, object> dictParam = new Dictionary<string, object>();
                dictParam.Add("Id", id);
                dictParam.Add("Naziv", naziv);
                dictParam.Add("Opis", opis);
                dictParam.Add("VremePripreme", vremePr);
                dictParam.Add("Kategorija", kategorija);
                dictParam.Add("UputstvoPripreme", uputstvoPr);
                obj =  await _neo4JClient.Cypher.Create("(m:Recept{id: $Id, naziv: $Naziv, opis: $Opis, vremePr: $VremePripreme, kategorija: $Kategorija, uputstvoPr: $UputstvoPripreme})")
                                                .WithParams(dictParam)
                                                .With("m{.*, Id:id(m)} AS recept")
                                                .Return(recept => recept.As<Recept>())
                                                .ResultsAsync;
                _logger.LogInformation("Recept created successfully");
            }
            catch (Exception e)
            {
                _logger.LogError("Error creating recept! " + e.Message);
            }
            yield return obj;
        }


        

public async IAsyncEnumerable<object> AddSastojak(string receptNaziv, int sastojakId)
{
    var obj = new object();
    try
    {
        obj = await _neo4JClient.Cypher.Match("(m:Recept), (reg: Sastojak)")
            .Where("m.Naziv=$receptNaziv and reg.Id=$sastojakId")
            .WithParam("receptNaziv", receptNaziv)
            .WithParam("sastojakId", sastojakId)
            .Create("(m)-[r:SADRZI]->(reg)")
            .With("m{.*, Id:id(m)} AS recept")
            .Return(recept => recept.As<Recept>())
            .ResultsAsync;
    }
    catch (Exception e)
    {
        _logger.LogError("Error adding recept! " + e.Message);
    }
    yield return obj;
}


        public async IAsyncEnumerable<object> AddPodkategorija(long receptId, long podkategorijaId)
        {
            var obj = new object();
            try
            {
                obj = await _neo4JClient.Cypher.Match("(m:Recept), (reg: Podkategorija)")
                                                .Where("id(m)=$receptId and id(reg)=$podkategorijaId")
                                                .WithParam("receptId",receptId)
                                                .WithParam("podkategorijaId",podkategorijaId)
                                                .Create("(m)-[r:PRIPADA_PODKATEGORIJI]->(reg)")
                                                .With("m{.*, Id:id(m)} AS recept")
                                                .Return(recept => recept.As<Recept>())
                                                .ResultsAsync;
            }
            catch(Exception e)
            {
                _logger.LogError("Error adding recept! " + e.Message);
            }
            yield return obj;
        } 

// public async IAsyncEnumerable<object> ReturnReceptByName(string name)
// {
//     var obj = new object();
//     try
//     {
//         obj = await _neo4JClient.Cypher.Match("(m:Recept)")
//                                         .Where((Recept m) => m.Naziv == name)
//                                         .With("m{.*, Id:id(m)} AS recept")
//                                         .Return(recept => recept.As<Recept>())
//                                         .ResultsAsync;
//     }
//     catch(Exception e)
//     {
//         _logger.LogError("Error returning recept");
//     } 
//     yield return obj;
// }

       public async Task<IEnumerable<Recept>> ReturnReceptByName(string naziv)
        {
            try
            {
                await _neo4JClient.ConnectAsync();

                var recepti = await _neo4JClient.Cypher
                    .Match("(recept:Recept)")
                    .Where((Recept recept) => recept.Naziv == naziv)
                    .Return(recept => recept.As<Recept>())
                    .ResultsAsync;

                return recepti;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Greška prilikom dohvatanja recepta po nazivu: {ex.Message}");
                throw;
            }


        }
                public async IAsyncEnumerable<object> ReturnReceptById(int id)
        {
            var obj = await _neo4JClient.Cypher.Match("(m:Recept)")
                                                .Where("id(m)= $Id")
                                                .WithParam("Id",id)
                                                .With("m{.*, Id:id(m)} AS recept")
                                                .Return(recept => recept.As<Recept>())
                                                .ResultsAsync;
            yield return obj;
        }
    }
}