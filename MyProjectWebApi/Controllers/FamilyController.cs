using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using MyProjectWebApi.Model;
using MyProjectWebApi.Model.FamilyInterface;
using MyProjectWebApi.Model.ResponseModel;
using MyProjectWebApi.Models.Entities;
using System.Data;
using System.Reflection;


namespace MyProjectWebApi.Controllers
{
    [ApiController]
    public class FamilyController : ControllerBase
    {
        private readonly IFamilyService _familyService;

        public FamilyController(IFamilyService familyService)
        {
            _familyService = familyService;

        }

        [Route("GetAllFamily")]
        [HttpGet]
        public IActionResult GetMembers()
        {

            var data = _familyService.GetMembers();
            return Ok(data);

        }

        [Route("AddFamily")]
        [HttpPost]

        public IActionResult AddFamily(FamilyAddRequestModel model)
        {
            _familyService.AddFamilyService(model);
            return Ok();
        }


        [Route("UpdateFamilyMembers")]
        [HttpPut]
        public IActionResult UpdatePerson(PersonModel model)
        {

            _familyService.UpdatePersonService(model);
            return Ok();

        }

        [Route("UpdateFamilyAnimals")]
        [HttpPut]
        public IActionResult UpdateAnimal(AnimalModel model)
        {
            _familyService.UpdateAnimalService(model);
            return Ok();
        }

        [Route("DeleteFamily")]
        [HttpDelete]

        public IActionResult DeleteFamily(int familyID)
        {
            _familyService.DeleteFamilyService(familyID);
            return Ok();
        }


    }
}

