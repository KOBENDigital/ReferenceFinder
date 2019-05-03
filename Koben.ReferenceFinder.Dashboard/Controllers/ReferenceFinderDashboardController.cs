using System;
using System.Web.Http;
using Koben.ReferenceFinder.DTOs;
using Koben.ReferenceFinder.Services;
using Umbraco.Core;
using Umbraco.Core.Models;
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
		private readonly IReferenceFinderDocumentTypesService _ReferenceFinderDocumentTypesService;

		public ReferenceFinderDashboardController()
		{
			_ReferenceFinderDocumentTypesService = new ReferenceFinderDocumentTypesService(ApplicationContext.Current.Services);;
			_ReferenceFinderMediaService = new ReferenceFinderMediaService(UmbracoContext.Current);
			_ReferenceFinderContentService = new ReferenceFinderContentService(UmbracoContext.Current);
			_ReferenceFinderService = new ReferenceFinderService(_ReferenceFinderContentService, _ReferenceFinderMediaService);
		}

		[HttpPost]
		public IHttpActionResult FindContentReferencesByUrl(SearchByUrlRequest request)
		{
			if (string.IsNullOrWhiteSpace(request?.TargetUrl) || !Uri.TryCreate(request.TargetUrl, UriKind.Absolute, out Uri uri))
			{
				return BadRequest($"A valid {nameof(request.TargetUrl)} must be provided. Ensure that the Url you are using is absolute");
			}

			IPublishedContent content = _ReferenceFinderContentService.TypedContent(uri);
			var references = _ReferenceFinderService.FindContentWithReferences(content.Id);

			return Json(new SearchResult(references));
		}

		[HttpPost]
		public IHttpActionResult FindMediaReferencesByUrl(SearchByUrlRequest request)
		{
			if (string.IsNullOrWhiteSpace(request?.TargetUrl) || !Uri.TryCreate(request.TargetUrl, UriKind.Absolute, out Uri uri))
			{
				return BadRequest($"A valid {nameof(request.TargetUrl)} must be provided. Ensure that the Url you are using is absolute");
			}
			
			IPublishedContent media = _ReferenceFinderMediaService.TypedMedia(uri);
			var references = _ReferenceFinderService.FindContentWithReferences(media.Id);

			return Json(new SearchResult(references));
		}
		
		[HttpPost]
		public IHttpActionResult FindContentReferencesById(SearchByIdRequest request)
		{
			if (request == null || request.TargetId < 0)
			{
				return BadRequest($"A valid {nameof(request)} must be provided.");
			}

			var references = _ReferenceFinderService.FindContentWithReferences(request.TargetId);

			return Json(new SearchResult(references));
		}

		[HttpPost]
		public IHttpActionResult FindMediaReferencesById(SearchByIdRequest request)
		{
			if (request == null || request.TargetId < 0)
			{
				return BadRequest($"A valid {nameof(request)} must be provided.");
			}

			var references = _ReferenceFinderService.FindContentWithReferences(request.TargetId);

			return Json(new SearchResult(references));
		}
		
		[HttpPost]
		public IHttpActionResult FindContentReferencesByDocumentType(SearchByDocumentTypeRequest request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.TargetDocumentType))
			{
				return BadRequest($"A valid {nameof(request)} must be provided.");
			}

			var references = _ReferenceFinderService.FindContentWithReferences(request.TargetDocumentType);

			return Json(new SearchResult(references));
		}

		[HttpGet]
		public IHttpActionResult GetDocumentTypeAliases()
		{
			var aliases = _ReferenceFinderDocumentTypesService.GetAllDocumentTypeAliases();

			return Json(aliases);
		}
	}
}
