using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StoneHouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.TagHelpers
{
    //in this class custom tag helper will be created

    //the 'PageLinkTagHelper' will be assigned inside a div tag inside a view
    //and add an attribute of page-model which will be the 'PaginInfo' in the view
    [HtmlTargetElement("div", Attributes ="page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        //a factory for creating 'IUrlHelper' instance
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        //ViewContext provides access  to hhtp context, http request and others
        [ViewContext]
        //HtmlAttributeNotBound means this attribute isnt the one you intent to set via tag helper attribute in html
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        //object of PagingInfo
        public PagingInfo PageModel { get; set; }

        //create propertie that will be set in the Apopointments controller and used in the Index View as well
        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //fetch the url inside the url helper
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            //target element is div
            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i <= PageModel.totalPage;i++)
            {
                TagBuilder tag = new TagBuilder("a");
                //repalce the colon with 'i' in the url which is the pagination numbers
                string url = PageModel.urlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;

                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage? PageClassSelected : PageClassNormal);

                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            //append everything inside the div tag
            //this is the parameter of this method
            output.Content.AppendHtml(result.InnerHtml);
        }


    }
}
