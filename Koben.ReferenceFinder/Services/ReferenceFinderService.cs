using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Umbraco.Core.Models;
using Umbraco.Web.PublishedCache.XmlPublishedCache;

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
			foreach (var property in content.Properties.Where(p => p.HasValue))
			{
				var type = property.GetType();
				var propertyTypeProp = type.GetField("PropertyType");

				if (propertyTypeProp == null)
				{
					continue;
				}

				var propertyType = propertyTypeProp.GetValue(property);
				var propertyTypeType = propertyType.GetType();
				var propertyEditorAliasProp = propertyTypeType.GetProperty("PropertyEditorAlias");

				if (propertyEditorAliasProp == null)
				{
					continue;
				}

				var propertyEditorAlias = propertyEditorAliasProp.GetValue(propertyType)?.ToString();

				Regex pattern = new Regex(@"Umbraco\.ContentPicker|Umbraco\.MediaPicker");

				if (!pattern.IsMatch(propertyEditorAlias ?? ""))
				{
					continue;
				}

				if (int.TryParse(property.Value.ToString(), out int propertyContentId))
				{
					if (contentId == propertyContentId && !haveReferences.ContainsKey(content.Id))
					{
						haveReferences.Add(content.Id, content);
						break;
					}

					var innerContent = _ContentService.TypedContent(propertyContentId);
					FindContentWithReferencesToContent_Recursive(contentId, innerContent, haveReferences);
					continue;
				}

				int[] publishedContentCollection = (property.Value as IEnumerable<int>)?.ToArray();

				if (publishedContentCollection != null && publishedContentCollection.Any())
				{
					foreach (var id in publishedContentCollection)
					{
						if (id == contentId && !haveReferences.ContainsKey(content.Id))
						{
							haveReferences.Add(content.Id, content);
							break;
						}
						
						var listContent = _ContentService.TypedContent(propertyContentId);
						FindContentWithReferencesToContent_Recursive(contentId, listContent, haveReferences);
					}
				}

				if (publishedContentCollection != null && publishedContentCollection.Any())
				{
				}
			}

			foreach (var child in content.Children)
			{
				FindContentWithReferencesToContent_Recursive(contentId, child, haveReferences);
			}
		}
	}
}