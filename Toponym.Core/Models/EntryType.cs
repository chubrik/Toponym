namespace Toponym.Core.Models
{
    public enum EntryType
    {
        Unknown = 0,

        // Populated

        Populated = 10,
        City,
        Dwelling,
        Hamlet,
        Town,
        Village,

        // Water

        Water = 20,
        Lake,
        Pond,
        River,
        Stream,

        // Locality

        Locality = 30
    }
}
