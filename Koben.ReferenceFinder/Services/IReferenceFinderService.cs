using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Koben.ReferenceFinder.Services
{
	public interface IReferenceFinderService
	{
		IEnumerable<IPublishedContent> FindContentWithReferencesToContent(int contentId);
		IEnumerable<IPublishedContent> FindContentWithReferencesToMedia(int mediaId);
	}
}