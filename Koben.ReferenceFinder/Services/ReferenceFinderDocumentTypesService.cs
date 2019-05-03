using System;
using System.Collections.Generic;
using System.Linq;
using Koben.ReferenceFinder.DTOs;
using Umbraco.Core.Services;

namespace Koben.ReferenceFinder.Services
{
	public class ReferenceFinderDocumentTypesService : IReferenceFinderDocumentTypesService
	{
		private readonly ServiceContext _ServiceContext;

		public ReferenceFinderDocumentTypesService(ServiceContext context)
		{
			_ServiceContext = context ?? throw new ArgumentNullException(nameof(context));
		}

		public ICollection<DocumentTypeDTO> GetAllDocumentTypeAliases()
		{
			IContentTypeService contentTypeService = _ServiceContext.ContentTypeService;

			return contentTypeService.GetAllContentTypes()
				.Select(t => new DocumentTypeDTO
				{
					Id = t.Id,
					Name = t.Name,
					Alias = t.Alias
				})
				.ToArray();
		}
	}
}