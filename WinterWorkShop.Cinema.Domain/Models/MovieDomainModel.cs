﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class MovieDomainModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool Current { get; set; }
        public double? Rating { get; set; }
        public int Year { get; set; }
        public bool HasOscar { get; set; }
        public List<TagDomainModel> Tags { get; set; }
        public List<ProjectionDomainModel> Projections { get; set; }
    }
}
