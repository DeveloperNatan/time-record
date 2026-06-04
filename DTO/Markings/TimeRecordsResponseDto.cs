namespace TimeRecord.DTO.Markings
{
    public class TimeRecordsResponseDto
    {
        public int Id { get; set; }
        public int Matriculation { get; set; }

        public DateTime RecordedAt { get; set; }
    }
}