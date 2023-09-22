using Microsoft.AspNetCore.Mvc;
using MyProjectWebApi.Model.ResponseModel;

namespace MyProjectWebApi.Model.FamilyInterface
{
    public interface IFamilyService
    {
        FamilyModel GetByID(int id);

        List<FamilyModel> GetMembers();

        void Add(FamilyAddRequestModel model);

        void UpdatePerson(PersonModel model);

        void UpdateAnimal(AnimalModel model);

        void Delete(int familyID);

    }
}
