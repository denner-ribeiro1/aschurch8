using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Application.AppServices
{
    public class PrinterAppService : IPrinterAppService
    {
        private readonly ITemplateAppService _templateAppService;
        private readonly IMembroAppService _membroAppService;
        private readonly ICartaAppService _cartaAppService;
        private long UsuarioId = 0;

        public PrinterAppService(ITemplateAppService templateAppService
                , IMembroAppService membroAppService
            , ICartaAppService cartaAppService)
        {
            _templateAppService = templateAppService;
            _membroAppService = membroAppService;
            _cartaAppService = cartaAppService;
        }

        public string GetHtmlToPrint(long templateId, string modelType, long modelId, long usuarioId)
        {
            this.UsuarioId = usuarioId;

            var template = _templateAppService.GetById(templateId, usuarioId);
            var model = GetModelByType(modelType, modelId);


            template.Conteudo = ReplaceTags(template, model);

            if (modelType == "Carta")
            {
                // Pega o membro da carta
                var membroId = (model as Carta).MembroId;
                var membro = GetModelByType("Membro", membroId);
                template.Conteudo = ReplaceTags(template, membro);
            }

            return template.Conteudo;
        }

        private object GetModelByType(string modelType, long modelId)
        {
            var model = new Object();

            switch (modelType)
            {
                case "Membro":
                    model = _membroAppService.GetById(modelId, this.UsuarioId);
                    break;

                case "Carta":
                    model = _cartaAppService.GetById(modelId, this.UsuarioId);
                    break;
            }
            return model;
        }

        private string ReplaceTagByModel(Template template, object model, string tag)
        {
            var conteudo = template.Conteudo;


            var value = GetPropValue(model, tag.Replace("[", "").Replace("]", ""));
            conteudo = conteudo.Replace(tag, value != null ? value.ToString() : "");

            return conteudo;
        }

        private string ReplaceTags(Template template, object model)
        {
            var tagsComuns = new List<TemplateTag>()
            {
                new TemplateTag("NomeCarta", "Nome da Carta", "[NomeCarta]"),
                new TemplateTag("DataAtual", "Data Atual", "[DataAtual]"),
                new TemplateTag("DataAtualExtenso", "Data Atual Extenso", "[DataAtualExtenso]")
            };

            var conteudo = template.Conteudo;
            foreach (var tag in template.TagsDisponiveis)
            {
                if (conteudo.Contains(tag.Tag))
                {
                    if (tagsComuns.Count(a => a.Tag == tag.Tag) > 0)
                    {
                        string value = string.Empty;
                        switch (tag.Tag)
                        {
                            case "[DataAtual]":
                                value = DateTime.Now.Date.ToString("dd/MM/yyyy");
                                break;
                            case "[DataAtualExtenso]":
                                var dataAtual = DateTime.Now.Date;
                                var culture = new System.Globalization.CultureInfo("pt-BR");
                                var dtfi = culture.DateTimeFormat;
                                var dia = dataAtual.Day;
                                var ano = dataAtual.Year;
                                var mes = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(dataAtual.Month));
                                value = $"{dia} de {mes} de {ano}";
                                break;
                            case "[NomeCarta]":
                                value = template.Nome;
                                break;
                        }
                        conteudo = conteudo.Replace(tag.Tag, value);
                    }
                    else
                    {
                        conteudo = ReplaceTagByModel(template, model, tag.Tag);
                    }
                }
                template.Conteudo = conteudo;
            }


            return conteudo;
        }

        private object GetPropValue(object src, string propName)
        {
            if (src == null || string.IsNullOrWhiteSpace(propName))
            {
                return "";
            }

            var value = "[" + propName + "]";
            try
            {
                var v = src.GetType().GetProperty(propName).GetValue(src, null);

                if (v is DateTimeOffset)
                {
                    value = ((DateTimeOffset)v).ToString("dd/MM/yyyy");
                }
                else
                {
                    value = v != null ? v.ToString() : "";
                }
            }
            catch (Exception)
            {
                value = "Erro ao converter a tag " + propName;
            }
            return value;
        }
    }
}