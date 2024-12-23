using ASChurchManager.Domain.Types;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class ArquivoMembroVM
    {
        public bool IsDelete { get; set; }

        public bool IsCurso { get; set; }

        public bool IsSave { get; set; }

        public bool IsReadOnly { get; set; }

        public SimNao Cadastrado { get; set; }

        public long? Id { get; set; }

        public int? MembroId { get; set; }

        [Display(Name = "Cursos Cadastrados")]
        public long? CursoId { get; set; }

        public IEnumerable<SelectListItem> SelectCursos { get; set; }

        [Display(Name = "Nome do Curso")]
        public string NomeCurso { get; set; }

        [Display(Name = "Local")]
        public string Local { get; set; }

        [Display(Name = "Data Início"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataInicioCurso { get; set; }

        [Display(Name = "Data Encerramento"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataEncerramentoCurso { get; set; }

        [Display(Name = "Carga Horária (Em Horas)")]
        public int? CargaHoraria { get; set; }

        [DataType(DataType.Upload)]
        public string Arquivo { get; set; }

        [DataType(DataType.Upload)]
        public string ArquivoCurso { get; set; }

        [Display(Name = "Descrição")]
        public string DescricaoArquivo { get; set; }

        public string NomeOriginal { get; set; }

        public string NomeArmazenado { get; set; }

        public long Tamanho { get; set; }

        public Stream ArquivoUpload { get; set; }

        public string MimeType { get; set; }

        public int Index { get; set; }
    }
}