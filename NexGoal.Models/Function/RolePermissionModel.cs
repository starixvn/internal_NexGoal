namespace NexGoal.Models.Function
{
    using System;
    using System.Xml.Serialization;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class RolePermissionModel
    {
        [XmlIgnore]
        public int RoleId { get; set; }

        public int FunctionId { get; set; }

        [XmlIgnore]
        public Policy Policy { get; set; }

        [XMLName("Policy")]
        public int XmlPolicy => (int)Policy;
    }
}