using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Koben.ReferenceFinder.Services
{
	public class ReferenceFinderService : IReferenceFinderService
	{
		private readonly IReferenceFinderContentService _ContentService;

		public ReferenceFinderService(IReferenceFinderContentService contentService)
		{
			_ContentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
		}

		public IEnumerable<IPublishedContent> FindContentWithReferences(int contentId)
		{
			Dictionary<int, IPublishedContent> haveReferences = new Dictionary<int, IPublishedContent>();
			
			foreach (var content in _ContentService.TypedContentAtRoot())
			{
				FindContentWithReferencesToContent_Recursive(contentId, content, haveReferences);
			}

			return haveReferences.Values;
		}

		private void FindContentWithReferencesToContent_Recursive(int contentId, IPublishedContent content, Dictionary<int, IPublishedContent> haveReferences)
		{
			foreach (var property in content.Properties)
			{
				IPublishedContent publishedContent = property.Value as IPublishedContent;

				if (publishedContent != null)
				{
					if (publishedContent.Id == contentId && !haveReferences.ContainsKey(content.Id))
					{
						haveReferences.Add(content.Id, content);
						break;
					}

					FindContentWithReferencesToContent_Recursive(contentId, publishedContent, haveReferences);
					continue;
				}

				IEnumerable<IPublishedContent> publishedContentCollection = property.Value as IEnumerable<IPublishedContent>;

				if (publishedContentCollection != null && publishedContentCollection.Any())
				{
					foreach (var listContent in publishedContentCollection)
					{
						if (listContent.Id == contentId && !haveReferences.ContainsKey(content.Id))
						{
							haveReferences.Add(content.Id, content);
							break;
						}

						FindContentWithReferencesToContent_Recursive(contentId, listContent, haveReferences);
						continue;
					}
				}
			}

			foreach (var child in content.Children)
			{
				FindContentWithReferencesToContent_Recursive(contentId, child, haveReferences);
			}
		}
	}
}