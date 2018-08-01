namespace NexGoal.Models.FlowTemplate
{
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Resources;

    public class CreateFlowTemplateModel
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.StringMaxLengthErrorMessage))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.TargetType))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public int TargetTypeId { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.StringMaxLengthErrorMessage))]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}