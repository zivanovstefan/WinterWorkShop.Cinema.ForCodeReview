﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class ProjectionService : IProjectionService
    {
        private readonly IProjectionsRepository _projectionsRepository;
        
        public ProjectionService(IProjectionsRepository projectionsRepository)
        {
            _projectionsRepository = projectionsRepository;
        }

        public async Task<IEnumerable<ProjectionDomainModel>> GetAllAsync()
        {
            var data = await _projectionsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    Id = item.Id,
                    MovieId = item.MovieId,
                    AuditoriumId = item.AuditoriumId,
                    ProjectionTime = item.DateTime,
                    MovieTitle = item.Movie.Title,
                    AditoriumName = item.Auditorium.AuditName
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<ProjectionDomainModel> GetProjectionByIdAsync(Guid id)
        {
            var data = await _projectionsRepository.GetByIdAsync(id);

            if (data == null)
            {
                var errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = Messages.PROJECTION_DOESNT_EXIST_ERROR,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return await Task.FromResult<ProjectionDomainModel>(null);
            }

            var result = new ProjectionDomainModel()
            {
                Id = data.Id,
                MovieId= data.MovieId,
                MovieTitle= data.Movie.Title,
                AuditoriumId = data.AuditoriumId,
                AditoriumName= data.Auditorium.AuditName,
                ProjectionTime = data.DateTime
            };

            return result;
        }

        public async Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel)
        {
            int projectionTime = 2;

            var projectionsAtSameTime = _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId)
                .Where(x => x.DateTime < domainModel.ProjectionTime.AddHours(projectionTime) && x.DateTime > domainModel.ProjectionTime.AddHours(-projectionTime))
                .ToList();

            if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTIONS_AT_SAME_TIME
                };
            }

            var newProjection = new Projection
            {
                MovieId = domainModel.MovieId,
                AuditoriumId = domainModel.AuditoriumId,
                DateTime = domainModel.ProjectionTime
            };

            var insertedProjection = _projectionsRepository.Insert(newProjection);

            if (insertedProjection == null)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_CREATION_ERROR
                };
            }

            _projectionsRepository.Save();
            CreateProjectionResultModel result = new CreateProjectionResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projection = new ProjectionDomainModel
                {
                    Id = insertedProjection.Id,
                    AuditoriumId = insertedProjection.AuditoriumId,
                    MovieId = insertedProjection.MovieId,
                    ProjectionTime = insertedProjection.DateTime
                }
            };

            return result;
        }

        public async Task<List<ProjectionDomainModel>> FilterProjections(int? cinemaId, int? auditoriumId, Guid? movieId, DateTime? dateFrom, DateTime? dateTo)
        {
            var data = await _projectionsRepository.GetAll();

            List<Projection> result = new List<Projection>();

            if (data == null)
            {
                return null;
            }

            if (cinemaId != null)
            {
                result = data.Where(x => x.Auditorium.CinemaId.Equals(cinemaId)).ToList();
            }

            if (auditoriumId != null)
            {
                result = result.Where(x => x.AuditoriumId.Equals(auditoriumId)).ToList();

                if (movieId != null)
                {
                    result = result.Where(x => x.MovieId.Equals(movieId)).ToList();
                }
            }

            if (dateFrom != null && dateTo != null)
            {
                result = data.Where(x => x.DateTime >= dateFrom && x.DateTime <= dateTo).ToList();
            }

            List<ProjectionDomainModel> results = new List<ProjectionDomainModel>();
            foreach (var item in result)
            {
                results.Add(new ProjectionDomainModel
                {
                    Id = item.Id,
                    MovieId = item.MovieId,
                    AuditoriumId = item.AuditoriumId,
                    ProjectionTime = item.DateTime,
                    MovieTitle = item.Movie.Title,
                    AditoriumName = item.Auditorium.AuditName

                });
            }
            return results;
        }
    }
}
