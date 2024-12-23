using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;

namespace ASChurchManager.Application.AppServices
{
    public class TemplateAppService : BaseAppService<Template>, ITemplateAppService
    {
        private readonly ITemplateRepository _templateService;

        public TemplateAppService(ITemplateRepository templateService)
            : base(templateService)
        {
            _templateService = templateService;
        }

    }
}