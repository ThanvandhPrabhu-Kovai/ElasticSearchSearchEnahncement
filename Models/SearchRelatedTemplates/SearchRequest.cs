﻿using ElasticSearchSearchEnhancement.Models.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchSearchEnhancement.Models.SearchRelatedTemplates
{
    public class SearchRequest
    {
        public SearchRequest()
        {
            this.Fields = new List<string>();
            this.SortBy = new List<SortByDefinition>();
            this.Filters = new List<FilterDefinition>();
            this.PageNumber = 1;
            this.PageSize = 10;
        }

        public IList<string> Fields { get; set; }

        public List<SortByDefinition> SortBy { get; set; }

        public List<FilterDefinition> Filters { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int? From { get; set; }

        public string Query { get; set; }

        public bool DoNotPaginate { get; set; }

        public List<string> FieldsToReturn { get; set; }
    }

}
