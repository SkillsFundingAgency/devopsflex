namespace DebugWebApplication.Controllers
{
    using System;
    using System.Web.Http;

    public class AnalyzerController : ApiController
    {
        public IHttpActionResult DoSomethingBad()
        {
            try
            {
                return Ok(new RandomMutablePoco());
            }
            catch
            {
                return BadRequest(string.Format("asd {0}", 123));
            }
        }
    }

    public class RandomMutablePoco
    {
        public string Name { get; set; }

        public string Stuff { get; set; }
    }
}
