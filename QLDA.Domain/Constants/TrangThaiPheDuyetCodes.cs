namespace QLDA.Domain.Constants;

/// <summary>
/// Mã trạng thái phê duyệt (dùng chung cho dự toán & nội dung trình duyệt)
/// </summary>
public static class TrangThaiPheDuyetCodes {
    public static class Loai {
        public const string DuToan = "DuToan";
        public const string NoiDung = "NoiDung";
    }

    public static class DuToan {
        public const string DuThao = "DT";
        public const string DaTrinh = "ĐTr";
        public const string DaDuyet = "ĐD";
        public const string TraLai = "TL";
        public const string Legacy = "LEG";
    }

    public static class NoiDung {
        public const string ChoXuLy = "CXL";
        public const string DaDuyet = "DD";
        public const string TuChoi = "TC";
        public const string TraLai = "TL";
        public const string DaKySo = "DKS";
        public const string DaChuyenQLVB = "DQLVB";
        public const string DaPhatHanh = "DPH";
    }
}
