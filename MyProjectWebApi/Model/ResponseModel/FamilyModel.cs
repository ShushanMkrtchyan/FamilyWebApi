using MyProjectWebApi.Models.Entities;

namespace MyProjectWebApi.Model.ResponseModel
{
    public class FamilyModel
    {

        public int ID { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public List<PersonModel> Members { get; set; }
        public List<AnimalModel> Animals { get; set; }


    }

    public class PersonModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int FamilyID { get; set; }
        public int MemberType { get; set; }

    }

    public class AnimalModel
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int? Age { get; set; }

        public string? Gender { get; set; }

        public int FamilyID { get; set; }


    }

    public class FamilyAddRequestModel
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public List<PersonModel> Persons { get; set; }
        public List<AnimalModel> Animals { get; set; }

    }

}
