using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain.AzureDtos
{
    public partial class TagsDto
    {
        public List<TagValueDto> Value { get; set; }
    }

    public partial class TagValueDto
    {
        public string Id { get; set; }
        public string TagName { get; set; }
        public TagCountDto Count { get; set; }
        public List<TagDetailValueDto> Values { get; set; }
    }

    public partial class TagCountDto
    {
        public string Type { get; set; }
        //public TypeEnum Type { get; set; }
        public long Value { get; set; }
    }

    public partial class TagDetailValueDto
    {
        public string Id { get; set; }
        public string TagValue { get; set; }
        public TagCountDto Count { get; set; }
    }
}
