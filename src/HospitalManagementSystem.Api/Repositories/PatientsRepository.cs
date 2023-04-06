﻿using HospitalManagementSystem.Api.Repositories.Interfaces;
using HospitalManagementSystem.Infra.MongoDBStructure.Interfaces;
using HospitalManagementSystem.Infra.MongoDBStructure;
using HospitalManagementSystem.Api.Models.Patients;
using HospitalManagementSystem.Api.Queries.Patients;
using MongoDB.Driver;
using HospitalManagementSystem.Api.Helpers;
using HospitalManagementSystem.Infra.MongoDBStructure.Extensions;

namespace HospitalManagementSystem.Api.Repositories
{
    public class PatientsRepository : ReadStore, IPatientsRepository
    {
        public PatientsRepository(
            IMongoFactory mongoFactory,
            Serilog.ILogger logger) : base(mongoFactory, logger)
        {
        }

        public async Task DeletePatient(string patientId)
        {
            var patientIdFilter = Builders<PatientReadModel>.Filter.Eq(x => x._id, patientId);

            await _db.GetCollection<PatientReadModel>(typeof(PatientReadModel).Name).DeleteOneAsync(patientIdFilter);
        }

        public async Task<PatientReadModel> GetPatientById(string patientId)
        {
            var filter = Builders<PatientReadModel>.Filter.Eq(x => x._id, patientId);

            return (await _db.GetCollection<PatientReadModel>(typeof(PatientReadModel).Name).FindAsync(filter)).FirstOrDefault();
        }

        public async Task<(List<PatientReadModel> patients, PatientsQueryDetail detail)> GetPatients(PatientsQueryModel query)
        {
            var page = (query.Page ?? 1) < 1 ? 1 : query.Page ?? 1;
            var pageSize = (query.PageSize ?? QueryHelper.DefaultPageSize) < 1 ? QueryHelper.DefaultPageSize : query.PageSize ?? QueryHelper.DefaultPageSize;

            var filters = new List<FilterDefinition<PatientReadModel>>();

            filters.AddFilter((x) => x._id, query.PatientId);
            filters.AddFilter((x) => x.DateOfBirth, query.DateOfBirth);
            filters.AddFilter((x) => x.AdmissionDate, query.AdmissionDate);
            filters.AddFilter((x) => x.Name, query.Name);
            filters.AddFilter((x) => x.IsAdmitted, query.isAdmitted);
            filters.AddFilter((x) => x.PatientStatus, query.Status);
            filters.AddFilter((x) => x.Gender, query.Gender);


            var filter = filters.Consolidate();

            var (sort, sortBy, sortDirection) = QueryHelper.GetPatientSortDetails(query.SortBy, query.SortDirection);

            var options = new FindOptions<PatientReadModel>
            {
                Skip = (page - 1) * pageSize,
                Limit = pageSize,
                Sort = sort
            };

            var countResult = await _db.GetCollection<PatientReadModel>(typeof(PatientReadModel).Name).CountDocumentsAsync(filter);
            var result = await _db.GetCollection<PatientReadModel>(typeof(PatientReadModel).Name).FindAsync(filter, options);

            var resultDetail = new PatientsQueryDetail
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = (int)countResult,
                TotalPages = (int)countResult / (int)pageSize + (countResult % pageSize > 0 ? 1 : 0),
                SortBy = sortBy,
                SortDirection = sortDirection,
                Status = query.Status,
                Name = query.Name,
                
            };

            return (result.ToList(), resultDetail);
        }

        public async Task UpsertPatient(PatientReadModel cmd)
        {
            var filter = Builders<PatientReadModel>.Filter.Eq(x => x._id, cmd._id);
            var update = Builders<PatientReadModel>.Update
                .Set(x => x._id, cmd._id)
                .Set(x => x.Name, cmd.Name)
                .Set(x => x.DateOfBirth, cmd.DateOfBirth)
                .Set(x => x.Gender, cmd.Gender)
                .Set(x => x.PhoneNumber, cmd.PhoneNumber)
                .Set(x => x.Email, cmd.Email)
                .Set(x => x.AdmissionDate, cmd.AdmissionDate)
                .Set(x => x.IsAdmitted, cmd.IsAdmitted)
                .Set(x => x.RoomId, cmd.RoomId)
                .Set(x => x.PatientStatus, cmd.PatientStatus);

            var options = new UpdateOptions { IsUpsert = true };

            await _db.GetCollection<PatientReadModel>(typeof(PatientReadModel).Name).UpdateOneAsync(filter, update, options);
        }
    }
}