namespace NexGoal.ViewModels.Discipline
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Resources;

    public class EditDisciplineViewModel
    {
        public int DisciplineId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.EffectiveDate))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public DateTime EffectiveDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string Content { get; set; }
    }
}