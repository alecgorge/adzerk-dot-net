using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Jil;
using StackExchange.Adzerk.Models;
using System.Net.Http;

namespace StackExchange.Adzerk
{
    public interface IClient
    {
        string CreateReport(IReport report);
        ReportResult PollForResult(string id);
        ReportResult RunReport(IReport report);

        IEnumerable<AdType> ListAdTypes();
        IEnumerable<Advertiser> ListAdvertisers();
        IEnumerable<Campaign> ListCampaigns();
        IEnumerable<Channel> ListChannels();
        IEnumerable<Creative> ListAdvertiserCreatives(long advertiserId);
        IEnumerable<Flight> ListFlights();
        IEnumerable<Flight> ListCampaignFlights(long campaignId);
        IEnumerable<Login> ListLogins();
        IEnumerable<Priority> ListPriorities();
        IEnumerable<Publisher> ListPublishers();
        IEnumerable<Site> ListSites();
        IEnumerable<Zone> ListZones();

        Campaign GetCampaign(long campaignId);
        Campaign UpdateCampaign(Campaign campaign);

        Flight GetFlight(long flightId);
        Flight UpdateFlight(Flight flight);
    }

    public class Client : IClient
    {
        public const int CURRENT_VERSION = 1;
        public const int POLL_DELAY = 1000;
        public const int LIST_PAGE_SIZE = 1000;

        private readonly string _apiKey;
        private readonly HttpClient _client;

        public Client(string apiKey, HttpClientHandler customHandler = null)
        {
            this._apiKey = apiKey;
            
            if(customHandler != null)
            {
                _client = new HttpClient(customHandler);
            }
            else
            {
                _client = new HttpClient();
            }
        }

        private string ApiRoute(string path, int? page)
        {
            if (page.HasValue)
                return $"http://api.adzerk.net/v{CURRENT_VERSION}/{path}?page={page}&pageSize={LIST_PAGE_SIZE}";
            else
                return $"http://api.adzerk.net/v{CURRENT_VERSION}/{path}";
        }

        private HttpResponseMessage ExecuteApiRequest(string route, int? page = null, HttpMethod method = null, string bodyKey = null, string bodyValue = null)
        {
            if (method == null)
            {
                method = HttpMethod.Get;
            }

            var request = new HttpRequestMessage(method, ApiRoute(route, page));

            request.Headers.Add("X-Adzerk-ApiKey", _apiKey);

            if (bodyKey != null)
            {
                request.Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>(bodyKey, bodyValue)
                });
            }

            var response = _client.SendAsync(request).Result;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var message = $"Adzerk API error ({response.StatusCode}): {response.Content.ReadAsStringAsync().Result}";
                throw new AdzerkApiException(message, new {request, response});
            }

            return response;
        }

        public string CreateReport(IReport report)
        {
            var response = ExecuteApiRequest("report/queue", null, HttpMethod.Post, "criteria", ReportSerializer.SerializeReport(report));

            try
            {
                var result = JSON.DeserializeDynamic(response.Content.ReadAsStringAsync().Result);
                return result.Id;
            }
            catch (Exception)
            {
                throw new AdzerkApiException("Report result does not contain report Id.", new {response.RequestMessage, response});
            }
        }

        public class ReportResultWrapper
        {
            public int Status;
            public string Message;
            public ReportResult Result;
        }

        private ReportResultWrapper pollForResult(string id)
        {
            var response = ExecuteApiRequest("report/queue/" + id);

            return JSON.Deserialize<ReportResultWrapper>(response.Content.ReadAsStringAsync().Result);
        }

        public ReportResult PollForResult(string id)
        {
            var res = pollForResult(id);

            if (res.Status == 1)
            {
                return null;
            }

            if (res.Status == 2)
            {
                return res.Result;
            }

            var message = $"Adzerk API error: {res.Message}";
            throw new AdzerkApiException(message, new {response = res});
        }

        public ReportResult RunReport(IReport report)
        {
            var id = CreateReport(report);

            var res = pollForResult(id);

            while (res.Status == 1)
            {
                Task.Delay(POLL_DELAY);

                res = pollForResult(id);
            }

            if (res.Status == 2)
            {
                return res.Result;
            }

            var message = $"Adzerk API error: {res.Message}";
            throw new AdzerkApiException(message, new {response = res});
        }

        private IEnumerable<T> List<T>(string resource)
        {
            int totalPages = 1;
            IEnumerable<T>[] totalItems;

            // Retrieve first page of information and setup for later stages.
            {
                var response = ExecuteApiRequest(resource, page: 1);

                try
                {
                    var result = JSON.Deserialize<ResultWrapper<T>>(response.Content.ReadAsStringAsync().Result);

                    totalPages = result.totalPages;
                    totalItems = new IEnumerable<T>[totalPages];
                    totalItems[0] = result.items;
                }
                catch (Exception ex)
                {
                    var message = $"Adzerk client error deserializing \"{resource}\" at page 0";
                    throw new AdzerkApiException(message, ex, new { response.RequestMessage, response });
                }
            }

            // Retrieve second and later pages of information.
            {
                for (var i = 1; i < totalPages; i++)
                {
                    var currentResponse = ExecuteApiRequest(resource, page: i + 1);

                    try
                    {
                        var currentResult = JSON.Deserialize<ResultWrapper<T>>(currentResponse.Content.ReadAsStringAsync().Result);
                        totalItems[i] = currentResult.items;

                        if (currentResult.totalPages > totalPages)
                        {
                            totalPages = currentResult.totalPages;
                            Array.Resize(ref totalItems, totalPages);
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = $"Adzerk client error deserializing \"{resource}\" at page {i}";
                        throw new AdzerkApiException(message, ex, new { currentResponse.RequestMessage, currentResponse });
                    }
                }
            }

            return totalItems.SelectMany(i => i);
        }

        public IEnumerable<AdType> ListAdTypes()
        {
            return List<AdType>("adtypes");
        }

        public IEnumerable<Advertiser> ListAdvertisers()
        {
            return List<Advertiser>("advertiser");
        }

        public IEnumerable<Creative> ListAdvertiserCreatives(long advertiserId)
        {
            var resource = $"advertiser/{advertiserId}/creatives";
            var creatives = List<Creative>(resource);
            return creatives;
        }

        public IEnumerable<Campaign> ListCampaigns()
        {
            var campaigns = List<CampaignDTO>("campaign");
            return campaigns.Select(c => c.ToCampaign());
        }

        public IEnumerable<Channel> ListChannels()
        {
            var channels = List<ChannelDTO>("channel");
            return channels.Select(c => c.ToChannel());
        }

        public IEnumerable<Flight> ListFlights()
        {
            var flights = List<FlightDTO>("flight");
            return flights.Select(f => f.ToFlight());
        }

        public IEnumerable<Flight> ListCampaignFlights(long campaignId)
        {
            var resource = $"campaign/{campaignId}/flight";
            var flights = List<FlightDTO>(resource);
            return flights.Select(f => f.ToFlight());
        }

        public IEnumerable<Login> ListLogins()
        {
            return List<Login>("login");
        }

        public IEnumerable<Priority> ListPriorities()
        {
            var priorities = List<PriorityDTO>("priority");
            return priorities.Select(p => p.ToPriority());
        }

        public IEnumerable<Publisher> ListPublishers()
        {
            var publishers = List<PublisherDTO>("publisher");
            return publishers.Select(p => p.ToPublisher());
        }

        public IEnumerable<Site> ListSites()
        {
            return List<Site>("site");
        }

        public IEnumerable<Zone> ListZones()
        {
            return List<Zone>("zone");
        }

        private string ResourceUrl(string resource, long id)
        {
            return $"{resource}/{id}";
        }

        private T Get<T>(string resource, long id)
        {
            var response = ExecuteApiRequest(ResourceUrl(resource, id));

            try
            {
                var result = JSON.Deserialize<T>(response.Content.ReadAsStringAsync().Result);
                return result;
            }
            catch (Exception ex)
            {
                var message = $"Adzerk client error deserializing \"{resource}\"";
                throw new AdzerkApiException(message, ex, new {response.RequestMessage, response});
            }
        }

        public Campaign GetCampaign(long campaignId)
        {
            var campaign = Get<CampaignDTO>("campaign", campaignId);
            return campaign.ToCampaign();
        }

        public Flight GetFlight(long flightId)
        {
            var flight = Get<FlightDTO>("flight", flightId);
            return flight.ToFlight();
        }

        private T Update<T>(string resource, long id, T dto)
        {
            var response = ExecuteApiRequest(ResourceUrl(resource, id), null, HttpMethod.Put, resource, JSON.Serialize(dto));

            try
            {
                var result = JSON.Deserialize<T>(response.Content.ReadAsStringAsync().Result);
                return result;
            }
            catch (Exception ex)
            {
                var message = $"Adzerk client error deserializing \"{resource}\"";
                throw new AdzerkApiException(message, ex, new {response.RequestMessage, response});
            }
        }

        public Campaign UpdateCampaign(Campaign campaign)
        {
            var dto = campaign.ToDTO();
            var updated = Update("campaign", campaign.Id, dto);
            return updated.ToCampaign();
        }

        public Flight UpdateFlight(Flight flight)
        {
            var dto = flight.ToDTO();
            var updated = Update("flight", flight.Id, dto);
            return updated.ToFlight();
        }
    }
}