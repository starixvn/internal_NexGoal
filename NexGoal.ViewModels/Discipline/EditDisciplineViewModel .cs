namespace NexGoal.ViewModels.Discipline
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Resources;

    public class EditDisciplineViewModel
    {
        public int DisciplineId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EffectiveDateRequired))]
        public DateTime EffectiveDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ContentRequired))]
        public string Content { get; set; }
    }
}