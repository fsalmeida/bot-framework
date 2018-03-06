﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using CampingAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CampingAPI.Controllers
{
    [Route("api/[controller]")]
    public class CampingController : Controller
    {
        [HttpGet]
        public CampingInfo Get()
        {
            CampingInfo campingInfo = new CampingInfo()
            {
                Address = "Rua do Camping Bonitão, 25 - CEP 02157-335 - São Paulo, SP, Brasil",
                ImagesUrl = new List<string>()
                {
                    "https://gurudacidade.com.br/wp-content/uploads/2018/02/camping_tents-0.jpg",
                    "https://s3.amazonaws.com/imagescloud/images/medias/camping/camping-tente.jpg",
                    "http://res.muenchen-p.de/.imaging/stk/responsive/image980/dms/lhm/tourismus/camping-l/document/camping-l.jpg",
                    "https://greatist.com/sites/default/files/styles/article_main/public/Campsite_featured.jpg?itok=ZZQ8wJwJ"
                },
                Name = "Terramar",
                InfrastructureItems = new List<string>() { "Fogão", "Chuveiro quente", "Banheiro", "Geladeira compartilhada", "WIFI" },
                SinglePersonTentPrice = 40,
                TwoPeopleTentPrice = 65,
                CampingAreaPrice = 30
            };
            return campingInfo;
        }
    }
}