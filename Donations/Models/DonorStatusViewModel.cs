public class DonorStatusViewModel
{
    public DateTime? NextAppointment { get; set; }
    public DateTime? EligibleFrom { get; set; }
    public bool IsEligible { get; set; }

    public string StatusMessage
    {
        get
        {
            if (NextAppointment.HasValue)
            {
                return $"Next appointment: {NextAppointment.Value:MMM d}";
            }

            if (!IsEligible && EligibleFrom.HasValue)
            {
                var daysUntilEligible = (EligibleFrom.Value - DateTime.Today).Days;
                return $"{daysUntilEligible} days until eligible";
            }

            if (IsEligible)
            {
                return "Eligible to donate";
            }

            return "No appointment history";
        }
    }
}