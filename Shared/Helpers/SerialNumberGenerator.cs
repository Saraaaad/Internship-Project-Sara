public static class SerialNumberGenerator
{
    public static string Generate(string departmentCode, int userId)
    {
        return $"{departmentCode}-{userId:D4}";
    }
}