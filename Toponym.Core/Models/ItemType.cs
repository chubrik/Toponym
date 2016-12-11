namespace Toponym.Core.Models {
    public enum ItemType {

        Unknown = 0,

        // Населённые пункты

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

        // Водные объекты

        WaterUnknown = 200,
        River,
        Stream,
        Lake,
        Pond
    }
}
