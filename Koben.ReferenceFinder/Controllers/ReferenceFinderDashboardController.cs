using System;
using System.Web.Http;
using Koben.ReferenceFinder.DTOs;
using Koben.ReferenceFinder.Services;
using Umbraco.Core;
using Umbraco.Web.Editors;

namespace Koben.ReferenceFinder.Controllers
{
	[Umbraco.Web.Mvc.PluginController("reference-finder")]
	public class ReferenceFinderDashboardController : UmbracoAuthorizedJsonController
	{
		private readonly IReferenceFinderService _ReferenceFinderService;

		public ReferenceFinderDashboardController()
		{
			_ReferenceFinderService = new ReferenceFinderService(Services.ContentService, Umbraco);
		}

		[HttpPost]
		public IHttpActionResult FindContentReferences(string searchText)
		{
			var content = Umbraco.TypedContent(new StringUdi(new Uri(searchText)));
			var references = _ReferenceFinderService.FindContentWithReferencesToContent(content.Id);

			return Json(new SearchResult(references));
		}

		[HttpPost]
		public IHttpActionResult FindMediaReferences(string searchText)
		{
			var media = Umbraco.TypedMedia(new StringUdi(new Uri(searchText)));
			var references = _ReferenceFinderService.FindContentWithReferencesToMedia(media.Id);

			return Json(new SearchResult(references));
		}
	}
}
