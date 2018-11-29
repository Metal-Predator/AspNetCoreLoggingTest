using System;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreLoggingTest.Controllers
{
    [Route("test")]
    public class TestController : ControllerBase
    {
        [HttpGet("echo")]
        public string Echo(string msg)
        {
            return msg;
        }

        [HttpGet("raise-error")]
        public void RaiseError(string msg)
        {
            throw new ApplicationException(msg);
        }
    }
}