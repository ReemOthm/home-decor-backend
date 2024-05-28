public static class Helper
{
    public static string GenerateSlug(string slug)
    {
        return slug.ToLower().Replace(" ", "-");
    }

}