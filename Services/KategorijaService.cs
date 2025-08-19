using System;
using System.Collections.Generic;
using FoodExplorer;
using FoodExplorer.Controllers;
using FoodExplorer.Models;
using Neo4jClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FoodExplorer.Modules
{
    public class KategorijaModule
    {
        private static IGraphClient _neo4JClient;
       
       private static ILogger _logger;

        public KategorijaModule(IGraphClient graphClient, ILogger logger)
        {
            _neo4JClient = graphClient;
            _logger = logger;
        }

public async IAsyncEnumerable<object> CreateKategorija(int id, string naziv)
{
    var obj = new object();
    try
    {
        var query = await _neo4JClient.Cypher
            .Create($"(m:Kategorija {{id: {id}, naziv: '{naziv}'}})")
            .Return(m => m.As<Kategorija>())
            .ResultsAsync;

        obj = query.SingleOrDefault();
        _logger.LogInformation("Kategorija created successfully");
    }
    catch (Exception e)
    {
        _logger.LogError("Error creating Kategorija! " + e.Message);
    }
    yield return obj;
}


        public async IAsyncEnumerable<object> AddRecept(long receptId, long sastojakId)
        {
            var obj = new object();
            try
            {
                obj = await _neo4JClient.Cypher.Match("(m:Recept), (reg: Sastojak)")
                                                .Where("id(m)=$receptId and id(reg)=$sastojakId")
                                                .WithParam("receptId",receptId)
                                                .WithParam("sastojakId",sastojakId)
                                                .Create("(m)-[r:SADRZI]->(reg)")
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

        public async IAsyncEnumerable<object> ReturnKategorije()
        {
            var obj = await _neo4JClient.Cypher.Match("(m:Kategorija)")
                                                .With("m{.*, Id:id(m)} AS kategorija")
                                                .Return(kategorija => kategorija.As<Kategorija>())
                                                .ResultsAsync;
            var mappedResults = obj.Select(kategorija => new
            {
            kategorija.Id,
            kategorija.Naziv,

            });

            yield return mappedResults;
        }

        public async IAsyncEnumerable<object> AddPodKategorija(long podkategorijaId, long kategorijaId)
        {
            var obj = new object();
            try
            {
                obj = await _neo4JClient.Cypher.Match("(m:Podkategorija), (reg: Kategorija)")
                                                .Where("id(m)=$podkategorijaId and id(reg)=$kategorijaId")
                                                .WithParam("podkategorijaId",podkategorijaId)
                                                .WithParam("kategorijaId",kategorijaId)
                                                .Create("(m)-[r:PRIPRADA_KATEGORIJI]->(reg)")
                                                .With("m{.*, Id:id(m)} AS podkategorija")
                                                .Return(podkategorija => podkategorija.As<Podkategorija>())
                                                .ResultsAsync;
            }
            catch(Exception e)
            {
                _logger.LogError("Error adding recept! " + e.Message);
            }
            yield return obj;
        } 

    }
 }