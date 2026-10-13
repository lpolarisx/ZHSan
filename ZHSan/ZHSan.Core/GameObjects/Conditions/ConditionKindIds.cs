namespace GameObjects.Conditions;

public static class ConditionKindIds
{
    // 逻辑运算符
    public const int Negate = 996;      // 条件取反
    public const int Or = 997;          // 逻辑或
    public const int And = 998;         // 逻辑与（预留）
    public const int Xor = 999;         // 逻辑异或（预留）
    
    // ID 区间定义
    public const int PersonRangeStart = 1000;
    public const int PersonRangeEnd = 2000;
    public const int ArchitectureRangeStart = 2000;
    public const int ArchitectureRangeEnd = 3000;
    public const int FactionRangeStart = 3000;
    public const int FactionRangeEnd = 4000;
    public const int LeaderRangeStart = 4000;
    public const int LeaderRangeEnd = 5000;
    
    public static bool IsInPersonRange(int id) => 
        id >= PersonRangeStart && id < PersonRangeEnd;
    public static bool IsInArchitectureRange(int id) => 
        id >= ArchitectureRangeStart && id < ArchitectureRangeEnd;
    public static bool IsInFactionRange(int id) => 
        id >= FactionRangeStart && id < FactionRangeEnd;
    public static bool IsInLeaderRange(int id) => 
        id >= LeaderRangeStart && id < LeaderRangeEnd;
}