using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace EasyData.Services
{
    public sealed class SubstringFilter : EasyFilter
    {
        public static string Class = "__substring";

        private SubstringFilterOptions filterOptions;

        public SubstringFilter(MetaData model): base(model) {}

        public override object Apply(MetaEntity entity, bool isLookup, object data)
        {
            if (filterOptions == null || string.IsNullOrWhiteSpace(filterOptions.FilterText))
            {
                return data;
            }

            return GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                   .Single(m => m.Name == "Apply"
                       && m.IsGenericMethodDefinition)
                   .MakeGenericMethod(entity.ClrType)
                   .Invoke(this, new object[] { entity, isLookup, data });
        }

        // Is callable by public Apply
        private IQueryable<T> Apply<T>(MetaEntity entity, bool isLookup, object data) where T: class
        {
            var query = (IQueryable<T>)data;
            return query.FullTextSearchQuery(filterOptions, GetFilterOptions(entity, isLookup));
        }

        public override async Task ReadFromJsonAsync(JsonReader reader, CancellationToken ct = default)
        {
            if (!await reader.ReadAsync(ct).ConfigureAwait(false)
               || reader.TokenType != JsonToken.StartObject)
            {
                throw new BadJsonFormatException(reader.Path);
            }

            filterOptions = new SubstringFilterOptions();
            while (await reader.ReadAsync(ct).ConfigureAwait(false)) 
            {
                if (reader.TokenType == JsonToken.PropertyName && "options".Equals(reader.Value))
                {
                    while (await reader.ReadAsync(ct).ConfigureAwait(false))
                    {
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            var propName = reader.Value.ToString();
                            switch (propName)
                            {
                                case "filterText":
                                    filterOptions.FilterText = await reader.ReadAsStringAsync(ct).ConfigureAwait(false);
                                    break;
                                case "matchCase":
                                    filterOptions.MatchCase = await reader.ReadAsBooleanAsync(ct).ConfigureAwait(false);
                                    break;
                                case "matchWholeWord":
                                    filterOptions.MatchWholeWord = await reader.ReadAsBooleanAsync(ct).ConfigureAwait(false);
                                    break;
                                case "relatedObjectsToFilter":
                                    var properties = new List<string>();
                                    while (await reader.ReadAsync(ct).ConfigureAwait(false))
                                    {
                                        if (reader.TokenType == JsonToken.EndArray)
                                        {
                                            break;
                                        }

                                        var property = await reader.ReadAsStringAsync(ct).ConfigureAwait(false);
                                        properties.Add(property);
                                    }
                                    filterOptions.RelatedObjectsToFilter = properties;
                                    break;
                                default:
                                    await reader.SkipAsync(ct).ConfigureAwait(false);
                                    break;
                            }
                        }
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
            }
        }

        private FullTextSearchOptions GetFilterOptions(MetaEntity entity, bool isLookup)
        {
            return new FullTextSearchOptions
            {
                Filter = (prop) => {

                    if (filterOptions.RelatedObjectsToFilter.)

                    var attr = entity?.FindAttribute(a => a.PropInfo == prop);
                    if (attr == null)
                        return false;

#pragma warning disable CS0618 // Type or member is obsolete
                    if (!attr.IsVisible || !attr.ShowOnView)
#pragma warning restore CS0618 // Type or member is obsolete
                        return false;

                    if (isLookup && !attr.ShowInLookup && !attr.IsPrimaryKey)
                        return false;

                    return true;
                },
                MatchCase = filterOptions.MatchCase.GetValueOrDefault(),
                MatchWholeWord = filterOptions.MatchWholeWord.GetValueOrDefault(),
                Depth = 1
            };
        }

        bool? IsRelatedObjectProperty(PropertyInfo propertyInfo) => filterOptions
            .RelatedObjectsToFilter?
            .Any(p => p.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase));
    }
}
