using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Koben.ReferenceFinder.Services
{
	public class ReferenceFinderMediaService : IReferenceFinderMediaService
	{
		private readonly UmbracoContext _UmbracoContext;
		private readonly UmbracoHelper _UmbracoHelper;

		public ReferenceFinderMediaService(UmbracoContext context)
		{
			_UmbracoContext = context ?? throw new ArgumentNullException(nameof(context));
			_UmbracoHelper = new UmbracoHelper(context);
		}

		public IEnumerable<IPublishedContent> TypedMediaAtRoot()
		{
			return _UmbracoHelper.TypedMediaAtRoot();
		}

		public IPublishedContent TypedMedia(Uri uri)
		{
			return _UmbracoContext.ContentCache.GetByRoute(uri.LocalPath);
		}
	}
}