namespace NexGoal.Models.PARecord
{
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Resources;

    public class EditPARecordByReviewerModel
    {
        public int PARecordId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.Grade))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string GradeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string Comment { get; set; }
    }
}