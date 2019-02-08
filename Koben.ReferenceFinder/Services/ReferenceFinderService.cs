using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Koben.ReferenceFinder.Services
{
	public class ReferenceFinderService : IReferenceFinderService
	{
		private readonly IContentService _ContentService;
		private readonly UmbracoHelper _Umbraco;

		public ReferenceFinderService(IContentService contentService, UmbracoHelper umbraco)
		{
			_ContentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
			_Umbraco = umbraco ?? throw new ArgumentNullException(nameof(umbraco));
		}

		public IEnumerable<IPublishedContent> FindContentWithReferencesToContent(int contentId)
		{
			List<IPublishedContent> haveReferences = new List<IPublishedContent>();
			
			foreach (var content in _Umbraco.TypedContentAtRoot())
			{
				FindContentWithReferencesToContent_Recursive(contentId, content, haveReferences);
			}

			return haveReferences;
		}

		public IEnumerable<IPublishedContent> FindContentWithReferencesToMedia(int mediaId)
		{
			List<IPublishedContent> haveReferences = new List<IPublishedContent>();

			foreach (var content in _Umbraco.TypedContentAtRoot())
			{
				FindContentWithReferencesToMedia_Recursive(mediaId, content, haveReferences);
			}

			return haveReferences;
		}

		private void FindContentWithReferencesToContent_Recursive(int contentId, IPublishedContent content, List<IPublishedContent> haveReferences)
		{
			foreach (var property in content.Properties)
			{
				IPublishedContent publishedContent = property.Value as IPublishedContent;

				if (publishedContent != null)
				{
					if (publishedContent.Id == contentId)
					{
						haveReferences.Add(content);
						return;
					}

					FindContentWithReferencesToContent_Recursive(contentId, publishedContent, haveReferences);
					continue;
				}

				IEnumerable<IPublishedContent> publishedContentCollection = property.Value as IEnumerable<IPublishedContent>;

				if (publishedContentCollection != null && publishedContentCollection.Any())
				{
					foreach (var listContent in publishedContentCollection)
					{
						if (listContent.Id == contentId)
						{
							haveReferences.Add(content);
							return;
						}

						FindContentWithReferencesToContent_Recursive(contentId, listContent, haveReferences);
						continue;
					}
				}
			}
		}

		private void FindContentWithReferencesToMedia_Recursive(int mediaId, IPublishedContent content, List<IPublishedContent> haveReferences)
		{
			foreach (var property in content.Properties)
			{
				IMedia media = property.Value as IMedia;

				if (media != null && media.Id == mediaId)
				{
					haveReferences.Add(content);
					continue;
				}

				IPublishedContent publishedContent = property.Value as IPublishedContent;

				if (publishedContent != null)
				{
					FindContentWithReferencesToMedia_Recursive(mediaId, publishedContent, haveReferences);
					continue;
				}

				IEnumerable<IPublishedContent> publishedContentCollection = property.Value as IEnumerable<IPublishedContent>;

				if (publishedContentCollection != null && publishedContentCollection.Any())
				{
					foreach (var listContent in publishedContentCollection)
					{
						FindContentWithReferencesToMedia_Recursive(mediaId, listContent, haveReferences);
						continue;
					}
				}
			}
		}
	}
}