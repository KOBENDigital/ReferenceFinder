using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace Koben.ReferenceFinder.DTOs
{
	public class SearchResult
	{
		public IEnumerable<SearchResultItem> References { get; set; }

		public SearchResult()
		{
			References = new List<SearchResultItem>();
		}

		public SearchResult(IEnumerable<IPublishedContent> references)
		{
			References = references.Select(r => new SearchResultItem
			{
				Id = r.Id,
				Name = r.Name,
				Url = r.Url
			}).ToList();
		}
	}
}
