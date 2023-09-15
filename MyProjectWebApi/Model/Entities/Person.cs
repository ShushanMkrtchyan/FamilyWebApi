using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectWebApi.Models.Entities
{
    public class Person:EntityBase
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int? Age {  get; set; }
        public string? Gender {  get; set; }

        public Type MemberType { get; set; }

        public  Family Family { get; set; }

        [ForeignKey(nameof(Family))]
        public int FamilyID { get; set; }
      
    }
}
