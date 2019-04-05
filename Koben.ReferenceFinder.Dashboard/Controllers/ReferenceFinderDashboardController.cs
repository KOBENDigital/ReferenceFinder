using System;
using System.Web.Http;
using Koben.ReferenceFinder.DTOs;
using Koben.ReferenceFinder.Services;
using Umbraco.Web;
using Umbraco.Web.Editors;

namespace Koben.ReferenceFinder.Dashboard.Controllers
{
	[Umbraco.Web.Mvc.PluginController("ReferenceFinder")]
	public class ReferenceFinderDashboardController : UmbracoAuthorizedJsonController
	{
		private readonly IReferenceFinderService _ReferenceFinderService;
		private readonly IReferenceFinderContentService _ReferenceFinderContentService;
		private readonly IReferenceFinderMediaService _ReferenceFinderMediaService;

		public ReferenceFinderDashboardController()
		{
			_ReferenceFinderContentService = new ReferenceFinderContentService(UmbracoContext.Current);
			_ReferenceFinderMediaService = new ReferenceFinderMediaService(UmbracoContext.Current);
			_ReferenceFinderService = new ReferenceFinderService(_ReferenceFinderContentService);
		}

		[HttpPost]
		public IHttpActionResult FindContentReferencesByUrl(SearchByUrlRequest request)
		{
			Uri uri;

			if (string.IsNullOrWhiteSpace(request?.TargetUrl) || !Uri.TryCreate(request.TargetUrl, UriKind.Absolute, out uri))
			{
				return BadRequest($"A valid {nameof(request.TargetUrl)} must be provided. Ensure that the Url you are using is absolute");
			}

			var content = _ReferenceFinderContentService.TypedContent(uri);
			var references = _ReferenceFinderService.FindContentWithReferences(content.Id);

			return Json(new SearchResult(references));
		}

		[HttpPost]
		public IHttpActionResult FindMediaReferencesByUrl(SearchByUrlRequest request)
		{
			Uri uri;

			if (string.IsNullOrWhiteSpace(request?.TargetUrl) || !Uri.TryCreate(request.TargetUrl, UriKind.Absolute, out uri))
			{
				return BadRequest($"A valid {nameof(request.TargetUrl)} must be provided. Ensure that the Url you are using is absolute");
			}

			var media = _ReferenceFinderMediaService.TypedMedia(uri);
			var references = _ReferenceFinderService.FindContentWithReferences(media.Id);

			return Json(new SearchResult(references));
		}
		
		[HttpPost]
		public IHttpActionResult FindContentReferencesById(SearchByIdRequest request)
		{
			if (request == null)
			{
				return BadRequest($"A valid {nameof(request)} must be provided.");
			}

			var references = _ReferenceFinderService.FindContentWithReferences(request.TargetId);

			return Json(new SearchResult(references));
		}

		[HttpPost]
		public IHttpActionResult FindMediaReferencesById(SearchByIdRequest request)
		{
			if (request == null)
			{
				return BadRequest($"A valid {nameof(request)} must be provided.");
			}

			var references = _ReferenceFinderService.FindContentWithReferences(request.TargetId);

			return Json(new SearchResult(references));
		}
	}
}
