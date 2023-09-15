using Microsoft.AspNetCore.Mvc;
using MyProjectWebApi.Model.ResponseModel;

namespace MyProjectWebApi.Model.FamilyInterface
{
    public interface IFamilyService
    {
        List<FamilyModel> GetMembers();

        void AddFamilyService(FamilyAddRequestModel model);

        void UpdatePersonService(PersonModel model);

        void UpdateAnimalService(AnimalModel model);

        void DeleteFamilyService(int familyID);

    }
}
