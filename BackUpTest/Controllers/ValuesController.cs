using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;

namespace BackUpTest.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IHttpActionResult Get()
        {
            // read connectionstring from config file
            var connectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;

            // read backup folder from config file ("C:/temp/")
            var backupFolder = "D:\\Temp\\";

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            // set backupfilename (you will get something like: "C:/temp/MyDatabase-2013-12-07.bak")
            var backupFileName = String.Format("{0}{1}-{2}.bak",
                backupFolder, sqlConStrBuilder.InitialCatalog,
                DateTime.Now.ToString("yyyy-MM-dd"));
            var fileName = String.Format("{0}-{1}.bak", sqlConStrBuilder.InitialCatalog,
                DateTime.Now.ToString("yyyy-MM-dd"));
            FileStream backupFileStream = null;
            var stream = new MemoryStream();
            byte[] fileBytes;
            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                var query = String.Format("BACKUP DATABASE {0} TO DISK='{1}'",
                    sqlConStrBuilder.InitialCatalog, backupFileName);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                fileBytes = File.ReadAllBytes(backupFileName);
                stream = new MemoryStream(fileBytes,true);

              //  backupFileStream = new FileStream(backupFileName, FileMode.OpenOrCreate); 
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileBytes)
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = ResponseMessage(result);

            return response;
          //  return stream;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
