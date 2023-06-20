using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace SpacialFacetedExamineSearch.Site.Controllers.Surface
{
    public class SearchController : SurfaceController
    {
        public SearchController(IUmbracoContextAccessor umbracoContextAccessor, 
            IUmbracoDatabaseFactory databaseFactory, ServiceContext services, 
            AppCaches appCaches, IProfilingLogger profilingLogger, 
            IPublishedUrlProvider publishedUrlProvider) 
                : base(umbracoContextAccessor, databaseFactory, services, appCaches, 
                      profilingLogger, publishedUrlProvider)
        {
        }
    }
}
