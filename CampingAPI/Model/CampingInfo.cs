using System.Collections.Generic;

namespace CampingAPI.Model
{
    public class CampingInfo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public List<string> ImagesUrl { get; set; }
        public List<string> InfrastructureItems { get; set; }
        public decimal SinglePersonTentPrice { get; set; }
        public decimal TwoPeopleTentPrice { get; set; }
        public decimal CampingAreaPrice { get; set; }
    }
}
