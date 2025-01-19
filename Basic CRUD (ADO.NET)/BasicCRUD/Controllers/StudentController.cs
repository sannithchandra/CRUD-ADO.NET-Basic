using BasicCRUD.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BasicCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        //List Synatx
        List<Student> students; // similar to db table
        private readonly IConfiguration _configuration;
        private readonly string connectionstring;
        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionstring = _configuration.GetConnectionString("DefaultConnection");

            students= new List<Student>();
            Student student1 = new Student() { Id = 1, Name = "Sannith", Branch = "CSE" };//db table record
            Student student2 = new Student() { Id = 2, Name = "Prasad", Branch = "CSE" };
            students.Add(student1);
            students.Add(student2);
 

        }

     
        [HttpGet("getStudents")] // used tyo get alll students data
        public async Task<IActionResult> GetStudents() 
        {
            List<Project> projectsList = new List<Project>(); //crearing obj for List of Project
            DataTable projectDatatable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionstring))
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlcommand = new SqlCommand("select * from Project", sqlConnection);
                sqlcommand.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter(sqlcommand);
                adapter.Fill(projectDatatable);
            }
            foreach (DataRow row in projectDatatable.Rows)
            {
                Project project = new Project();//crearing obj for Project
                project.ProjectId = Convert.ToInt32(row["ProjectId"]);
                project.ProjectName = row["ProjectName"].ToString();
                project.DateOfStart = row["DateOfStart"].ToString();
                project.TeamSize = Convert.ToInt32(row["TeamSize"]);
                project.Active = Convert.ToBoolean(row["Active"]);
                project.Status = row["Status"].ToString();
                project.ClientLocationId = row["ClientLocationId"].ToString();
                projectsList.Add(project);
            }

            return Ok(projectsList);
        }

        [HttpGet("getStudentById/{id}")]
        public IActionResult getStudentById(int id)
        {
            //
            // List<Project> result = new List<Project>();
            //Project result = null;
            Project project = null;//crearing obj for Project
            DataTable projectDatatable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionstring))
            {
                sqlConnection.Open();
                SqlCommand sqlcommand = new SqlCommand($"select * from project where ProjectId = {id}", sqlConnection);
                //sqlcommand.Parameters.AddWithValue("@id", id);
                //command.Parameters.Add("@Id", SqlDbType.Int32).Value = Id;
                sqlcommand.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter(sqlcommand);
                adapter.Fill(projectDatatable);
            }

            foreach (DataRow row in projectDatatable.Rows)

            {
                project = new Project();
                project.ProjectId = Convert.ToInt32(row["ProjectId"]);
                project.ProjectName = row["ProjectName"].ToString();
                project.DateOfStart = row["DateOfStart"].ToString();
                project.TeamSize = Convert.ToInt32(row["TeamSize"]);
                project.Active = Convert.ToBoolean(row["Active"]);
                project.Status = row["Status"].ToString();
                project.ClientLocationId = row["ClientLocationId"].ToString();
                //result.Add(project);
            }

            //return Ok(result);
            //++++++++++++++++++++++++++++
            if (project == null)
            {
                return Ok("student not found");
            }
            
            return Ok(project);
            //return Ok(Project);

        }

        [HttpPost("addStudent")]
        public IActionResult AddStudent([FromBody] Project project)
        {

            //project;//crearing obj for Project
            if (project == null)
            {
                return BadRequest("Project object is null.");
            }

            using (SqlConnection sqlConnection = new SqlConnection(connectionstring))
            {
                string query = "INSERT INTO Project(ProjectId, ProjectName, DateOfStart, TeamSize, Active, Status, ClientLocationId) " +
                               "VALUES (@ProjectId, @ProjectName, @DateOfStart, @TeamSize, @Active, @Status, @ClientLocationId)";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@ProjectId", project.ProjectId);
                sqlCommand.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                sqlCommand.Parameters.AddWithValue("@DateOfStart", project.DateOfStart);
                sqlCommand.Parameters.AddWithValue("@TeamSize", project.TeamSize);
                sqlCommand.Parameters.AddWithValue("@Active", project.Active);
                sqlCommand.Parameters.AddWithValue("@Status", project.Status);
                sqlCommand.Parameters.AddWithValue("@ClientLocationId", project.ClientLocationId);

                sqlConnection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok("Project added successfully.");
                }
                else
                {
                    return StatusCode(500, "Error adding project.");
                }
            }

        }

        [HttpPut("updateStudent/{id}")]
        public IActionResult updateStudent(int id, [FromBody] Project project)
        {
            //List<Student> studentFound = students.Where(x => x.Id == id).ToList();
            //if (studentFound.Count > 0)
            //{
            //    studentFound[0].Name = student.Name;
            //    studentFound[0].Branch = student.Branch;
            //}
            //else { 
            // return BadRequest();
            //}
            //return Ok(students);
            if (id != project.ProjectId)
            {
                return BadRequest("Project ID mismatch.");
            }

            using (SqlConnection sqlConnection = new SqlConnection(connectionstring))
            {
                string query = "UPDATE Project SET ProjectName = @ProjectName, DateOfStart = @DateOfStart, TeamSize = @TeamSize, Active = @Active, Status = @Status, ClientLocationId = @ClientLocationId WHERE ProjectId = @ProjectId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@ProjectId", project.ProjectId);
                sqlCommand.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                sqlCommand.Parameters.AddWithValue("@DateOfStart", project.DateOfStart);
                sqlCommand.Parameters.AddWithValue("@TeamSize", project.TeamSize);
                sqlCommand.Parameters.AddWithValue("@Active", project.Active);
                sqlCommand.Parameters.AddWithValue("@Status", project.Status);
                sqlCommand.Parameters.AddWithValue("@ClientLocationId", project.ClientLocationId);
                sqlConnection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok("Project updated successfully.");
                }
                else
                {
                    return NotFound("Project not found.");
                }
            }



        }

        [HttpDelete("deleteStudent/{id}")]
        public IActionResult deleteStudent(int id)
        {
            //List<Student> studentFound = students.Where(x => x.Id == id).ToList();
            //if (studentFound.Count > 0)
            //{
            //    students.Remove(studentFound[0]);
            //}
            //else
            //{
            //    return BadRequest();
            //}
            //return Ok(students);
            using (SqlConnection sqlConnection = new SqlConnection(connectionstring))
            {
                // First, check if the project exists
                string checkQuery = "SELECT COUNT(1) FROM Project WHERE ProjectId = @ProjectId";
                SqlCommand checkCommand = new SqlCommand(checkQuery, sqlConnection);
                checkCommand.Parameters.AddWithValue("@ProjectId", id);

                sqlConnection.Open();
                int projectExists = (int)checkCommand.ExecuteScalar();

                if (projectExists == 0)
                {
                    // Project does not exist
                    return NotFound($"Project with ID {id} not found.");
                }

                // Now perform the DELETE since we know the project exists
                string deleteQuery = "DELETE FROM Project WHERE ProjectId = @ProjectId";
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);
                deleteCommand.Parameters.AddWithValue("@ProjectId", id);

                deleteCommand.ExecuteNonQuery();

                return Ok($"Project with ID {id} deleted successfully.");
            }



        }



    }
}
