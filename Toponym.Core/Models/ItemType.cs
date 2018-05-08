namespace Toponym.Core.Models
{
    public enum ItemType
    {
        Unknown = 0,

        // Populated places

        PopulatedUnknown = 100,
        Agrogorodok,
        Gorod,
        GorodskojPoselok,
        Derevnya,
        KurortnyPoselok,
        Poselok,
        PoselokGorodskogoTipa,
        RabochiPoselok,
        Selo,
        SelskiNaselennyPunkt,
        Hutor,

        // Water objects

        WaterUnknown = 200,
        River,
        Stream,
        Lake,
        Pond
    }
}
