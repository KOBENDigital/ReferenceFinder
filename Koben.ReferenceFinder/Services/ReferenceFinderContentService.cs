using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Koben.ReferenceFinder.Services
{
	public class ReferenceFinderContentService : IReferenceFinderContentService
	{
		private readonly UmbracoContext _UmbracoContext;
		private readonly UmbracoHelper _UmbracoHelper;

		public ReferenceFinderContentService(UmbracoContext context)
		{
			_UmbracoContext = context ?? throw new ArgumentNullException(nameof(context));
			_UmbracoHelper = new UmbracoHelper(context);
		}

		public IEnumerable<IPublishedContent> TypedContentAtRoot()
		{
			return _UmbracoHelper.TypedContentAtRoot();
		}

		public IPublishedContent TypedContent(Uri uri)
		{
			return _UmbracoContext.ContentCache.GetByRoute(uri.LocalPath);
		}

		public IPublishedContent TypedContent(int id)
		{
			return _UmbracoHelper.TypedContent(id);
		}
	}
}