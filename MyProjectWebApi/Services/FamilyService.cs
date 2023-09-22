using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyProjectWebApi.Model.FamilyInterface;
using MyProjectWebApi.Model.ResponseModel;
using System.Data;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using MyProjectWebApi.Models.Entities;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Xml;
using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace MyProjectWebApi.Model.FamilyService
{

    public class FamilyService : IFamilyService
    {
        private readonly IConfiguration _configuration;
        public FamilyService(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public FamilyModel GetByID(int id)
        {
            var sql = " select top(1) * from family where ID = @ID" +
                " select * from person where person.FamilyID = @ID" +
                " select * from animal where animal.FamilyID = @ID";

            var family = new FamilyModel();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection")))
            {
                connection.Open();
                var param = new { ID = id };

                using (var reader = connection.QueryMultiple(sql, param))
                {
                    family = reader.Read<FamilyModel>().FirstOrDefault();
                    family.Members = reader.Read<PersonModel>().ToList();
                    family.Animals = reader.Read<AnimalModel>().ToList();                 
                  
                }
            }
            return family;
        }


        public List<FamilyModel> GetMembers()
        {
            var sql = " select * from family" +
                " select * from person" +
                " select * from animal";

            var members = new List<FamilyModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection")))
            {
                connection.Open();

                using (var reader = connection.QueryMultiple(sql))
                {
                    members = reader.Read<FamilyModel>().ToList();
                    var persons = reader.Read<PersonModel>();
                    var animals = reader.Read<AnimalModel>();

                    members.ForEach(fam =>
                    {
                        fam.Members = persons.Where(p => p.FamilyID == fam.ID).ToList();
                        fam.Animals = animals.Where(p => p.FamilyID == fam.ID).ToList();
                    });

                }

            }
            return members;


        }

        //  Additional solution of GetMembers.

        //    Dictionary<int, FamilyModel> familyDictionary = new Dictionary<int, FamilyModel>();
        //    Dictionary<int, PersonModel> personDictionary = new Dictionary<int, PersonModel>();
        //    Dictionary<int, AnimalModel> animalDictionary = new Dictionary<int, AnimalModel>();

        //    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection")))
        //    {
        //        connection.Open();

        //        SqlCommand cmd = new SqlCommand("SELECT * FROM family JOIN person ON person.FamilyID = family.ID LEFT JOIN Animal ON Animal.FamilyID = family.ID", connection);
        //        SqlDataReader reader = cmd.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            int familyID = reader.GetInt32(0);
        //            string familyTitle = reader.GetString(1);
        //            string familyAddress = reader.GetString(2);

        //            var personID = reader.GetInt32(3);
        //            var person = new PersonModel
        //            {
        //                ID = personID,
        //                FirstName = reader.GetString(4),
        //                LastName = reader.GetString(5),
        //                Age = reader.GetInt32(6),
        //                Gender = reader.GetString(7)
        //            };

        //            var animalID = reader.IsDBNull(10) ? 0 : reader.GetInt32(10);
        //            var animal = new AnimalModel
        //            {
        //                ID = animalID,
        //                Name = reader.IsDBNull(11) ? null : reader.GetString(11),
        //                Gender = reader.IsDBNull(12) ? null : reader.GetString(12),
        //                Age = reader.IsDBNull(13) ? 0 : reader.GetInt32(13)
        //            };

        //            if (!familyDictionary.TryGetValue(familyID, out var family))
        //            {
        //                family = new FamilyModel
        //                {
        //                    ID = familyID,
        //                    Title = familyTitle,
        //                    Address = familyAddress,
        //                    Members = new List<PersonModel>(),
        //                    Animals = new List<AnimalModel>()
        //                };
        //                familyDictionary[familyID] = family;
        //            }

        //            if (!personDictionary.TryGetValue(personID, out var existingPerson))
        //            {
        //                family.Members.Add(person);
        //                personDictionary[personID] = person;
        //            }

        //            if (animalID != 0 && !animalDictionary.TryGetValue(animalID, out var existingAnimal))
        //            {
        //                family.Animals.Add(animal);
        //                animalDictionary[animalID] = animal;
        //            }
        //        }
        //     
        //    }

        //    return new List<FamilyModel>(familyDictionary.Values);
        //}


        public void Add(FamilyAddRequestModel model)
        {
            string addFamilyQuery = "INSERT INTO family (Title, Address) VALUES (@Title, @Address); SELECT SCOPE_IDENTITY()";
            string addPersonQuery = "INSERT INTO person (FirstName, LastName, Age, Gender, Type, FamilyID) VALUES (@FirstName, @LastName, @Age, @Gender, @MemberType, @FamilyID)";
            string addAnimalQuery = "INSERT INTO animal (Name, Gender, Age, FamilyID) VALUES (@Name, @Gender, @Age, @FamilyID)";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection")))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(addFamilyQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", model.Title);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    var familyPK = cmd.ExecuteScalar();

                    foreach (var person in model.Persons)
                    {
                        using (SqlCommand addPersonCmd = new SqlCommand(addPersonQuery, connection))
                        {
                            addPersonCmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                            addPersonCmd.Parameters.AddWithValue("@LastName", person.LastName);
                            addPersonCmd.Parameters.AddWithValue("@Age", person.Age);
                            addPersonCmd.Parameters.AddWithValue("@Gender", person.Gender);
                            addPersonCmd.Parameters.AddWithValue("@MemberType", person.MemberType);
                            addPersonCmd.Parameters.AddWithValue("@FamilyID", familyPK);
                            addPersonCmd.ExecuteNonQuery();
                        }
                    }

                    foreach (var animal in model.Animals)
                    {
                        using (SqlCommand addAnimalCmd = new SqlCommand(addAnimalQuery, connection))
                        {
                            addAnimalCmd.Parameters.AddWithValue("@Name", animal.Name);
                            addAnimalCmd.Parameters.AddWithValue("@Gender", animal.Gender);
                            addAnimalCmd.Parameters.AddWithValue("@Age", animal.Age);
                            addAnimalCmd.Parameters.AddWithValue("@FamilyID", familyPK);
                            addAnimalCmd.ExecuteNonQuery();
                        }
                    }
                }

            }
        }

        public void UpdatePerson(PersonModel model)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection")))
            {
                connection.Open();

                string updateQuery = "UPDATE FamilyDB.dbo.Person SET FirstName = @FirstName, LastName = @LastName," +
                                     "Age = @Age, Gender = @Gender" +
                                     " WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@Age", model.Age);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@ID", model.ID);

                    int rowsAffacted = cmd.ExecuteNonQuery();

                    if (rowsAffacted == 0)
                    {
                        throw new Exception("No rows were updated.The provided ID may not exist");
                    }

                }

            }

        }

        public void UpdateAnimal(AnimalModel model)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection")))
            {
                connection.Open();

                string updateQuery = "UPDATE FamilyDB.dbo.Animal SET Name = @Name,Age = @Age,Gender= @Gender WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Age", model.Age);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@ID", model.ID);

                    int rowsAffacted = cmd.ExecuteNonQuery();

                    if (rowsAffacted == 0)
                    {
                        throw new Exception("No rows were updated.The provided ID may not exist");
                    }
                }

            }
        }

        public void Delete(int familyID)
        {

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection")))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;

                    try
                    {
                        cmd.CommandText = "DELETE FROM FamilyDB.dbo.Person WHERE FamilyID = @FamilyID";
                        cmd.Parameters.AddWithValue("@FamilyID", familyID);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "DELETE FROM FamilyDB.dbo.Animal WHERE FamilyID = @FamilyID";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "DELETE FROM FamilyDB.dbo.Family WHERE ID = @FamilyID";
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

        }


    }

}



