namespace DebugWebApplication.Controllers
{
    using System;
    using System.Web.Http;

    public class AnalyzerController : ApiController
    {
        public IHttpActionResult DoSomethingBad() // only IF return type is assignable to IHttpActionResult
        {
            try // only IF try/catch exists
            {
                return Ok(new RandomMutablePoco()); // ideally you want to also change this to a normal strongly typed return and change the method signature, but this is a lot more work as it also needs a signature change
            }
            catch (Exception)
            // catch (Exception ex) // What this should be instead - Check for uniqueness of exception variable name
            {
                return BadRequest("ops!"); // only IF return BadRequest is in place
                //throw new HttpException(400, "ops!", ex); // What this should do instead
                // also need to include using System.Web; if it's not included yet or prepend that to the Exception and let R# do the rest
            }
        }
    }

    public class RandomMutablePoco
    {
        public string Name { get; set; }

        public string Stuff { get; set; }
    }
}
