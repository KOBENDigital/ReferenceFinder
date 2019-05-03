using System.Collections.Generic;
using Koben.ReferenceFinder.DTOs;

namespace Koben.ReferenceFinder.Services
{
	public interface IReferenceFinderDocumentTypesService
	{
		ICollection<DocumentTypeDTO> GetAllDocumentTypeAliases();
	}
}