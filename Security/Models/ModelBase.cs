namespace Cfe.Security.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ModelBase : Util.IModelBase
    {
        public ModelBase()
        {
            this.Status = 0;
            this.Created = DateTime.Now;
            this.Modified = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Estado", Description = "Estado del registro.")]
        public int Status { get; set; }

        [Required, Display(Name = "Creado", Description = "Fecha de creación.."), DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }
        
        [Required, Display(Name = "Modificado", Description = "Fecha de modificación."), DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime Modified { get; set; }
    }
}