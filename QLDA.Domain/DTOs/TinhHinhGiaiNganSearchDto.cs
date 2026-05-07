// Re-export DTOs from Domain layer for Application layer use
// This maintains backward compatibility and follows Clean Architecture
namespace QLDA.Domain.DTOs;

public record TinhHinhGiaiNganSearchDto 
{
        public int Nam { get; set; }
        public int? NguonVonId { get; set; }
        public int? LoaiDuAnTheoNamId { get; set; }
        public int? LoaiDuAnId { get; set; }

}
