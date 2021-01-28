using CSRO.Client.Core.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface ILocationsService
    {
        string ConvertToAzureRegion(string name);
        Task<List<IdName>> GetLocations();
    }

    public class LocationsService : ILocationsService
    {
        private Dictionary<string, Region> _locDic = new Dictionary<string, Region>
        {
            { "Europe North", Region.EuropeNorth },
            { "Europe West", Region.EuropeWest },
            { "Asia South East", Region.AsiaSouthEast },
            { "Asia East", Region.AsiaEast },
            { "US Central", Region.USCentral},
            { "US East 2", Region.USEast2 },
            { "Switzerland North", Region.SwitzerlandNorth },
            { "Switzerland West", Region.SwitzerlandWest },

        };

        List<IdName> _list = null;

        public LocationsService()
        {

        }

        public string ConvertToAzureRegion(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return Region.Create(name).Name;
        }

        public async Task<List<IdName>> GetLocations()
        {
            if (_list != null)
                return _list;
            else
            {
                _list = new List<IdName>();
                foreach (var item in _locDic)
                {
                    _list.Add(new IdName { Id = item.Value.Name, Name = item.Key });
                }
                return _list;
            }
        }
    }
}
