using System.ComponentModel.DataAnnotations.Schema;

namespace MyProjectWebApi.Models.Entities
{
    public class Animal:EntityBase
    {
        public string Name { get; set; }  

        public int? Age {  get; set; }

        public string? Gender { get; set; }

        public  Family Family { get; set; }

        [ForeignKey(nameof(Family))]
        public int FamilyID {  get; set; }
    }
}
