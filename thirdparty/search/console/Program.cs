using System;
using System.Linq;
using System.Threading;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Spatial;

namespace linuxacademy.az203.thirdparty.search
{
    class Program
    {
        static void Main(string[] args)
        {
            // get configuration information
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var searchServiceName = configuration["SearchServiceName"];
            var adminApiKey = configuration["SearchServiceAdminApiKey"];
            var queryApiKey = configuration["SearchServiceQueryApiKey"];

            // get management client
            var serviceClient = new SearchServiceClient(
                searchServiceName, 
                new SearchCredentials(adminApiKey));

            // Create index...
            var definition = new Index()
            {
                Name = "hotels",
                Fields = FieldBuilder.BuildForType<Hotel>()
            };

            serviceClient.Indexes.Create(definition);

            var indexClientForUpload = 
                serviceClient.Indexes.GetClient("hotels");

            // Upload documents...
            var hotels = new Hotel[]
            {
                new Hotel()
                { 
                    HotelId = "1", 
                    BaseRate = 199.0, 
                    Description = "Best hotel in town",
                    DescriptionFr = "Meilleur hôtel en ville",
                    HotelName = "Fancy Stay",
                    Category = "Luxury", 
                    Tags = new[] { "pool", "view", "wifi", "concierge" },
                    ParkingIncluded = false, 
                    SmokingAllowed = false,
                    LastRenovationDate = new DateTimeOffset(2010, 6, 27, 0, 0, 0, TimeSpan.Zero), 
                    Rating = 5, 
                    Location = GeographyPoint.Create(47.678581, -122.131577)
                },
                new Hotel()
                { 
                    HotelId = "2", 
                    BaseRate = 79.99,
                    Description = "Cheapest hotel in town",
                    DescriptionFr = "Hôtel le moins cher en ville",
                    HotelName = "Roach Motel",
                    Category = "Budget",
                    Tags = new[] { "motel", "budget" },
                    ParkingIncluded = true,
                    SmokingAllowed = true,
                    LastRenovationDate = new DateTimeOffset(1982, 4, 28, 0, 0, 0, TimeSpan.Zero),
                    Rating = 1,
                    Location = GeographyPoint.Create(49.678581, -122.131577)
                },
                new Hotel() 
                { 
                    HotelId = "3", 
                    BaseRate = 129.99,
                    Description = "Close to town hall and the river"
                }
            };

            var batch = IndexBatch.Upload(hotels);
            indexClientForUpload.Documents.Index(batch);

            var indexClientForQuery = new SearchIndexClient(
                searchServiceName, 
                "hotels", 
                new SearchCredentials(queryApiKey));

            // Search the entire index for the term 'budget' and return only the hotelName field
            var parameters = new SearchParameters()
            {
                Select = new[] { "hotelName" }
            };

            var results = indexClientForQuery.Documents
                                             .Search<Hotel>("budget", 
                                                            parameters);
            WriteDocuments(results);

            // Apply a filter to the index to find hotels cheaper than $150 per night,
            // and return the hotelId and description
            parameters = new SearchParameters()
            {
                Filter = "baseRate lt 150",
                Select = new[] { "hotelId", "description" }
            };

            results = indexClientForQuery.Documents
                                         .Search<Hotel>("*", 
                                                        parameters);
            WriteDocuments(results);

            // Search the entire index, order by a specific field (lastRenovationDate
            // in descending order, take the top two results, and show only hotelName and lastRenovationDate
            parameters = new SearchParameters()
            {
                OrderBy = new[] { "lastRenovationDate desc" },
                Select = new[] { "hotelName", "lastRenovationDate" },
                Top = 2
            };

            results = indexClientForQuery.Documents
                                         .Search<Hotel>("*", 
                                                        parameters);
            WriteDocuments(results);

            // Search the entire index for the term 'motel'
            parameters = new SearchParameters();
            results = indexClientForQuery.Documents.Search<Hotel>("motel", parameters);
            WriteDocuments(results);
        }

        private static void WriteDocuments(DocumentSearchResult<Hotel> searchResults)
        {
            foreach (SearchResult<Hotel> result in searchResults.Results)
            {
                Console.WriteLine(result.Document);
            }

            Console.WriteLine();
        }
    }
}
