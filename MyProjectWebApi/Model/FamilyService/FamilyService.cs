using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyProjectWebApi.Model.FamilyInterface;
using MyProjectWebApi.Model.ResponseModel;
using System.Data;

namespace MyProjectWebApi.Model.FamilyService
{

    public class FamilyService : IFamilyService
    {
        private readonly IConfiguration _configuration;
        public FamilyService(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public List<FamilyModel> GetMembers()
        {

            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection"));
            SqlCommand cmd = new SqlCommand("SELECT * from family  join person on person.FamilyID = Family.ID left join Animal on Animal.FamilyID = Family.ID", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            List<SqlResult> members = new List<SqlResult>();
            foreach (DataRow row in dt.Rows)
            {

                var result = new SqlResult()
                {
                    FamilyID = Convert.ToInt32(row[0]),
                    FamilyTitle = Convert.ToString(row[1]),
                    FamilyAddress = Convert.ToString(row[2]),
                    MemberID = Convert.ToInt32(row[3]),
                    FirstName = Convert.ToString(row[4]),
                    LastName = Convert.ToString(row[5]),
                    MemberAge = Convert.ToInt32(row[6]),
                    MemberGender = Convert.ToString(row[7]),
                    Type = Convert.ToInt32(row[8]),

                    AnimalID = row[10] != DBNull.Value ? Convert.ToInt32(row[10]) : 0,
                    AnimalName = row[10] != DBNull.Value ? Convert.ToString(row[11]) : null,
                    AnimalGender = row[10] != DBNull.Value ? Convert.ToString(row[12]) : null,
                    AnimalAge = row[10] != DBNull.Value ? Convert.ToInt32(row[13]) : 0

                };
                members.Add(result);

            }

            var data = members.GroupBy(m => new { m.FamilyID, m.FamilyTitle, m.FamilyAddress })
                .Select(m => new FamilyModel
                {
                    Address = m.Key.FamilyAddress,
                    Title = m.Key.FamilyTitle,
                    ID = m.Key.FamilyID,
                    Members = m.Select(p => new PersonModel
                    {
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Age = p.MemberAge,
                        Gender = p.MemberGender,
                        ID = p.MemberID
                    }).DistinctBy(p=>p.ID).ToList(),
                    Animals = m.Select(a => new AnimalModel
                    {
                        Age = a.AnimalAge,
                        Gender = a.AnimalGender,
                        Name = a.AnimalName,
                        ID = a.AnimalID
                    })
                    .Where(a => a.ID != 0)
                    .DistinctBy(a => a.ID).ToList()

                }).ToList();
            return data;
        }


        public void AddFamilyService(FamilyAddRequestModel model)
        {

            string addQuery = $"Insert into family values ('{model.Title}','{model.Address}'); SELECT SCOPE_IDENTITY()";
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection"));

            using (SqlCommand cmd = new SqlCommand(addQuery, connection))

            {
                connection.Open();
                var familyPK = cmd.ExecuteScalar();


                var familyPersonQueryPart = model.Persons
                    .Select(p => $"('{p.FirstName}','{p.LastName}','{p.Age}','{p.Gender}','{p.MemberType}','{familyPK}')");

                var familyPersonQuery = $"Insert into person values {String.Join(',', familyPersonQueryPart)}";
                SqlCommand addPersoncmd = new SqlCommand(familyPersonQuery, connection);
                addPersoncmd.ExecuteNonQuery();

                var familyAnimalQueryPart = model.Animals
                    .Select(a => $"('{a.Name}','{a.Gender}','{a.Age}','{familyPK}')");
                var familyAnimalQuery = $"Insert into animal values{string.Join(',', familyAnimalQueryPart)}";
                SqlCommand addAnimalcmd = new SqlCommand(familyAnimalQuery, connection);
                addAnimalcmd.ExecuteNonQuery();

                connection.Close();
            }


        }


        public void UpdatePersonService(PersonModel model)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection"));
            string updateQuery = "UPDATE FamilyDB.dbo.Person SET FirstName = @FirstName, LastName = @LastName WHERE ID = @ID";

            using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
            {
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@ID", model.ID);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }

        public void UpdateAnimalService(AnimalModel model)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection"));
            string updateQuery = "UPDATE FamilyDB.dbo.Animal SET Name = @Name WHERE ID = @ID";

            using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@ID", model.ID);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }


        public void DeleteFamilyService(int familyID)
        {

            var cmdtext = "BEGIN TRANSACTION;" +
                "DECLARE @FamilyID INT;" +
                $"SET @FamilyID = {familyID};" +
                "DELETE FROM FamilyDB.dbo.Person WHERE FamilyID = @FamilyID;" +
                "DELETE FROM FamilyDB.dbo.Animal WHERE FamilyID = @FamilyID;" +
                "DELETE FROM FamilyDB.dbo.Family WHERE ID = @FamilyID;" +
                "COMMIT;";

            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default Connection"));

            using (SqlCommand cmd = new SqlCommand(cmdtext, connection))
            {             
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

            }


        }





    }
}

