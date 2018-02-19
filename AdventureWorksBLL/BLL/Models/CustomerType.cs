namespace AdventureWorks.BLL
{
    public enum CustomerType { STORE, PERSON, UNKNOWN }

    public static class CustomerTypeExtension
    {
        public static string GetDisplayValue(this CustomerType customerType)
        {
            switch (customerType)
            {
                case CustomerType.STORE:
                    return "Store";
                case CustomerType.PERSON:
                    return "Person";
                case CustomerType.UNKNOWN:
                default:
                    return "Unknown";
            }
        }
    }
}
