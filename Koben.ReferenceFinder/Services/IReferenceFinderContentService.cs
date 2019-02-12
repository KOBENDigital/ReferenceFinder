using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Koben.ReferenceFinder.Services
{
	public interface IReferenceFinderContentService
	{
		IEnumerable<IPublishedContent> TypedContentAtRoot();
		IPublishedContent TypedContent(Uri uri);
	}
}
