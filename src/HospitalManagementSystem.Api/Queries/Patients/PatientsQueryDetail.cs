﻿namespace HospitalManagementSystem.Api.Queries.Patients
{
    public class PatientsQueryDetail
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public string? Status { get; set; }
        public string? Name { get; set; }
    }
}