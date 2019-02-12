using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Koben.ReferenceFinder.Services
{
	public interface IReferenceFinderMediaService
	{
		IEnumerable<IPublishedContent> TypedMediaAtRoot();
		IPublishedContent TypedMedia(Uri uri);
	}
}