using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class BulkOperationResultDto<T>
    {
        public int TotalRecordsReceived { get; set; }
        public int SuccessfullyProcessedCount { get; set; }
        public int FailedCount => Errors.Count;
        public bool HasErrors => Errors.Any();

        public List<T> Errors { get; set; } = new List<T>();
    }
}
