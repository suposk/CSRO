using CSRO.Server.Infrastructure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = CSRO.Server.Entities.Entity;
using AdoModels = CSRO.Common.AdoServices.Models;
using CSRO.Common.AdoServices.Models;

namespace CSRO.Server.Ado.Api.Services
{
    public class AdoPropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
          new(StringComparer.OrdinalIgnoreCase)
          {
               { nameof(ProjectAdo.Id), new PropertyMappingValue(new List<string>() { nameof(ProjectAdo.Id) } )},
               { nameof(ProjectAdo.Name), new PropertyMappingValue(new List<string>() { nameof(ProjectAdo.Name) } )},
               { nameof(ProjectAdo.Organization), new PropertyMappingValue(new List<string>() { nameof(ProjectAdo.Organization) } )},
               { nameof(ProjectAdo.IsDeleted), new PropertyMappingValue(new List<string>() { nameof(ProjectAdo.IsDeleted) } )},
               { nameof(ProjectAdo.CreatedAt), new PropertyMappingValue(new List<string>() { nameof(ProjectAdo.CreatedAt) } )},
          };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public AdoPropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<ProjectAdo, Entity.AdoProject>(_authorPropertyMapping));
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            Dictionary<string, PropertyMappingValue> propertyMapping = null;
            try
            {
                propertyMapping = GetPropertyMapping<TSource, TDestination>();
            }
            catch
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
           <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            //throw new Exception($"Cannot find exact property mapping instance " +
            //    $"for <{typeof(TSource)},{typeof(TDestination)}");

            return null;
        }

    }
}
