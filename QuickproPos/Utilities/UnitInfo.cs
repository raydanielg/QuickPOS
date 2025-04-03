using QuickproPos.Data.Setting;

namespace QuickproPos.Utilities
{
    public class UnitInfo
    {
        public List<Unit> GetUnitInfo()
        {
            var info = new List<Unit>();
            info.Add(new Unit()
            {
                UnitName = "BAGS",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "BOTTLES",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "BOX",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "BUNDLES",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "DOZENS",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "LITRE",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "METERS",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "MILILITRE",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "NUMBERS",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "PACKS",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "PAIRS",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "PIECES",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "ROOLS",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "SQUARE FEET",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            info.Add(new Unit()
            {
                UnitName = "SQUARE METERS",
                NoOfDecimalplaces = 2,
                AddedDate = DateTime.UtcNow
            });
            return info;
        }
    }
}
