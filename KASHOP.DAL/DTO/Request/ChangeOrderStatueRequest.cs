using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Request
{
    public class ChangeOrderStatueRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatusEnum status {  get; set; }
    }
}
