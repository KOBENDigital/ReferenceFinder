using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Umbraco.Core.Models;
using Umbraco.Web.PublishedCache.XmlPublishedCache;

namespace Koben.ReferenceFinder.Services
{
	public class ReferenceFinderService : IReferenceFinderService
	{
		private readonly IReferenceFinderContentService _ContentService;
		private readonly IReferenceFinderMediaService _MediaService;

		public ReferenceFinderService(IReferenceFinderContentService contentService, IReferenceFinderMediaService mediaService)
		{
			_ContentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
			_MediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
		}

		public IEnumerable<IPublishedContent> FindContentWithReferences(int contentId)
		{
			Dictionary<int, IPublishedContent> haveReferences = new Dictionary<int, IPublishedContent>();
			HashSet<int> contentSearched = new HashSet<int>();
			
			foreach (var content in _ContentService.TypedContentAtRoot())
			{
				FindContentWithReferencesToContent_Recursive(contentId, content, haveReferences, contentSearched);
			}

			return haveReferences.Values;
		}

		private void FindContentWithReferencesToContent_Recursive(int contentId, IPublishedContent content, Dictionary<int, IPublishedContent> haveReferences, HashSet<int> contentSearched)
		{
			if (contentSearched.Contains(content.Id))
			{
				return;
			}

			contentSearched.Add(content.Id);

			//loop through all properties with a value
			foreach (var property in content.Properties.Where(p => p.HasValue))
			{
				var type = property.GetType();
				var propertyTypeProp = type.GetField("PropertyType");

				//if we fail to determine the type of the property, skip and keep looping
				if (propertyTypeProp == null)
				{
					continue;
				}

				var propertyType = propertyTypeProp.GetValue(property);
				var propertyTypeType = propertyType.GetType();
				var propertyEditorAliasProp = propertyTypeType.GetProperty("PropertyEditorAlias");
				
				//if we fail to determine the alias of the property's editor, skip and keep looping
				if (propertyEditorAliasProp == null)
				{
					continue;
				}

				var propertyEditorAlias = propertyEditorAliasProp.GetValue(propertyType)?.ToString();

				Regex contentPickerPattern = new Regex(@"Umbraco\.ContentPicker", RegexOptions.Compiled);
				Regex mediaPickerPattern = new Regex(@"Umbraco\.MediaPicker", RegexOptions.Compiled);

				bool isContentPicker = contentPickerPattern.IsMatch(propertyEditorAlias ?? "");
				bool isMediaPicker = mediaPickerPattern.IsMatch(propertyEditorAlias ?? "");

				// we only care about content and media picker properties atm
				if (!isContentPicker && !isMediaPicker)
				{
					continue;
				}

				if(isContentPicker)
				{
					//try to parse the property value as an int. single ints are used for a single content/media item
					if (int.TryParse(property.Value.ToString(), out int propertyContentId))
					{
						if (contentId == propertyContentId && !haveReferences.ContainsKey(content.Id))
						{
							//this property contains a reference to the id we are searching for, therefore we store the id of the content indicating where the content is referenced
							haveReferences.Add(content.Id, content);
							break;
						}

						//this property does not contain a reference to the id we are searching for, therefore we need to search each property
						var innerContent = _ContentService.TypedContent(propertyContentId);
						FindContentWithReferencesToContent_Recursive(contentId, innerContent, haveReferences, contentSearched);
						continue;
					}

					//property value was not an int, therefore check if it is a collection of IPublishedContents. an array of IPublishedContents are used for multiple content/media items
					IPublishedContent[] publishedContentCollection = (property.Value as IEnumerable<IPublishedContent>)?.ToArray();

					if (publishedContentCollection != null && publishedContentCollection.Any())
					{
						//loop through each id and check if it matches the id we are searching for
						foreach (var publishedContent in publishedContentCollection)
						{
							if (publishedContent.Id == contentId && !haveReferences.ContainsKey(content.Id))
							{
								//this property contains a reference to the id we are searching for, therefore we store the id of the content indicating where the content is referenced
								haveReferences.Add(content.Id, content);
								break;
							}
						
							FindContentWithReferencesToContent_Recursive(contentId, publishedContent, haveReferences, contentSearched);
						}
					}

					//property value was not a collection of IPublishedContents, therefore check if it is a collection of ints. an array of ints are used for multiple content/media items
					int[] publishedContentIdCollection = (property.Value as IEnumerable<int>)?.ToArray();

					if (publishedContentIdCollection != null && publishedContentIdCollection.Any())
					{
						//loop through each id and check if it matches the id we are searching for
						foreach (var id in publishedContentIdCollection)
						{
							if (id == contentId && !haveReferences.ContainsKey(content.Id))
							{
								//this property contains a reference to the id we are searching for, therefore we store the id of the content indicating where the content is referenced
								haveReferences.Add(content.Id, content);
								break;
							}
						
							var listContent = _ContentService.TypedContent(propertyContentId);
							FindContentWithReferencesToContent_Recursive(contentId, listContent, haveReferences, contentSearched);
						}
					}
				}

				if(!isMediaPicker)
				{
					continue;
				}
				
				//try to parse the property value as an int. single ints are used for a single content/media item
				if (int.TryParse(property.Value.ToString(), out int propertyMediaId))
				{
					if (contentId != propertyMediaId || haveReferences.ContainsKey(content.Id))
					{
						continue;
					}
					
					//this property contains a reference to the id we are searching for, therefore we store the id of the content indicating where the content is referenced
					haveReferences.Add(content.Id, content);
				}

				//property value was not an int, therefore check if it is a collection of ints. an array of ints are used for multiple content/media items
				IPublishedContent[] publishedMediaCollection = (property.Value as IEnumerable<IPublishedContent>)?.ToArray();

				if (publishedMediaCollection != null && publishedMediaCollection.Any())
				{
					//loop through each id and check if it matches the id we are searching for
					foreach (var publishedMedia in publishedMediaCollection)
					{
						if (publishedMedia.Id != contentId || haveReferences.ContainsKey(content.Id))
						{
							continue;
						}
						
						//this property contains a reference to the id we are searching for, therefore we store the id of the content indicating where the content is referenced
						haveReferences.Add(content.Id, content);
					}
				}
			}

			foreach (var child in content.Children)
			{
				FindContentWithReferencesToContent_Recursive(contentId, child, haveReferences, contentSearched);
			}
		}

		public IEnumerable<IPublishedContent> FindContentWithReferences(string documentType)
		{
			Dictionary<int, IPublishedContent> haveReferences = new Dictionary<int, IPublishedContent>();
			HashSet<int> contentSearched = new HashSet<int>();
			
			foreach (var content in _ContentService.TypedContentAtRoot())
			{
				FindContentWithReferencesToContent_Recursive(documentType, content, haveReferences, contentSearched);
			}

			return haveReferences.Values;
		}

		private void FindContentWithReferencesToContent_Recursive(string documentType, IPublishedContent content, Dictionary<int, IPublishedContent> haveReferences, HashSet<int> contentSearched)
		{
			if (contentSearched.Contains(content.Id))
			{
				return;
			}

			contentSearched.Add(content.Id);

			if(content.DocumentTypeAlias == documentType && !haveReferences.ContainsKey(content.Id))
			{
				haveReferences.Add(content.Id, content);
			}

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

				Regex pattern = new Regex(@"Umbraco\.ContentPicker");

				if (!pattern.IsMatch(propertyEditorAlias ?? ""))
				{
					continue;
				}

				if (int.TryParse(property.Value.ToString(), out int propertyContentId))
				{
					if (contentSearched.Contains(propertyContentId))
					{
						continue;
					}

					var innerContent = _ContentService.TypedContent(propertyContentId);

					FindContentWithReferencesToContent_Recursive(documentType, innerContent, haveReferences, contentSearched);
					continue;
				}

				int[] publishedContentCollection = (property.Value as IEnumerable<int>)?.ToArray();

				if (publishedContentCollection != null && publishedContentCollection.Any())
				{
					foreach (var id in publishedContentCollection)
					{
						if (contentSearched.Contains(propertyContentId))
						{
							continue;
						}

						var listContent = _ContentService.TypedContent(id);
						
						FindContentWithReferencesToContent_Recursive(documentType, listContent, haveReferences, contentSearched);
					}
				}
			}

			foreach (var child in content.Children)
			{
				FindContentWithReferencesToContent_Recursive(documentType, child, haveReferences, contentSearched);
			}
		}
	}
}