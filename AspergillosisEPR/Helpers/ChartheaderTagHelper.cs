using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Helpers
{
    public class ChartheaderTagHelper : TagHelper
    {
        public bool IsAnonymous { get; set; }
        public bool AnonymousOnly { get; set; }
        public string FullName { get; set; }
        public string RM2Number { get; set; }
        public string Identity { get; set; }
        public string ChartName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";           
            output.TagMode = TagMode.StartTagAndEndTag;
            if (IsAnonymous && AnonymousOnly)
            {
                output.Content.SetHtmlContent($@"{ChartName} for Patient with Identifier: {Identity}");
            } else
            {
                output.Content.SetHtmlContent($@"{ChartName} for {FullName} with Identifier: {RM2Number}");
            }
        }
    }
}
