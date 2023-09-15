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

        public int MemberType { get; set; }
    }

    public class AnimalModel
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int? Age { get; set; }

        public string? Gender { get; set; }
    }

    public class SqlResult
    {
        public int FamilyID { get; set; }
        public string FamilyTitle { get; set; }
        public string FamilyAddress { get; set; }
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int Type { get;set; }

        public int MemberAge { get; set; }

        public string MemberGender { get; set; }

        public int AnimalID { get; set; }
        public string AnimalName { get; set; }

        public string AnimalGender { get; set; }    
        public int AnimalAge { get; set; }  



    }
}
