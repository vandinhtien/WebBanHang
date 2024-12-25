namespace SV21T1020742.Web.Models
{
    public class OrderSearchInput : PaginationSearchInput
    {
        public int Status { get; set; } = 0;

        public string TimeRange { get; set; } = "";

        public DateTime? FromTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TimeRange))
                    return null;
                string[] times = TimeRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = times[0].Trim().ToDateTime();
                    return value;
                }
                return null;
            }
        }

        public DateTime? ToTime
        {
            get
            {

                if (string.IsNullOrWhiteSpace(TimeRange))
                    return null;
                string[] times = TimeRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = times[1].Trim().ToDateTime();
                    if (value.HasValue)
                        value = value.Value.AddMilliseconds(86399998); //86399999
                    return value;
                }
                return null;
            }
        }
    }
}
