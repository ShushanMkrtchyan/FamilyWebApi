using MyProjectWebApi.Model.ResponseModel;
using MyProjectWebApi.Models.Entities;

namespace MyProjectWebApi.Model
{
    public class FamilyAddRequestModel
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public List<PersonModel> Persons { get; set; }  
        public List<AnimalModel> Animals { get; set; }

    }

    
}
