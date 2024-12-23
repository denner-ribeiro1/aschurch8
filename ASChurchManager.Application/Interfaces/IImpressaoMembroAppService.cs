using System.Collections.Generic;
using PdfSharpCore.Pdf;

namespace ASChurchManager.Application.Interfaces;

public interface IImpressaoMembroAppService
{
    PdfDocument GerarCarteirinha(IEnumerable<Domain.Entities.Carteirinha> carts,
                                 bool atualizaDtValidade = false);

    
    PdfDocument GerarFichaMembro(int membroId,
                                 bool imprimirCurso = false);
}
