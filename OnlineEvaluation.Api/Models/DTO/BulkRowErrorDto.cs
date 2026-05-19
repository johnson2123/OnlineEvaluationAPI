namespace OnlineEvaluation.Api.Models.DTO
{
    public class BulkRowErrorDto
    {
        public int RowNumber { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

    }
}
