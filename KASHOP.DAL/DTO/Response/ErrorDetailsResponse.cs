using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Response
{
    public class ErrorDetailsResponse
    {
        public int statusCode {  get; set; }
        public string message { get; set; }
        public string innerError { get; set; }
    }
}
